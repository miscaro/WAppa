using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WeatherApp.Data;
using WeatherApp.Dtos.FavoriteLocation;
using WeatherApp.Dtos.Weather;
using WeatherApp.Models;
using WeatherApp.Services;
using Microsoft.Extensions.Logging;

namespace WeatherApp.Controllers
{
    /// <summary>
    /// Controller per la gestione delle località preferite degli utenti.
    /// Permette di aggiungere, visualizzare e rimuovere località preferite.
    /// Richiede autenticazione per tutte le operazioni.
    /// </summary>
    [Authorize] // Tutti gli endpoint richiedono autenticazione
    [Route("api/[controller]")]  // La route di base sarà /api/FavoriteLocations
    [ApiController]
    public class FavoriteLocationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IGeocodingService _geocodingService;
        private readonly IWeatherService _weatherService;
        private readonly ILogger<FavoriteLocationsController> _logger;

        /// <summary>
        /// Costruttore con iniezione delle dipendenze.
        /// </summary>
        /// <param name="context">Contesto del database per l'accesso ai dati</param>
        /// <param name="geocodingService">Servizio per la conversione di nomi in coordinate</param>
        /// <param name="weatherService">Servizio per il recupero dei dati meteo</param>
        /// <param name="logger">Logger per il tracciamento delle operazioni</param>
        public FavoriteLocationsController(
            ApplicationDbContext context,
            IGeocodingService geocodingService,
            IWeatherService weatherService,
            ILogger<FavoriteLocationsController> logger)
        {
            _context = context;
            _geocodingService = geocodingService;
            _weatherService = weatherService;
            _logger = logger;
        }

        /// <summary>
        /// Recupera l'ID dell'utente corrente dal token JWT.
        /// </summary>
        /// <returns>L'ID dell'utente autenticato</returns>
        /// <exception cref="UnauthorizedAccessException">Se l'ID utente non è valido o non è presente nel token</exception>
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                _logger.LogError("User ID claim (NameIdentifier) not found or invalid in token for user: {User}", User.Identity?.Name ?? "Unknown");
                throw new UnauthorizedAccessException("Impossibile determinare l'ID utente dal token.");
            }
            return userId;
        }

        /// <summary>
        /// Aggiunge una nuova località preferita per l'utente corrente.
        /// </summary>
        /// <param name="addLocationDto">Dati della località da aggiungere</param>
        /// <returns>Dettagli della località aggiunta, inclusi i dati meteo correnti</returns>
        /// <response code="201">Località aggiunta con successo</response>
        /// <response code="400">Richiesta non valida</response>
        /// <response code="401">Non autenticato</response>
        /// <response code="404">Località non trovata</response>
        /// <response code="409">Località già presente tra i preferiti</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ServiceResponse<FavoriteLocationResponseDto>>> AddFavoriteLocation(AddFavoriteLocationDto addLocationDto)
        {
            var serviceResponse = new ServiceResponse<FavoriteLocationResponseDto>();
            int userId;
            try
            {
                userId = GetCurrentUserId();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("AddFavoriteLocation: Tentativo di accesso non autorizzato. Messaggio: {Message}", ex.Message);
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return Unauthorized(serviceResponse);
            }

            _logger.LogInformation("Utente {UserId} sta tentando di aggiungere una località preferita: {LocationName}", userId, addLocationDto.LocationName);
            
            // Geocodifica il nome della località per ottenere le coordinate
            var geocodeResponse = await _geocodingService.GetCoordinatesAsync(addLocationDto.LocationName);
            if (!geocodeResponse.Success || geocodeResponse.Data == null)
            {
                _logger.LogWarning("Utente {UserId} - Geocodifica fallita per {LocationName}: {GeoMessage}", userId, addLocationDto.LocationName, geocodeResponse.Message);
                serviceResponse.Success = false;
                serviceResponse.Message = geocodeResponse.Message ?? $"Impossibile trovare le coordinate per '{addLocationDto.LocationName}'.";
                return NotFound(serviceResponse);
            }

            var geoData = geocodeResponse.Data;
            _logger.LogInformation("Utente {UserId} - Geocodifica per {LocationName} completata: {GeoName}, Lat={Lat}, Lon={Lon}", 
                userId, addLocationDto.LocationName, geoData.Name, geoData.Latitude, geoData.Longitude);

            // Verifica se la località è già tra i preferiti dell'utente
            var existingLocation = await _context.FavoriteLocations
                .FirstOrDefaultAsync(fl => fl.UserId == userId && fl.Latitude == geoData.Latitude && fl.Longitude == geoData.Longitude);

            if (existingLocation != null)
            {
                _logger.LogInformation("Utente {UserId} - La località '{GeoName}' è già tra i preferiti.", userId, geoData.Name);
                serviceResponse.Success = false;
                serviceResponse.Message = $"La località '{geoData.Name}' è già tra i tuoi preferiti.";
                return Conflict(serviceResponse);
            }

            // Crea una nuova località preferita
            var newFavoriteLocation = new FavoriteLocation
            {
                Name = geoData.Name,
                Latitude = geoData.Latitude,
                Longitude = geoData.Longitude,
                UserId = userId
            };

            // Salva la nuova località nel database
            _context.FavoriteLocations.Add(newFavoriteLocation);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Utente {UserId} - Nuova località preferita salvata con ID {FavId} per '{Name}'.", 
                userId, newFavoriteLocation.Id, newFavoriteLocation.Name);

            // Recupera i dati meteo per la nuova località
            var weatherResponse = await _weatherService.GetWeatherDataAsync(
                newFavoriteLocation.Latitude, 
                newFavoriteLocation.Longitude, 
                newFavoriteLocation.Name);
                
            _logger.LogInformation("Utente {UserId} - Dati meteo recuperati per la nuova località {Name}: Successo={WeatherSuccess}", 
                userId, newFavoriteLocation.Name, weatherResponse.Success);

            // Prepara la risposta con i dettagli della località e i dati meteo
            serviceResponse.Data = new FavoriteLocationResponseDto
            {
                Id = newFavoriteLocation.Id,
                Name = newFavoriteLocation.Name,
                Latitude = newFavoriteLocation.Latitude,
                Longitude = newFavoriteLocation.Longitude,
                WeatherData = weatherResponse.Success ? weatherResponse.Data : null
            };
            serviceResponse.Message = $"Località '{newFavoriteLocation.Name}' aggiunta ai preferiti.";
            
            return CreatedAtAction(nameof(GetFavoriteLocationById), new { id = newFavoriteLocation.Id }, serviceResponse);
        }

        /// <summary>
        /// Recupera tutte le località preferite dell'utente corrente.
        /// </summary>
        /// <returns>Lista delle località preferite con i relativi dati meteo</returns>
        /// <response code="200">Operazione completata con successo</response>
        /// <response code="401">Non autenticato</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ServiceResponse<List<FavoriteLocationResponseDto>>>> GetFavoriteLocations()
        {
            var serviceResponse = new ServiceResponse<List<FavoriteLocationResponseDto>>();
            int userId;
            try
            {
                userId = GetCurrentUserId();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("GetFavoriteLocations: Tentativo di accesso non autorizzato. Messaggio: {Message}", ex.Message);
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return Unauthorized(serviceResponse);
            }

            _logger.LogInformation("Utente {UserId} sta recuperando le località preferite.", userId);
            
            // Recupera tutte le località preferite dell'utente
            var favoriteLocations = await _context.FavoriteLocations
                                        .Where(fl => fl.UserId == userId)
                                        .ToListAsync();

            var responseDtos = new List<FavoriteLocationResponseDto>();
            
            // Per ogni località, recupera i dati meteo aggiornati
            foreach (var location in favoriteLocations)
            {
                var weatherResponse = await _weatherService.GetWeatherDataAsync(
                    location.Latitude, 
                    location.Longitude, 
                    location.Name);
                    
                if (!weatherResponse.Success)
                {
                    _logger.LogWarning("Utente {UserId} - Impossibile recuperare i dati meteo per la località '{LocationName}' (ID: {LocationId}). Messaggio: {WeatherMessage}", 
                        userId, location.Name, location.Id, weatherResponse.Message);
                }
                
                responseDtos.Add(new FavoriteLocationResponseDto
                {
                    Id = location.Id,
                    Name = location.Name,
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    WeatherData = weatherResponse.Success ? weatherResponse.Data : null
                });
            }

            serviceResponse.Data = responseDtos;
            serviceResponse.Message = responseDtos.Any() 
                ? "Località preferite recuperate con successo." 
                : "Nessuna località preferita trovata per l'utente.";
                
            _logger.LogInformation("Utente {UserId} - Recuperate {Count} località preferite.", userId, responseDtos.Count);
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Recupera una specifica località preferita in base all'ID.
        /// </summary>
        /// <param name="id">ID della località preferita</param>
        /// <returns>Dettagli della località preferita con i dati meteo correnti</returns>
        /// <response code="200">Località trovata</response>
        /// <response code="401">Non autenticato</response>
        /// <response code="404">Località non trovata o accesso negato</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ServiceResponse<FavoriteLocationResponseDto>>> GetFavoriteLocationById(int id)
        {
            var serviceResponse = new ServiceResponse<FavoriteLocationResponseDto>();
            int userId;
            try
            {
                userId = GetCurrentUserId();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("GetFavoriteLocationById: Tentativo di accesso non autorizzato per ID {LocationId}. Messaggio: {Message}", id, ex.Message);
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return Unauthorized(serviceResponse);
            }

            _logger.LogInformation("Utente {UserId} sta recuperando la località preferita con ID {LocationId}.", userId, id);
            
            // Cerca la località preferita per ID, assicurandosi che appartenga all'utente corrente
            var favoriteLocation = await _context.FavoriteLocations
                .FirstOrDefaultAsync(fl => fl.Id == id && fl.UserId == userId);

            if (favoriteLocation == null)
            {
                _logger.LogWarning("Utente {UserId} - Località preferita ID {LocationId} non trovata o accesso negato.", userId, id);
                serviceResponse.Success = false;
                serviceResponse.Message = "Località preferita non trovata o accesso negato.";
                return NotFound(serviceResponse);
            }
            
            // Recupera i dati meteo aggiornati per la località
            var weatherResponse = await _weatherService.GetWeatherDataAsync(
                favoriteLocation.Latitude, 
                favoriteLocation.Longitude, 
                favoriteLocation.Name);
                
            if (!weatherResponse.Success)
            {
                _logger.LogWarning("Utente {UserId} - Impossibile recuperare i dati meteo per la località '{LocationName}' (ID: {LocationId}). Messaggio: {WeatherMessage}", 
                    userId, favoriteLocation.Name, favoriteLocation.Id, weatherResponse.Message);
            }

            // Prepara la risposta con i dettagli della località e i dati meteo
            serviceResponse.Data = new FavoriteLocationResponseDto
            {
                Id = favoriteLocation.Id,
                Name = favoriteLocation.Name,
                Latitude = favoriteLocation.Latitude,
                Longitude = favoriteLocation.Longitude,
                WeatherData = weatherResponse.Success ? weatherResponse.Data : null
            };
            
            _logger.LogInformation("Utente {UserId} - Località preferita ID {LocationId} recuperata con successo.", userId, id);
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Rimuove una località preferita in base all'ID.
        /// </summary>
        /// <param name="id">ID della località da rimuovere</param>
        /// <returns>Conferma dell'operazione</returns>
        /// <response code="200">Località rimossa con successo</response>
        /// <response code="401">Non autenticato</response>
        /// <response code="404">Località non trovata o accesso negato</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ServiceResponse<bool>>> DeleteFavoriteLocation(int id)
        {
            var serviceResponse = new ServiceResponse<bool>();
            int userId;
            try
            {
                userId = GetCurrentUserId();
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("DeleteFavoriteLocation: Tentativo di accesso non autorizzato per ID {LocationId}. Messaggio: {Message}", id, ex.Message);
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
                return Unauthorized(serviceResponse);
            }

            _logger.LogInformation("Utente {UserId} sta tentando di rimuovere la località preferita ID {LocationId}.", userId, id);
            
            // Cerca la località preferita per ID, assicurandosi che appartenga all'utente corrente
            var favoriteLocation = await _context.FavoriteLocations
                .FirstOrDefaultAsync(fl => fl.Id == id && fl.UserId == userId);

            if (favoriteLocation == null)
            {
                _logger.LogWarning("Utente {UserId} - Località preferita ID {LocationId} non trovata o accesso negato.", userId, id);
                serviceResponse.Success = false;
                serviceResponse.Message = "Località preferita non trovata o accesso negato.";
                return NotFound(serviceResponse);
            }

            // Rimuove la località dal database
            _context.FavoriteLocations.Remove(favoriteLocation);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Utente {UserId} - Località preferita ID {LocationId} ('{LocationName}') rimossa con successo.", 
                userId, favoriteLocation.Id, favoriteLocation.Name);
                
            serviceResponse.Data = true;
            serviceResponse.Message = $"Località '{favoriteLocation.Name}' rimossa dai preferiti.";
            return Ok(serviceResponse);
        }
    }
}

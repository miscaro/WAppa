using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeatherApp.Dtos.Weather;
using WeatherApp.Services;
using Microsoft.Extensions.Logging;

namespace WeatherApp.Controllers
{
    /// <summary>
    /// Controller per la gestione delle richieste relative alle previsioni meteo.
    /// Supporta la ricerca per coordinate geografiche o per nome della località.
    /// </summary>
    [Route("api/[controller]")]  // La route di base sarà /api/Weather
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly IGeocodingService _geocodingService;
        private readonly ILogger<WeatherController> _logger;

        /// <summary>
        /// Costruttore con iniezione delle dipendenze necessarie.
        /// </summary>
        /// <param name="weatherService">Servizio per il recupero dei dati meteo</param>
        /// <param name="geocodingService">Servizio per la conversione di nomi di località in coordinate</param>
        /// <param name="logger">Logger per il tracciamento delle operazioni</param>
        public WeatherController(
            IWeatherService weatherService,
            IGeocodingService geocodingService,
            ILogger<WeatherController> logger)
        {
            _weatherService = weatherService;
            _geocodingService = geocodingService;
            _logger = logger;
        }

        /// <summary>
        /// Recupera i dati meteo per una località specificata tramite coordinate o nome.
        /// </summary>
        /// <param name="latitude">Latitudine della posizione (opzionale se viene fornito 'query')</param>
        /// <param name="longitude">Longitudine della posizione (opzionale se viene fornito 'query')</param>
        /// <param name="query">Nome della località o indirizzo (opzionale se vengono fornite le coordinate)</param>
        /// <returns>Dettagli meteo per la località richiesta</returns>
        /// <response code="200">Dati meteo recuperati con successo</response>
        /// <response code="400">Parametri della richiesta non validi</response>
        /// <response code="404">Località non trovata</response>
        /// <response code="500">Errore durante l'elaborazione della richiesta</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ServiceResponse<WeatherForecastDto>>> GetWeather(
            [FromQuery] double? latitude,
            [FromQuery] double? longitude,
            [FromQuery] string? query)
        {
            var response = new ServiceResponse<WeatherForecastDto>();
            _logger.LogInformation("WeatherController: Ricevuta richiesta con Lat={Lat}, Lon={Lon}, Query='{Query}'", latitude, longitude, query);

            // Gestione richiesta tramite coordinate geografiche
            if (latitude.HasValue && longitude.HasValue)
            {
                _logger.LogInformation("WeatherController: Utilizzo delle coordinate fornite Lat={Lat}, Lon={Lon} per il meteo.", 
                    latitude.Value, longitude.Value);
                
                // Utilizza le coordinate fornite direttamente
                response = await _weatherService.GetWeatherDataAsync(
                    latitude.Value, 
                    longitude.Value, 
                    locationNameFromGeocoding: query);
            }
            // Gestione richiesta tramite nome località (geocoding)
            else if (!string.IsNullOrWhiteSpace(query))
            {
                _logger.LogInformation("WeatherController: Geocodifica della query '{Query}'", query);
                var geocodeResponse = await _geocodingService.GetCoordinatesAsync(query);

                if (!geocodeResponse.Success || geocodeResponse.Data == null)
                {
                    _logger.LogWarning("WeatherController: Geocodifica fallita o nessun dato per la query '{Query}'. Messaggio: {Msg}", 
                        query, geocodeResponse.Message);
                    response.Success = false;
                    response.Message = geocodeResponse.Message ?? $"Impossibile trovare le coordinate per '{query}'.";
                    return NotFound(response);
                }

                var geoData = geocodeResponse.Data;
                _logger.LogInformation("WeatherController: Geocodifica per '{Query}' completata. Risultato: Nome='{Name}', Lat={Lat}, Lon={Lon}", 
                    query, geoData.Name, geoData.Latitude, geoData.Longitude);

                // Recupero i dati meteo utilizzando le coordinate ottenute dal geocoding
                response = await _weatherService.GetWeatherDataAsync(
                    geoData.Latitude, 
                    geoData.Longitude, 
                    geoData.Name);
            }
            else
            {
                // Nessun parametro valido fornito
                _logger.LogWarning("WeatherController: Parametri della richiesta non validi. Fornire latitudine/longitudine o una query di ricerca.");
                response.Success = false;
                response.Message = "Fornire almeno 'latitude' e 'longitude' o un parametro 'query'.";
                return BadRequest(response);
            }

            if (!response.Success)
            {
                _logger.LogWarning("WeatherController: Chiamata a WeatherService fallita o nessun dato per Lat={Lat}, Lon={Lon}, Query='{Query}'. Messaggio: {Message}", 
                    latitude, longitude, query, response.Message);
                
                // Restituisce 404 se non trova le coordinate, altrimenti 500
                return StatusCode(
                    response.Data == null && response.Message?.Contains("Could not find coordinates") == true ? 404 : 500, 
                    response);
            }
            
            _logger.LogInformation("WeatherController: Dati meteo recuperati con successo per Lat={Lat}, Lon={Lon}, Query='{Query}'", 
                latitude, longitude, query);
            return Ok(response);
        }
    }
}

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WeatherApp.Dtos.Geocoding;

namespace WeatherApp.Services
{
    /// <summary>
    /// Servizio per la conversione di nomi di località in coordinate geografiche utilizzando l'API di OpenMeteo.
    /// </summary>
    public class GeocodingService : IGeocodingService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<GeocodingService> _logger;
        private const string OpenMeteoGeocodingApiUrl = "https://geocoding-api.open-meteo.com/v1/search";

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="GeocodingService"/>
        /// </summary>
        /// <param name="httpClientFactory">Factory per creare istanze di HttpClient.</param>
        /// <param name="logger">Logger per la registrazione degli eventi.</param>
        public GeocodingService(IHttpClientFactory httpClientFactory, ILogger<GeocodingService> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// Ottiene le coordinate geografiche per una località specificata.
        /// </summary>
        /// <param name="query">Il nome della località da cercare.</param>
        /// <returns>Un oggetto <see cref="ServiceResponse{GeocodingServiceResultDto}"/> contenente i dettagli della località e le coordinate.</returns>
        public async Task<ServiceResponse<GeocodingServiceResultDto>> GetCoordinatesAsync(string query)
        {
            var response = new ServiceResponse<GeocodingServiceResultDto>();
            if (string.IsNullOrWhiteSpace(query))
            {
                response.Success = false;
                response.Message = "Query non può essere vuota.";
                return response;
            }

            var httpClient = _httpClientFactory.CreateClient("GeocodingClient");
            // Costruiamo l'URL con i parametri: nome della città, prendi solo il primo risultato (count=1), lingua italiana, formato json
            string requestUrl = $"{OpenMeteoGeocodingApiUrl}?name={Uri.EscapeDataString(query)}&count=1&language=it&format=json";

            try
            {
                var apiResponse = await httpClient.GetFromJsonAsync<OpenMeteoGeocodingResponse>(requestUrl);

                if (apiResponse?.Results != null && apiResponse.Results.Any())
                {
                    var bestResult = apiResponse.Results.First();
                    response.Data = new GeocodingServiceResultDto
                    {
                        Name = bestResult.Name,
                        Latitude = bestResult.Latitude,
                        Longitude = bestResult.Longitude,
                        Country = bestResult.Country,
                        Admin1 = bestResult.Admin1
                    };
                    response.Message = "Coordinate trovate con successo.";
                }
                else
                {
                    response.Success = false;
                    response.Message = $"Nessuna coordinate trovata per '{query}'.";
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Errore durante il recupero dei dati di geocoding per la query: {Query}", query);
                response.Success = false;
                response.Message = "Errore durante il recupero dei dati di geocoding. Riprova più tardi.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore imprevisto in GeocodingService per la query: {Query}", query);
                response.Success = false;
                response.Message = "Errore imprevisto. Riprova più tardi.";
            }

            return response;
        }
    }
}

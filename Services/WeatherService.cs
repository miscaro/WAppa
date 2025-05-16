using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WeatherApp.Dtos.Weather;

namespace WeatherApp.Services
{
    /// <summary>
    /// Servizio per il recupero e la gestione dei dati meteorologici utilizzando l'API di OpenMeteo.
    /// Fornisce metodi per ottenere previsioni meteo attuali e future basate su coordinate geografiche.
    /// </summary>
    public class WeatherService : IWeatherService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<WeatherService> _logger;
        private const string OpenMeteoApiUrl = "https://api.open-meteo.com/v1/forecast";

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="WeatherService"/>
        /// </summary>
        /// <param name="httpClientFactory">Factory per creare istanze di HttpClient.</param>
        /// <param name="logger">Logger per la registrazione degli eventi.</param>
        public WeatherService(IHttpClientFactory httpClientFactory, ILogger<WeatherService> logger)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Recupera i dati meteo per una specifica posizione geografica.
        /// </summary>
        /// <param name="latitude">La latitudine della posizione.</param>
        /// <param name="longitude">La longitudine della posizione.</param>
        /// <param name="locationNameFromGeocoding">Il nome della località (opzionale).</param>
        /// <returns>Un oggetto <see cref="ServiceResponse{WeatherForecastDto}"/> contenente i dati meteo.</returns>
        public async Task<ServiceResponse<WeatherForecastDto>> GetWeatherDataAsync(double latitude, double longitude, string? locationNameFromGeocoding = null)
        {
            var response = new ServiceResponse<WeatherForecastDto>();
            var httpClient = _httpClientFactory.CreateClient("OpenMeteoClient");

            string currentParams = "temperature_2m,relative_humidity_2m,apparent_temperature,is_day,precipitation,rain,showers,snowfall,weather_code,wind_speed_10m,wind_direction_10m";
            string dailyParams = "weather_code,temperature_2m_max,temperature_2m_min,apparent_temperature_max,apparent_temperature_min,sunrise,sunset,precipitation_sum,rain_sum,showers_sum,snowfall_sum,precipitation_probability_max,wind_speed_10m_max,wind_gusts_10m_max,uv_index_max";
            
            string latStr = latitude.ToString(CultureInfo.InvariantCulture);
            string lonStr = longitude.ToString(CultureInfo.InvariantCulture);

            string requestUrl = $"{OpenMeteoApiUrl}?latitude={latStr}&longitude={lonStr}&current={currentParams}&daily={dailyParams}&timezone=auto&forecast_days=7";
            
            _logger.LogInformation("Recupero dati meteo da URL: {RequestUrl} per il nome della località (se fornito): {LocationName}", requestUrl, locationNameFromGeocoding ?? "N/A");

            try
            {
                var apiResponse = await httpClient.GetFromJsonAsync<OpenMeteoWeatherResponseDto>(requestUrl);

                if (apiResponse != null)
                {
                    // Passa locationNameFromGeocoding al metodo di mapping
                    response.Data = MapToWeatherForecastDto(apiResponse, locationNameFromGeocoding);
                    response.Message = "Dati meteo recuperati con successo.";
                }
                else
                {
                    _logger.LogWarning("Impossibile recuperare i dati meteo o la risposta API era null per lat: {Latitude}, lon: {Longitude}", latitude, longitude);
                    response.Success = false;
                    response.Message = "Impossibile recuperare i dati meteo o la risposta API era null.";
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Errore HTTP durante il recupero dei dati meteo per lat: {Latitude}, lon: {Longitude}", latitude, longitude);
                response.Success = false;
                response.Message = "Errore durante il recupero dei dati meteo. Riprova più tardi.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Errore imprevisto in WeatherService per lat: {Latitude}, lon: {Longitude}", latitude, longitude);
                response.Success = false;
                response.Message = "Errore imprevisto. Riprova più tardi.";
            }

            return response;
        }

        /// <summary>
        /// Converte la risposta dell'API di OpenMeteo in un oggetto WeatherForecastDto.
        /// </summary>
        /// <param name="apiData">I dati ricevuti dall'API di OpenMeteo.</param>
        /// <param name="explicitLocationName">Il nome esplicito della località (opzionale).</param>
        /// <returns>Un oggetto <see cref="WeatherForecastDto"/> con i dati meteo formattati.</returns>
        private WeatherForecastDto MapToWeatherForecastDto(OpenMeteoWeatherResponseDto apiData, string? explicitLocationName)
        {
            var forecastDto = new WeatherForecastDto
            {
                // Usa explicitLocationName se fornito, altrimenti usa un fallback (o lascialo vuoto)
                LocationName = explicitLocationName ?? apiData.Timezone.Split('/').LastOrDefault() ?? string.Empty,
                Latitude = apiData.Latitude,
                Longitude = apiData.Longitude,
                Timezone = apiData.Timezone,
                Current = apiData.CurrentWeather != null ? new CurrentConditionsDto
                {
                    Time = apiData.CurrentWeather.Time,
                    Temperature = apiData.CurrentWeather.Temperature,
                    ApparentTemperature = apiData.CurrentWeather.ApparentTemperature,
                    RelativeHumidity = apiData.CurrentWeather.RelativeHumidity,
                    Precipitation = apiData.CurrentWeather.Precipitation,
                    WeatherCode = apiData.CurrentWeather.WeatherCode,
                    WindSpeed = apiData.CurrentWeather.WindSpeed,
                    WeatherDescription = GetWeatherDescription(apiData.CurrentWeather.WeatherCode)
                } : null,
                Daily = new List<DailyForecastDto>()
            };

            if (apiData.Daily?.Time != null && apiData.Daily.Time.Any())
            {
                for (int i = 0; i < apiData.Daily.Time.Count; i++)
                {
                    // Assicura che gli array di dati giornalieri abbiano abbastanza elementi
                    // Questo previene IndexOutOfRangeException se alcuni dati sono mancanti
                    if (i < apiData.Daily.TemperatureMax.Count &&
                        i < apiData.Daily.TemperatureMin.Count &&
                        i < apiData.Daily.ApparentTemperatureMax.Count &&
                        i < apiData.Daily.ApparentTemperatureMin.Count &&
                        i < apiData.Daily.WeatherCode.Count &&
                        i < apiData.Daily.Sunrise.Count &&
                        i < apiData.Daily.Sunset.Count &&
                        i < apiData.Daily.PrecipitationSum.Count &&
                        // i < apiData.Daily.PrecipitationProbabilityMax.Count && // PrecipitationProbabilityMax può essere nullabile e avere meno elementi
                        i < apiData.Daily.WindSpeedMax.Count)
                    {
                        forecastDto.Daily.Add(new DailyForecastDto
                        {
                            Date = apiData.Daily.Time[i],
                            TemperatureMax = apiData.Daily.TemperatureMax[i],
                            TemperatureMin = apiData.Daily.TemperatureMin[i],
                            ApparentTemperatureMax = apiData.Daily.ApparentTemperatureMax[i],
                            ApparentTemperatureMin = apiData.Daily.ApparentTemperatureMin[i],
                            WeatherCode = apiData.Daily.WeatherCode[i],
                            Sunrise = apiData.Daily.Sunrise[i],
                            Sunset = apiData.Daily.Sunset[i],
                            PrecipitationSum = apiData.Daily.PrecipitationSum[i],
                            PrecipitationProbabilityMax = (apiData.Daily.PrecipitationProbabilityMax != null && i < apiData.Daily.PrecipitationProbabilityMax.Count) ? apiData.Daily.PrecipitationProbabilityMax[i] : null,
                            WindSpeedMax = apiData.Daily.WindSpeedMax[i],
                            WeatherDescription = GetWeatherDescription(apiData.Daily.WeatherCode[i])
                        });
                    }
                    else
                    {
                        _logger.LogWarning("Inconsistenza lunghezza array dati giornalieri per l'indice {Index} a lat: {Latitude}, lon: {Longitude}. Salto questo giorno.", i, apiData.Latitude, apiData.Longitude);
                    }
                }
            }
            return forecastDto;
        }

        /// <summary>
        /// Restituisce una descrizione testuale del codice meteo fornito.
        /// </summary>
        /// <param name="weatherCode">Il codice meteo da convertire in descrizione.</param>
        /// <returns>Una stringa che descrive le condizioni meteo.</returns>
        private string GetWeatherDescription(int weatherCode)
        {
            return weatherCode switch
            {
                0 => "Sereno",
                1 => "Prevalentemente sereno",
                2 => "Parzialmente nuvoloso",
                3 => "Nuvoloso",
                45 => "Nebbia",
                48 => "Nebbia con brina",
                51 => "Pioggerella leggera",
                53 => "Pioggerella moderata",
                55 => "Pioggerella densa",
                56 => "Pioggerella gelata leggera",
                57 => "Pioggerella gelata densa",
                61 => "Pioggia leggera",
                63 => "Pioggia moderata",
                65 => "Pioggia forte",
                66 => "Pioggia gelata leggera",
                67 => "Pioggia gelata forte",
                71 => "Nevicata leggera",
                73 => "Nevicata moderata",
                75 => "Nevicata forte",
                77 => "Gragnola",
                80 => "Rovescio leggero",
                81 => "Rovescio moderato",
                82 => "Rovescio violento",
                85 => "Rovescio di neve leggero",
                86 => "Rovescio di neve forte",
                95 => "Temporale leggero o moderato",
                96 => "Temporale con grandine leggera",
                99 => "Temporale con grandine forte",
                _ => "Non disponibile"
            };
        }
    }
}

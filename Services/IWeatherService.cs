using System.Threading.Tasks;
using WeatherApp.Dtos.Weather;

namespace WeatherApp.Services
{
    /// <summary>
    /// Interfaccia che definisce il contratto per il servizio meteorologico.
    /// Fornisce metodi per recuperare dati meteo attuali e previsioni.
    /// </summary>
    public interface IWeatherService
    {
        /// <summary>
        /// Recupera i dati meteo per una specifica posizione geografica.
        /// </summary>
        /// <param name="latitude">La latitudine della posizione.</param>
        /// <param name="longitude">La longitudine della posizione.</param>
        /// <param name="locationNameFromGeocoding">Il nome della località (opzionale).</param>
        /// <returns>Un oggetto <see cref="ServiceResponse{WeatherForecastDto}"/> contenente i dati meteo.</returns>
        Task<ServiceResponse<WeatherForecastDto>> GetWeatherDataAsync(double latitude, double longitude, string? locationNameFromGeocoding = null);
    }
}

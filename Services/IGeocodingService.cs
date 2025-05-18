using System.Threading.Tasks;
using WeatherApp.Dtos.Geocoding;

namespace WeatherApp.Services
{
    /// <summary>
    /// Interfaccia che definisce il contratto per il servizio di geocoding.
    /// Fornisce metodi per convertire nomi di località in coordinate geografiche.
    /// </summary>
    public interface IGeocodingService
    {
        /// <summary>
        /// Ottiene le coordinate geografiche per una località specificata.
        /// </summary>
        /// <param name="query">Il nome della località da cercare.</param>
        /// <returns>Un oggetto <see cref="ServiceResponse{GeocodingServiceResultDto}"/> contenente i dettagli della località e le coordinate.</returns>
        Task<ServiceResponse<GeocodingServiceResultDto>> GetCoordinatesAsync(string query);
    }
}

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WeatherApp.Dtos.Geocoding
{
    /// <summary>
    /// DTO (Data Transfer Object) che rappresenta la risposta JSON dell'API di geocoding di OpenMeteo.
    /// Contiene una lista di risultati di geocoding e informazioni sulla generazione della risposta.
    /// </summary>
    public class OpenMeteoGeocodingResponse
    {
        /// <summary>
        /// Lista di risultati di geocoding restituiti dall'API.
        /// Può essere null se non ci sono risultati o in caso di errore.
        /// </summary>
        [JsonPropertyName("results")]
        public List<OpenMeteoGeocodingResult>? Results { get; set; }

        /// <summary>
        /// Tempo impiegato dal server per generare la risposta, in millisecondi.
        /// Utile per il monitoraggio delle prestazioni.
        /// </summary>
        [JsonPropertyName("generationtime_ms")]
        public double GenerationTimeMs { get; set; }
    }
}

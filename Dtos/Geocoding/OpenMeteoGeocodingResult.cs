using System.Text.Json.Serialization;

namespace WeatherApp.Dtos.Geocoding
{
    /// <summary>
    /// DTO (Data Transfer Object) che rappresenta un singolo risultato di geocoding
    /// restituito dall'API di OpenMeteo.
    /// Contiene le informazioni geografiche dettagliate di una località.
    /// </summary>
    public class OpenMeteoGeocodingResult
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        /// <summary>
        /// Codice ISO a due lettere del paese.
        /// Esempio: "IT" per Italia, "FR" per Francia, "DE" per Germania
        /// </summary>
        [JsonPropertyName("country_code")]
        public string CountryCode { get; set; } = string.Empty;

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Nome della regione o dello stato in cui si trova la località.
        /// Può essere null se l'informazione non è disponibile.
        /// Esempio: "Lazio", "Piedmont", "Lombardy"
        /// </summary>
        [JsonPropertyName("admin1")]
        public string? Admin1 { get; set; } // Regione o stato, può essere nullo
    }
}

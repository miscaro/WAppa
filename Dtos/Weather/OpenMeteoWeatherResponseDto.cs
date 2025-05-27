using System.Text.Json.Serialization;

namespace WeatherApp.Dtos.Weather
{
    /// <summary>
    /// DTO (Data Transfer Object) che rappresenta la risposta completa dell'API di OpenMeteo.
    /// Contiene tutti i dati meteorologici restituiti dal servizio.
    /// </summary>
    public class OpenMeteoWeatherResponseDto
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }

        /// <summary>
        /// Tempo impiegato dal server per generare la risposta in millisecondi.
        /// Utile per il monitoraggio delle prestazioni.
        /// </summary>
        [JsonPropertyName("generationtime_ms")]
        public double GenerationTimeMs { get; set; }

        /// <summary>
        /// Offset UTC in secondi rispetto al fuso orario della località.
        /// Esempio: 7200 per l'ora legale in Europa Centrale (UTC+2).
        /// </summary>
        [JsonPropertyName("utc_offset_seconds")]
        public int UtcOffsetSeconds { get; set; }

        /// <summary>
        /// Fuso orario della località in formato IANA.
        /// Esempio: "Europe/Rome", "America/New_York"
        /// </summary>
        [JsonPropertyName("timezone")]
        public string Timezone { get; set; } = string.Empty;

        /// <summary>
        /// Abbreviazione del fuso orario.
        /// Esempio: "CEST" per Central European Summer Time.
        /// </summary>
        [JsonPropertyName("timezone_abbreviation")]
        public string TimezoneAbbreviation { get; set; } = string.Empty;

        /// <summary>
        /// Altitudine della località in metri sopra il livello del mare.
        /// </summary>
        [JsonPropertyName("elevation")]
        public double Elevation { get; set; }

        /// <summary>
        /// Dati meteo attuali per la località.
        /// La proprietà è mappata su "current_weather" per compatibilità con l'API.
        /// </summary>
        [JsonPropertyName("current_weather")]
        public OpenMeteoCurrentWeatherDto? CurrentWeather { get; set; }

        /// <summary>
        /// Unità di misura utilizzate per i valori delle previsioni giornaliere.
        /// </summary>
        [JsonPropertyName("daily_units")]
        public OpenMeteoDailyUnitsDto? DailyUnits { get; set; }

        /// <summary>
        /// Dati delle previsioni giornaliere per i prossimi giorni.
        /// </summary>
        [JsonPropertyName("daily")]
        public OpenMeteoDailyDataDto? Daily { get; set; }
    }
}

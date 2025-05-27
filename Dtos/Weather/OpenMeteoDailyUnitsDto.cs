using System.Text.Json.Serialization;

namespace WeatherApp.Dtos.Weather
{
    /// <summary>
    /// DTO (Data Transfer Object) che specifica le unità di misura utilizzate per i dati giornalieri di OpenMeteo.
    /// Ogni proprietà specifica l'unità di misura per il corrispondente campo in OpenMeteoDailyDataDto.
    /// </summary>
    public class OpenMeteoDailyUnitsDto
    {
        /// <summary>
        /// Unità di misura per le date (sempre "iso8601").
        /// </summary>
        [JsonPropertyName("time")]
        public string Time { get; set; } = string.Empty;

        /// <summary>
        /// Unità di misura per i codici meteorologici (sempre "wmo code").
        /// </summary>
        [JsonPropertyName("weather_code")]
        public string WeatherCode { get; set; } = string.Empty;

        /// <summary>
        /// Unità di misura per le temperature massime (solitamente "°C").
        /// </summary>
        [JsonPropertyName("temperature_2m_max")]
        public string TemperatureMax { get; set; } = string.Empty;

        /// <summary>
        /// Unità di misura per le temperature minime (solitamente "°C").
        /// </summary>
        [JsonPropertyName("temperature_2m_min")]
        public string TemperatureMin { get; set; } = string.Empty;

        /// <summary>
        /// Unità di misura per le temperature massime percepite (solitamente "°C").
        /// </summary>
        [JsonPropertyName("apparent_temperature_max")]
        public string ApparentTemperatureMax { get; set; } = string.Empty;

        /// <summary>
        /// Unità di misura per le temperature minime percepite (solitamente "°C").
        /// </summary>
        [JsonPropertyName("apparent_temperature_min")]
        public string ApparentTemperatureMin { get; set; } = string.Empty;

        /// <summary>
        /// Unità di misura per l'orario dell'alba (sempre "iso8601").
        /// </summary>
        [JsonPropertyName("sunrise")]
        public string Sunrise { get; set; } = string.Empty;

        /// <summary>
        /// Unità di misura per l'orario del tramonto (sempre "iso8601").
        /// </summary>
        [JsonPropertyName("sunset")]
        public string Sunset { get; set; } = string.Empty;

        /// <summary>
        /// Unità di misura per le precipitazioni totali (solitamente "mm").
        /// </summary>
        [JsonPropertyName("precipitation_sum")]
        public string PrecipitationSum { get; set; } = string.Empty;

        /// <summary>
        /// Unità di misura per la pioggia (solitamente "mm").
        /// </summary>
        [JsonPropertyName("rain_sum")]
        public string RainSum { get; set; } = string.Empty;

        /// <summary>
        /// Unità di misura per i rovesci (solitamente "mm").
        /// </summary>
        [JsonPropertyName("showers_sum")]
        public string ShowersSum { get; set; } = string.Empty;

        /// <summary>
        /// Unità di misura per la neve (solitamente "cm").
        /// </summary>
        [JsonPropertyName("snowfall_sum")]
        public string SnowfallSum { get; set; } = string.Empty;

        /// <summary>
        /// Unità di misura per la probabilità di precipitazioni (sempre "%").
        /// </summary>
        [JsonPropertyName("precipitation_probability_max")]
        public string PrecipitationProbabilityMax { get; set; } = string.Empty;

        /// <summary>
        /// Unità di misura per la velocità del vento (solitamente "km/h").
        /// </summary>
        [JsonPropertyName("wind_speed_10m_max")]
        public string WindSpeedMax { get; set; } = string.Empty;

        /// <summary>
        /// Unità di misura per le raffiche di vento (solitamente "km/h").
        /// </summary>
        [JsonPropertyName("wind_gusts_10m_max")]
        public string WindGustsMax { get; set; } = string.Empty;

        /// <summary>
        /// Unità di misura per l'indice UV (sempre "").
        /// </summary>
        [JsonPropertyName("uv_index_max")]
        public string UvIndexMax { get; set; } = string.Empty;

    }
}

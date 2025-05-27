using System.Text.Json.Serialization;

namespace WeatherApp.Dtos.Weather
{
    /// <summary>
    /// DTO (Data Transfer Object) che rappresenta i dati meteo attuali restituiti da OpenMeteo.
    /// Mappato direttamente dalla risposta JSON dell'API.
    /// </summary>
    public class OpenMeteoCurrentWeatherDto
    {
        /// <summary>
        /// Temperatura attuale a 2 metri dal suolo in gradi Celsius.
        /// </summary>
        [JsonPropertyName("temperature_2m")]
        public double Temperature { get; set; }

        /// <summary>
        /// Umidità relativa a 2 metri dal suolo in percentuale (0-100).
        /// </summary>
        [JsonPropertyName("relative_humidity_2m")]
        public int RelativeHumidity { get; set; }

        /// <summary>
        /// Temperatura percepita a 2 metri dal suolo in gradi Celsius.
        /// Tiene conto di umidità e vento.
        /// </summary>
        [JsonPropertyName("apparent_temperature")]
        public double ApparentTemperature { get; set; }

        /// <summary>
        /// Indica se è giorno (1) o notte (0) al momento della misurazione.
        /// Utile per determinare l'icona meteo corretta.
        /// </summary>
        [JsonPropertyName("is_day")]
        public int IsDay { get; set; }

        /// <summary>
        /// Precipitazioni totali (pioggia, neve, pioggia gelata) nelle ultime ore in millimetri.
        /// </summary>
        [JsonPropertyName("precipitation")]
        public double Precipitation { get; set; }

        /// <summary>
        /// Quantità di pioggia nelle ultime ore in millimetri.
        /// </summary>
        [JsonPropertyName("rain")]
        public double Rain { get; set; }

        /// <summary>
        /// Quantità di rovesci nelle ultime ore in millimetri.
        /// </summary>
        [JsonPropertyName("showers")]
        public double Showers { get; set; }

        /// <summary>
        /// Quantità di neve nelle ultime ore in centimetri.
        /// </summary>
        [JsonPropertyName("snowfall")]
        public double Snowfall { get; set; }

        /// <summary>
        /// Codice numerico che descrive le condizioni meteorologiche attuali.
        /// Viene utilizzato per determinare l'icona meteo corrispondente.
        /// </summary>
        [JsonPropertyName("weather_code")]
        public int WeatherCode { get; set; }

        /// <summary>
        /// Velocità del vento a 10 metri dal suolo in km/h.
        /// </summary>
        [JsonPropertyName("wind_speed_10m")]
        public double WindSpeed { get; set; }

        /// <summary>
        /// Direzione del vento in gradi (0-360°).
        /// 0° = vento da Nord, 90° = vento da Est, 180° = vento da Sud, 270° = vento da Ovest.
        /// </summary>
        [JsonPropertyName("wind_direction_10m")]
        public int WindDirection { get; set; }

        /// <summary>
        /// Data e ora della misurazione in formato ISO 8601.
        /// Esempio: "2023-05-13T14:30:00"
        /// </summary>
        [JsonPropertyName("time")]
        public string Time { get; set; } = string.Empty;
    }
}

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WeatherApp.Dtos.Weather
{
    /// <summary>
    /// DTO (Data Transfer Object) che rappresenta i dati giornalieri delle previsioni meteo di OpenMeteo.
    /// Ogni proprietà è una lista dove ogni elemento corrisponde a un giorno.
    /// </summary>
    public class OpenMeteoDailyDataDto
    {
        /// <summary>
        /// Lista delle date per le quali sono disponibili le previsioni in formato YYYY-MM-DD.
        /// </summary>
        [JsonPropertyName("time")]
        public List<string> Time { get; set; } = new List<string>();

        /// <summary>
        /// Lista dei codici meteorologici per ogni giorno.
        /// Ogni codice descrive le condizioni meteo prevalenti della giornata.
        /// </summary>
        [JsonPropertyName("weather_code")]
        public List<int> WeatherCode { get; set; } = new List<int>();

        /// <summary>
        /// Lista delle temperature massime a 2 metri dal suolo in gradi Celsius per ogni giorno.
        /// </summary>
        [JsonPropertyName("temperature_2m_max")]
        public List<double> TemperatureMax { get; set; } = new List<double>();

        /// <summary>
        /// Lista delle temperature minime a 2 metri dal suolo in gradi Celsius per ogni giorno.
        /// </summary>
        [JsonPropertyName("temperature_2m_min")]
        public List<double> TemperatureMin { get; set; } = new List<double>();

        /// <summary>
        /// Lista delle temperature massime percepite a 2 metri dal suolo in gradi Celsius per ogni giorno.
        /// Tiene conto di umidità e vento.
        /// </summary>
        [JsonPropertyName("apparent_temperature_max")]
        public List<double> ApparentTemperatureMax { get; set; } = new List<double>();

        /// <summary>
        /// Lista delle temperature minime percepite a 2 metri dal suolo in gradi Celsius per ogni giorno.
        /// Tiene conto di umidità e vento.
        /// </summary>
        [JsonPropertyName("apparent_temperature_min")]
        public List<double> ApparentTemperatureMin { get; set; } = new List<double>();

        /// <summary>
        /// Lista degli orari dell'alba per ogni giorno in formato ISO 8601.
        /// </summary>
        [JsonPropertyName("sunrise")]
        public List<string> Sunrise { get; set; } = new List<string>();

        /// <summary>
        /// Lista degli orari del tramonto per ogni giorno in formato ISO 8601.
        /// </summary>
        [JsonPropertyName("sunset")]
        public List<string> Sunset { get; set; } = new List<string>();

        /// <summary>
        /// Lista delle precipitazioni totali (pioggia, neve, pioggia gelata) in millimetri per ogni giorno.
        /// </summary>
        [JsonPropertyName("precipitation_sum")]
        public List<double> PrecipitationSum { get; set; } = new List<double>();

        /// <summary>
        /// Lista delle quantità di pioggia in millimetri per ogni giorno.
        /// </summary>
        [JsonPropertyName("rain_sum")]
        public List<double> RainSum { get; set; } = new List<double>();

        /// <summary>
        /// Lista delle quantità di rovesci in millimetri per ogni giorno.
        /// </summary>
        [JsonPropertyName("showers_sum")]
        public List<double> ShowersSum { get; set; } = new List<double>();

        /// <summary>
        /// Lista delle quantità di neve in centimetri per ogni giorno.
        /// </summary>
        [JsonPropertyName("snowfall_sum")]
        public List<double> SnowfallSum { get; set; } = new List<double>();

        /// <summary>
        /// Lista delle probabilità massime di precipitazioni in percentuale (0-100) per ogni giorno.
        /// Può contenere valori null se i dati non sono disponibili.
        /// </summary>
        [JsonPropertyName("precipitation_probability_max")]
        public List<int?> PrecipitationProbabilityMax { get; set; } = new List<int?>();

        /// <summary>
        /// Lista delle velocità massime del vento a 10 metri dal suolo in km/h per ogni giorno.
        /// </summary>
        [JsonPropertyName("wind_speed_10m_max")]
        public List<double> WindSpeedMax { get; set; } = new List<double>();

        /// <summary>
        /// Lista delle raffiche di vento massime a 10 metri dal suolo in km/h per ogni giorno.
        /// </summary>
        [JsonPropertyName("wind_gusts_10m_max")]
        public List<double> WindGustsMax { get; set; } = new List<double>();

        /// <summary>
        /// Lista degli indici UV massimi per ogni giorno.
        /// Può contenere valori null se i dati non sono disponibili.
        /// </summary>
        [JsonPropertyName("uv_index_max")]
        public List<double?> UvIndexMax { get; set; } = new List<double?>(); // Può essere null

    }
}

using System.Collections.Generic;

namespace WeatherApp.Dtos.Weather
{
    /// <summary>
    /// DTO (Data Transfer Object) che rappresenta una previsione meteo completa per una località.
    /// Contiene le condizioni meteo attuali e le previsioni giornaliere.
    /// </summary>
    public class WeatherForecastDto
    {
        public string LocationName { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Timezone { get; set; } = string.Empty;

        /// <summary>
        /// Condizioni meteo attuali per la località.
        /// Può essere null se i dati non sono disponibili.
        /// </summary>
        public CurrentConditionsDto? Current { get; set; }

        /// <summary>
        /// Lista delle previsioni giornaliere per i prossimi giorni.
        /// La lista è ordinata per data crescente.
        /// </summary>
        public List<DailyForecastDto> Daily { get; set; } = new List<DailyForecastDto>();
    }
}

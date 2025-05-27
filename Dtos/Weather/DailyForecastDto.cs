namespace WeatherApp.Dtos.Weather
{
    /// <summary>
    /// DTO (Data Transfer Object) che rappresenta la previsione meteo per un singolo giorno.
    /// Contiene le informazioni meteorologiche previste per una data specifica.
    /// </summary>
    public class DailyForecastDto
    {
        public string Date { get; set; } = string.Empty;

        /// <summary>
        /// Temperatura massima prevista per il giorno in gradi Celsius.
        /// </summary>
        public double TemperatureMax { get; set; }

        /// <summary>
        /// Temperatura minima prevista per il giorno in gradi Celsius.
        /// </summary>
        public double TemperatureMin { get; set; }

        /// <summary>
        /// Temperatura massima percepita per il giorno in gradi Celsius.
        /// Tiene conto di umidità e vento.
        /// </summary>
        public double ApparentTemperatureMax { get; set; }

        /// <summary>
        /// Temperatura minima percepita per il giorno in gradi Celsius.
        /// Tiene conto di umidità e vento.
        /// </summary>
        public double ApparentTemperatureMin { get; set; }

        /// <summary>
        /// Codice numerico che descrive le condizioni meteorologiche previste.
        /// Viene utilizzato per determinare l'icona meteo corrispondente.
        /// </summary>
        public int WeatherCode { get; set; } // Per le icone

        /// <summary>
        /// Orario dell'alba in formato HH:MM.
        /// Esempio: "06:30"
        /// </summary>
        public string Sunrise { get; set; } = string.Empty;

        /// <summary>
        /// Orario del tramonto in formato HH:MM.
        /// Esempio: "20:45"
        /// </summary>
        public string Sunset { get; set; } = string.Empty;

        /// <summary>
        /// Totale delle precipitazioni previste per il giorno in millimetri.
        /// Include pioggia, neve e pioggia gelata.
        /// </summary>
        public double PrecipitationSum { get; set; }

        /// <summary>
        /// Massima probabilità di precipitazioni durante il giorno in percentuale (0-100%).
        /// Può essere null se i dati non sono disponibili.
        /// </summary>
        public int? PrecipitationProbabilityMax { get; set; }

        /// <summary>
        /// Velocità massima del vento prevista durante il giorno in km/h.
        /// </summary>
        public double WindSpeedMax { get; set; }

        /// <summary>
        /// Descrizione testuale delle condizioni meteorologiche previste.
        /// Esempio: "Pioggia moderata", "Cielo sereno", "Nubi sparse"
        /// </summary>
        public string? WeatherDescription { get; set; }
    }
}

namespace WeatherApp.Dtos.Weather
{
    /// <summary>
    /// DTO (Data Transfer Object) che rappresenta le condizioni meteo attuali per una località.
    /// Contiene le informazioni meteorologiche aggiornate al momento della richiesta.
    /// </summary>
    public class CurrentConditionsDto
    {
        /// <summary>
        /// Data e ora della misurazione in formato ISO 8601.
        /// Esempio: "2023-05-13T14:30:00"
        /// </summary>
        public string Time { get; set; } = string.Empty;

        /// <summary>
        /// Temperatura attuale a 2 metri dal suolo in gradi Celsius.
        /// </summary>
        public double Temperature { get; set; }

        /// <summary>
        /// Temperatura percepita a 2 metri dal suolo in gradi Celsius.
        /// Tiene conto di umidità e vento per indicare come la temperatura viene percepita.
        /// </summary>
        public double ApparentTemperature { get; set; }

        /// <summary>
        /// Umidità relativa in percentuale (0-100%).
        /// Indica la quantità di vapore acqueo presente nell'aria.
        /// </summary>
        public int RelativeHumidity { get; set; }

        /// <summary>
        /// Precipitazioni totali (pioggia, neve, pioggia gelata) nelle ultime ore in millimetri.
        /// </summary>
        public double Precipitation { get; set; }

        /// <summary>
        /// Codice numerico che descrive le condizioni meteorologiche attuali.
        /// Viene utilizzato per determinare l'icona meteo corrispondente.
        /// </summary>
        public int WeatherCode { get; set; }

        /// <summary>
        /// Velocità del vento a 10 metri dal suolo in km/h.
        /// </summary>
        public double WindSpeed { get; set; }

        /// <summary>
        /// Descrizione testuale delle condizioni meteorologiche attuali.
        /// Esempio: "Pioggia leggera", "Cielo sereno", "Nuvole sparse"
        /// </summary>
        public string? WeatherDescription { get; set; }
    }
}

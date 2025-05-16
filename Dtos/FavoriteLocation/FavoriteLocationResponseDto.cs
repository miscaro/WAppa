using WeatherApp.Dtos.Weather;

namespace WeatherApp.Dtos.FavoriteLocation
{
    /// <summary>
    /// DTO (Data Transfer Object) per la risposta delle località preferite.
    /// Contiene le informazioni complete di una località preferita, inclusi i dati meteo aggiornati.
    /// </summary>
    public class FavoriteLocationResponseDto
    {

        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        /// <summary>
        /// Dati meteo aggiornati per questa località.
        /// Può essere null se i dati meteo non sono disponibili o non sono stati richiesti.
        /// </summary>
        public WeatherForecastDto? WeatherData { get; set; }
    }
}

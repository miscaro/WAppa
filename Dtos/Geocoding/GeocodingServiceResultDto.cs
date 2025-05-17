namespace WeatherApp.Dtos.Geocoding
{
    /// <summary>
    /// DTO (Data Transfer Object) che rappresenta il risultato di un'operazione di geocoding.
    /// Contiene le informazioni geografiche di una località.
    /// </summary>
    public class GeocodingServiceResultDto
    {
        public string Name { get; set; } = string.Empty;

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Nome della regione o dello stato in cui si trova la località.
        /// Può essere null se l'informazione non è disponibile.
        /// Esempio: "Lazio", "Piemonte", "Lombardy"
        /// </summary>
        public string? Admin1 { get; set; }
    }
}

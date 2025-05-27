using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeatherApp.Models
{
    /// <summary>
    /// Rappresenta una località preferita salvata da un utente.
    /// Mappato alla tabella FavoriteLocations nel database.
    /// </summary>
    public class FavoriteLocation
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il nome della località è obbligatorio")]
        [StringLength(200, ErrorMessage = "Il nome della località non può superare i 200 caratteri")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La latitudine è obbligatoria")]
        [Range(-90.0, 90.0, ErrorMessage = "La latitudine deve essere compresa tra -90 e 90 gradi")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "La longitudine è obbligatoria")]
        [Range(-180.0, 180.0, ErrorMessage = "La longitudine deve essere compresa tra -180 e 180 gradi")]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "L'ID utente è obbligatorio")]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}
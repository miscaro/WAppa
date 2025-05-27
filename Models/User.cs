using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Models
{
    /// <summary>
    /// Rappresenta un utente registrato nell'applicazione.
    /// Mappato alla tabella Users nel database.
    /// </summary>
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Il nome utente è obbligatorio")]
        [StringLength(100, ErrorMessage = "Il nome utente non può superare i 100 caratteri")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria")]
        public string PasswordHash { get; set; } = string.Empty;

        public virtual ICollection<FavoriteLocation> FavoriteLocations { get; set; } = new List<FavoriteLocation>();
    }
}
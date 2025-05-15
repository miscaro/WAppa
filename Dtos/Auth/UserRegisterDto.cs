using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Dtos.Auth
{
    /// <summary>
    /// DTO (Data Transfer Object) per la registrazione di un nuovo utente.
    /// Contiene i dati necessari per creare un nuovo account utente nel sistema.
    /// </summary>
    public class UserRegisterDto
    {
        [Required(ErrorMessage = "Il nome utente è obbligatorio")]
        [StringLength(100, MinimumLength = 3, 
            ErrorMessage = "Il nome utente deve essere compreso tra 3 e 100 caratteri.")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password scelta dall'utente.
        /// Verrà crittografata prima di essere salvata nel database.
        /// </summary>
        [Required(ErrorMessage = "La password è obbligatoria")]
        [StringLength(100, MinimumLength = 6, 
            ErrorMessage = "La password deve contenere almeno 6 caratteri.")]
        public string Password { get; set; } = string.Empty;
    }
}

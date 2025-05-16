using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Dtos.Auth
{
    /// <summary>
    /// DTO (Data Transfer Object) per la richiesta di autenticazione di un utente.
    /// Contiene le credenziali necessarie per effettuare l'accesso al sistema.
    /// </summary>
    public class UserLoginDto
    {
        [Required(ErrorMessage = "Il nome utente è obbligatorio")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password in formato testo in chiaro.
        /// Verrà confrontata con l'hash memorizzato nel database durante l'autenticazione.
        /// </summary>
        [Required(ErrorMessage = "La password è obbligatoria")]
        public string Password { get; set; } = string.Empty;
    }
}

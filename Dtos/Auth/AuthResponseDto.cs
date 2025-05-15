namespace WeatherApp.Dtos.Auth
{
    /// <summary>
    /// DTO (Data Transfer Object) per la risposta di autenticazione.
    /// Contiene il token JWT e le informazioni dell'utente autenticato.
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// Token JWT generato per l'utente autenticato.
        /// Deve essere incluso nell'header Authorization di tutte le richieste successive.
        /// </summary>
        public string Token { get; set; } = string.Empty;
        
        public string Username { get; set; } = string.Empty;
        
    }
}

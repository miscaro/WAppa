using System.Threading.Tasks;
using WeatherApp.Dtos.Auth;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    /// <summary>
    /// Interfaccia che definisce il contratto per il servizio di autenticazione.
    /// Fornisce metodi per la registrazione e il login degli utenti.
    /// </summary>
    public interface IAuthService
    {
        /// <summary>
        /// Registra un nuovo utente nel sistema.
        /// </summary>
        /// <param name="user">L'oggetto utente da registrare.</param>
        /// <param name="password">La password in chiaro dell'utente.</param>
        /// <returns>Un oggetto <see cref="ServiceResponse{int}"/> contenente l'ID dell'utente registrato in caso di successo.</returns>
        Task<ServiceResponse<int>> Register(User user, string password);

        /// <summary>
        /// Esegue il login di un utente con username e password.
        /// </summary>
        /// <param name="username">Lo username dell'utente.</param>
        /// <param name="password">La password dell'utente.</param>
        /// <returns>Un oggetto <see cref="ServiceResponse{AuthResponseDto}"/> contenente il token JWT e i dettagli dell'utente in caso di successo.</returns>
        Task<ServiceResponse<AuthResponseDto>> Login(string username, string password);
    }
}

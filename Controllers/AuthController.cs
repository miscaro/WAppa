using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WeatherApp.Dtos.Auth;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.Controllers
{
    /// <summary>
    /// Controller per la gestione delle operazioni di autenticazione e registrazione utenti.
    /// Espone endpoint per il login e la registrazione degli utenti.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]  // La route di base sarà /api/Auth
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        /// <summary>
        /// Costruttore del controller con iniezione delle dipendenze.
        /// </summary>
        /// <param name="authService">Servizio per la gestione della logica di autenticazione</param>
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Endpoint per la registrazione di un nuovo utente.
        /// </summary>
        /// <param name="request">Dati di registrazione dell'utente</param>
        /// <returns>Risposta con il risultato dell'operazione</returns>
        /// <response code="200">Registrazione avvenuta con successo</response>
        /// <response code="400">Richiesta non valida o dati mancanti</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register(UserRegisterDto request)
        {
            var user = new User { Username = request.Username };
            var response = await _authService.Register(user, request.Password);

            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }

        /// <summary>
        /// Endpoint per l'accesso di un utente registrato.
        /// </summary>
        /// <param name="request">Credenziali di accesso</param>
        /// <returns>Token JWT e informazioni utente in caso di successo</returns>
        /// <response code="200">Login avvenuto con successo</response>
        /// <response code="401">Credenziali non valide</response>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            var response = await _authService.Login(request.Username, request.Password);

            if (!response.Success)
            {
                return Unauthorized(response);
            }
            return Ok(response);
        }
    }
}

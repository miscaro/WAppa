using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WeatherApp.Data;
using WeatherApp.Dtos.Auth;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    /// <summary>
    /// Servizio per la gestione dell'autenticazione e dell'autorizzazione degli utenti.
    /// Fornisce metodi per il login, la registrazione e la generazione di token JWT.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="AuthService"/>
        /// </summary>
        /// <param name="context">Il contesto del database.</param>
        /// <param name="configuration">La configurazione dell'applicazione.</param>
        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Esegue il login di un utente con username e password.
        /// </summary>
        /// <param name="username">Lo username dell'utente.</param>
        /// <param name="password">La password dell'utente.</param>
        /// <returns>Un oggetto <see cref="ServiceResponse{AuthResponseDto}"/> contenente il token JWT e i dettagli dell'utente in caso di successo.</returns>
        public async Task<ServiceResponse<AuthResponseDto>> Login(string username, string password)
        {
            var response = new ServiceResponse<AuthResponseDto>();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());

            if (user == null)
            {
                response.Success = false;
                response.Message = "Utente non trovato.";
                return response;
            }
            // Verifica la password (BCrypt.Verify)
            else if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                response.Success = false;
                response.Message = "Password non corretta.";
                return response;
            }
            else
            {
                // Password corretta, crea il token
                response.Data = new AuthResponseDto
                {
                    Username = user.Username,
                    Token = CreateToken(user)
                };
                response.Message = "Accesso effettuato con successo!";
            }
            return response;
        }

        /// <summary>
        /// Registra un nuovo utente nel sistema.
        /// </summary>
        /// <param name="user">L'oggetto utente da registrare.</param>
        /// <param name="password">La password in chiaro dell'utente.</param>
        /// <returns>Un oggetto <see cref="ServiceResponse{int}"/> contenente l'ID dell'utente registrato in caso di successo.</returns>
        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            var response = new ServiceResponse<int>();
            if (await UserExists(user.Username))
            {
                response.Success = false;
                response.Message = "Username già esistente.";
                return response;
            }

            // Crea l'hash della password
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            user.PasswordHash = passwordHash;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            response.Data = user.Id; // Restituisce l'ID del nuovo utente
            response.Message = "Utente registrato con successo!";
            return response;
        }

        /// <summary>
        /// Verifica se un utente con lo specifico username esiste già nel database.
        /// </summary>
        /// <param name="username">Lo username da verificare.</param>
        /// <returns>True se l'utente esiste, altrimenti false.</returns>
        private async Task<bool> UserExists(string username)
        {
            if (await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Crea un token JWT per l'utente specificato.
        /// </summary>
        /// <param name="user">L'utente per cui generare il token.</param>
        /// <returns>Una stringa che rappresenta il token JWT.</returns>
        /// <exception cref="Exception">Viene lanciata quando la chiave segreta non è configurata in appsettings.json</exception>
        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            // Prende la chiave segreta da appsettings.json
            var appSettingsToken = _configuration.GetSection("AppSettings:Token").Value;
            if (string.IsNullOrEmpty(appSettingsToken))
                throw new Exception("AppSettings:Token non configurato in appsettings.json");

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettingsToken));
            SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1), // Token valido per 1 giorno
                SigningCredentials = creds
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token); // Il token JWT come stringa
        }
    }
}

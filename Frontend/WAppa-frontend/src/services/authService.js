import axios from 'axios';

// URL base per le API di autenticazione, costruito a partire dalla variabile d'ambiente VITE_API_BASE_URL
// Esempio: se VITE_API_BASE_URL=https://localhost:7123/api, allora AUTH_API_URL diventa https://localhost:7123/api/auth
const AUTH_API_URL = `${import.meta.env.VITE_API_BASE_URL}/auth`;

/**
 * Effettua il login dell'utente con le credenziali fornite.
 * @param {Object} credentials - Le credenziali di accesso.
 * @param {string} credentials.username - Nome utente o email.
 * @param {string} credentials.password - Password dell'utente.
 * @returns {Promise<object>} La risposta dal backend contenente il token JWT e i dati utente.
 * @throws {Error} Se le credenziali sono errate o si verifica un errore durante la richiesta.
 * @example
 * try {
 *   const result = await loginUser({ username: 'utente', password: 'password' });
 *   console.log(result.data); // Dati dell'utente e token JWT
 * } catch (error) {
 *   console.error("Errore di accesso:", error.message);
 * }
 */
export const loginUser = async (credentials) => {
  try {
    const response = await axios.post(`${AUTH_API_URL}/login`, credentials);
    // Il backend restituisce una ServiceResponse<AuthResponseDto>
    return response.data;
  } catch (error) {
    console.error('Login API error:', error.response || error.request || error.message);
    if (error.response && error.response.data) {
      throw error.response.data;
    }
    throw new Error(error.message || 'Si è verificato un errore durante il login.');
  }
};

/**
 * Registra un nuovo utente nel sistema.
 * @param {Object} userData - Dati per la registrazione del nuovo utente.
 * @param {string} userData.username - Nome utente univoco.
 * @param {string} userData.email - Email dell'utente.
 * @param {string} userData.password - Password per l'account.
 * @returns {Promise<object>} La risposta dal backend con l'ID dell'utente creato.
 * @throws {Error} Se la registrazione fallisce o i dati non sono validi.
 * @example
 * try {
 *   const result = await registerUser({ 
 *     username: 'nuovoutente',
 *     email: 'email@esempio.com',
 *     password: 'passwordSicura123' 
 *   });
 *   console.log("Registrazione completata per l'ID:", result.data);
 * } catch (error) {
 *   console.error("Errore durante la registrazione:", error.message);
 * }
 */
export const registerUser = async (userData) => {
  try {
    const response = await axios.post(`${AUTH_API_URL}/register`, userData);
    // Il backend restituisce una ServiceResponse<int> con l'ID dell'utente creato
    return response.data;
  } catch (error) {
    console.error('Register API error:', error.response || error.request || error.message);
    if (error.response && error.response.data) {
      throw error.response.data;
    }
    throw new Error(error.message || 'Si è verificato un errore durante la registrazione.');
  }
};

// --- Funzioni di utilità per la gestione del token JWT ---

// Chiave utilizzata per memorizzare il token nel localStorage
const TOKEN_KEY = 'weatherAppToken';

/**
 * Salva il token JWT nel localStorage.
 * @param {string} token - Il token JWT da salvare.
 * @returns {void}
 */
export const saveToken = (token) => {
  if (token) {
    localStorage.setItem(TOKEN_KEY, token);
  } else {
    localStorage.removeItem(TOKEN_KEY); // Rimuove il token se è nullo/undefined
  }
};

/**
 * Recupera il token JWT dal localStorage.
 * @returns {string|null} Il token JWT se presente, altrimenti null.
 */
export const getToken = () => {
  return localStorage.getItem(TOKEN_KEY);
};

/**
 * Rimuove il token JWT dal localStorage.
 * Utilizzato durante il logout.
 * @returns {void}
 */
export const removeToken = () => {
  localStorage.removeItem(TOKEN_KEY);
};

/**
 * Configura l'header di autorizzazione per le richieste Axios.
 * Se è presente un token valido, lo imposta nell'header 'Authorization'.
 * @returns {void}
 */
export const setAuthHeader = () => {
  const token = getToken();
  if (token) {
    // Imposta l'header di autorizzazione per tutte le richieste future
    axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  } else {
    // Rimuove l'header di autorizzazione se non c'è token
    delete axios.defaults.headers.common['Authorization'];
  }
};
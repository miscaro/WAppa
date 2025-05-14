import axios from 'axios';

// URL base per le API delle località preferite, costruito a partire dalla variabile d'ambiente VITE_API_BASE_URL
// Esempio: se VITE_API_BASE_URL=https://localhost:7123/api, allora FAVORITES_API_URL diventa https://localhost:7123/api/favoritelocations
const FAVORITES_API_URL = `${import.meta.env.VITE_API_BASE_URL}/favoritelocations`;

/**
 * Recupera tutte le località preferite per l'utente attualmente autenticato.
 * Richiede un token JWT valido nell'header della richiesta.
 * @returns {Promise<object>} La risposta dal backend, che contiene:
 *                            { 
 *                              data: Array<FavoriteLocationResponseDto>, 
 *                              success: boolean, 
 *                              message: string 
 *                            }
 * @throws {Error} Se la chiamata API fallisce o l'utente non è autenticato.
 * @example
 * try {
 *   const result = await getFavoriteLocations();
 *   console.log(result.data); // Array di località preferite
 * } catch (error) {
 *   console.error("Errore:", error.message);
 * }
 */
export const getFavoriteLocations = async () => {
  try {
    const response = await axios.get(FAVORITES_API_URL);
    // Il backend restituisce ServiceResponse<List<FavoriteLocationResponseDto>>
    return response.data;
  } catch (error) {
    console.error('Get Favorite Locations API error:', error.response || error.request || error.message);
    if (error.response && error.response.data) {
      throw error.response.data;
    }
    throw new Error(error.message || 'An unknown error occurred while fetching favorite locations.');
  }
};

/**
 * Aggiunge una nuova località preferita per l'utente attualmente autenticato.
 * @param {string} locationName - Il nome della località da aggiungere (es. "Roma", "New York").
 * @returns {Promise<object>} La risposta dal backend, che contiene:
 *                            { 
 *                              data: FavoriteLocationResponseDto, 
 *                              success: boolean, 
 *                              message: string 
 *                            }
 * @throws {Error} Se la chiamata API fallisce o la località non è valida.
 * @example
 * try {
 *   const result = await addFavoriteLocation("Roma");
 *   console.log(result.data); // Dettagli della località aggiunta
 * } catch (error) {
 *   console.error("Errore:", error.message);
 * }
 */
export const addFavoriteLocation = async (locationName) => {
  try {
    // Crea il payload per la richiesta POST
    const payload = { locationName };
    const response = await axios.post(FAVORITES_API_URL, payload);
    // Il backend restituisce ServiceResponse<FavoriteLocationResponseDto>
    return response.data;
  } catch (error) {
    console.error('Add Favorite Location API error:', error.response || error.request || error.message);
    if (error.response && error.response.data) {
      throw error.response.data;
    }
    throw new Error(error.message || 'An unknown error occurred while adding favorite location.');
  }
};

/**
 * Rimuove una località preferita per l'utente attualmente autenticato.
 * @param {number} locationId - L'ID univoco della località da rimuovere.
 * @returns {Promise<object>} La risposta dal backend, che contiene:
 *                            { 
 *                              data: string | null, 
 *                              success: boolean, 
 *                              message: string 
 *                            }
 * @throws {Error} Se la chiamata API fallisce o la località non esiste.
 * @example
 * try {
 *   const result = await deleteFavoriteLocation(123);
 *   console.log(result.message); // Messaggio di conferma
 * } catch (error) {
 *   console.error("Errore:", error.message);
 * }
 */
export const deleteFavoriteLocation = async (locationId) => {
  try {
    // Effettua una richiesta DELETE all'endpoint specifico per la località
    const response = await axios.delete(`${FAVORITES_API_URL}/${locationId}`);
    // Il backend restituisce ServiceResponse<string>
    return response.data;
  } catch (error) {
    console.error('Delete Favorite Location API error:', error.response || error.request || error.message);
    if (error.response && error.response.data) {
      throw error.response.data;
    }
    throw new Error(error.message || 'An unknown error occurred while deleting favorite location.');
  }
};
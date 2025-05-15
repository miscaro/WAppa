import axios from 'axios';

// URL base per le API meteo, costruito a partire dalla variabile d'ambiente VITE_API_BASE_URL
// Esempio: se VITE_API_BASE_URL=https://localhost:7123/api, allora WEATHER_API_URL diventa https://localhost:7123/api/weather
const WEATHER_API_URL = `${import.meta.env.VITE_API_BASE_URL}/weather`;

/**
 * Cerca i dati meteo per una località specificata da una query (nome città, CAP).
 * @param {string} query - La stringa di ricerca per la località (es. "Roma" o "00100").
 * @returns {Promise<object>} La risposta dal backend, che dovrebbe contenere
 *                            { data: WeatherForecastDto, success: boolean, message: string }
 * @throws {Error} Se la chiamata API fallisce o il backend restituisce un errore.
 * @example
 * try {
 *   const result = await searchWeatherByQuery("Roma");
 *   console.log(result.data); // Dati meteo per Roma
 * } catch (error) {
 *   console.error("Errore:", error.message);
 * }
 */
export const searchWeatherByQuery = async (query) => {
  try {
    // Effettua una richiesta GET all'endpoint /weather con il parametro di query
    const response = await axios.get(WEATHER_API_URL, {
      params: { query } 
    });
    // Il backend restituisce ServiceResponse<WeatherForecastDto>
    return response.data;
  } catch (error) {
    console.error('Search Weather by Query API error:', error.response || error.request || error.message);
    if (error.response && error.response.data) {
      throw error.response.data; // Rilancia l'oggetto di errore del backend
    }
    throw new Error(error.message || 'An unknown error occurred while searching weather by query.');
  }
};

/**
 * Recupera i dati meteo per coordinate geografiche specifiche.
 * @param {number} latitude - La latitudine della posizione.
 * @param {number} longitude - La longitudine della posizione.
 * @returns {Promise<object>} La risposta dal backend contenente i dati meteo.
 * @throws {Error} Se la chiamata API fallisce o le coordinate non sono valide.
 * @example
 * try {
 *   const result = await getWeatherByCoordinates(41.9028, 12.4964);
 *   console.log(result.data); // Dati meteo per le coordinate specificate
 * } catch (error) {
 *   console.error("Errore:", error.message);
 * }
 */
export const getWeatherByCoordinates = async (latitude, longitude) => {
  try {
    // Effettua una richiesta GET all'endpoint /weather con i parametri di latitudine e longitudine
    const response = await axios.get(WEATHER_API_URL, {
      params: { latitude, longitude }
    });
    return response.data;
  } catch (error) {
    console.error('Get Weather by Coordinates API error:', error.response || error.request || error.message);
    if (error.response && error.response.data) {
      throw error.response.data;
    }
    throw new Error(error.message || 'An unknown error occurred while fetching weather by coordinates.');
  }
};
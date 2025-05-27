import React, { useState } from 'react';
import { searchWeatherByQuery } from '../services/weatherService';

/**
 * Pagina principale dell'applicazione
 * Mostra un form di ricerca e i risultati meteo per la località cercata
 */
function HomePage() {
  // Stati per gestire la ricerca, i risultati e lo stato di caricamento
  const [searchQuery, setSearchQuery] = useState('');
  const [weatherResult, setWeatherResult] = useState(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');

  // Gestisce la ricerca del meteo per la località inserita
  const handleSearch = async (event) => {
    event.preventDefault();
    // Verifica che sia stato inserito un termine di ricerca
    if (!searchQuery.trim()) {
      setError('Please enter a city name or ZIP code to search.');
      setWeatherResult(null);
      return;
    }
    
    setIsLoading(true);
    setError('');
    setWeatherResult(null);

    try {
      // Chiamata al servizio meteo
      const response = await searchWeatherByQuery(searchQuery);
      if (response.success && response.data) {
        console.log("Weather search result on HomePage:", response.data);
        setWeatherResult(response.data);
      } else {
        setError(response.message || 'Could not find weather for the specified location.');
      }
    } catch (err) {
      setError(err.message || 'An error occurred during the weather search.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="homepage-container"> 
      <h2>Benvenuto su WAppa!</h2>
      <p>L'app meteo più wappa che ci sia!</p>

      {/* Form di ricerca meteo */}
      <form onSubmit={handleSearch} className="weather-search-form"> 
        <div>
            <input
              type="text"
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              placeholder="Inserisci città o CAP"
              disabled={isLoading}
            />
        </div>
        <button type="submit" disabled={isLoading}>
          {isLoading ? 'Ricerca in corso...' : 'Cerca'}
        </button>
      </form>

      {error && <p className="error-message">{error}</p>}

      {isLoading && <div className="loading-message" style={{ marginTop: '20px' }}>Caricamento dati meteo...</div>}

      {/* Sezione risultati meteo */}
      {weatherResult && (
        <div className="card weather-result-card"> 
          <h3>Previsioni meteo per {weatherResult.locationName || searchQuery}</h3> 
          <p>Lat: {weatherResult.latitude.toFixed(2)}, Lon: {weatherResult.longitude.toFixed(2)}</p>

          {/* Condizioni meteo attuali */}
          {weatherResult.current ? (
            <div className="weather-info-section">
              <strong>Condizioni attuali:</strong>
              <p>Temp: {weatherResult.current.temperature}°C (Percepita: {weatherResult.current.apparentTemperature}°C)</p>
              <p>Condizioni: {weatherResult.current.weatherDescription} (Codice: {weatherResult.current.weatherCode})</p>
              <p>Umidità: {weatherResult.current.relativeHumidity}%</p>
              <p>Vento: {weatherResult.current.windSpeed} km/h</p>
            </div>
          ) : (
              <p className="italic-message">Condizioni meteo attuali non disponibili.</p>
          )}

          {/* Previsioni giornaliere */}
          {weatherResult.daily && weatherResult.daily.length > 0 ? (
            <div className="weather-info-section">
              <strong>Previsioni giornaliere:</strong>
              {weatherResult.daily.slice(0, 3).map((day, index) => (
                <div key={index} style={{ marginTop: '5px', paddingTop: '5px', borderTop: index > 0 ? '1px dashed #f0f0f0' : 'none'}}>
                  <p>
                    <strong>{new Date(day.date).toLocaleDateString('it-IT', { weekday: 'short', day: 'numeric', month: 'short' })}:</strong> 
                    Max {day.temperatureMax}°C, Min {day.temperatureMin}°C - {day.weatherDescription}
                  </p>
                </div>
              ))}
            </div>
          ) : (
            <p className="italic-message">Previsioni giornaliere non disponibili.</p>
          )}
        </div>
      )}
    </div>
  );
}

export default HomePage;
import React, { useState, useEffect, useCallback } from 'react';
import { useAuth } from '../contexts/AuthContext';
import { 
  getFavoriteLocations, 
  addFavoriteLocation, 
  deleteFavoriteLocation 
} from '../services/favoriteLocationService';

/**
 * Pagina Dashboard dell'utente autenticato
 * Mostra le località preferite dell'utente e permette di gestirle
 */
function DashboardPage() {
  // Stati per gestire le località preferite, il caricamento e gli errori
  const { currentUser } = useAuth();
  const [favorites, setFavorites] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [newLocationName, setNewLocationName] = useState('');
  const [isAdding, setIsAdding] = useState(false);

  // Funzione per recuperare le località preferite dell'utente
  const fetchFavorites = useCallback(async () => {
    setIsLoading(true);
    setError('');
    try {
      const response = await getFavoriteLocations();
      if (response.success) {
        setFavorites(response.data || []);
      } else {
        setError(response.message || 'Failed to load favorites.');
        setFavorites([]);
      }
    } catch (err) {
      setError(err.message || 'An error occurred while fetching favorites.');
      setFavorites([]);
    } finally {
      setIsLoading(false);
    }
  }, []);

  // Carica le località preferite al montaggio del componente
  useEffect(() => {
    fetchFavorites();
  }, [fetchFavorites]);

  // Gestisce l'aggiunta di una nuova località preferita
  const handleAddLocation = async (event) => {
    event.preventDefault();
    if (!newLocationName.trim()) {
      setError('Please enter a location name.');
      return;
    }
    setIsAdding(true);
    setError('');
    try {
      const response = await addFavoriteLocation(newLocationName);
      if (response.success) {
        fetchFavorites(); // Ricarica l'elenco aggiornato
        setNewLocationName('');
      } else {
        setError(response.message || 'Failed to add location.');
      }
    } catch (err) {
      setError(err.message || 'An error occurred while adding location.');
    } finally {
      setIsAdding(false);
    }
  };

  // Gestisce l'eliminazione di una località preferita
  const handleDeleteLocation = async (locationId) => {
    setError(''); 
    try {
      const response = await deleteFavoriteLocation(locationId);
      if (response.success) {
        fetchFavorites();
      } else {
        setError(response.message || 'Failed to delete location.');
      }
    } catch (err) {
      setError(err.message || 'An error occurred while deleting location.');
    }
  };

  // Mostra un messaggio di caricamento durante il recupero dei dati
  if (isLoading) {
    return <div className="loading-message">Loading your favorite locations...</div>;
  }

  return (
    <div className="dashboard-page-container">
      <h2>My Dashboard</h2>
      {currentUser && <p>Welcome back, {currentUser.username}!</p>}

      {/* Form per aggiungere una nuova località preferita */}
      <h3>Add New Favorite Location</h3>
      <form onSubmit={handleAddLocation}>
        <div>
          <input
            type="text"
            value={newLocationName}
            onChange={(e) => setNewLocationName(e.target.value)}
            placeholder="Inserisci città o CAP"
            disabled={isAdding}
          />
        </div>
        <button type="submit" disabled={isAdding}>
          {isAdding ? 'Aggiunta in corso...' : 'Aggiungi posizione'}
        </button>
      </form>

      {error && <p className="error-message">{error}</p>}

      {/* Lista delle località preferite */}
      <h3>Le tue posizioni preferite</h3>
      {favorites.length === 0 && !isLoading && (
        <p>Non hai ancora posizioni preferite. Aggiungi una sopra!</p>
      )}
      
      <div className="weather-card-container">
        {favorites.map((fav) => (
          <div key={fav.id} className="card">
            <h4>{fav.name}</h4>
            <p>Lat: {fav.latitude.toFixed(2)}, Lon: {fav.longitude.toFixed(2)}</p>
            
            {/* Sezione condizioni meteo attuali */}
            {fav.weatherData ? (
              <>
                {fav.weatherData.current ? (
                  <div className="weather-info-section">
                    <strong>Condizioni attuali:</strong>
                    <p>Temp: {fav.weatherData.current.temperature}°C (Percepita: {fav.weatherData.current.apparentTemperature}°C)</p>
                    <p>Condizioni: {fav.weatherData.current.weatherDescription} (Codice: {fav.weatherData.current.weatherCode})</p>
                    <p>Umidità: {fav.weatherData.current.relativeHumidity}%</p>
                    <p>Vento: {fav.weatherData.current.windSpeed} km/h</p>
                  </div>
                ) : (
                  <p className="italic-message">Condizioni meteo attuali non disponibili.</p>
                )}

                {/* Sezione previsioni giornaliere */}
                {fav.weatherData.daily && fav.weatherData.daily.length > 0 ? (
                  <div className="weather-info-section">
                    <strong>Previsioni giornaliere:</strong>
                    {fav.weatherData.daily.slice(0, 3).map((day, index) => (
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
              </>
            ) : (
              <p className="italic-message">Dati meteo non disponibili per questa posizione.</p>
            )}

            {/* Pulsante per rimuovere la località preferita */}
            <button 
              onClick={() => handleDeleteLocation(fav.id)} 
              className="delete-button"
            >
              Delete
            </button>
          </div>
        ))}
      </div>
    </div>
  );
}

export default DashboardPage;

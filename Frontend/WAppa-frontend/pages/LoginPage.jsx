import React, { useState, useEffect } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { loginUser } from '../services/authService';
import { useAuth } from '../contexts/AuthContext';

/**
 * Pagina di accesso all'applicazione
 * Gestisce l'autenticazione degli utenti esistenti
 */
function LoginPage() {
  // Stati per gestire il form e lo stato di caricamento
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  
  const navigate = useNavigate();
  const location = useLocation();
  const { login: contextLogin, isAuthenticated } = useAuth();

  // Reindirizza l'utente alla pagina precedente dopo il login, o alla dashboard di default
  const from = location.state?.from?.pathname || '/dashboard';

  // Se l'utente è già autenticato, reindirizza alla pagina di destinazione
  useEffect(() => {
    if (isAuthenticated()) {
      navigate(from, { replace: true });
    }
  }, [isAuthenticated, navigate, from]);

  // Gestisce il submit del form di login
  const handleSubmit = async (event) => {
    event.preventDefault();
    setError('');
    setLoading(true);

    // Validazione di base dei campi
    if (!username || !password) {
      setError('Tutti i campi sono obbligatori.');
      setLoading(false);
      return;
    }

    try {
      // Chiamata al servizio di autenticazione
      const response = await loginUser({ username, password });       
      if (response.success && response.data?.token) {
        // Aggiorna il contesto di autenticazione con il token ricevuto
        contextLogin(response.data.token);
        // Reindirizza alla pagina di destinazione
        navigate(from, { replace: true });
      } else {
        setError(response.message || 'Accesso fallito per un problema imprevisto.');
      }
    } catch (err) {
      setError(err.message || 'Accesso fallito. Controlla le tue credenziali o la connessione.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div> 
      <h2>Accesso</h2>
      <form onSubmit={handleSubmit}>
        <div>
          <label htmlFor="username">Nome utente:</label>
          <input
            type="text"
            id="username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            disabled={loading}
            required
            autoComplete="username"
          />
        </div>
        <div>
          <label htmlFor="password">Password:</label>
          <input
            type="password"
            id="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            disabled={loading}
            required
            autoComplete="current-password"
          />
        </div>
        {error && <p className="error-message">{error}</p>} 
        <button type="submit" disabled={loading}>
          {loading ? 'Accesso in corso...' : 'Accesso'}
        </button>
      </form>
      <p style={{ marginTop: '20px' }}>
        Non hai un account? <Link to="/register" style={{ color: '#3498db', fontWeight: 'bold' }}>Registrati qui</Link>
      </p>
    </div>
  );
}

export default LoginPage;

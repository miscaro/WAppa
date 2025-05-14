import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { registerUser } from '../services/authService'; 

/**
 * Pagina di registrazione per nuovi utenti
 * Gestisce la creazione di un nuovo account utente
 */
function RegisterPage() {
  // Stati per gestire il form, gli errori e lo stato di caricamento
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();

  // Gestisce il submit del form di registrazione
  const handleSubmit = async (event) => {
    event.preventDefault();
    setError('');
    setSuccessMessage('');
    setLoading(true);

    // Validazione dei campi obbligatori
    if (!username || !password || !confirmPassword) {
      setError('Tutti i campi sono obbligatori.');
      setLoading(false);
      return;
    }

    // Validazione della lunghezza minima della password
    if (password.length < 6) {
        setError('La password deve contenere almeno 6 caratteri.');
        setLoading(false);
        return;
    }

    // Verifica che le password coincidano
    if (password !== confirmPassword) {
      setError('Le password non coincidono.');
      setLoading(false);
      return;
    }
    
    try {
      // Chiamata al servizio di registrazione
      const response = await registerUser({ username, password });

      if (response.success) {
        console.log('Registration successful. User ID:', response.data);
        // Mostra il messaggio di successo e reindirizza al login dopo 3 secondi
        setSuccessMessage(response.message + ' Ora puoi accedere.');
        setUsername('');
        setPassword('');
        setConfirmPassword('');
        setTimeout(() => {
            navigate('/login');
        }, 3000); 
      } else {
        setError(response.message || 'Registration failed due to an unexpected issue.');
      }
    } catch (err) {
      console.error('Registration error caught in component:', err);
      setError(err.message || 'Registration failed. Please try again or check network connection.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <h2>Registrazione</h2>
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
          />
        </div>
        <div>
          <label htmlFor="password">Password (min 6 caratteri):</label>
          <input
            type="password"
            id="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            disabled={loading}
            required
          />
        </div>
        <div>
          <label htmlFor="confirmPassword">Conferma Password:</label>
          <input
            type="password"
            id="confirmPassword"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            disabled={loading}
            required
          />
        </div>
        {error && <p style={{ color: 'red' }}>{error}</p>}
        {successMessage && <p style={{ color: 'green' }}>{successMessage}</p>}
        <button type="submit" disabled={loading}>
          {loading ? 'Registrazione in corso...' : 'Registrazione'}
        </button>
      </form>
      <p>
        Hai già un account? <Link to="/login">Accedi qui</Link>
      </p>
    </div>
  );
}

export default RegisterPage;

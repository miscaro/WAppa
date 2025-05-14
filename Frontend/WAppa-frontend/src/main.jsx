import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App.jsx';
import './index.css';
import { AuthProvider } from './contexts/AuthContext.jsx'; 

/**
 * Punto di ingresso principale dell'applicazione React
 * Avvia il rendering dell'app all'interno del div con id="root"
 * e fornisce il contesto di autenticazione a tutti i componenti figli
 */
ReactDOM.createRoot(document.getElementById('root')).render(
  <React.StrictMode>
    {/* AuthProvider fornisce il contesto di autenticazione a tutta l'app */}
    <AuthProvider> 
      <App />
    </AuthProvider>
  </React.StrictMode>
);

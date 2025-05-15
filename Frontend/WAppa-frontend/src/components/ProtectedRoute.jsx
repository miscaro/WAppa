import React from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';

/**
 * Componente per proteggere le rotte che richiedono autenticazione
 * Reindirizza gli utenti non autenticati alla pagina di login
 * @param {Object} props - Le proprietà del componente
 * @param {React.ReactNode} props.children - I componenti figli da proteggere
 * @returns {React.ReactNode} Il componente protetto o un reindirizzamento al login
 */
const ProtectedRoute = ({ children }) => {
  // Utilizza il contesto di autenticazione per verificare lo stato dell'utente
  const { isAuthenticated, loadingAuth } = useAuth();
  // Ottiene la posizione corrente per il reindirizzamento dopo il login
  const location = useLocation(); 

  // Mostra un caricamento durante la verifica dell'autenticazione
  if (loadingAuth) {
    return <div>Verifica autenticazione in corso...</div>;
  }

  // Se l'utente non è autenticato, reindirizza alla pagina di login
  // mantenendo la posizione di provenienza per il reindirizzamento post-login
  if (!isAuthenticated()) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  // Se l'utente è autenticato, mostra i componenti figli
  // Se viene utilizzato come wrapper di rotta, utilizza Outlet per i figli annidati
  return children || <Outlet />;
};

export default ProtectedRoute;
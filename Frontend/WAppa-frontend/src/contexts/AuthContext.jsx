import React, { createContext, useContext, useState, useEffect } from 'react';
import { getToken, saveToken, removeToken, setAuthHeader } from '../services/authService';
import { jwtDecode } from 'jwt-decode';

// Crea un contesto React per l'autenticazione
export const AuthContext = createContext(null);

/**
 * Provider del contesto di autenticazione
 * Gestisce lo stato di autenticazione dell'utente e fornisce metodi per login/logout
 */
export const AuthProvider = ({ children }) => {
  // Stati per gestire l'utente corrente, il token JWT e lo stato di caricamento
  const [currentUser, setCurrentUser] = useState(null);
  const [token, setTokenState] = useState(null); 
  const [loadingAuth, setLoadingAuth] = useState(true); 
  
  // Resetta lo stato di autenticazione
  const resetAuthState = () => {
    removeToken();
    setTokenState(null);
    setCurrentUser(null);
    setAuthHeader(); // Rimuove l'header di autorizzazione
  };

  // Effetto per l'inizializzazione dello stato di autenticazione al caricamento
  useEffect(() => {
    let isMounted = true;

    const initializeAuth = () => {
      console.log("AuthContext: Inizializzazione stato di autenticazione...");
      const existingToken = getToken();

      if (existingToken) {
        try {
          const decodedToken = jwtDecode(existingToken);
          const currentTime = Date.now() / 1000; // Tempo in secondi

          // Verifica se il token è scaduto
          if (decodedToken.exp < currentTime) {
            console.log("AuthContext: Token scaduto, rimozione...");
            if (isMounted) {
              resetAuthState();
            }
          } else {
            // Estrae username e ID utente dal token
            const usernameFromToken = decodedToken.unique_name || decodedToken.name;
            const userIdFromToken = decodedToken.nameid;

            // Log di warning per eventuali claim mancanti
            if (!usernameFromToken) {
                console.warn("AuthContext: Username claim (unique_name or name) non trovato nel token:", decodedToken);
            }
            if (!userIdFromToken) {
                console.warn("AuthContext: User ID claim (nameid) non trovato nel token:", decodedToken);
            }

            console.log(`AuthContext: Token valido. Username: ${usernameFromToken}, ID: ${userIdFromToken}`);
            
            if (isMounted) {
              setCurrentUser({ 
                username: usernameFromToken, 
                id: userIdFromToken 
              });
              setTokenState(existingToken);
              setAuthHeader(); // Imposta l'header di autorizzazione
            }
          }
        } catch (error) {
          console.error("AuthContext: Errore nel decodificare il token o token invalido:", error);
          if (isMounted) {
            resetAuthState();
          }
        }
      } else {
        console.log("AuthContext: Nessun token esistente trovato.");
      }

      if (isMounted) {
        setLoadingAuth(false);
        console.log("AuthContext: Inizializzazione autenticazione completata.");
      }
    };

    initializeAuth();

    // Pulizia all'unmount
    return () => {
      isMounted = false;
    };
  }, []);

  /**
   * Effettua il login dell'utente con il token JWT
   * @param {string} jwtToken - Token JWT ricevuto dal server
   */
  const login = (jwtToken) => {
    console.log("AuthContext: Login in corso...");
    saveToken(jwtToken); // Salva il token nello storage locale
    setAuthHeader();    // Imposta l'header di autorizzazione
    
    try {
      const decodedToken = jwtDecode(jwtToken);
      const usernameFromToken = decodedToken.unique_name || decodedToken.name;
      const userIdFromToken = decodedToken.nameid;

      // Log di warning per claim mancanti
      if (!usernameFromToken) {
          console.warn("AuthContext: Username claim (unique_name or name) non trovato nel token:", decodedToken);
      }
      if (!userIdFromToken) {
          console.warn("AuthContext: User ID claim (nameid) non trovato nel token:", decodedToken);
      }

      console.log(`AuthContext: Login effettuato. Username: ${usernameFromToken}, ID: ${userIdFromToken}`);
      
      // Aggiorna lo stato con i dati dell'utente
      setCurrentUser({ 
        username: usernameFromToken,
        id: userIdFromToken
      });
      setTokenState(jwtToken); 
    } catch (error) {
      console.error("AuthContext: Errore nel decodificare il token:", error);
      resetAuthState(); // In caso di errore, resetta lo stato
    }
  };

  // Effettua il logout dell'utente
  const logout = () => {
    console.log("AuthContext: Logout in corso...");
    resetAuthState();
  };

  // Verifica se l'utente è autenticato
  const isAuthenticated = () => {
    return !!token && !!currentUser && !!currentUser.username;
  };

  // Valore del contesto esposto ai componenti figli
  const value = {
    currentUser,
    token,
    isAuthenticated,
    login,
    logout,
    loadingAuth
  };

  // Mostra un caricamento durante l'inizializzazione
  if (loadingAuth) {
    return <div>Verifica stato di autenticazione...</div>;
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};

/**
 * Hook personalizzato per accedere al contesto di autenticazione
 * @returns {Object} Contesto di autenticazione
 * @throws {Error} Se usato al di fuori di un AuthProvider
 */
export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth deve essere usato all\'interno di un AuthProvider');
  }
  return context;
};

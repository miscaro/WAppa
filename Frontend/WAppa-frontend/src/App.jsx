import { BrowserRouter as Router, Routes, Route, Link, useNavigate } from 'react-router-dom';
import './App.css'; 
import { useAuth } from './contexts/AuthContext';
import HomePage from './pages/HomePage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';
import DashboardPage from './pages/DashboardPage';
import NotFoundPage from './pages/NotFoundPage';
import ProtectedRoute from './components/ProtectedRoute';

/**
 * Componente di navigazione principale dell'applicazione
 * Mostra la barra di navigazione in base allo stato di autenticazione
 */
function Navigation() {
  const { isAuthenticated, logout, currentUser } = useAuth();
  const navigate = useNavigate();

  // Gestisce il logout dell'utente
  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <nav> 
      <ul> 
        <li><Link to="/">Home</Link></li>
        {/* Mostra i link in base allo stato di autenticazione */}
        {isAuthenticated() ? (
          // Menu per utenti autenticati
          <>
            <li><Link to="/dashboard">Dashboard</Link></li>
            <li>
              <button onClick={handleLogout} className="nav-button">
                Logout ({currentUser?.username || 'User'})
              </button>
            </li>
          </>
        ) : (
          // Menu per utenti non autenticati
          <>
            <li><Link to="/login">Accesso</Link></li>
            <li><Link to="/register">Registrazione</Link></li>
          </>
        )}
      </ul>
    </nav>
  );
}

/**
 * Componente principale dell'applicazione
 * Definisce il routing e la struttura di base dell'app
 */
function App() {
  const { loadingAuth } = useAuth();

  // Mostra un caricamento durante la verifica dello stato di autenticazione
  if (loadingAuth) {
    return <div className="loading-app">Loading application...</div>; 
  }

  return (
    <Router>
      <div className="App"> 
        <Navigation />
        
        {/* Definizione delle rotte dell'applicazione */}
        <Routes>
          {/* Rotta pubblica */}
          <Route path="/" element={<HomePage />} />
          
          {/* Rotta di accesso */}
          <Route path="/login" element={<LoginPage />} />
          
          {/* Rotta di registrazione */}
          <Route path="/register" element={<RegisterPage />} />
          
          {/* Rotta protetta - richiede autenticazione */}
          <Route 
            path="/dashboard" 
            element={
              <ProtectedRoute>
                <DashboardPage />
              </ProtectedRoute>
            } 
          />
          
          {/* Rotta catch-all per pagine non trovate */}
          <Route path="*" element={<NotFoundPage />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;

import React from 'react';
import { Link } from 'react-router-dom';

function NotFoundPage() {
  return (
    <div>
      <h2>404 - Pagina non trovata</h2>
      <p>La pagina che stai cercando non esiste.</p>
      <Link to="/">Torna alla home</Link>
    </div>
  );
}

export default NotFoundPage;

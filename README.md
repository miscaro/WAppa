# WAppa - Applicazione Meteo

## 📋 Descrizione
WAppa è un'applicazione web moderna per la visualizzazione delle previsioni meteorologiche, sviluppata come progetto per l'esame di Applicazioni Web Mobile e Cloud. L'applicazione permette agli utenti di:

- 🌦️ Visualizzare le previsioni meteo attuali e future per qualsiasi località
- 🔐 Registrarsi e accedere al proprio account personale
- ⭐ Salvare le località preferite
- 📊 Ottenere informazioni dettagliate su temperatura, umidità, vento e altro
- 🌍 Cercare località in tutto il mondo

## 🛠️ Tecnologie Utilizzate

### Backend (ASP.NET Core 8.0)
- **ASP.NET Core 8.0** - Framework per lo sviluppo di API web
- **Entity Framework Core 8.0** - ORM per l'accesso ai dati
- **SQLite** - Database relazionale integrato
- **JWT** - Per l'autenticazione sicura
- **OpenMeteo API** - Servizio di dati meteorologici
- **Swagger/OpenAPI** - Documentazione delle API
- **BCrypt.Net-Next** - Per l'hashing delle password
- **Docker** - Containerizzazione dell'applicazione

### Frontend (React 18)
- **React 18** - Libreria JavaScript per interfacce utente
- **Vite 4.4** - Strumento di build e sviluppo ultra-veloce
- **React Router 6** - Gestione delle rotte
- **Axios** - Client HTTP per le chiamate API
- **JWT Decode** - Gestione dei token JWT lato client
- **Tailwind CSS** - Framework CSS utility-first

## 🚀 Avvio Rapido

### Prerequisiti
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (opzionale, per l'esecuzione in container)

### Metodo 1: Esecuzione con Docker (consigliato)

1. **Clona il repository**
   ```bash
   git clone [URL_DEL_REPOSITORY]
   cd WAppa-main
   ```

2. **Crea un file .env** nella radice del progetto con il seguente contenuto:
   ```env
   # Backend
   BACKEND_HOST_PORT=5278
   BACKEND_INTERNAL_PORT=80
   ASPNETCORE_ENVIRONMENT=Development
   
   # Database
   CONNECTIONSTRINGS_DEFAULTCONNECTION=Data Source=/app/Data/weatherapp.db
   
   # JWT Token (sostituisci con una chiave sicura in produzione)
   APPSETTINGS_TOKEN=la_tua_chiave_segreta_molto_lunga_e_sicura_123!
   
   # Frontend
   FRONTEND_HOST_PORT=5173
   ```

3. **Avvia i container**
   ```bash
   docker-compose up --build
   ```

4. **Apri il browser** all'indirizzo `http://localhost:5173`

### Metodo 2: Esecuzione manuale

#### Backend

1. **Configura il database**
   ```bash
   cd WAppa-main
   dotnet tool install --global dotnet-ef
   dotnet ef database update
   ```

2. **Avvia il backend**
   ```bash
   dotnet run --project WAppa.csproj
   ```
   Il server sarà disponibile su `http://localhost:5278`

#### Frontend

1. **Installa le dipendenze**
   ```bash
   cd Frontend/WAppa-frontend
   npm install
   ```

2. **Avvia il server di sviluppo**
   ```bash
   npm run dev
   ```
   L'applicazione sarà disponibile su `http://localhost:5173`

## 📚 Documentazione API

L'API è documentata con Swagger/OpenAPI ed è disponibile all'indirizzo:
- `http://localhost:5278/swagger` (in sviluppo)
- `/swagger` (in produzione)

### Endpoints principali

#### Autenticazione
- `POST /api/Auth/register` - Registra un nuovo utente
- `POST /api/Auth/login` - Effettua il login

#### Meteo
- `GET /api/Weather/current` - Ottieni le condizioni meteo attuali
- `GET /api/Weather/forecast` - Ottieni le previsioni meteo

#### Località Preferite
- `GET /api/FavoriteLocations` - Ottieni le località preferite
- `POST /api/FavoriteLocations` - Aggiungi una località preferita
- `DELETE /api/FavoriteLocations/{id}` - Rimuovi una località preferita

## 🔧 Configurazione

### Backend
Il file `appsettings.json` contiene le configurazioni principali:
- **ConnectionStrings**: Stringa di connessione al database
- **AppSettings:Token**: Chiave segreta per la generazione dei token JWT
- **Logging**: Configurazione dei log

### Frontend
Il file `Frontend/WAppa-frontend/vite.config.js` contiene le configurazioni di Vite.

## 📄 Licenza

Questo progetto è concesso in licenza sotto la Licenza MIT - vedi il file [LICENSE](LICENSE) per i dettagli.

## 👥 Autori

- **Mariano Iscaro**
- **Antonio Pennino**
- **Andrea Imani Nobar**

// Program.cs - Punto di ingresso principale dell'applicazione ASP.NET Core
// Questo file configura i servizi e il middleware dell'applicazione

using Microsoft.EntityFrameworkCore;
using WeatherApp.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using WeatherApp.Services;

// Crea un'istanza del costruttore dell'applicazione
var builder = WebApplication.CreateBuilder(args);

// --- Inizio Sezione Configurazione Servizi ---

// Configurazione CORS per consentire richieste dal frontend
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173") // URL del frontend React (server di sviluppo Vite)
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                          // In caso di produzione del sito dovremmo specificare gli URL del frontend deployato
                          // Esempio: policy.WithOrigins("https://app-meteo.com")
                      });
});

// Aggiungi il supporto per i controller API
builder.Services.AddControllers();

// Configura il contesto del database con SQLite come fallback
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=weatherapp.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Registra i servizi personalizzati
builder.Services.AddHttpClient(); // Per le richieste HTTP esterne
builder.Services.AddScoped<IGeocodingService, GeocodingService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IWeatherService, WeatherService>();

// Configura l'autenticazione JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenKey = builder.Configuration.GetSection("AppSettings:Token").Value;
        if (string.IsNullOrEmpty(tokenKey))
            throw new Exception("AppSettings:Token non configurato in appsettings.json");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// Configura Swagger per la documentazione API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WeatherApp API", Version = "v1" });

    // Aggiunge supporto per l'autenticazione JWT in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Autorizzazione JWT con Bearer token. Esempio: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
// --- Fine Sezione Configurazione Servizi ---


// Costruisci l'applicazione
var app = builder.Build();

// --- Inizio Applicazione Migrazioni Database ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<ApplicationDbContext>();
        if (dbContext.Database.GetPendingMigrations().Any())
        {
            app.Logger.LogInformation("Applicazione delle migrazioni del database in corso...");
            dbContext.Database.Migrate();
            app.Logger.LogInformation("Migrazioni del database applicate con successo.");
        }
        else
        {
            app.Logger.LogInformation("Nessuna migrazione del database in sospeso da applicare.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Si è verificato un errore durante l'applicazione delle migrazioni del database.");
    }
}
// --- Fine Applicazione Migrazioni Database ---

// --- Inizio Configurazione Pipeline Richieste HTTP ---

// Configura l'ambiente di sviluppo
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherApp API V1");
    });
    app.UseDeveloperExceptionPage(); // Mostra eccezioni dettagliate in sviluppo
}
else
{
    app.UseHsts();
}

// Middleware per il reindirizzamento HTTPS
app.UseHttpsRedirection();

// Middleware CORS - deve essere posizionato prima di UseRouting, UseAuthentication, UseAuthorization
app.UseCors(MyAllowSpecificOrigins);

// Middleware per l'autenticazione e l'autorizzazione
app.UseAuthentication();
app.UseAuthorization();

// Mappa i controller
app.MapControllers();

// --- Fine Configurazione Pipeline Richieste HTTP ---

// Avvia l'applicazione
app.Run();

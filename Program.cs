// Program.cs (esempio per .NET 6+)
using Microsoft.EntityFrameworkCore;
using WeatherApp.Data; // Assicurati che il namespace del tuo DbContext sia corretto

var builder = WebApplication.CreateBuilder(args);

// Aggiungi servizi al container.
builder.Services.AddControllers();

// Configurazione di Entity Framework Core con SQLite
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=weatherapp.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));


// Aggiungi Swagger/OpenAPI (di solito è già presente nel template Web API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configura la pipeline di richieste HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Sarà importante aggiungere qui l'autenticazione e l'autorizzazione più avanti
// app.UseAuthentication();
// app.UseAuthorization();

app.MapControllers();

app.Run();
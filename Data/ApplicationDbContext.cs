using Microsoft.EntityFrameworkCore;
using WeatherApp.Models;

namespace WeatherApp.Data
{
    /// <summary>
    /// Rappresenta il contesto del database dell'applicazione e fornisce accesso alle tabelle del database.
    /// Ereditando da DbContext, fornisce funzionalità per l'accesso ai dati utilizzando Entity Framework Core.
    /// </summary>
    public class ApplicationDbContext : DbContext
    {
        /// <summary>
        /// Inizializza una nuova istanza della classe <see cref="ApplicationDbContext"/>.
        /// </summary>
        /// <param name="options">Le opzioni di configurazione per questo contesto.</param>
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<FavoriteLocation> FavoriteLocations { get; set; }

        /// <summary>
        /// Configura il modello di dati e le relazioni tra le entità.
        /// Viene chiamato quando il modello è stato inizializzato ma prima che venga creato lo schema del database.
        /// </summary>
        /// <param name="modelBuilder">Il generatore di modelli utilizzato per configurare il modello di dati.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurazione per rendere lo Username univoco
            // Questo crea un indice univoco sulla colonna Username della tabella Users
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Configurazione della relazione uno-a-molti tra User e FavoriteLocation
            // Un utente può avere più località preferite, ma ogni località appartiene a un solo utente
            modelBuilder.Entity<User>()
                .HasMany(u => u.FavoriteLocations)     // Un utente ha molte località preferite
                .WithOne(fl => fl.User)                // Ogni località ha un solo utente
                .HasForeignKey(fl => fl.UserId)         // La chiave esterna in FavoriteLocation
                .OnDelete(DeleteBehavior.Cascade);      // Eliminazione a cascata: se un utente viene cancellato,
                                                      // vengono eliminate anche tutte le sue località preferite
        }
    }
}
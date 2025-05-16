using System.ComponentModel.DataAnnotations;

namespace WeatherApp.Dtos.FavoriteLocation
{
    /// <summary>
    /// DTO (Data Transfer Object) per l'aggiunta di una nuova località preferita.
    /// Contiene il nome della località che l'utente desidera aggiungere ai preferiti.
    /// </summary>
    public class AddFavoriteLocationDto
    {
        [Required(ErrorMessage = "Il nome della località è obbligatorio")]
        [StringLength(100, MinimumLength = 2, 
            ErrorMessage = "Il nome della località deve essere compreso tra 2 e 100 caratteri.")]
        public string LocationName { get; set; } = string.Empty;
    }
}

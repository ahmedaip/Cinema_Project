using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema_Project.ViewModels
{
    public class UpdateMovieVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool Status { get; set; }
        public DateTime DateTime { get; set; }
        public decimal Rate { get; set; }
        public int CategoryId { get; set; }
        public int CinemaId { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace Cinema_Project.Models
{
    public class MovieSubImags
    {
        public int Id { get; set; }
        public string Img { get; set; }
        public int MovieId { get; set; }
        [ForeignKey(nameof(MovieId))]
        public Movie Movie { get; set; }

    }
}

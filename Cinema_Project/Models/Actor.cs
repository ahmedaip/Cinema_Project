namespace Cinema_Project.Models
{
    public class Actor
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public decimal Rate { get; set; }
        public string Img { get; set; }
       
        public List<Movie> Movies { get; set; } = new();
    }
}

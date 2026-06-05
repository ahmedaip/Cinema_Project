namespace Cinema_Project.ViewModels
{
    public class MovieVM
    {
        public IEnumerable<Movie> Movies { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}

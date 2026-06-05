namespace Cinema_Project.ViewModels
{
    public class CinemaVM
    {
        public IEnumerable<Cinema> Cinemas { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}

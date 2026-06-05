namespace Cinema_Project.ViewModels
{
    public class CreateUpdateMovieVM
    {
        public Movie Movie { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Cinema> Cinemas { get; set; }
        public IEnumerable<Actor> Actors { get; set; }
        public IEnumerable<MovieSubImags> MovieSubImags { get; set; }


    }
}

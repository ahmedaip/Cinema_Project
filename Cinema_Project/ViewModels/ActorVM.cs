namespace Cinema_Project.ViewModels
{
    public class ActorVM
    {
        public IEnumerable<Actor> Actors { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
    }
}

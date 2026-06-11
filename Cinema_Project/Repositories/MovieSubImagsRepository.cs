namespace Cinema_Project.Repositories
{
    public class MovieSubImagsRepository :Repository<MovieSubImags> , IMovieSubImagsRepository
    {
        public MovieSubImagsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void DeleteRange(IEnumerable<MovieSubImags> movieSubImags)
        {
            _context.MovieSubImags.RemoveRange(movieSubImags);
        }
    }
}

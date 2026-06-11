namespace Cinema_Project.Repositories
{
    public interface IMovieSubImagsRepository : IRepository<MovieSubImags>
    {
        void DeleteRange(IEnumerable<MovieSubImags> movieSubImags);
    }
}

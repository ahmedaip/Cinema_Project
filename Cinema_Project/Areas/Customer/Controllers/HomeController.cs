using Cinema_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Cinema_Project.Areas.Customer.Controllers
{
    [Area(CD.CUSTOMER_AREA)]
    public class HomeController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly MovieService _movieService;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
            _movieService = new MovieService();
        }
        public IActionResult Index(string movieName, int page = 1)
        {


            var movies = _context.Movies.Include(c => c.Category).Include(c => c.Cinema).Include(a => a.Actors).AsQueryable();
            if (movieName != null)
            {
                movies = movies.Where(c => c.Name.Contains(movieName.Trim()));
                ViewBag.movieName = movieName;
            }
            int totalPages = (int)Math.Ceiling(movies.Count() / 5.0);
            movies = movies.Skip((page - 1) * 5).Take(5);
            return View(new MovieVM()
            {
                Movies = movies.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page
            });
        }



        public IActionResult MovieDetails(int id)
        {
            var movie = _context.Movies
                                .Include(m => m.Category)
                                .Include(m => m.Cinema)
                                .Include(m => m.Actors)
                                .FirstOrDefault(m => m.Id == id);

            if (movie is null)
            {
                return NotFound();
            }
            var subImages = _context.MovieSubImags
                                    .Where(x => x.MovieId == id)
                                    .ToList();

            return View(new CreateUpdateMovieVM()
            {
                Movie = movie,
                Actors = movie.Actors ?? new List<Actor>(),
                MovieSubImags = subImages ?? new List<MovieSubImags>(),
                Categories = new List<Category>(),
                Cinemas = new List<Cinema>()
            });
        }

    }
}

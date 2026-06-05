using Cinema_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Cinema_Project.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    public class MovieController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly MovieService _movieService;

        public MovieController()
        {
            _context = new ApplicationDbContext();
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


        [HttpGet]
        public IActionResult Create()
        {
            var categories = _context.Categories.AsEnumerable();
            var cinemas = _context.Cinemas.AsEnumerable();
            var actors = _context.Actors.AsEnumerable();
            return View(new CreateUpdateMovieVM()
            {
                Categories = categories,
                Cinemas = cinemas,
                Actors = actors,
            });
        }

        [HttpPost]
        public IActionResult Create(Movie movie, IFormFile ImageFile, List<IFormFile> SubImageFiles, List<int> ActorIds)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var filename = _movieService.SaveFile(ImageFile);

                movie.MainImg = filename;
            }
            var savedproduct = _context.Movies.Add(movie);
            _context.SaveChanges();


            if (SubImageFiles != null && SubImageFiles.Count > 0)
            {
                foreach (var image in SubImageFiles)
                {
                    var filename = _movieService.SaveFile(image, MovieImageType.SubImage);
                    _context.MovieSubImags.Add(new MovieSubImags()

                    {
                        MovieId = savedproduct.Entity.Id,
                        Img = filename
                    });
                }
                _context.SaveChanges();
            }


            var actors = _context.Actors
                .Where(a => ActorIds.Contains(a.Id))
                .ToList();

            movie.Actors = actors;

            _context.SaveChanges();


            return RedirectToAction(nameof(Index));
        }




        [HttpGet]
        public IActionResult Edit(int id)
        {
            var movie = _context.Movies.Include(a => a.Actors).SingleOrDefault(c => c.Id == id);

            if (movie is null)
                return NotFound();

            var categoreis = _context.Categories.AsQueryable();
            var cinemas = _context.Cinemas.AsQueryable();
            var actors = _context.Actors.AsQueryable();
            return View(new CreateUpdateMovieVM()
            {
                Movie = movie,
                Categories = categoreis,
                Cinemas = cinemas,
                Actors = actors,
                MovieSubImags = _context.MovieSubImags.Where(ms => ms.MovieId == id)
            });
        }


        [HttpPost]
        public IActionResult Edit(Movie movie, IFormFile ImageFile, List<IFormFile> SubImageFiles, List<int> ActorIds)
        {
            ActorIds ??= new List<int>();
            var movieInDb = _context.Movies.AsNoTracking().Include(m => m.Actors).FirstOrDefault(m => m.Id == movie.Id);

            movieInDb.Actors.Clear();

            var actors = _context.Actors
                .Where(a => ActorIds.Contains(a.Id))
                .ToList();

            foreach (var a in actors)
            {
                movieInDb.Actors.Add(a);
            }

            _context.SaveChanges();

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var filename = _movieService.SaveFile(ImageFile);
                movie.MainImg = filename;

                _movieService.RemoveFile(movieInDb.MainImg);
            }


            else
            {
                movie.MainImg = movieInDb.MainImg;
            }
            _context.Movies.Update(movie);
            _context.SaveChanges();

            if (SubImageFiles != null && SubImageFiles.Count > 0)
            {

                var oldSubImages = _context.MovieSubImags.Where(p => p.MovieId == movie.Id);
                _context.MovieSubImags.RemoveRange(oldSubImages);
                foreach (var item in oldSubImages)
                {
                    _movieService.RemoveFile(item.Img, MovieImageType.SubImage);
                }

                foreach (var image in SubImageFiles)
                {
                    var filename = _movieService.SaveFile(image, MovieImageType.SubImage);

                    _context.MovieSubImags.Add(new MovieSubImags()
                    {
                        MovieId = movie.Id,
                        Img = filename
                    });

                }
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));

        }


        public IActionResult Delete(int id)
        {
            var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            else
            {
                _movieService.RemoveFile(movie.MainImg);
                _context.Movies.Remove(movie);
                var movieSubImages = _context.MovieSubImags.Where(ms => ms.MovieId== movie.Id);
                foreach (var image in movieSubImages)
                {
                    _movieService.RemoveFile(image.Img, MovieImageType.SubImage);
                }
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            
        }
    }
}

using Cinema_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Cinema_Project.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    public class MovieController : Controller
    {
        private readonly IRepository<Movie> _movieRepository;
        private readonly IRepository<Category> _categoryRepository;
        private readonly IRepository<Cinema> _cinemaRepository;
        private readonly IRepository<Actor> _actorRepository;
        private readonly IMovieSubImagsRepository _movieSubImagsRepository;
        private readonly MovieService _movieService = new MovieService();

        //public MovieController()
        //{
        //    _movieRepository = new Repository<Movie>();
        //    _categoryRepository = new Repository<Category>();
        //    _cinemaRepository = new Repository<Cinema>();
        //    _actorRepository = new Repository<Actor>();
        //    _movieSubImagsRepository = new MovieSubImagsRepository();
        //    _movieService = new MovieService();
        //}

        public MovieController(IRepository<Movie> movieRepository, IRepository<Category> categoryRepository, IRepository<Cinema> cinemaRepository, IRepository<Actor> actorRepository, IMovieSubImagsRepository movieSubImagsRepository)
        {
            _movieRepository = movieRepository;
            _categoryRepository = categoryRepository;
            _cinemaRepository = cinemaRepository;
            _actorRepository = actorRepository;
            _movieSubImagsRepository = movieSubImagsRepository;
        }

        public async Task<IActionResult> Index(string movieName, int page = 1)
        {
            //var movies = _context.Movies.Include(c => c.Category).Include(c => c.Cinema).Include(a => a.Actors).AsQueryable();
            var movies = await _movieRepository.GetAllAsync(includes: [c => c.Category, c => c.Cinema, a => a.Actors]);
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


        public async Task<IActionResult> MovieDetails(int id)
        {
            //var movie = _context.Movies
            //                    .Include(m => m.Category)
            //                    .Include(m => m.Cinema)
            //                    .Include(m => m.Actors)
            //                    .FirstOrDefault(m => m.Id == id);
            var movie = await _movieRepository.GetOneAsync(includes: [m => m.Category, m => m.Cinema, m => m.Actors], filter: m => m.Id == id);

            if (movie is null)
            {
                return NotFound();
            }
            //var subImages = _context.MovieSubImags
            //                        .Where(x => x.MovieId == id)
            //                        .ToList();
            var subImages = await _movieSubImagsRepository.GetAllAsync(x => x.MovieId == id);


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
        public async Task<IActionResult> Create()
        {
            //var categories = _context.Categories.AsEnumerable();
            var categories = await _categoryRepository.GetAllAsync();
            //var cinemas = _context.Cinemas.AsEnumerable();
            var cinemas = await _cinemaRepository.GetAllAsync();
            //var actors = _context.Actors.AsEnumerable();
            var actors = await _actorRepository.GetAllAsync();
            return View(new CreateUpdateMovieVM()
            {
                Categories = categories,
                Cinemas = cinemas,
                Actors = actors,
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(Movie movie, IFormFile ImageFile, List<IFormFile> SubImageFiles, List<int> ActorIds)
        {
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var filename = _movieService.SaveFile(ImageFile);

                movie.MainImg = filename;
            }
            //var savedproduct = _context.Movies.Add(movie);
            var savedproduct = await _movieRepository.CreateAsync(movie);
            //_context.SaveChanges();
            await _movieRepository.CommitAsync();


            if (SubImageFiles != null && SubImageFiles.Count > 0)
            {
                foreach (var image in SubImageFiles)
                {
                    var filename = _movieService.SaveFile(image, MovieImageType.SubImage);
                    //_context.MovieSubImags.Add(new MovieSubImags()
                    await _movieSubImagsRepository.CreateAsync(new MovieSubImags()

                    {
                        MovieId = savedproduct.Entity.Id,
                        Img = filename
                    });
                }
                //_context.SaveChanges();
                await _movieSubImagsRepository.CommitAsync();
            }


            //var actors = _context.Actors
            //    .Where(a => ActorIds.Contains(a.Id))
            //    .ToList();
            var actors = await _actorRepository.GetAllAsync(a => ActorIds.Contains(a.Id));

            savedproduct.Entity.Actors = actors.ToList();

            //_context.SaveChanges();
            await _movieRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //var movie = _context.Movies.Include(a => a.Actors).SingleOrDefault(c => c.Id == id);
            var movie = await _movieRepository.GetOneAsync(includes: [a => a.Actors], filter: c => c.Id == id);

            if (movie is null)
                return NotFound();

            //var categories = _context.Categories.AsEnumerable();
            var categories = await _categoryRepository.GetAllAsync();
            //var cinemas = _context.Cinemas.AsEnumerable();
            var cinemas = await _cinemaRepository.GetAllAsync();
            //var actors = _context.Actors.AsEnumerable();
            var actors = await _actorRepository.GetAllAsync();
            return View(new CreateUpdateMovieVM()
            {
                Movie = movie,
                Categories = categories,
                Cinemas = cinemas,
                Actors = actors,
                //MovieSubImags = _context.MovieSubImags.Where(ms => ms.MovieId == id)
                MovieSubImags = await _movieSubImagsRepository.GetAllAsync(ms => ms.MovieId == id)
            });
        }


        //[HttpPost]
        //public async Task<IActionResult> Edit(Movie movie, IFormFile ImageFile, List<IFormFile> SubImageFiles, List<int> ActorIds)
        //{
        //    ActorIds ??= new List<int>();
        //    //var movieInDb = _context.Movies.AsNoTracking().Include(m => m.Actors).FirstOrDefault(m => m.Id == movie.Id);
        //    var movieInDb = await _movieRepository.GetOneAsync( includes: [m => m.Actors], filter: m => m.Id == movie.Id);

        //    movieInDb.Actors.Clear();

        //    //var actors = _context.Actors
        //    //    .Where(a => ActorIds.Contains(a.Id))
        //    //    .ToList();

        //    var actors = await _actorRepository.GetAllAsync(a => ActorIds.Contains(a.Id));


        //    foreach (var actor in actors)
        //    {
        //        movieInDb.Actors.Add(actor);
        //    }

        //    //_context.SaveChanges();
        //    await _movieRepository.CommitAsync();

        //    if (ImageFile != null && ImageFile.Length > 0)
        //    {
        //        var filename = _movieService.SaveFile(ImageFile);
        //        movie.MainImg = filename;

        //        _movieService.RemoveFile(movieInDb.MainImg);
        //    }
        //    else
        //    {
        //        movie.MainImg = movieInDb.MainImg;
        //    }
        //    //_context.Movies.Update(movie);
        //    _movieRepository.Update(movie);
        //    //_context.SaveChanges();
        //    await _movieRepository.CommitAsync();

        //    if (SubImageFiles != null && SubImageFiles.Count > 0)
        //    {
        //        //var oldSubImages = _context.MovieSubImags.Where(p => p.MovieId == movie.Id);
        //        var oldSubImages = await _movieSubImagsRepository.GetAllAsync(p => p.MovieId == movie.Id);
        //        //_context.MovieSubImags.RemoveRange(oldSubImages);
        //        _movieSubImagsRepository.DeleteRange(oldSubImages);
        //        foreach (var item in oldSubImages)
        //        {
        //            _movieService.RemoveFile(item.Img, MovieImageType.SubImage);
        //        }

        //        foreach (var image in SubImageFiles)
        //        {
        //            var filename = _movieService.SaveFile(image, MovieImageType.SubImage);

        //            //_context.MovieSubImags.Add(new MovieSubImags()
        //            await _movieSubImagsRepository.CreateAsync(new MovieSubImags()
        //            {
        //                MovieId = movie.Id,
        //                Img = filename
        //            });

        //        }
        //        //_context.SaveChanges();
        //        await _movieSubImagsRepository.CommitAsync();
        //    }
        //    return RedirectToAction(nameof(Index));
        //}

        [HttpPost]
        public async Task<IActionResult> Edit(
                UpdateMovieVM movie,
                IFormFile ImageFile,
                List<IFormFile> SubImageFiles,
                List<int> ActorIds)
        {
            ActorIds ??= new List<int>();

            var movieInDb = await _movieRepository.GetOneAsync(
                includes: [m => m.Actors],
                filter: m => m.Id == movie.Id);

            if (movieInDb == null)
                return NotFound();

            // Update scalar properties
            movieInDb.Name = movie.Name;
            movieInDb.Description = movie.Description;
            movieInDb.Price = movie.Price;
            movieInDb.DateTime = movie.DateTime;
            movieInDb.Rate = movie.Rate;
            movieInDb.Status = movie.Status;
            movieInDb.CategoryId = movie.CategoryId;
            movieInDb.CinemaId = movie.CinemaId;

            // Update Main Image
            if (ImageFile != null && ImageFile.Length > 0)
            {
                var filename = _movieService.SaveFile(ImageFile);

                _movieService.RemoveFile(movieInDb.MainImg);

                movieInDb.MainImg = filename;
            }

            // Remove all old actors
            movieInDb.Actors.Clear();

            // Add new actors
            var actors = await _actorRepository.GetAllAsync(a => ActorIds.Contains(a.Id));

            foreach (var actor in actors)
            {
                movieInDb.Actors.Add(actor);
            }

            await _movieRepository.CommitAsync();

            // Update Sub Images
            if (SubImageFiles != null && SubImageFiles.Count > 0)
            {
                var oldSubImages =
                    await _movieSubImagsRepository.GetAllAsync(
                        p => p.MovieId == movie.Id);

                _movieSubImagsRepository.DeleteRange(oldSubImages);

                foreach (var item in oldSubImages)
                {
                    _movieService.RemoveFile(
                        item.Img,
                        MovieImageType.SubImage);
                }

                foreach (var image in SubImageFiles)
                {
                    var filename = _movieService.SaveFile(
                        image,
                        MovieImageType.SubImage);

                    await _movieSubImagsRepository.CreateAsync(
                        new MovieSubImags
                        {
                            MovieId = movie.Id,
                            Img = filename
                        });
                }

                await _movieSubImagsRepository.CommitAsync();
            }

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int id)
        {
            //var movie = _context.Movies.FirstOrDefault(m => m.Id == id);
            var movie = await _movieRepository.GetOneAsync(m => m.Id == id);
            if (movie == null)
            {
                return NotFound();
            }
            else
            {
                _movieService.RemoveFile(movie.MainImg);
                //_context.Movies.Remove(movie);
                _movieRepository.Delete(movie);
                //var movieSubImages = _context.MovieSubImags.Where(ms => ms.MovieId == movie.Id);
                var movieSubImages = await _movieSubImagsRepository.GetAllAsync(ms => ms.MovieId == movie.Id);
                foreach (var image in movieSubImages)
                {
                    _movieService.RemoveFile(image.Img, MovieImageType.SubImage);
                }
                await _movieSubImagsRepository.CommitAsync();
                await _movieRepository.CommitAsync();
                return RedirectToAction(nameof(Index));
            }

        }
    }
}

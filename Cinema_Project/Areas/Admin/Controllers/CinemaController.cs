using Cinema_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Project.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    public class CinemaController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IRepository<Cinema> _cinemaRepository;
        private readonly CinemaService _cinemaService = new CinemaService();

        //public CinemaController()
        //{
        //    _context = new ApplicationDbContext();
        //    _cinemaRepository = new Repository<Cinema>();
        //    _cinemaService = new CinemaService();
        //}

        public CinemaController(IRepository<Cinema> cinemaRepository)
        {
            _cinemaRepository = cinemaRepository;
        }

        public async Task<IActionResult> Index(string cinemaName, int page = 1)
        {


            //var cinemas = _context.Cinemas.AsQueryable();
            var cinemas = await _cinemaRepository.GetAllAsync();
            if (cinemaName != null)
            {
                cinemas = cinemas.Where(c => c.Name.Contains(cinemaName));
                ViewBag.cinemaName = cinemaName;
            }
            int totalPages = (int)Math.Ceiling(cinemas.Count() / 5.0);
            cinemas = cinemas.Skip((page - 1) * 5).Take(5);
            return View(new CinemaVM()
            {
                Cinemas = cinemas.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page
            });
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateUpdateCinemaVM());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUpdateCinemaVM createUpdateCinemaVM)
        {
            if (createUpdateCinemaVM.ImageFile == null || createUpdateCinemaVM.ImageFile.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "Image is required");
                return View(createUpdateCinemaVM);
            }
            var cinema = new Cinema()
            {
                Name = createUpdateCinemaVM.Name,
                Description = createUpdateCinemaVM.Description,
                Address = createUpdateCinemaVM.Address,
                Status = createUpdateCinemaVM.Status
            };

            if (createUpdateCinemaVM.ImageFile != null && createUpdateCinemaVM.ImageFile.Length > 0)
            {
                var fileName = _cinemaService.SaveFile(createUpdateCinemaVM.ImageFile);
                cinema.Img = fileName;
            }
            //_context.Cinemas.Add(cinema);
            await _cinemaRepository.CreateAsync(cinema);
            //_context.SaveChanges();
            await _cinemaRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }




        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //var cinema = _context.Cinemas.SingleOrDefault(c => c.Id == id);
            var cinema = await _cinemaRepository.GetOneAsync(c => c.Id == id);
            if (cinema is null)
                return NotFound();
            return View(cinema);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Cinema cinema, IFormFile? ImageFile)
        {
            ModelState.Remove("Img");
            ModelState.Remove("ImageFile");

            if (!ModelState.IsValid)
                return View(cinema);

            //var cinemaInDb = _context.Cinemas.AsNoTracking().FirstOrDefault(b => b.Id == cinema.Id);
            var cinemaInDb = await _cinemaRepository.GetOneAsync(isTracking: false , filter: c => c.Id == cinema.Id);

            if (cinemaInDb == null)
                return NotFound();

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = _cinemaService.SaveFile(ImageFile);
                cinema.Img = fileName;
                _cinemaService.RemoveFile(cinemaInDb.Img);
            }
            else
            {
                cinema.Img = cinemaInDb.Img;
            }

            //_context.Cinemas.Update(cinema);
            _cinemaRepository.Update(cinema);
            await _cinemaRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            //var cinema = _context.Cinemas.SingleOrDefault(c => c.Id == id);
            var cinema = await _cinemaRepository.GetOneAsync(c => c.Id == id);
            if (cinema is null)
                return NotFound();

            _cinemaService.RemoveFile(cinema.Img);


            //_context.Cinemas.Remove(cinema);
            _cinemaRepository.Delete(cinema);
            await _cinemaRepository.CommitAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}

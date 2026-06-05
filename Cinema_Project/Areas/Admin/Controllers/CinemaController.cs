using Cinema_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Project.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    public class CinemaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly CinemaService _cinemaService;

        public CinemaController()
        {
            _context = new ApplicationDbContext();
            _cinemaService = new CinemaService();
        }
        public IActionResult Index(string cinemaName, int page = 1)
        {


            var cinemas = _context.Cinemas.AsQueryable();
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
        public IActionResult Create(CreateUpdateCinemaVM createUpdateCinemaVM)
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
            _context.Cinemas.Add(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }




        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cinema = _context.Cinemas.SingleOrDefault(c => c.Id == id);
            if (cinema is null)
                return NotFound();
            return View(cinema);
        }


        [HttpPost]
        public IActionResult Edit(Cinema cinema, IFormFile? ImageFile)
        {
            ModelState.Remove("Img");
            ModelState.Remove("ImageFile");

            if (!ModelState.IsValid)
                return View(cinema);

            var cinemaInDb = _context.Cinemas.AsNoTracking().FirstOrDefault(b => b.Id == cinema.Id);

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

            _context.Cinemas.Update(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }


        public IActionResult Delete(int id)
        {
            var cinema = _context.Cinemas.SingleOrDefault(c => c.Id == id);
            if (cinema is null)
                return NotFound();

            _cinemaService.RemoveFile(cinema.Img);


            _context.Cinemas.Remove(cinema);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }
    }
}

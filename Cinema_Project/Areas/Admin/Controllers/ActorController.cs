using Cinema_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema_Project.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    public class ActorController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IRepository<Actor> _actorRepository;
        private readonly ActorService _actorService = new ActorService();

        //public ActorController()
        //{
        //    _context = new ApplicationDbContext();
        //    _actorRepository = new Repository<Actor>();
        //    _actorService = new ActorService();
        //}

        public ActorController(IRepository<Actor> actorRepository)
        {
            _actorRepository = actorRepository;
        }

        public async Task<IActionResult> Index(string actorName, int page = 1)
        {


            //var actors = _context.Actors.AsQueryable();
            var actors = await _actorRepository.GetAllAsync();
            if (actorName != null)
            {
                actors = actors.Where(c => c.Name.Contains(actorName));
                ViewBag.actorName = actorName;
            }
            int totalPages = (int)Math.Ceiling(actors.Count() / 5.0);
            actors = actors.Skip((page - 1) * 5).Take(5);
            return View(new ActorVM()
            {
                Actors = actors.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page
            });
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateUpdateActorVM());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUpdateActorVM createUpdateActorVM)
        {
            if (createUpdateActorVM.ImageFile == null || createUpdateActorVM.ImageFile.Length == 0)
            {
                ModelState.AddModelError("ImageFile", "Image is required");
                return View(createUpdateActorVM);
            }
            var actor = new Actor()
            {
                Name = createUpdateActorVM.Name,
                Gender = createUpdateActorVM.Gender,
                Rate = createUpdateActorVM.Rate
            };

            if (createUpdateActorVM.ImageFile != null && createUpdateActorVM.ImageFile.Length > 0)
            {
                var fileName = _actorService.SaveFile(createUpdateActorVM.ImageFile);
                actor.Img = fileName;
            }
            //_context.Actors.Add(actor);
            await _actorRepository.CreateAsync(actor);
            //_context.SaveChanges();
            await _actorRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //var actor = _context.Actors.SingleOrDefault(c => c.Id == id);
            var actor = await _actorRepository.GetOneAsync(a => a.Id == id);
            if (actor is null)
                return NotFound();
            return View(actor);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(Actor actor, IFormFile? ImageFile)
        {
            ModelState.Remove("Img");
            ModelState.Remove("ImageFile");

            if (!ModelState.IsValid)
                return View(actor);

            //var actorInDb = _context.Actors.AsNoTracking().FirstOrDefault(b => b.Id == actor.Id);
            var actorInDb = await _actorRepository.GetOneAsync(isTracking: false , filter: a => a.Id == actor.Id);

            if (actorInDb == null)
                return NotFound();

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var fileName = _actorService.SaveFile(ImageFile);
                actor.Img = fileName;
                _actorService.RemoveFile(actorInDb.Img);
            }
            else
            {
                actor.Img = actorInDb.Img;
            }

            //_context.Actors.Update(actor);
            _actorRepository.Update(actor);
            //_context.SaveChanges();
            await _actorRepository.CommitAsync(); 
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int id)
        {
            //var actor = _context.Actors.SingleOrDefault(c => c.Id == id);
            var actor = await _actorRepository.GetOneAsync(c => c.Id == id);
            if (actor is null)
                return NotFound();

            _actorService.RemoveFile(actor.Img);

            //_context.Actors.Remove(actor);
            _actorRepository.Delete(actor);
            //_context.SaveChanges();
            await _actorRepository.CommitAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}

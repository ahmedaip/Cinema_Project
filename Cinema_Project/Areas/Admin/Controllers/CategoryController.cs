using Cinema_Project.DataAccess;
using Cinema_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Project.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController()
        {
            _context = new ApplicationDbContext();
        }
        public IActionResult Index(string categoryName, int page = 1)
        {


            var categories = _context.Categories.AsQueryable();
            if (categoryName != null)
            {
                categories = categories.Where(c => c.Name.Contains(categoryName));
                ViewBag.categoryName = categoryName;
            }
            int totalPages = (int)Math.Ceiling(categories.Count() / 5.0);
            categories = categories.Skip((page - 1) * 5).Take(5);
            return View(new CategoryVM()
            {
                Categories = categories.AsEnumerable(),
                TotalPages = totalPages,
                CurrentPage = page
            });
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateUpdateCategoryVM createUpdateCategoryVM)
        {

            if (!ModelState.IsValid)
                return View(createUpdateCategoryVM);
            var category = new Category()
            {
                Name = createUpdateCategoryVM.Name,
                Description = createUpdateCategoryVM.Description,
                Status = createUpdateCategoryVM.Status
            };
            _context.Categories.Add(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }




        [HttpGet]
        public IActionResult Edit(int id)
        {
            var category = _context.Categories.SingleOrDefault(c => c.Id == id);
            if (category is null)
                return NotFound();
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);
            
            _context.Categories.Update(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }


        public IActionResult Delete(int id)
        {
            var category = _context.Categories.SingleOrDefault(c => c.Id == id);
            if (category is null)
                return NotFound();

            _context.Categories.Remove(category);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));

        }
    }
}

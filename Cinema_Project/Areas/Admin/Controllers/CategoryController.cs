using Cinema_Project.DataAccess;
using Cinema_Project.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Cinema_Project.Areas.Admin.Controllers
{
    [Area(CD.ADMIN_AREA)]
    public class CategoryController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private readonly IRepository<Category> _categoryRepository;

        //public CategoryController()
        //{
        //    _categoryRepository = new Repository<Category>();
        //}

        public CategoryController(IRepository<Category> categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index(string categoryName, int page = 1)
        {
            var categories = await _categoryRepository.GetAllAsync();
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
        public async Task<IActionResult> Create(CreateUpdateCategoryVM createUpdateCategoryVM)
        {

            if (!ModelState.IsValid)
                return View(createUpdateCategoryVM);
            var category = new Category()
            {
                Name = createUpdateCategoryVM.Name,
                Description = createUpdateCategoryVM.Description,
                Status = createUpdateCategoryVM.Status
            };
            //_context.Categories.Add(category);
             await _categoryRepository.CreateAsync(category);
            //_context.SaveChanges();
            await _categoryRepository.CommitAsync();
            return RedirectToAction(nameof(Index));
        }




        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //var category = _context.Categories.SingleOrDefault(c => c.Id == id);
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category is null)
                return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (!ModelState.IsValid)
                return View(category);
            
            //_context.Categories.Update(category);
            _categoryRepository.Update(category);
            //_context.SaveChanges();
            await _categoryRepository.CommitAsync();
            return RedirectToAction(nameof(Index));

        }


        public async Task<IActionResult> Delete(int id)
        {
            //var category = _context.Categories.SingleOrDefault(c => c.Id == id);
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category is null)
                return NotFound();

            //_context.Categories.Remove(category);
            _categoryRepository.Delete(category);
            await _categoryRepository.CommitAsync();
            return RedirectToAction(nameof(Index));

        }
    }
}

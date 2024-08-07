using MenuApp.Data;
using Microsoft.EntityFrameworkCore;

namespace MenuApp.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public IActionResult Index()
    {
        return RedirectToAction("Index", "Admin");
    }

    [Route("/Page404")]
    public IActionResult Page404()
    {
        return View();
    }

    [Route("/menu/{slug}")]
    public async Task<IActionResult> UserPage(string slug)
    {
        var products = await _dbContext.Product
            .Include(x => x.Brand).Include(x => x.Category)
            .Where(x => x.Brand.BrandUrl == slug && x.IsActive && !x.IsDeleted)
            .ToListAsync();

        ViewData["slug"] = slug;

        return View(products);
    }
}
using MenuApp.Data;
using Microsoft.EntityFrameworkCore;

namespace MenuApp.ViewComponents;

public class ProductCategoryViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _dbContext;

    public ProductCategoryViewComponent(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IViewComponentResult> InvokeAsync(string slug)
    {
        var categories = await _dbContext.Category.Where(x => x.Brand.BrandUrl == slug).ToListAsync();
        return View("../ProductCategoryViewComponent", categories);
    }
}
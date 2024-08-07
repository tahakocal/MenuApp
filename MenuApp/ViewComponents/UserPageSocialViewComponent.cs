using MenuApp.Data;
using Microsoft.EntityFrameworkCore;

namespace MenuApp.ViewComponents;

public class UserPageSocialViewComponent : ViewComponent
{
    private readonly ApplicationDbContext _dbContext;

    public UserPageSocialViewComponent(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IViewComponentResult> InvokeAsync(string slug)
    {
        var brands = await _dbContext.Brand.Where(x => x.BrandUrl == slug).FirstOrDefaultAsync();
        return View("../UserPageSocialViewComponent", brands);
    }
}
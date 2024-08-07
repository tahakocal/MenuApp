using MenuApp.Core.Helpers;
using MenuApp.Data;
using MenuApp.Data.Entities;
using MenuApp.Data.Entities.Identity;
using MenuApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MenuApp.Controllers;

[Route("Admin")]
[Authorize]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager, IWebHostEnvironment environment)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _environment = environment;
    }

    [Route("Index")]
    public IActionResult Index()
    {
        return View();
    }

    #region Product

    [Route("ListProducts")]
    public async Task<IActionResult> ListProducts()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var selectedBrandId = HttpContext.Session.GetInt32("SelectedBrandId");
        if (selectedBrandId == 0) return RedirectToAction("CreateBrand");

        var brands = await _dbContext.Brand.Where(x => x.UserId == user.Id).ToListAsync();
        if (!brands.Any()) return RedirectToAction("CreateBrand");

        var products = await _dbContext.Product
            .Where(x => x.UserId == user.Id && x.BrandId == selectedBrandId && x.IsDeleted == false)
            .ToListAsync();

        return View(products);
    }

    [Route("CreateProduct")]
    public async Task<IActionResult> CreateProduct()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var selectedBrandId = HttpContext.Session.GetInt32("SelectedBrandId");
        if (selectedBrandId == null) return RedirectToAction("CreateBrand");

        var categories = await _dbContext.Category.Where(x => x.BrandId == selectedBrandId).ToListAsync();
        var viewModel = new ProductViewModel
        {
            Category = categories
        };

        return View(viewModel);
    }

    [HttpPost("CreateProduct")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(ProductViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var selectedBrandId = HttpContext.Session.GetInt32("SelectedBrandId");
        if (selectedBrandId == null) return RedirectToAction("CreateBrand");

        if (ModelState.IsValid)
        {
            var imagePaths = new List<string>();

            if (model.UploadedImages != null && model.UploadedImages.Any())
                foreach (var image in model.UploadedImages)
                {
                    var guid = Guid.NewGuid();
                    var fileName = Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "uploads", user.Id, guid + fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    imagePaths.Add($"/uploads/{user.Id}/{guid + fileName}");
                }

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                ImagePaths = imagePaths,
                DisplayOrder = model.DisplayOrder,
                UserId = user.Id,
                BrandId = selectedBrandId,
                CategoryId = model.CategoryId,
                IsActive = model.IsActive,
                IsDeleted = false,
            };

            await _dbContext.Product.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("ListProducts");
        }

        return View(model);
    }

    [HttpGet("EditProduct/{id}")]
    public async Task<IActionResult> EditProduct(int id)
    {
        var selectedBrandId = HttpContext.Session.GetInt32("SelectedBrandId");
        if (selectedBrandId == null) return RedirectToAction("CreateBrand");

        var product = await _dbContext.Product.Include(x => x.Category).FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return NotFound();

        var categories = await _dbContext.Category.Where(x => x.BrandId == selectedBrandId).ToListAsync();

        var viewModel = new ProductViewModel
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            ImagePaths = product.ImagePaths,
            Category = categories,
            IsActive = product.IsActive,
        };

        return View(viewModel);
    }

    [HttpPost("EditProduct/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(ProductViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var selectedBrandId = HttpContext.Session.GetInt32("SelectedBrandId");
        if (selectedBrandId == null) return RedirectToAction("CreateBrand");

        var product = await _dbContext.Product.FirstOrDefaultAsync(p => p.Id == model.Id);
        if (product == null) return NotFound();

        if (ModelState.IsValid)
        {
            var imagePaths = product.ImagePaths ?? new List<string>();

            if (model.UploadedImages != null && model.UploadedImages.Any())
            {
                foreach (var image in model.UploadedImages)
                {
                    var guid = Guid.NewGuid();
                    var fileName = Path.GetFileName(image.FileName);
                    var filePath = Path.Combine(_environment.WebRootPath, "uploads", user.Id, guid + fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }

                    imagePaths.Add($"/uploads/{user.Id}/{guid + fileName}");
                }

                product.ImagePaths = imagePaths;
            }

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.DisplayOrder = model.DisplayOrder;
            product.CategoryId = model.CategoryId;
            product.IsActive = model.IsActive;

            _dbContext.Product.Update(product);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("ListProducts");
        }

        model.Category = await _dbContext.Category.Where(x => x.BrandId == selectedBrandId).ToListAsync();
        model.ImagePaths = product.ImagePaths;

        return View(model);
    }


    [HttpPost("DeleteProduct/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var selectedBrandId = HttpContext.Session.GetInt32("SelectedBrandId");
        if (selectedBrandId == null) return RedirectToAction("CreateBrand");

        var product = await _dbContext.Product.FirstOrDefaultAsync(p => p.Id == id && p.BrandId == selectedBrandId);
        if (product == null) return NotFound();

        product.IsActive = false;
        product.IsDeleted = true;

        _dbContext.Product.Update(product);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("ListProducts");
    }

    #endregion

    #region Brand

    [Route("ListBrands")]
    public async Task<IActionResult> ListBrands()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var brands = await _dbContext.Brand
            .Where(x => x.UserId == user.Id && x.IsDeleted == false).ToListAsync();

        return View(brands);
    }

    [Route("CreateBrand")]
    public IActionResult CreateBrand()
    {
        return View();
    }

    [HttpPost("CreateBrand")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBrand(BrandViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        if (ModelState.IsValid)
        {
            var imagePaths = new List<string>();

            if (model.Logo != null && model.Logo.FileName.Any())
            {
                var guid = Guid.NewGuid();
                var fileName = Path.GetFileName(model.Logo.FileName);
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", user.Id, guid + fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Logo.CopyToAsync(stream);
                }

                imagePaths.Add($"/uploads/{user.Id}/{guid + fileName}");
            }

            var brand = new Brand
            {
                Name = model.Name,
                Description = model.Description,
                Phone = model.Phone,
                Email = model.Email,
                Logo = imagePaths.FirstOrDefault(),
                BrandUrl = HelperMethods.GenerateSlug(),
                Facebook = model.Facebook,
                Instagram = model.Instagram,
                Tiktok = model.Tiktok,
                SocialX = model.SocialX,
                UserId = user.Id,
                IsActive = true,
                IsDeleted = false
            };

            await _dbContext.Brand.AddAsync(brand);
            await _dbContext.SaveChangesAsync();

            var defaultCategory1 = new Category
            {
                Name = "Yiyecek",
                Description = "Yiyecek Kategorisi",
                DisplayOrder = 0,
                BrandId = brand.Id
            };
            var defaultCategory2 = new Category
            {
                Name = "İçecek",
                Description = "İçecek Kategorisi",
                DisplayOrder = 0,
                BrandId = brand.Id
            };

            await _dbContext.Category.AddAsync(defaultCategory1);
            await _dbContext.Category.AddAsync(defaultCategory2);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("ListBrands");
        }

        return View(model);
    }

    [HttpGet("EditBrand/{id}")]
    public async Task<IActionResult> EditBrand(int id)
    {
        var brand = await _dbContext.Brand.FirstOrDefaultAsync(p => p.Id == id);
        if (brand == null) return NotFound();

        var brandModel = new BrandViewModel
        {
            Name = brand.Name,
            Description = brand.Description,
            Phone = brand.Phone,
            Email = brand.Email,
            LogoPath = brand.Logo,
            Facebook = brand.Facebook,
            Instagram = brand.Instagram,
            Tiktok = brand.Tiktok,
            SocialX = brand.SocialX,
            IsActive = brand.IsActive,
        };

        return View(brandModel);
    }

    [HttpPost("EditBrand/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditBrand(BrandViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        if (ModelState.IsValid)
        {
            var brand = await _dbContext.Brand.FirstOrDefaultAsync(p => p.Id == model.Id);

            if (brand == null) return NotFound();

            var imagePaths = new List<string>();

            if (model.Logo != null && model.Logo.FileName.Any())
            {
                var guid = Guid.NewGuid();
                var fileName = Path.GetFileName(model.Logo.FileName);
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", user.Id, guid + fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Logo.CopyToAsync(stream);
                }

                imagePaths.Add($"/uploads/{user.Id}/{guid + fileName}");
                brand.Logo = imagePaths.FirstOrDefault();
            }

            brand.Name = model.Name;
            brand.Description = model.Description;
            brand.DisplayOrder = model.DisplayOrder;
            brand.Phone = model.Phone;
            brand.Email = model.Email;
            brand.Facebook = model.Facebook;
            brand.Instagram = model.Instagram;
            brand.Tiktok = model.Tiktok;
            brand.SocialX = model.SocialX;
            brand.IsActive = model.IsActive;

            _dbContext.Brand.Update(brand);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("ListCategories");
        }

        return View(model);
    }

    [HttpPost("DeleteBrand/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBrand(int id)
    {
        var selectedBrandId = HttpContext.Session.GetInt32("SelectedBrandId");
        if (selectedBrandId == null) return RedirectToAction("CreateBrand");

        var brand = await _dbContext.Brand.FirstOrDefaultAsync(p => p.Id == id);

        if (brand == null) return NotFound();

        brand.IsActive = false;
        brand.IsDeleted = true; 
        
        _dbContext.Brand.Update(brand);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("ListBrands");
    }

    #endregion

    #region Category

    [Route("ListCategories")]
    public async Task<IActionResult> ListCategories()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var brands = await _dbContext.Brand.Where(x => x.UserId == user.Id).ToListAsync();
        if (!brands.Any()) return RedirectToAction("CreateBrand");

        var selectedBrandId = HttpContext.Session.GetInt32("SelectedBrandId");
        if (selectedBrandId == 0) return RedirectToAction("CreateBrand");

        var categories = await _dbContext.Category.Include(x => x.Brand)
            .Where(x => x.Brand.UserId == user.Id && x.BrandId == selectedBrandId && x.IsDeleted == false)
            .ToListAsync();

        return View(categories);
    }

    [Route("CreateCategory")]
    public IActionResult CreateCategory()
    {
        return View();
    }

    [HttpPost("CreateCategory")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(CategoryViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        var selectedBrandId = HttpContext.Session.GetInt32("SelectedBrandId");
        if (selectedBrandId == null) return RedirectToAction("CreateBrand");

        if (ModelState.IsValid)
        {
            var brand = new Category
            {
                Name = model.Name,
                Description = model.Description,
                BrandId = selectedBrandId,
                DisplayOrder = model.DisplayOrder,
                IsActive = true,
                IsDeleted = false
            };

            await _dbContext.Category.AddAsync(brand);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("ListCategories");
        }

        return View(model);
    }

    [HttpGet("EditCategory/{id}")]
    public async Task<IActionResult> EditCategory(int id)
    {
        var category = await _dbContext.Category.FirstOrDefaultAsync(p => p.Id == id);
        if (category == null) return NotFound();

        var viewModel = new CategoryViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            DisplayOrder = category.DisplayOrder,
            IsActive = category.IsActive
        };

        return View(viewModel);
    }

    [HttpPost("EditCategory/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(CategoryViewModel model)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound();

        if (ModelState.IsValid)
        {
            var category = await _dbContext.Category.FirstOrDefaultAsync(p => p.Id == model.Id);

            if (category == null) return NotFound();

            category.Name = model.Name;
            category.Description = model.Description;
            category.DisplayOrder = model.DisplayOrder;
            category.IsActive = model.IsActive;

            _dbContext.Category.Update(category);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("ListCategories");
        }

        return View(model);
    }

    [HttpPost("DeleteCategories/{id}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategories(int id)
    {
        var selectedBrandId = HttpContext.Session.GetInt32("SelectedBrandId");
        if (selectedBrandId == null) return RedirectToAction("CreateBrand");

        var category = await _dbContext.Category.FirstOrDefaultAsync(p => p.Id == id && p.BrandId == selectedBrandId);

        if (category == null) return NotFound();
        
        category.IsActive = false;
        category.IsDeleted = true; 

        _dbContext.Category.Update(category);
        await _dbContext.SaveChangesAsync();

        return RedirectToAction("ListCategories");
    }

    #endregion

    #region Utils

    [HttpGet("GetBrandsByUserId")]
    public async Task<IActionResult> GetBrandsByUserId()
    {
        var userId = _userManager.GetUserId(User);
        var brands = await _dbContext.Brand
            .Where(b => b.UserId == userId)
            .Select(b => new { b.Id, b.Name })
            .ToListAsync();

        var selectedBrandId = HttpContext.Session.GetInt32("SelectedBrandId");

        return Json(new { brands, selectedBrandId });
    }

    [IgnoreAntiforgeryToken]
    [HttpPost("SaveSelectedBrandId")]
    public IActionResult SaveSelectedBrandId([FromBody] BrandIdModel model)
    {
        HttpContext.Session.SetInt32("SelectedBrandId", model.BrandId);

        return Json(new { success = true });
    }

    [HttpPost("DeleteImage")]
    public async Task<IActionResult> DeleteImage(int id, string imagePath)
    {
        var selectedBrandId = HttpContext.Session.GetInt32("SelectedBrandId");
        if (selectedBrandId == null) return RedirectToAction("CreateBrand");

        var product = await _dbContext.Product.FirstOrDefaultAsync(p => p.Id == id && p.BrandId == selectedBrandId);

        if (product == null) return NotFound();

        var imagePaths = product.ImagePaths;

        if (imagePaths.Contains(imagePath))
        {
            imagePaths.Remove(imagePath);
            product.ImagePaths = imagePaths;

            _dbContext.Product.Update(product);
            await _dbContext.SaveChangesAsync();
        }

        return Json(new { success = true });
    }

    #endregion
}
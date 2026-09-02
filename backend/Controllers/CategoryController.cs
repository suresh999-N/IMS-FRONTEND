using IMSBackend.Contracts;
using IMSBackend.Data;
using IMSBackend.DTOs;
using IMSBackend.Models;
using IMSBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuditLogService _auditLogService;

        public CategoryController(AppDbContext context, AuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var categories = await _context.Categories
                .Include(category => category.SubCategories.Where(subCategory => !subCategory.IsDeleted))
                .AsNoTracking()
                .Where(category => !category.IsDeleted)
                .OrderBy(category => category.Name)
                .ToListAsync();
            var totalSubCategories = await _context.SubCategories
                .AsNoTracking()
                .Where(subCategory => !subCategory.IsDeleted)
                .CountAsync();

            var response = new CategoryListResponseDto
            {
                TotalCategories = categories.Count,
                TotalSubCategories = totalSubCategories,
                CategoriesWithChildrenCount = categories.Count(category => category.SubCategories.Any()),
                Categories = categories.Select(c => ToCategoryResponse(c, categories)).ToList()
            };

            return Ok(ApiResponse<CategoryListResponseDto>.Ok(
                response,
                traceId: HttpContext.TraceIdentifier));
        }

        [HttpGet("main")]
        public async Task<IActionResult> GetMain(CancellationToken cancellationToken)
        {
            var categories = await _context.Categories
                .Include(category => category.SubCategories.Where(subCategory => !subCategory.IsDeleted))
                .AsNoTracking()
                .Where(category => category.ParentId == null && !category.IsDeleted)
                .OrderBy(category => category.Name)
                .ToListAsync(cancellationToken);

            return Ok(ApiResponse<object>.Ok(
                new { categories = categories.Select(c => ToCategoryResponse(c, categories)).ToList() },
                traceId: HttpContext.TraceIdentifier));
        }

        [HttpGet("sub/{parentId:int}")]
        public async Task<IActionResult> GetSub(int parentId, CancellationToken cancellationToken)
        {
            var parentExists = await _context.Categories
                .AsNoTracking()
                .AnyAsync(category => category.CategoryId == parentId && !category.IsDeleted, cancellationToken);

            if (!parentExists)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Category not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var subCategories = await _context.SubCategories
                .Include(subCategory => subCategory.Category)
                .AsNoTracking()
                .Where(subCategory => subCategory.CategoryId == parentId && !subCategory.IsDeleted)
                .OrderBy(subCategory => subCategory.Name)
                .ToListAsync(cancellationToken);

            var children = subCategories.Select(ToChildSubCategoryResponse).ToList();

            return Ok(ApiResponse<object>.Ok(
                new
                {
                    parentId,
                    totalSubCategories = children.Count,
                    subCategories = children,
                    categories = children
                },
                traceId: HttpContext.TraceIdentifier));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryDto dto, CancellationToken cancellationToken)
        {
            var validationMessage = await ValidateCategoryDto(dto, null, cancellationToken);
            if (validationMessage != null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    validationMessage,
                    traceId: HttpContext.TraceIdentifier));
            }

            var category = new Category
            {
                Name = Clean(dto.Name),
                ParentId = dto.ParentId,
                Description = Clean(dto.Description)
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync(
                "CREATE_CATEGORY",
                "Categories",
                category.CategoryId,
                $"Category {category.Name} created",
                "categories",
                cancellationToken);

            var savedCategory = await _context.Categories
                .Include(item => item.SubCategories.Where(subCategory => !subCategory.IsDeleted))
                .AsNoTracking()
                .FirstAsync(item => item.CategoryId == category.CategoryId, cancellationToken);

            return Ok(ApiResponse<CategoryResponseDto>.Ok(
                ToCategoryResponse(savedCategory),
                "Category created successfully.",
                HttpContext.TraceIdentifier));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, CategoryDto dto, CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(item => item.CategoryId == id && !item.IsDeleted, cancellationToken);

            if (category == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Category not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var validationMessage = await ValidateCategoryDto(dto, id, cancellationToken);
            if (validationMessage != null)
            {
                return BadRequest(ApiResponse<object>.Fail(
                    validationMessage,
                    traceId: HttpContext.TraceIdentifier));
            }

            category.Name = Clean(dto.Name);
            category.ParentId = dto.ParentId;
            category.Description = Clean(dto.Description);

            await _context.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync(
                "UPDATE_CATEGORY",
                "Categories",
                category.CategoryId,
                $"Category {category.Name} updated",
                "categories",
                cancellationToken);

            var savedCategory = await _context.Categories
                .Include(item => item.SubCategories.Where(subCategory => !subCategory.IsDeleted))
                .AsNoTracking()
                .FirstAsync(item => item.CategoryId == id, cancellationToken);

            return Ok(ApiResponse<CategoryResponseDto>.Ok(
                ToCategoryResponse(savedCategory),
                "Category updated successfully.",
                HttpContext.TraceIdentifier));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            var category = await _context.Categories
                .Include(item => item.SubCategories.Where(subCategory => !subCategory.IsDeleted))
                .FirstOrDefaultAsync(item => item.CategoryId == id && !item.IsDeleted, cancellationToken);

            if (category == null)
            {
                return NotFound(ApiResponse<object>.Fail(
                    "Category not found.",
                    traceId: HttpContext.TraceIdentifier));
            }

            if (category.SubCategories.Any())
            {
                return Conflict(ApiResponse<object>.Fail(
                    "Delete or move subcategories before deleting this category.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var hasChildCategories = await _context.Categories
                .AsNoTracking()
                .AnyAsync(item => item.ParentId == id && !item.IsDeleted, cancellationToken);

            if (hasChildCategories)
            {
                return Conflict(ApiResponse<object>.Fail(
                    "Delete or move child categories before deleting this category.",
                    traceId: HttpContext.TraceIdentifier));
            }

            var hasProducts = await _context.Products
                .AsNoTracking()
                .AnyAsync(item => item.CategoryId == id && !item.IsDeleted, cancellationToken);

            if (hasProducts)
            {
                return Conflict(ApiResponse<object>.Fail(
                    "This category cannot be deleted because products are assigned to it.",
                    traceId: HttpContext.TraceIdentifier));
            }

            category.IsDeleted = true;
            await _context.SaveChangesAsync(cancellationToken);

            await _auditLogService.LogAsync(
                "DELETE_CATEGORY",
                "Categories",
                category.CategoryId,
                $"Category {category.Name} deleted",
                "categories",
                cancellationToken);

            return Ok(ApiResponse<object>.Ok(
                new { categoryId = id },
                "Category deleted successfully.",
                HttpContext.TraceIdentifier));
        }

        private async Task<string?> ValidateCategoryDto(
            CategoryDto dto,
            int? categoryId,
            CancellationToken cancellationToken)
        {
            var name = Clean(dto.Name);

            if (string.IsNullOrWhiteSpace(name))
            {
                return "Category name is required.";
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(name, @"^[A-Za-z\s]+$"))
            {
                return "Name can contain only letters and spaces.";
            }

            if (dto.ParentId.HasValue)
            {
                if (categoryId.HasValue && dto.ParentId.Value == categoryId.Value)
                {
                    return "A category cannot be its own parent.";
                }

                var parentExists = await _context.Categories
                    .AsNoTracking()
                    .AnyAsync(item => item.CategoryId == dto.ParentId.Value && !item.IsDeleted, cancellationToken);

                if (!parentExists)
                {
                    return "Selected parent category was not found.";
                }
            }

            var duplicateExists = await _context.Categories
                .AsNoTracking()
                .AnyAsync(item =>
                    !item.IsDeleted &&
                    item.CategoryId != categoryId &&
                    item.ParentId == dto.ParentId &&
                    item.Name.ToLower() == name.ToLower(),
                    cancellationToken);

            return duplicateExists
                ? "A category with this name already exists under the selected parent."
                : null;
        }

        private static CategoryResponseDto ToCategoryResponse(Category category, List<Category>? allCategories = null)
        {
            var subCatChildren = category.SubCategories
                .Where(subCategory => !subCategory.IsDeleted)
                .OrderBy(subCategory => subCategory.Name)
                .Select(subCategory =>
                {
                    var child = ToChildSubCategoryResponse(subCategory);
                    child.CategoryName = category.Name;
                    return child;
                })
                .ToList();

            var categoryChildren = allCategories != null
                ? allCategories
                    .Where(c => c.ParentId == category.CategoryId && !c.IsDeleted)
                    .Select(c => new ChildSubCategoryDto
                    {
                        SubCategoryId = c.CategoryId,
                        CategoryId = category.CategoryId,
                        Name = c.Name,
                        Description = c.Description,
                        Status = "Active",
                        CreatedAt = DateTime.UtcNow,
                        CategoryName = category.Name
                    })
                    .ToList()
                : new List<ChildSubCategoryDto>();

            var combinedChildren = subCatChildren
                .Concat(categoryChildren.Where(cc => !subCatChildren.Any(sc => sc.Name.Equals(cc.Name, StringComparison.OrdinalIgnoreCase))))
                .ToList();

            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                Name = category.Name,
                ParentId = category.ParentId,
                Description = category.Description,
                Status = "Active",
                SubcategoryCount = combinedChildren.Count,
                ChildSubCategories = combinedChildren
            };
        }

        private static ChildSubCategoryDto ToChildSubCategoryResponse(SubCategory subCategory)
        {
            return new ChildSubCategoryDto
            {
                SubCategoryId = subCategory.SubCategoryId,
                CategoryId = subCategory.CategoryId,
                Name = subCategory.Name,
                Description = subCategory.Description,
                Status = NormalizeStatus(subCategory.Status),
                CreatedAt = subCategory.CreatedAt,
                CategoryName = subCategory.Category?.Name
            };
        }

        private static string Clean(string? value)
            => string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();

        private static string NormalizeStatus(string? value)
        {
            var status = Clean(value);
            return string.IsNullOrWhiteSpace(status)
                ? "Active"
                : char.ToUpperInvariant(status[0]) + status[1..].ToLowerInvariant();
        }
    }
}

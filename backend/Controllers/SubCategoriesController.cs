using IMSBackend.Data;
using IMSBackend.Models;
using IMSBackend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IMSBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubCategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly AuditLogService _auditLogService;

        public SubCategoriesController(AppDbContext context, AuditLogService auditLogService)
        {
            _context = context;
            _auditLogService = auditLogService;
        }

        // =====================================
        // GET ALL SUBCATEGORIES
        // =====================================
        [HttpGet]
        public async Task<IActionResult> GetSubCategories(
            int page = 1,
            int pageSize = 500,
            string? search = null,
            string? status = null)
        {
            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 500);

            var query =
                from sub in _context.SubCategories
                join cat in _context.Categories
                    on sub.CategoryId equals cat.CategoryId
                where !sub.IsDeleted && !cat.IsDeleted

                select new
                {
                    sub.SubCategoryId,
                    sub.Name,
                    sub.Description,
                    sub.Status,
                    sub.CreatedAt,

                    CategoryId = cat.CategoryId,
                    CategoryName = cat.Name
                };

            // ================= STATUS FILTER =================

            if (!string.IsNullOrWhiteSpace(status) && !status.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                var normStatus = status.Trim().ToLower();
                if (normStatus == "active")
                {
                    query = query.Where(x => string.IsNullOrEmpty(x.Status) || x.Status.ToLower() == "active");
                }
                else
                {
                    query = query.Where(x => x.Status != null && x.Status.ToLower() == normStatus);
                }
            }

            // ================= SEARCH =================

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                query = query.Where(x =>
                    x.Name.ToLower().Contains(search) ||
                    x.CategoryName.ToLower().Contains(search));
            }

            // ================= TOTAL =================

            var totalRecords =
                await query.CountAsync();

            // ================= PAGINATION =================

            var data = await query
                .OrderByDescending(x => x.SubCategoryId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(new
            {
                page,
                pageSize,
                totalRecords,

                totalPages =
                    (int)Math.Ceiling(
                        (double)totalRecords / pageSize),

                data
            });
        }

        // =====================================
        // GET BY ID
        // =====================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            var data = await (
                from sub in _context.SubCategories
                join cat in _context.Categories
                    on sub.CategoryId equals cat.CategoryId

                where sub.SubCategoryId == id
                where !sub.IsDeleted && !cat.IsDeleted

                select new
                {
                    sub.SubCategoryId,
                    sub.Name,
                    sub.Description,
                    sub.Status,
                    sub.CreatedAt,

                    CategoryId = cat.CategoryId,
                    CategoryName = cat.Name
                }
            ).FirstOrDefaultAsync();

            if (data == null)
            {
                return NotFound(new
                {
                    message = "SubCategory not found"
                });
            }

            return Ok(data);
        }

        // =====================================
        // CREATE
        // =====================================
        [HttpPost]
        public async Task<IActionResult> CreateSubCategory(
            SubCategory model)
        {
            if (string.IsNullOrWhiteSpace(model.Name) || !System.Text.RegularExpressions.Regex.IsMatch(model.Name.Trim(), @"^[A-Za-z\s]+$"))
            {
                return BadRequest(new { message = "Name can contain only letters and spaces." });
            }
            var categoryExists = await _context.Categories
                .AsNoTracking()
                .AnyAsync(category =>
                    category.CategoryId == model.CategoryId &&
                    !category.IsDeleted);

            if (!categoryExists)
            {
                return BadRequest(new
                {
                    message = "Selected category was not found."
                });
            }

            model.CreatedAt = DateTime.Now;
            model.IsDeleted = false;
            if (string.IsNullOrWhiteSpace(model.Status))
            {
                model.Status = "Active";
            }

            _context.SubCategories.Add(model);

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "CREATE_SUBCATEGORY",
                "Sub Categories",
                model.SubCategoryId,
                $"Sub Category {model.Name} created",
                "sub_categories");

            return Ok(new
            {
                message =
                    "SubCategory created successfully"
            });
        }

        // =====================================
        // UPDATE
        // =====================================
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubCategory(
            int id,
            SubCategory model)
        {
            var subCategory =
                await _context.SubCategories
                    .FirstOrDefaultAsync(x =>
                        x.SubCategoryId == id &&
                        !x.IsDeleted);

            if (subCategory == null)
            {
                return NotFound(new
                {
                    message = "SubCategory not found"
                });
            }

            if (string.IsNullOrWhiteSpace(model.Name) || !System.Text.RegularExpressions.Regex.IsMatch(model.Name.Trim(), @"^[A-Za-z\s]+$"))
            {
                return BadRequest(new { message = "Name can contain only letters and spaces." });
            }

            var categoryExists = await _context.Categories
                .AsNoTracking()
                .AnyAsync(category =>
                    category.CategoryId == model.CategoryId &&
                    !category.IsDeleted);

            if (!categoryExists)
            {
                return BadRequest(new
                {
                    message = "Selected category was not found."
                });
            }

            subCategory.CategoryId =
                model.CategoryId;

            subCategory.Name =
                model.Name;

            subCategory.Description =
                model.Description;

            subCategory.Status =
                model.Status;

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "UPDATE_SUBCATEGORY",
                "Sub Categories",
                subCategory.SubCategoryId,
                $"Sub Category {subCategory.Name} updated",
                "sub_categories");

            return Ok(new
            {
                message =
                    "SubCategory updated successfully"
            });
        }

        // =====================================
        // DELETE
        // =====================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubCategory(
            int id)
        {
            var subCategory =
                await _context.SubCategories
                    .FirstOrDefaultAsync(x =>
                        x.SubCategoryId == id &&
                        !x.IsDeleted);

            if (subCategory == null)
            {
                return NotFound(new
                {
                    message = "SubCategory not found"
                });
            }

            var hasActiveProducts = await _context.Products
                .AsNoTracking()
                .AnyAsync(product =>
                    product.SubCategoryId == id &&
                    !product.IsDeleted);

            if (hasActiveProducts)
            {
                return Conflict(new
                {
                    message = "This subcategory cannot be deleted because active products are linked to it."
                });
            }

            subCategory.IsDeleted = true;

            await _context.SaveChangesAsync();

            await _auditLogService.LogAsync(
                "DELETE_SUBCATEGORY",
                "Sub Categories",
                subCategory.SubCategoryId,
                $"Sub Category {subCategory.Name} deleted",
                "sub_categories");

            return Ok(new
            {
                message =
                    "SubCategory deleted successfully"
            });
        }
    }
}

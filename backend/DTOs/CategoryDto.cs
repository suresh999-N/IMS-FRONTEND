using System.ComponentModel.DataAnnotations;

namespace IMSBackend.DTOs
{
    public class CategoryDto
    {
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Name can contain only letters and spaces.")]
        public string Name { get; set; } = string.Empty;

        public int? ParentId { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = "Active";
    }

    public sealed class CategoryListResponseDto
    {
        public int TotalCategories { get; set; }

        public int TotalSubCategories { get; set; }

        public int CategoriesWithChildrenCount { get; set; }

        public List<CategoryResponseDto> Categories { get; set; } = new();
    }

    public sealed class CategoryResponseDto
    {
        public int CategoryId { get; set; }

        public int Id => CategoryId;

        public string Name { get; set; } = string.Empty;

        public int? ParentId { get; set; }

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = "Active";

        public int SubcategoryCount { get; set; }

        public List<ChildSubCategoryDto> ChildSubCategories { get; set; } = new();
    }

    public sealed class ChildSubCategoryDto
    {
        public int SubCategoryId { get; set; }

        public int Id => SubCategoryId;

        public int CategoryId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; }

        public string? CategoryName { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMSBackend.Models
{
    [Table("sub_categories")]
    public class SubCategory
    {
        [Key]
        [Column("sub_category_id")]
        public int SubCategoryId { get; set; }

        [Column("category_id")]
        public int CategoryId { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; } = false;

        // =========================
        // NAVIGATION PROPERTY
        // =========================

        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
}

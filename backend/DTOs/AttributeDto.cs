using System.ComponentModel.DataAnnotations;

public class ProductAttributeDto
{
    public int? AttributeId { get; set; }

    [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Name can contain only letters and spaces.")]
    public string Name { get; set; } = string.Empty;
}
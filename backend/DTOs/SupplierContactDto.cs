namespace IMSBackend.DTOs.Suppliers
{
    public class SupplierContactDto
    {
        public string? Name { get; set; }

        public string? Designation { get; set; }

        public string? Department { get; set; }

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public bool IsPrimary { get; set; }
    }
}
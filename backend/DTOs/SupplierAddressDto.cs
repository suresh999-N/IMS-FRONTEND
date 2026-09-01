namespace IMSBackend.DTOs.Suppliers
{
    public class SupplierAddressDto
    {
        public string? AddressType { get; set; }

        public string? AddressLine { get; set; }

        public string? City { get; set; }

        public string? State { get; set; }

        public string? Country { get; set; }

        public string? Pincode { get; set; }
    }
}
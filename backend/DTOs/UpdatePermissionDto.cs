namespace IMSBackend.DTOs
{
    public class UpdatePermissionDto
    {
        public int PermissionId { get; set; }

        public bool CanView { get; set; }

        public bool CanAdd { get; set; }

        public bool CanEdit { get; set; }

        public bool CanDelete { get; set; }
    }
}
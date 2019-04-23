namespace AppIdentity
{
    public class RegistrationResponse
    {
        public bool DuplicateEmail { get; set; }
        public bool DuplicateDisplayName { get; set; }
        public bool Success { get; set; }
    }
}
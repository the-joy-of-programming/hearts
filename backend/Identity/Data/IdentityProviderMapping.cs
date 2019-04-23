namespace AppIdentity
{
    public class IdentityProviderMapping
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public string IdentityProviderName { get; set; }
        public string IdentityProviderSub { get; set; }
    }
}
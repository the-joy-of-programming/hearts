using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AppIdentity
{
    public class IdentityService
    {

        private readonly IdentityContext identityContext;

        public IdentityService(IdentityContext identityContext)
        {
            this.identityContext = identityContext;
        }
        
        public async Task<User> Login(string providerName, string providerSub)
        {
            var mapping = await identityContext
             .IdentityProviderMappings
             .Include(c => c.User)
             .FirstOrDefaultAsync(c => c.IdentityProviderName == providerName && c.IdentityProviderSub == providerSub);
            if (mapping == null)
            {
                return null;
            } else
            {
                return mapping.User;
            }
        }

        public async Task<RegistrationResult> Register(string providerName, string providerSub, string displayName, string email)
        {
            var nameTaken = await identityContext.Users.AnyAsync(u => u.DisplayName.ToLower() == displayName.ToLower());
            var emailTaken = await identityContext.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
            if (nameTaken || emailTaken)
            {
                return RegistrationResult.Failure(nameTaken, emailTaken);
            }

            var user = new User { DisplayName = displayName, Email = email };
            await identityContext.Users.AddAsync(user);
            await identityContext.IdentityProviderMappings.AddAsync(new IdentityProviderMapping { User = user, IdentityProviderName = providerName, IdentityProviderSub = providerSub });
            await identityContext.SaveChangesAsync();

            return RegistrationResult.Success(user);            
        }

    }
}
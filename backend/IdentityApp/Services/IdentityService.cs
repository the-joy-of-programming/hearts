using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace IdentityApp
{
    public class IdentityService
    {

        private const string NoOneDisplayName = "noone";

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

        public async Task<User> LoginAsNoOne()
        {
            var user = await identityContext
            .Users
            .FirstOrDefaultAsync(u => u.DisplayName == NoOneDisplayName);
            if (user == null)
            {
                user = new User { DisplayName = NoOneDisplayName, Email = "noone@nowhere.com" };
                await identityContext.AddAsync(user);
                await identityContext.SaveChangesAsync();
            }
            return user;
        }

        public Task<User> GetUserById(int userId)
        {
            return identityContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<RegistrationResult> Register(string providerName, string providerSub, string displayName, string email)
        {
            var nameTaken = displayName.ToLower() == NoOneDisplayName || await identityContext.Users.AnyAsync(u => u.DisplayName.ToLower() == displayName.ToLower());
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
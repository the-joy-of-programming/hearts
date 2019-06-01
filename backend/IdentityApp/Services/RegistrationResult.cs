namespace IdentityApp
{
    public readonly struct RegistrationResult
    {
        public bool DuplicateName { get; }
        public bool DuplicateEmail { get; }
        public User NewUser { get; }

        private RegistrationResult(bool duplicateName, bool duplicateEmail, User newUser)
        {
            DuplicateName = duplicateName;
            DuplicateEmail = duplicateEmail;
            NewUser = newUser;
        }

        public static RegistrationResult Success(User newUser)
        {
            return new RegistrationResult(false, false, newUser);
        }

        public static RegistrationResult Failure(bool duplicateName, bool duplicateEmail)
        {
            return new RegistrationResult(duplicateName, duplicateEmail, null);
        }
    }
}
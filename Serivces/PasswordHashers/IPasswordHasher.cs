namespace AuthenticationServer.API.Serivces.PasswordHasher
{
    public interface IPasswordHasher
    {
        public string HashPassword(string password);
        bool VerifyPassword(string password, string passwordHash);
    }
}

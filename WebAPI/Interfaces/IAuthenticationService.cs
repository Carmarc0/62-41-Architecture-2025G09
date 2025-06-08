namespace WebAPI.Interfaces
{
    public interface IAuthenticationService
    {
        Task<bool> Authenticate(string username, string password);
        Task<bool> AuthenticateAdmin(string username, string password);
        Task<bool> AuthenticateStudent(string username, string password);
    }
}
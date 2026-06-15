namespace WebApi.Services
{
    public interface ILoginService
    {
        Task<string> ValidateUserLogin(string login, string password);
        Task<bool> CheckJwtToken(string token);
    }
}

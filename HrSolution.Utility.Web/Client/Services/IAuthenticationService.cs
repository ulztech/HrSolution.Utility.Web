using HrSolution.Common.Shared.Jwt;

namespace HrSolution.Utility.Web.Client.Services
{
    public interface IAuthenticationService
    {
        UserModel User { get; }
        Task Initialize();
        Task Login(string username, string password);
        Task Logout();
    }
}

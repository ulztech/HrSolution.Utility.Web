
using HrSolution.Common.Shared.Jwt;

namespace HrSolution.Utility.Web.Client.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserModel>> GetAll();
    }
}


using HrSolution.Common.Shared.Jwt;

namespace HrSolution.Utility.Web.Client.Services
{ 
    public class UserService : IUserService
    {
        private IHttpService _httpService;

        public UserService(IHttpService httpService)
        {
            _httpService = httpService;
        }

        public async Task<IEnumerable<UserModel>> GetAll()
        {
            return await _httpService.Get<IEnumerable<UserModel>>("/users");
        }
    }
}
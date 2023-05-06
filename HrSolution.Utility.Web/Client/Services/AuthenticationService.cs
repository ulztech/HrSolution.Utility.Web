using HrSolution.Common.Shared.Jwt;
using Microsoft.AspNetCore.Components; 

namespace HrSolution.Utility.Web.Client.Services
{ 
    public class AuthenticationService : IAuthenticationService
    {
        private IHttpService _httpService;
        private NavigationManager _navigationManager;
        private ILocalStorageService _localStorageService;

        public UserModel User { get; private set; }

        public AuthenticationService(
            IHttpService httpService,
            NavigationManager navigationManager,
            ILocalStorageService localStorageService
        ) {
            _httpService = httpService;
            _navigationManager = navigationManager;
            _localStorageService = localStorageService;
        }

        public async Task Initialize()
        {
            User = await _localStorageService.GetItem<UserModel>("user");
        }

        public async Task Login(string username, string password)
        {
            var result = await _httpService.Post<LoginResult>($"/auth/login/{username}/{password}", null);

            if (result != null && result.success== true)
            {
                User = result.user;

                await _localStorageService.SetItem("user", User);
            }
            else
            {
                User = null;
            }
        }

        public async Task Logout()
        {
            User = null;
            await _localStorageService.RemoveItem("user");
            _navigationManager.NavigateTo("login");
        }
    }
}
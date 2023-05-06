using HrSolution.Common.Shared;

namespace HrSolution.Utility.Web.Server.Helpers
{
    public class AppHostEnvironment : IAppHostEnvironment
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContext;
        public AppHostEnvironment(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _httpContext = httpContext;
        }

        public string ContentRootPath()
        {
            return _webHostEnvironment.ContentRootPath;
        }

        public string WebRootPath()
        {
            return _webHostEnvironment.WebRootPath;
        }

        public string BaseUrl()
        {
            var request = _httpContext?.HttpContext?.Request;

            if (request != null)
            {
                return $"{request.Scheme}://{request.Host}{request.PathBase}";
            }

            return "/";
        }

        public string Controller()
        {
            var controller = _httpContext?.HttpContext?.Request?.RouteValues["controller"]?.ToString() ?? string.Empty;

            return controller;
        }

        public string Action()
        {
            var action = _httpContext?.HttpContext?.Request?.RouteValues["action"]?.ToString() ?? string.Empty;

            return action;
        }
        public string Area()
        {
            var area = _httpContext?.HttpContext?.Request?.RouteValues["area"]?.ToString() ?? string.Empty;

            return area;
        }
    }
}

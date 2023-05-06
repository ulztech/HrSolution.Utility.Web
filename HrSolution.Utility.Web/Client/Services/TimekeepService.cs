using HrSolution.Dto.Models.Timekeep; 

namespace HrSolution.Utility.Web.Client.Services
{
    public class TimekeepService : ITimekeepService
    {
        private readonly IHttpService _httpService;

        private readonly ILogger<TimekeepService> _logger;

        private readonly ILocalStorageService _storage;

        private TimeInOutResultDto result = new();

        public TimekeepService(IHttpService httpService, ILogger<TimekeepService> logger, ILocalStorageService storage)
        {
            _httpService = httpService;
            _logger = logger;
            _storage = storage;
        }

        public TimeInOutResultDto ConfirmClockInOut(TimeInOutDto timeInOutInfo)
        {
            try
            {
                var url = "";

                if (timeInOutInfo.IsClockIn)
                {
                    url = $"ThirdParty/timein/{timeInOutInfo.EmpCode}/true/{(timeInOutInfo.DateIn).GetValueOrDefault().ToString("yyyy-MM-dd hh:mm tt")}";
                }
                else
                {
                    url = $"ThirdParty/timein/{timeInOutInfo.EmpCode}/false/{(timeInOutInfo.DateOut).GetValueOrDefault().ToString("yyyy-MM-dd hh:mm tt")}";
                }

                _httpService.Post(url, timeInOutInfo).GetAwaiter();
                 
                return new TimeInOutResultDto() { Status = true, Message = "Data has been saved!" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reading this ID {timeInOutInfo.EmpCode}");
            }

            return new();
        }
    }
}

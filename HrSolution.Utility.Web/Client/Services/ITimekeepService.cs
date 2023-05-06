using HrSolution.Dto.Models.Timekeep;

namespace HrSolution.Utility.Web.Client.Services
{
    public interface ITimekeepService
    {
        TimeInOutResultDto ConfirmClockInOut(TimeInOutDto timeInOutInfo);
    }
}
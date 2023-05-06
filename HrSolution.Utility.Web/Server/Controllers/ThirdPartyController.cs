using HrSolution.Common;
using HrSolution.Common.Enum; 
using HrSolution.Common.Shared.Contract; 
using HrSolution.Data;
using HrSolution.Domain.Manager;
using HrSolution.Dto;
using HrSolution.Dto.Models.Timekeep;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc; 
using Threenine.Data; 

namespace HrSolution.Utility.Web.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ThirdPartyController : ControllerBase
    {
        private readonly IHrDataConnection _dbConnection;
        private readonly DateTimeFunction _dateTimeFunction;
        private readonly AttendanceManager _attendanceManager;
        private readonly IFileService _fileService;
        private readonly IMapper _mapper;
        public ThirdPartyController(IHrDataConnection dbConnection, IMapper mapper, DateTimeFunction dateTimeFunction, AttendanceManager attendanceManager, IFileService fileService)
        {
            _dbConnection = dbConnection;
            _mapper = mapper;
            _dateTimeFunction = dateTimeFunction;
            _attendanceManager = attendanceManager;
            _fileService = fileService;
        }

        [HttpGet("GetEmployees")]
        public IList<hremp> GetEmployees()
        {
            using (var uow = new UnitOfWork<UixeDbContext>(_dbConnection.Context))
            {
                return uow.Context.Hremps.Select(m => _mapper.Map<hremp>(m)).ToList();
            }
        }

        [HttpGet("GetEmployee/{code}")]
        public IActionResult GetEmployee(string code)
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                using (var uow = new UnitOfWork<UixeDbContext>(_dbConnection.Context))
                {
                    var result = uow.Context.Hremps.Where(m => m.HrempCode.ToLower() == code.ToLower()).Select(m => _mapper.Map<hremp>(m)).FirstOrDefault();

                    if (result != null)
                    {
                        return Ok(result);
                    }
                }
            }

            return BadRequest();
        }

        [HttpGet("Scan/{code}")]
        public IActionResult ScanQrCode(string code)
        {
            var currentDateTime = _dateTimeFunction.ConvertUtcTimeToLocalTimeZone(DateTime.UtcNow);

            TimeInOutDto result = new();


            Hrtimelog? timeLog = null;
            hremp? emp = null;  

            if (!string.IsNullOrWhiteSpace(code))
            {
                using (var uow = new UnitOfWork<UixeDbContext>(_dbConnection.Context))
                {
                    emp = uow.Context.Hremps.Where(m => m.HrempCode.ToLower() == code.ToLower()).Select(m => _mapper.Map<hremp>(m)).FirstOrDefault();

                    if (emp != null)
                    {
                        result.EmpCode = emp.hremp_code;
                        result.EmpName = emp.hremp_name;
                        result.ImageId = emp.hremp_imageid;

                        timeLog = uow.Context.Hrtimelogs.Where(m => m.HrtimelogHrempId == emp.hremp_id && m.HrtimelogDateout == null).OrderByDescending(m => m.HrtimelogDatein).FirstOrDefault();
                        
                        if (timeLog != null && currentDateTime.Subtract(timeLog.HrtimelogDatein + timeLog.HrtimelogTimein).Hours > 20)
                        {
                            result.IsClockIn= true;
                            result.DateIn = currentDateTime;
                            result.TimeIn = currentDateTime.TimeOfDay;
                            result.TimeInInfo = $"{currentDateTime.ToString("hh:mm tt").ToUpper()} [ {currentDateTime.ToString("MMMM dd, yyyy").ToUpper()} ]";
                        }
                        else if (timeLog != null && currentDateTime.Subtract(timeLog.HrtimelogDatein + timeLog.HrtimelogTimein).TotalMinutes < 16)
                        {
                            result.SkipSaving = true;
                            result.IsClockIn = false;
                            result.DateIn = timeLog.HrtimelogDatein;
                            result.TimeIn = timeLog.HrtimelogTimein;
                            result.TimeInInfo = $"{(timeLog.HrtimelogDatein + timeLog.HrtimelogTimein).ToString("hh:mm tt").ToUpper()} [ {timeLog.HrtimelogDatein.ToString("MMMM dd, yyyy").ToUpper()} ]";
                            result.Message = "Minimum of 15 minutes interval between clockin is required.";
                        }
                        else if (timeLog != null)
                        {
                            result.IsClockIn = false;
                            result.DateIn = timeLog.HrtimelogDatein;
                            result.TimeIn = timeLog.HrtimelogTimein;
                            result.DateOut = currentDateTime;
                            result.TimeOut = currentDateTime.TimeOfDay;
                            result.TimeInInfo = $"{(timeLog.HrtimelogDatein + timeLog.HrtimelogTimein).ToString("hh:mm tt").ToUpper()} [ {timeLog.HrtimelogDatein.ToString("MMMM dd, yyyy").ToUpper()} ]";
                            result.TimeOutInfo = $"{currentDateTime.ToString("hh:mm tt").ToUpper()} [ {currentDateTime.ToString("MMMM dd, yyyy").ToUpper()} ]";
                        }
                        else
                        {
                            result.IsClockIn = true;
                            result.DateIn = currentDateTime;
                            result.TimeIn = currentDateTime.TimeOfDay;
                            result.TimeInInfo = $"{currentDateTime.ToString("hh:mm tt").ToUpper()} [ {currentDateTime.ToString("MMMM dd, yyyy").ToUpper()} ]";
                       }

                        result.ImageId = emp.hremp_imageid;
                    }
                    else
                    {
                        result.HasError = true;
                    }

                    return Ok(result);
                } 
            }

            return BadRequest();
        }

        [HttpGet("GetServerTime")]
        public string GetServerTime()
        {
            var currentDateTime = DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt");

            return currentDateTime;
        }

        [HttpPost("timein/{empCode}/{isClockIn}/{date}")]  
        public TimeInOutResultDto TimeIn(string empCode, bool isClockIn, DateTime date)
        { 
            var currentDateTime = _dateTimeFunction.ConvertUtcTimeToLocalTimeZone(DateTime.UtcNow);

            Hrtimelog? timeLog = null;
            hremp? emp = null; 

            using (var uow = new UnitOfWork<UixeDbContext>(_dbConnection.Context))
            {
                var entity = uow.Context.Hremps.Where(m => m.HrempCode.ToLower().Equals(empCode.ToLower())).FirstOrDefault();

                if (entity != null)
                {
                    emp = _mapper.Map<hremp>(entity);

                   timeLog =  uow.Context.Hrtimelogs
                        .Where(m => m.HrtimelogHrempId == emp.hremp_id && m.HrtimelogDateout == null).OrderByDescending(m => m.HrtimelogDatein).FirstOrDefault();  
                } 
            }

            if (emp != null)
            {
                using (var uow = new UnitOfWork<UixeDbContext>(_dbConnection.Context))
                { 
                        if (timeLog != null && !isClockIn)
                        {
                            timeLog.HrtimelogTimeout = date.TimeOfDay;
                            timeLog.HrtimelogDateout = date;
                            timeLog.HrtimelogEdited = currentDateTime;
                            timeLog.HrtimelogEditedby = empCode;

                            uow.Context.Hrtimelogs.Update(timeLog); 
                        }
                        else if (isClockIn)
                        {
                            timeLog = new Hrtimelog()
                            {
                                HrtimelogTimeout = null,
                                HrtimelogDateout = null,
                                HrtimelogHrempId = emp.hremp_id,
                                HrtimelogCreated = currentDateTime,
                                HrtimelogCreatedby = empCode,
                                HrtimelogTimein = date.TimeOfDay,
                                HrtimelogDatein = date
                            };

                            uow.Context.Hrtimelogs.Add(timeLog); 
                        }

                        uow.Commit(); 
                }

                return new TimeInOutResultDto()
                {
                    Status = true,
                    //EmpCode = emp.hremp_code,
                    //EmpName = emp.hremp_name,
                    //HasError = false,
                    Message = $"Data has been saved"
                }; 
            }

            return new TimeInOutResultDto(); 
        }
         
        [HttpGet("GetAttendance")]
        public IActionResult GetAttendance()
        {  
            var result = _attendanceManager.GetAll();

            return Ok(result);
        }

        [HttpGet("GetImage/{imageId}")]
        public async Task<ActionResult> GetImageAsync(string imageId)
        {
            var imageName = string.IsNullOrEmpty(imageId ?? "") ? GlobalGuid.DefaultImageKey : imageId.Trim();

            var filePath = _fileService.GetFilePath(FileTypeEnum.ImageFile, imageName);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            return File(memory, "image/jpeg");


            //return File(_fileService.GetFilePath(FileTypeEnum.ImageFile, imageName), "image/jpeg"); 
        }
    }
}

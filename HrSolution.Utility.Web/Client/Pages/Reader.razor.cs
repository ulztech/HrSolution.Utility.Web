using BlazorBarcodeScanner.ZXing.JS; 
using HrSolution.Dto;
using HrSolution.Dto.Models.Timekeep;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.WebUtilities;  

namespace HrSolution.Utility.Web.Client.Pages
{
    public partial class Reader
    {
        private BarcodeReader _reader;
        //private int StreamWidth = 720;
        //private int StreamHeight = 540;

        private string LocalBarcodeText;
        private int _currentVideoSourceIdx = 0;

        private string _imgSrc = string.Empty;

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                if (!string.IsNullOrWhiteSpace(_reader.SelectedVideoInputId))
                {
                    _currentVideoSourceIdx = SourceIndexFromId();
                }
            }
        }

        private int SourceIndexFromId()
        {
            int result = 0;
            var inputs = _reader.VideoInputDevices.ToList();
            for (result = 0; result < inputs.Count; result++)
            {
                if (inputs[result].DeviceId.Equals(_reader.SelectedVideoInputId))
                {
                    break;
                }
            }
            return result;
        }

        private async void LocalReceivedBarcodeText(BarcodeReceivedEventArgs args)
        {
            await InvokeAsync(() => {
                readingData = true;
                errorMessage = "";

                this.LocalBarcodeText = args.BarcodeText;

                StateHasChanged();

                _reader.StopDecoding();

                ReadQrCodeAsync();

            });
        }

        private async void ReadQrCodeAsync()
        {
            try
            {
                var result = await Http.Get<TimeInOutDto>($"ThirdParty/Scan/{LocalBarcodeText}");

                if (result != null && !result.HasError)
                {
                    var queryStrDict = new Dictionary<string, string>
                    {
                        ["Id"] = result.EmpCode,
                        ["EmployeeName"] = result.EmpName
                    };

                    await Storage.SetItem(typeof(TimeInOutDto).Name, result);

                    //NavigationManager.NavigateTo(
                    //    QueryHelpers.AddQueryString("/readerresult", queryStrDict)
                    //);

                    NavigationManager.NavigateTo("/readerresult");
                }
            }
            catch { }

            _reader.StartDecoding();

            errorMessage = "Employee information not found. Please check your ID and scan again.";

            readingData = false;

            StateHasChanged();
        }

        private async void CapturePicture()
        {
            _imgSrc = await _reader.Capture();
            StateHasChanged();
        }

        private void OnVideoSourceNext(MouseEventArgs args)
        {
            var inputs = _reader.VideoInputDevices.ToList();

            if (inputs.Count == 0)
            {
                return;
            }

            _currentVideoSourceIdx++;
            if (_currentVideoSourceIdx >= inputs.Count)
            {
                _currentVideoSourceIdx = 0;
            }

            _reader.SelectVideoInput(inputs[_currentVideoSourceIdx]);
        }
    }
}

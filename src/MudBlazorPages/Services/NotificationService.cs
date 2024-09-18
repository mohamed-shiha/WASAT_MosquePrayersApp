using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace MudBlazorPages.Services
{
    public class NotificationService
    {
        private readonly IJSRuntime _jsRuntime;

        public NotificationService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task<bool> IsSupportedByBrowserAsync()
        {
            return await _jsRuntime.InvokeAsync<bool>("eval", "('Notification' in window)");
        }

        public async Task<NotificationPermission> RequestPermissionAsync()
        {
            var result = await _jsRuntime.InvokeAsync<string>("eval", "Notification.requestPermission()");
            return ParsePermission(result);
        }

        public async Task ShowNotificationAsync(string title, string body)
        {
            await _jsRuntime.InvokeVoidAsync("eval", $"new Notification('{title}', {{ body: '{body}' }})");
        }

        private NotificationPermission ParsePermission(string permission)
        {
            return permission.ToLower() switch
            {
                "granted" => NotificationPermission.Granted,
                "denied" => NotificationPermission.Denied,
                _ => NotificationPermission.Default
            };
        }
    }

    public enum NotificationPermission
    {
        Default,
        Granted,
        Denied
    }
}

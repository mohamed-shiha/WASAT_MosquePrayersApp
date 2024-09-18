using Microsoft.JSInterop;

namespace MudBlazorPages.Services
{
    public class CacheService
    {
        private readonly IJSRuntime _jsRuntime;

        public CacheService(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async Task ClearCacheOnLogout()
        {
            await ClearBrowserStorage();
            await ClearHttpCache();
        }

        private async Task ClearBrowserStorage()
        {
            await _jsRuntime.InvokeVoidAsync("eval", @"
                localStorage.clear();
                sessionStorage.clear();
                document.cookie.split(';').forEach(function(c) {
                    document.cookie = c.replace(/^ +/, '').replace(/=.*/, '=;expires=' + new Date().toUTCString() + ';path=/');
                });
            ");
        }

        private async Task ClearHttpCache()
        {
            await _jsRuntime.InvokeVoidAsync("eval", @"
                if ('caches' in window) {
                    caches.keys().then(function(names) {
                        for (let name of names)
                            caches.delete(name);
                    });
                }
            ");
        }
    }
}
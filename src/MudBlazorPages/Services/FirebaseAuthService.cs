
using Microsoft.JSInterop;

namespace MudBlazorPages.Services
{
    public class AuthStateProvider
    {
        private bool _isAuthenticated;
        private string _userEmail;
        private string _userRole;

        public bool IsAuthenticated => _isAuthenticated;
        public string UserEmail => _userEmail;
        public string UserRole => _userRole;

        public event Action OnAuthStateChanged;

        public void SetAuthenticationState(bool isAuthenticated, string userEmail = null, string userRole = null)
        {
            if (_isAuthenticated != isAuthenticated || _userEmail != userEmail || _userRole != userRole)
            {
                _isAuthenticated = isAuthenticated;
                _userEmail = userEmail;
                _userRole = userRole;
                NotifyAuthenticationStateChanged();
            }
        }

        private void NotifyAuthenticationStateChanged() => OnAuthStateChanged?.Invoke();
    }

    public class FirebaseAuthService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly AuthStateProvider _authStateProvider;

        public FirebaseAuthService(IJSRuntime jsRuntime, AuthStateProvider authStateProvider)
        {
            _jsRuntime = jsRuntime;
            _authStateProvider = authStateProvider;
        }

        public async Task<string> RegisterWithEmailAndPasswordAsync(string email, string password)
        {
            string user = await _jsRuntime.InvokeAsync<string>("firebaseAuth.register", email, password);
            return user;
        }

        public async Task<string> SignInWithEmailAndPasswordAsync(string email, string password)
        {
            try
            {
                var result = await _jsRuntime.InvokeAsync<AuthResult>("firebaseAuth.signIn", email, password);

                if (!string.IsNullOrEmpty(result.Token))
                {
                    var role = await _jsRuntime.InvokeAsync<string>("firebaseAuth.getUserRole");
                    _authStateProvider.SetAuthenticationState(true, result.Email, role);
                    return result.Token;
                }
                else
                {
                    _authStateProvider.SetAuthenticationState(false);
                    throw new Exception("Sign-in failed: No token received");
                }
            }
            catch (JSException ex)
            {
                _authStateProvider.SetAuthenticationState(false);
                throw new Exception($"Sign-in failed: {ex.Message}");
            }
        }

        public async Task SignOutAsync()
        {
            await _jsRuntime.InvokeVoidAsync("firebaseAuth.signOut");
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            var user = await _jsRuntime.InvokeAsync<UserInfo>("firebaseAuth.getCurrentUser");
            var isAuthenticated = user != null;
            if (isAuthenticated)
            {
                var role = await _jsRuntime.InvokeAsync<string>("firebaseAuth.getUserRole");
                _authStateProvider.SetAuthenticationState(isAuthenticated, user.Email, role);
            }
            else
            {
                _authStateProvider.SetAuthenticationState(false);
            }
            return isAuthenticated;
        }

        public async Task<string> GetTokenAsync()
        {
            return await _jsRuntime.InvokeAsync<string>("firebaseAuth.getIdToken");
        }

        // Inner classes for deserialization
        private class AuthResult
        {
            public string Token { get; set; }
            public string Email { get; set; }
        }
        private class UserInfo
        {
            public string Email { get; set; }
        }
    }

}

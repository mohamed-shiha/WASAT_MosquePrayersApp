using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace MudBlazorPages.Services
{
    public class AuthStateProvider
    {
        private bool _isAuthenticated;

        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            private set
            {
                if (_isAuthenticated != value)
                {
                    _isAuthenticated = value;
                    NotifyAuthenticationStateChanged();
                }
            }
        }

        public event Action OnAuthStateChanged;

        public void SetAuthenticationState(bool isAuthenticated)
        {
            IsAuthenticated = isAuthenticated;
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
            return await _jsRuntime.InvokeAsync<string>("firebaseAuth.register", email, password);
        }

        public async Task<string> SignInWithEmailAndPasswordAsync(string email, string password)
        {
            try
            {
                var token = await _jsRuntime.InvokeAsync<string>("firebaseAuth.signIn", email, password);

                if (!string.IsNullOrEmpty(token))
                {
                    _authStateProvider.SetAuthenticationState(true);
                    return token;
                }
                else
                {
                    // If the token is null or empty, the sign-in was not successful
                    _authStateProvider.SetAuthenticationState(false);
                    throw new Exception("Sign-in failed: No token received");
                }
            }
            catch (JSException ex)
            {
                // Handle any JavaScript exceptions (e.g., Firebase auth errors)
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
            var user = await _jsRuntime.InvokeAsync<object>("firebaseAuth.getCurrentUser");
            return user != null;
        }

        public async Task<string> GetTokenAsync()
        {
            return await _jsRuntime.InvokeAsync<string>("firebaseAuth.getIdToken");
        }
    }

}

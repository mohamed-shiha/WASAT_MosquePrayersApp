﻿using Microsoft.AspNetCore.Components;
using MudBlazorPages.Services;

namespace MudBlazorPages.Components
{
    public class AuthenticatedComponentBase : ComponentBase, IDisposable
    {
        [Inject] protected AuthStateProvider AuthStateProvider { get; set; }
        [Inject] protected NavigationManager NavigationManager { get; set; }
        [Inject] protected FirebaseAuthService AuthService { get; set; }

        protected bool IsAuthenticated => AuthStateProvider.IsAuthenticated;
        protected string UserRole => AuthStateProvider.UserRole;
        protected bool _disposed = false;

        protected bool IsInRole(string role) => UserRole == role;

        protected override void OnInitialized()
        {
            AuthStateProvider.OnAuthStateChanged += StateHasChanged;
        }

        void IDisposable.Dispose()
        {
            _disposed = true;
            AuthStateProvider.OnAuthStateChanged -= StateHasChanged;
        }
    }


}

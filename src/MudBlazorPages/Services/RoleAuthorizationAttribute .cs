using Microsoft.AspNetCore.Authorization;

namespace MudBlazorPages.Services
{
    public class RoleAuthorizationAttribute : AuthorizeAttribute
    {
        public RoleAuthorizationAttribute(params string[] roles) : base()
        {
            Roles = string.Join(",", roles);
        }
    }
}

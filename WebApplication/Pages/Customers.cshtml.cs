using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication.Constants;

namespace WebApplication.Pages
{
    [Authorize(Roles = RoleConstants.CanAccessCustomerList)]
    public class CustomersModel : PageModel
    {
    }
}

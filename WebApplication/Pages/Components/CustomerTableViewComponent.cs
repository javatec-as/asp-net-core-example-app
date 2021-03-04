using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Pages.Components
{
    public class CustomerTableViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(bool showAll)
        {
            return View(showAll ? "Default" : "SmallTable");
        }
    }
}
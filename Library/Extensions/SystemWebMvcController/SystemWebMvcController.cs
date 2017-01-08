using System.Linq;

namespace Library.Extensions.SystemWebMvcController
{
    public static class Extensions
    {
        public static string SoleErrorMessage(this System.Web.Mvc.Controller controller, string modelKey)
        {
            return controller.ModelState[modelKey].Errors.First().ErrorMessage;
        }
    }
}
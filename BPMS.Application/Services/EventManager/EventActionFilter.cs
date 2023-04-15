namespace BPMS.Application.Services.EventManager;

// todo: uncomment later 
//public sealed class EventActionFilter : ActionFilterAttribute
//{
//    public override void OnActionExecuting(ActionExecutingContext filterContext)
//    {
//        if (EventDataStoreService.EventIsOK)
//        {
//            base.OnActionExecuting(filterContext);
//        }
//        else
//        {
//            string notOKPath = "/License/LicenseIsNotValid";

//            if (filterContext.HttpContext.Request.Path != notOKPath)
//            {
//                filterContext.HttpContext.Response.Redirect(notOKPath);
//            }
//        }
//    }
//}
using System.Security.Claims;
using BPMS.Infrastructure.Services;

namespace BPMS.Infrastructure.Filters;

// todo: uncomment later
//public class RouteAuthorizeAttribute : TypeFilterAttribute
//{
//    public string PermissionPolicy { get; set; }
//    public RouteAuthorizeAttribute(string claimValue) : base(typeof(RouteAuthorizeFilter))
//    {
//        Arguments = new object[] { new Claim("Route", claimValue) };
//    }
//}

//public class RouteAuthorizeFilter: IAuthorizationFilter
//{
//    private readonly Claim _claim;

//    public RouteAuthorizeFilter(Claim claim)
//    {
//        _claim = claim;
//    }

//    public void OnAuthorization(AuthorizationFilterContext context)
//    {
//        var userService = (IUserService)context.HttpContext.RequestServices.GetService(typeof(IUserService))!;
//        var permissionService = (IPermissionService)context.HttpContext.RequestServices.GetService(typeof(IPermissionService))!;

//        if (context.HttpContext.User.Identity is { Name: { } })
//        {
//            string username = context.HttpContext.User.Identity.Name;
//            var user = userService.GetUser(username);
//            bool isAuthorized = permissionService.CheckUserHasRoutePermission(user.Id, _claim.Value);

//            if (!isAuthorized)
//            {
//                context.Result = new ForbidResult();
//            }
//        }
//        else
//        {
//            context.Result = new UnauthorizedResult();
//        }
//    }
//}
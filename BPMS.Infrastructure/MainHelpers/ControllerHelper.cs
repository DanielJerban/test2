using BPMS.Domain.Common.Dtos.Permission;
using BPMS.Infrastructure.Filters;
using System.ComponentModel;
using System.Reflection;

namespace BPMS.Infrastructure.MainHelpers;

public class ControllerHelper
{
    // todo: uncomment later
    //public IEnumerable<ControllerPolicyDTO> GetControllersNameAndPolicy()
    //{
    //    var assembly = Assembly.GetCallingAssembly();
    //    var controllers = assembly.GetTypes()
    //        .Where(type => type.IsSubclassOf(typeof(Controller)))
    //        .ToList();

    //    var controllerList = (
    //        from controller in controllers
    //        let attribute = (RouteAuthorizeAttribute)Attribute.GetCustomAttribute(controller, typeof(RouteAuthorizeAttribute))
    //        let descAttr = (DescriptionAttribute)Attribute.GetCustomAttribute(controller, typeof(DescriptionAttribute))
    //        let controllerPermissionPolicy = attribute == null ? "" : attribute.PermissionPolicy
    //        let name = descAttr == null ? "" : descAttr.Description
    //        select new ControllerPolicyDTO
    //        {
    //            Name = name,
    //            Policy = controllerPermissionPolicy,
    //            ActionPolicies = GetActionPolicyList(controller)
    //        }
    //    ).ToList();

    //    return controllerList;
    //}

    // todo: uncomment later
    //public static IEnumerable<ActionPolicyDTO> GetActionPolicyList(Type controller)
    //{
    //    var actions = controller.GetRuntimeMethods()
    //        .Where(c => c.IsPublic &&
    //                    !c.IsDefined(typeof(NonActionAttribute)) &&
    //                    c is { IsSpecialName: false, IsConstructor: false } &&
    //                    c.DeclaringType != typeof(object) &&
    //                    c.DeclaringType != typeof(ControllerBase)).ToList();

    //    var actionList = (from actionDescriptor in actions
    //                      let attribute = actionDescriptor.GetCustomAttributes(typeof(RouteAuthorizeAttribute), false).LastOrDefault() as RouteAuthorizeAttribute
    //                      let descAttr = actionDescriptor.GetCustomAttributes(typeof(DescriptionAttribute), false).LastOrDefault() as DescriptionAttribute
    //                      let actionPolicy = attribute == null ? "" : attribute.PermissionPolicy
    //                      let name = descAttr == null ? "" : descAttr.Description
    //                      where attribute != null
    //                      select new ActionPolicyDTO
    //                      {
    //                          Name = name,
    //                          Policy = actionPolicy
    //                      }).ToList();

    //    return actionList;
    //}
}
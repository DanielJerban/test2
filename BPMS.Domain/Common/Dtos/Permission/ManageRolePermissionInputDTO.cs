using BPMS.Domain.Common.Enums;

namespace BPMS.Domain.Common.Dtos.Permission;

public class ManageRolePermissionInputDTO
{
    public Guid RoleId { get; set; }
    public string PermissionType { get; set; }
    public string PermissionValue { get; set; }
    public PermissionManagementState PermissionManagementState { get; set; }
    public string PermissionName { get; set; }
}
using BPMS.Domain.Common.Enums;

namespace BPMS.Domain.Entities;

public class PermissionLog
{
    public Guid Id { get; set; }
    public Guid StaffId { get; set; }
    public string ClaimType { get; set; }
    public string FullName { get; set; }
    public string ClaimValue { get; set; }
    public string ClaimDesc { get; set; }
    public Guid RoleId { get; set; }
    public string RoleName { get; set; }
    public Guid CreatorUserId { get; set; }
    public PermissionActionType ActionType { get; set; }
    public int CreateDate { get; set; }
    public string CreateTime { get; set; }
    public DateTime CreateDateTime { get; set; }
    public string CreatorUsername { get; set; }
}
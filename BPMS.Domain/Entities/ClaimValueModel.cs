using BPMS.Domain.Common.Enums;

namespace BPMS.Domain.Entities;

public class ClaimValueModel
{
    public string ClaimValue { get; set; }
    public PermissionActionType ActionType { get; set; }
}
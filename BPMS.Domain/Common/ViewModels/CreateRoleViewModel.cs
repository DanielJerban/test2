using BPMS.Domain.Entities;

namespace BPMS.Domain.Common.ViewModels;

public class CreateRoleViewModel
{

    public IEnumerable<Role> Roles { get; set; }
    public Role Role { get; set; }
}
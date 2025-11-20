namespace AutoPartesRazor.ViewModels;

public class ManageRolePermissionsViewModel
{
    public string RoleId { get; set; }
    public string RoleName { get; set; }
    public List<PermissionGroupViewModel> PermissionGroups { get; set; }
}
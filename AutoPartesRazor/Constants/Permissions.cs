namespace AutoPartesRazor.Constants;

public static class Permissions
{
    // Tipo de claim para permisos
    public const string ClaimType = "Permission";

    // Permisos de Usuarios
    public static class Users
    {
        public const string View = "Permissions.Users.View";
        public const string Create = "Permissions.Users.Create";
        public const string Edit = "Permissions.Users.Edit";
        public const string Delete = "Permissions.Users.Delete";
        public const string ManageRoles = "Permissions.Users.ManageRoles";
    }

    // Permisos de Productos
    public static class Products
    {
        public const string View = "Permissions.Products.View";
        public const string Create = "Permissions.Products.Create";
        public const string Edit = "Permissions.Products.Edit";
        public const string Delete = "Permissions.Products.Delete";
        public const string ManageStock = "Permissions.Products.ManageStock";
    }

    // Permisos de Órdenes
    public static class Orders
    {
        public const string View = "Permissions.Orders.View";
        public const string Create = "Permissions.Orders.Create";
        public const string Edit = "Permissions.Orders.Edit";
        public const string Delete = "Permissions.Orders.Delete";
        public const string Approve = "Permissions.Orders.Approve";
        public const string Cancel = "Permissions.Orders.Cancel";
    }

    // Permisos de Categorías
    public static class Categories
    {
        public const string View = "Permissions.Categories.View";
        public const string Create = "Permissions.Categories.Create";
        public const string Edit = "Permissions.Categories.Edit";
        public const string Delete = "Permissions.Categories.Delete";
    }

    // Permisos de Reportes
    public static class Reports
    {
        public const string ViewSales = "Permissions.Reports.ViewSales";
        public const string ViewInventory = "Permissions.Reports.ViewInventory";
        public const string ViewUsers = "Permissions.Reports.ViewUsers";
        public const string Export = "Permissions.Reports.Export";
    }

    // Permisos de Roles
    public static class Roles
    {
        public const string View = "Permissions.Roles.View";
        public const string Create = "Permissions.Roles.Create";
        public const string Edit = "Permissions.Roles.Edit";
        public const string Delete = "Permissions.Roles.Delete";
        public const string ManagePermissions = "Permissions.Roles.ManagePermissions";
    }

    // Método para obtener todos los permisos organizados
    public static List<PermissionGroup> GetAllPermissions()
    {
        return new List<PermissionGroup>
        {
            new PermissionGroup
            {
                GroupName = "Usuarios",
                Permissions = new List<Permission>
                {
                    new Permission { Name = Users.View, Description = "Ver usuarios" },
                    new Permission { Name = Users.Create, Description = "Crear usuarios" },
                    new Permission { Name = Users.Edit, Description = "Editar usuarios" },
                    new Permission { Name = Users.Delete, Description = "Eliminar usuarios" },
                    new Permission { Name = Users.ManageRoles, Description = "Gestionar roles de usuarios" }
                }
            },
            new PermissionGroup
            {
                GroupName = "Productos",
                Permissions = new List<Permission>
                {
                    new Permission { Name = Products.View, Description = "Ver productos" },
                    new Permission { Name = Products.Create, Description = "Crear productos" },
                    new Permission { Name = Products.Edit, Description = "Editar productos" },
                    new Permission { Name = Products.Delete, Description = "Eliminar productos" },
                    new Permission { Name = Products.ManageStock, Description = "Gestionar inventario" }
                }
            },
            new PermissionGroup
            {
                GroupName = "Órdenes",
                Permissions = new List<Permission>
                {
                    new Permission { Name = Orders.View, Description = "Ver órdenes" },
                    new Permission { Name = Orders.Create, Description = "Crear órdenes" },
                    new Permission { Name = Orders.Edit, Description = "Editar órdenes" },
                    new Permission { Name = Orders.Delete, Description = "Eliminar órdenes" },
                    new Permission { Name = Orders.Approve, Description = "Aprobar órdenes" },
                    new Permission { Name = Orders.Cancel, Description = "Cancelar órdenes" }
                }
            },
            new PermissionGroup
            {
                GroupName = "Categorías",
                Permissions = new List<Permission>
                {
                    new Permission { Name = Categories.View, Description = "Ver categorías" },
                    new Permission { Name = Categories.Create, Description = "Crear categorías" },
                    new Permission { Name = Categories.Edit, Description = "Editar categorías" },
                    new Permission { Name = Categories.Delete, Description = "Eliminar categorías" }
                }
            },
            new PermissionGroup
            {
                GroupName = "Reportes",
                Permissions = new List<Permission>
                {
                    new Permission { Name = Reports.ViewSales, Description = "Ver reportes de ventas" },
                    new Permission { Name = Reports.ViewInventory, Description = "Ver reportes de inventario" },
                    new Permission { Name = Reports.ViewUsers, Description = "Ver reportes de usuarios" },
                    new Permission { Name = Reports.Export, Description = "Exportar reportes" }
                }
            },
            new PermissionGroup
            {
                GroupName = "Roles y Permisos",
                Permissions = new List<Permission>
                {
                    new Permission { Name = Roles.View, Description = "Ver roles" },
                    new Permission { Name = Roles.Create, Description = "Crear roles" },
                    new Permission { Name = Roles.Edit, Description = "Editar roles" },
                    new Permission { Name = Roles.Delete, Description = "Eliminar roles" },
                    new Permission { Name = Roles.ManagePermissions, Description = "Gestionar permisos de roles" }
                }
            }
        };
    }
}

// Clases auxiliares
public class PermissionGroup
{
    public string GroupName { get; set; }
    public List<Permission> Permissions { get; set; }
}

public class Permission
{
    public string Name { get; set; }
    public string Description { get; set; }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AutoPartesRazor.Models;
using AutoPartesRazor.Data;
using AutoPartesRazor.Models.Enum;

namespace AutoPartesRazor.Data;
public class DatabaseSeeder
{
    private readonly AutoPartesRazorContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly Random _random = new Random();

    public DatabaseSeeder(
        AutoPartesRazorContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    /// <summary>
    /// Método principal que ejecuta toda la carga de datos
    /// </summary>
    public async Task SeedAsync()
    {
        try
        {
            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║     INICIANDO CARGA DE DATOS DE PRUEBA - AUTOPARTES       ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            // Asegurar que la base de datos existe
            await _context.Database.EnsureCreatedAsync();

            // 1. CREAR ROLES
            Console.WriteLine("🔐 Creando roles del sistema...");
            await CreateRolesAsync();
            Console.WriteLine("✅ Roles creados correctamente\n");

            // 2. CREAR USUARIOS
            Console.WriteLine("👥 Creando usuarios...");
            var users = await CreateUsersAsync();
            Console.WriteLine($"✅ {users.Count} usuarios creados\n");

            // 3. CREAR CATEGORÍAS
            Console.WriteLine("📁 Creando categorías de autopartes...");
            var categories = await CreateCategoriesAsync();
            Console.WriteLine($"✅ {categories.Count} categorías creadas\n");

            // 4. CREAR MARCAS
            Console.WriteLine("🏷️  Creando marcas automotrices...");
            var brands = await CreateBrandsAsync();
            Console.WriteLine($"✅ {brands.Count} marcas creadas\n");

            // 5. CREAR PROVEEDORES
            Console.WriteLine("🏭 Creando proveedores...");
            var suppliers = await CreateSuppliersAsync();
            Console.WriteLine($"✅ {suppliers.Count} proveedores creados\n");

            // 6. CREAR PRODUCTOS
            Console.WriteLine("📦 Creando repuestos y accesorios...");
            var products = await CreateProductsAsync(categories, brands);
            Console.WriteLine($"✅ {products.Count} productos creados\n");

            // 7. CREAR RELACIONES PRODUCTO-PROVEEDOR
            Console.WriteLine("🔗 Creando relaciones producto-proveedor...");
            await CreateProductSuppliersAsync(products, suppliers);
            Console.WriteLine("✅ Relaciones producto-proveedor creadas\n");

            // 8. CREAR ÓRDENES DE COMPRA
            Console.WriteLine("📋 Creando órdenes de compra...");
            await CreatePurchaseOrdersAsync(products, suppliers);
            Console.WriteLine("✅ Órdenes de compra creadas\n");

            // 9. CREAR ÓRDENES DE VENTA
            Console.WriteLine("🛒 Creando órdenes de venta...");
            await CreateSalesOrdersAsync(users, products);
            Console.WriteLine("✅ Órdenes de venta creadas\n");

            // 10. CREAR RESEÑAS DE PRODUCTOS
            Console.WriteLine("⭐ Creando reseñas de productos...");
            await CreateProductReviewsAsync(users, products);
            Console.WriteLine("✅ Reseñas de productos creadas\n");

            // 11. CREAR CUPONES
            Console.WriteLine("🎫 Creando cupones de descuento...");
            await CreateCouponsAsync(users, products);
            Console.WriteLine("✅ Cupones creados\n");

            // 12. CREAR NOTIFICACIONES
            Console.WriteLine("🔔 Creando notificaciones...");
            await CreateNotificationsAsync(users);
            Console.WriteLine("✅ Notificaciones creadas\n");

            // 13. CREAR RECLAMOS
            Console.WriteLine("📝 Creando reclamos de clientes...");
            await CreateClaimsAsync(users);
            Console.WriteLine("✅ Reclamos creados\n");

            // 14. CREAR MOVIMIENTOS DE STOCK
            Console.WriteLine("📊 Creando movimientos de stock...");
            await CreateStockMovementsAsync(products);
            Console.WriteLine("✅ Movimientos de stock creados\n");

            Console.WriteLine("\n╔═══════════════════════════════════════════════════════════╗");
            Console.WriteLine("║        ✅ DATOS DE PRUEBA CREADOS EXITOSAMENTE ✅         ║");
            Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");

            PrintCredentials();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ ERROR CRÍTICO: {ex.Message}");
            Console.WriteLine($"📍 Stack Trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"🔍 Inner Exception: {ex.InnerException.Message}");
            }
            throw;
        }
    }

    #region Roles

    private async Task CreateRolesAsync()
    {
        string[] roleNames = { "Admin", "Employee", "Client" };

        foreach (var roleName in roleNames)
        {
            var roleExist = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
                Console.WriteLine($"   → Rol '{roleName}' creado");
            }
        }
    }

    #endregion

    #region Usuarios

    private async Task<List<User>> CreateUsersAsync()
    {
        var users = new List<User>();

        // SUPER ADMINISTRADOR
        var superAdmin = await CreateUserIfNotExistsAsync(
            username: "superadmin@autopartes.com",
            email: "superadmin@autopartes.com",
            fullName: "Super Administrador",
            role: "Admin",
            address: "Sede Central - Av. Córdoba 2500, Buenos Aires",
            phone: "1145678900",
            password: "SuperAdmin123!"
        );
        if (superAdmin != null) users.Add(superAdmin);

        // ADMINISTRADOR
        var admin = await CreateUserIfNotExistsAsync(
            username: "admin@autopartes.com",
            email: "admin@autopartes.com",
            fullName: "Administrador Principal",
            role: "Admin",
            address: "Av. Córdoba 2500, Buenos Aires",
            phone: "1145678901",
            password: "Admin123!"
        );
        if (admin != null) users.Add(admin);

        // EMPLEADOS
        var empleados = new[]
        {
            ("vendedor@autopartes.com", "Roberto Díaz", "Av. Corrientes 1500, Buenos Aires", "1156789011"),
            ("deposito@autopartes.com", "Miguel Torres", "Av. Warnes 2200, Buenos Aires", "1167890122"),
            ("atencion@autopartes.com", "Laura Fernández", "Av. Santa Fe 1800, Buenos Aires", "1178901233")
        };

        foreach (var (email, nombre, direccion, telefono) in empleados)
        {
            var empleado = await CreateUserIfNotExistsAsync(
                username: email,
                email: email,
                fullName: nombre,
                role: "Employee",
                address: direccion,
                phone: telefono,
                password: "Empleado123!"
            );
            if (empleado != null) users.Add(empleado);
        }

        // CLIENTES
        var clientes = new[]
        {
            ("Taller Mecánico San Martín", "taller.mecanico@email.com", "Av. San Martín 3450, Buenos Aires"),
            ("Gomería Central", "gomeria.central@email.com", "Av. Rivadavia 7890, Buenos Aires"),
            ("Auto Service Express", "autoservice@email.com", "Av. Cabildo 2100, Buenos Aires"),
            ("Juan Pérez", "juan.perez@email.com", "Calle Alsina 567, Buenos Aires"),
            ("María Rodríguez", "maria.rodriguez@email.com", "Av. Santa Fe 2100, Buenos Aires"),
            ("Carlos Gómez", "carlos.gomez@email.com", "Calle Lavalle 890, Buenos Aires"),
            ("Ana Martínez", "ana.martinez@email.com", "Av. Callao 456, Buenos Aires"),
            ("Pedro López", "pedro.lopez@email.com", "Av. Belgrano 1234, Buenos Aires")
        };

        foreach (var (nombre, email, direccion) in clientes)
        {
            var cliente = await CreateUserIfNotExistsAsync(
                username: email,
                email: email,
                fullName: nombre,
                role: "Client",
                address: direccion,
                phone: $"11{_random.Next(40000000, 69999999)}",
                password: "Cliente123!"
            );
            if (cliente != null) users.Add(cliente);
        }

        return users;
    }

    private async Task<User?> CreateUserIfNotExistsAsync(
        string username,
        string email,
        string fullName,
        string role,
        string address,
        string phone,
        string password)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser != null)
        {
            Console.WriteLine($"   ⚠️  Usuario '{email}' ya existe, omitiendo...");
            return existingUser;
        }

        var user = new User
        {
            UserName = username,
            Email = email,
            FullName = fullName,
            Role = role,
            Address = address,
            PhoneNumber = phone,
            EmailConfirmed = true,
            RegistrationDate = DateTime.Now.AddDays(-_random.Next(30, 730))
        };

        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, role);
            Console.WriteLine($"   ✓ Usuario '{email}' creado como {role}");
            return user;
        }
        else
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            Console.WriteLine($"   ✗ Error creando '{email}': {errors}");
            return null;
        }
    }

    #endregion

    #region Categorías

    private async Task<List<Category>> CreateCategoriesAsync()
    {
        if (await _context.Categories.AnyAsync())
        {
            Console.WriteLine("   ⚠️  Las categorías ya existen, omitiendo...");
            return await _context.Categories.ToListAsync();
        }

        var categories = new List<Category>
        {
            new Category { Name = "Motor" },
            new Category { Name = "Frenos" },
            new Category { Name = "Suspensión" },
            new Category { Name = "Transmisión" },
            new Category { Name = "Sistema Eléctrico" },
            new Category { Name = "Refrigeración" },
            new Category { Name = "Escape" },
            new Category { Name = "Filtros" },
            new Category { Name = "Aceites y Lubricantes" },
            new Category { Name = "Neumáticos" },
            new Category { Name = "Batería" },
            new Category { Name = "Iluminación" },
            new Category { Name = "Accesorios" },
            new Category { Name = "Carrocería" }
        };

        _context.Categories.AddRange(categories);
        await _context.SaveChangesAsync();
        return categories;
    }

    #endregion

    #region Marcas

    private async Task<List<Brand>> CreateBrandsAsync()
    {
        if (await _context.Brands.AnyAsync())
        {
            Console.WriteLine("   ⚠️  Las marcas ya existen, omitiendo...");
            return await _context.Brands.ToListAsync();
        }

        var brands = new List<Brand>
        {
            new Brand { Name = "Bosch" },
            new Brand { Name = "Brembo" },
            new Brand { Name = "NGK" },
            new Brand { Name = "Mann Filter" },
            new Brand { Name = "Mobil" },
            new Brand { Name = "Castrol" },
            new Brand { Name = "Monroe" },
            new Brand { Name = "Philips" },
            new Brand { Name = "Pirelli" },
            new Brand { Name = "Michelin" },
            new Brand { Name = "Denso" },
            new Brand { Name = "Gates" },
            new Brand { Name = "Valeo" },
            new Brand { Name = "Moura" },
            new Brand { Name = "Genérica" }
        };

        _context.Brands.AddRange(brands);
        await _context.SaveChangesAsync();
        return brands;
    }

    #endregion

    #region Proveedores

    private async Task<List<Supplier>> CreateSuppliersAsync()
    {
        if (await _context.Suppliers.AnyAsync())
        {
            Console.WriteLine("   ⚠️  Los proveedores ya existen, omitiendo...");
            return await _context.Suppliers.ToListAsync();
        }

        var suppliers = new List<Supplier>
        {
            new Supplier
            {
                Name = "Distribuidora Repuestos SA",
                Email = "ventas@repuestos.com.ar",
                Phone = "1145678901",
                Address = "Parque Industrial Pilar, Buenos Aires"
            },
            new Supplier
            {
                Name = "Importadora AutoMotors",
                Email = "pedidos@automotors.com.ar",
                Phone = "1156789012",
                Address = "Av. Warnes 1200, Buenos Aires"
            },
            new Supplier
            {
                Name = "Mayorista El Repuesto",
                Email = "contacto@elrepuesto.com.ar",
                Phone = "1167890123",
                Address = "Ruta Panamericana Km 38, Buenos Aires"
            },
            new Supplier
            {
                Name = "Autopartes Express SRL",
                Email = "info@autopartesexpress.com.ar",
                Phone = "1178901234",
                Address = "Av. General Paz 5000, Buenos Aires"
            },
            new Supplier
            {
                Name = "Lubricantes del Sur",
                Email = "ventas@lubricantesdelsur.com.ar",
                Phone = "1189012345",
                Address = "Av. Constituyentes 3500, Buenos Aires"
            }
        };

        _context.Suppliers.AddRange(suppliers);
        await _context.SaveChangesAsync();
        return suppliers;
    }

    #endregion

    #region Productos

    private async Task<List<Product>> CreateProductsAsync(List<Category> categories, List<Brand> brands)
    {
        if (await _context.Products.AnyAsync())
        {
            Console.WriteLine("   ⚠️  Los productos ya existen, omitiendo...");
            return await _context.Products.ToListAsync();
        }

        var products = new List<Product>
        {
            // MOTOR (Categoría 0)
            new Product
            {
                Name = "Bujías NGK Platino (Set x4)",
                Description = "Bujías de platino de alta duración. Compatible con motores nafteros. Mayor rendimiento y economía de combustible.",
                Price = 28000m,
                Stock = 45,
                MinimumStock = 15,
                CategoryId = categories[0].Id,
                BrandId = brands[2].Id,
                ActualStock = 45,
                LastStockCheck = DateTime.Now.AddDays(-5)
            },
            new Product
            {
                Name = "Kit de Distribución Completo",
                Description = "Kit completo con correa, tensor y rodillos. Compatible con múltiples modelos. Incluye instructivo de instalación.",
                Price = 85000m,
                Stock = 12,
                MinimumStock = 5,
                CategoryId = categories[0].Id,
                BrandId = brands[11].Id,
                ActualStock = 12,
                LastStockCheck = DateTime.Now.AddDays(-3)
            },
            new Product
            {
                Name = "Bomba de Agua Original",
                Description = "Bomba de agua de calidad OEM. Garantía de 12 meses. Alta durabilidad y rendimiento.",
                Price = 45000m,
                Stock = 20,
                MinimumStock = 8,
                CategoryId = categories[0].Id,
                BrandId = brands[0].Id,
                ActualStock = 20,
                LastStockCheck = DateTime.Now.AddDays(-7)
            },

            // FRENOS (Categoría 1)
            new Product
            {
                Name = "Pastillas de Freno Delanteras Brembo",
                Description = "Pastillas cerámicas de alto rendimiento. Menor ruido y polvo. Excelente poder de frenado.",
                Price = 42000m,
                Stock = 35,
                MinimumStock = 12,
                CategoryId = categories[1].Id,
                BrandId = brands[1].Id,
                ActualStock = 35,
                LastStockCheck = DateTime.Now.AddDays(-2)
            },
            new Product
            {
                Name = "Discos de Freno Ventilados (Par)",
                Description = "Discos ventilados de hierro fundido. Mejor disipación de calor. Incluye tornillos de fijación.",
                Price = 68000m,
                Stock = 15,
                MinimumStock = 6,
                CategoryId = categories[1].Id,
                BrandId = brands[0].Id,
                ActualStock = 15,
                LastStockCheck = DateTime.Now.AddDays(-4)
            },
            new Product
            {
                Name = "Líquido de Frenos DOT 4 (500ml)",
                Description = "Líquido de frenos sintético DOT 4. Alto punto de ebullición. Para sistemas ABS.",
                Price = 4500m,
                Stock = 80,
                MinimumStock = 20,
                CategoryId = categories[1].Id,
                BrandId = brands[5].Id,
                ActualStock = 80,
                LastStockCheck = DateTime.Now.AddDays(-1)
            },

            // SUSPENSIÓN (Categoría 2)
            new Product
            {
                Name = "Amortiguadores Monroe Delanteros (Par)",
                Description = "Amortiguadores hidráulicos de gas. Tecnología Monroe OESpectrum. Garantía 2 años.",
                Price = 95000m,
                Stock = 10,
                MinimumStock = 4,
                CategoryId = categories[2].Id,
                BrandId = brands[6].Id,
                ActualStock = 10,
                LastStockCheck = DateTime.Now.AddDays(-6)
            },
            new Product
            {
                Name = "Kit Rotulas Dirección Completo",
                Description = "Kit con rótulas internas y externas. Incluye terminales axiales. Para tren delantero completo.",
                Price = 52000m,
                Stock = 14,
                MinimumStock = 5,
                CategoryId = categories[2].Id,
                BrandId = brands[14].Id,
                ActualStock = 14,
                LastStockCheck = DateTime.Now.AddDays(-8)
            },

            // FILTROS (Categoría 7)
            new Product
            {
                Name = "Filtro de Aceite Mann Filter",
                Description = "Filtro de aceite de alta eficiencia. Compatible múltiples motores. Cambio cada 10.000km.",
                Price = 6500m,
                Stock = 120,
                MinimumStock = 40,
                CategoryId = categories[7].Id,
                BrandId = brands[3].Id,
                ActualStock = 120,
                LastStockCheck = DateTime.Now
            },
            new Product
            {
                Name = "Filtro de Combustible Original",
                Description = "Filtro de nafta/gasoil alta calidad. Protección bomba combustible. Cambio 15.000km.",
                Price = 8500m,
                Stock = 80,
                MinimumStock = 25,
                CategoryId = categories[7].Id,
                BrandId = brands[3].Id,
                ActualStock = 80,
                LastStockCheck = DateTime.Now.AddDays(-2)
            },

            // ACEITES (Categoría 8)
            new Product
            {
                Name = "Aceite Motor Mobil Super 3000 5W-40 (4L)",
                Description = "Aceite sintético multivehículo. Protección superior motor. Normas API SN, ACEA A3/B4.",
                Price = 32000m,
                Stock = 45,
                MinimumStock = 15,
                CategoryId = categories[8].Id,
                BrandId = brands[4].Id,
                ActualStock = 45,
                LastStockCheck = DateTime.Now.AddDays(-3)
            },
            new Product
            {
                Name = "Aceite Castrol Edge 5W-30 (4L)",
                Description = "Aceite totalmente sintético. Tecnología Fluid Titanium. Máxima protección.",
                Price = 38000m,
                Stock = 35,
                MinimumStock = 12,
                CategoryId = categories[8].Id,
                BrandId = brands[5].Id,
                ActualStock = 35,
                LastStockCheck = DateTime.Now.AddDays(-4)
            },

            // BATERÍA (Categoría 10)
            new Product
            {
                Name = "Batería Moura 12V 65Ah",
                Description = "Batería sellada libre de mantenimiento. 650A arranque en frío. Garantía 12 meses.",
                Price = 95000m,
                Stock = 22,
                MinimumStock = 8,
                CategoryId = categories[10].Id,
                BrandId = brands[13].Id,
                ActualStock = 22,
                LastStockCheck = DateTime.Now.AddDays(-5)
            },

            // NEUMÁTICOS (Categoría 9)
            new Product
            {
                Name = "Neumático Pirelli P7 195/65 R15",
                Description = "Neumático premium todo terreno. Excelente adherencia en mojado. Bajo ruido rodadura.",
                Price = 85000m,
                Stock = 24,
                MinimumStock = 8,
                CategoryId = categories[9].Id,
                BrandId = brands[8].Id,
                ActualStock = 24,
                LastStockCheck = DateTime.Now.AddDays(-10)
            },
            new Product
            {
                Name = "Neumático Michelin Primacy 205/55 R16",
                Description = "Neumático de alto rendimiento. Mayor durabilidad y confort. Eficiencia energética A.",
                Price = 95000m,
                Stock = 20,
                MinimumStock = 8,
                CategoryId = categories[9].Id,
                BrandId = brands[9].Id,
                ActualStock = 20,
                LastStockCheck = DateTime.Now.AddDays(-12)
            },

            // ILUMINACIÓN (Categoría 11)
            new Product
            {
                Name = "Kit Luces LED H7 6000K",
                Description = "Kit conversión LED para faros principales. 6000 lúmenes. Luz blanca brillante.",
                Price = 22000m,
                Stock = 30,
                MinimumStock = 10,
                CategoryId = categories[11].Id,
                BrandId = brands[7].Id,
                ActualStock = 30,
                LastStockCheck = DateTime.Now.AddDays(-7)
            },

            // ACCESORIOS (Categoría 12)
            new Product
            {
                Name = "Alfombras Goma Premium (Set x4)",
                Description = "Alfombras de goma universales. Fácil limpieza. Antideslizantes con bordes elevados.",
                Price = 12000m,
                Stock = 45,
                MinimumStock = 15,
                CategoryId = categories[12].Id,
                BrandId = brands[14].Id,
                ActualStock = 45,
                LastStockCheck = DateTime.Now.AddDays(-15)
            },

            // PRODUCTOS CON STOCK BAJO (ALERTAS)
            new Product
            {
                Name = "Sensor Oxígeno Lambda Universal",
                Description = "Sensor de oxígeno universal. Compatible múltiples vehículos. Mejora consumo.",
                Price = 45000m,
                Stock = 3,
                MinimumStock = 8,
                CategoryId = categories[4].Id,
                BrandId = brands[0].Id,
                ActualStock = 3,
                LastStockCheck = DateTime.Now.AddDays(-1)
            },
            new Product
            {
                Name = "Kit Empacaduras Motor Completo",
                Description = "Juego completo de juntas motor. Incluye retenes y o-rings. Calidad OEM.",
                Price = 55000m,
                Stock = 2,
                MinimumStock = 6,
                CategoryId = categories[0].Id,
                BrandId = brands[14].Id,
                ActualStock = 2,
                LastStockCheck = DateTime.Now
            }
        };

        _context.Products.AddRange(products);
        await _context.SaveChangesAsync();
        return products;
    }

    #endregion

    #region Relaciones Producto-Proveedor

    private async Task CreateProductSuppliersAsync(List<Product> products, List<Supplier> suppliers)
    {
        if (await _context.ProductSuppliers.AnyAsync())
        {
            Console.WriteLine("   ⚠️  Las relaciones producto-proveedor ya existen, omitiendo...");
            return;
        }

        var productSuppliers = new List<ProductSupplier>();

        foreach (var product in products)
        {
            // Asignar 1-3 proveedores aleatorios a cada producto
            var numSuppliers = _random.Next(1, 4);
            var selectedSuppliers = suppliers.OrderBy(x => _random.Next()).Take(numSuppliers);

            foreach (var supplier in selectedSuppliers)
            {
                productSuppliers.Add(new ProductSupplier
                {
                    ProductId = product.Id,
                    SupplierId = supplier.Id,
                    SupplyPrice = Math.Round(product.Price * 0.65m, 2) // 65% del precio de venta
                });
            }
        }

        _context.ProductSuppliers.AddRange(productSuppliers);
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Órdenes de Compra

    private async Task CreatePurchaseOrdersAsync(List<Product> products, List<Supplier> suppliers)
    {
        if (await _context.PurchaseOrders.AnyAsync())
        {
            Console.WriteLine("   ⚠️  Las órdenes de compra ya existen, omitiendo...");
            return;
        }

        var purchaseOrders = new List<PurchaseOrder>();
        var statuses = new[] { "Pending", "Approved", "Received", "Cancelled" };

        for (int i = 0; i < 25; i++)
        {
            var product = products[_random.Next(products.Count)];
            var supplier = suppliers[_random.Next(suppliers.Count)];
            var quantity = _random.Next(5, 50);
            var unitPrice = Math.Round(product.Price * 0.65m, 2);

            purchaseOrders.Add(new PurchaseOrder
            {
                ProductId = product.Id,
                SupplierId = supplier.Id,
                Quantity = quantity,
                UnitPrice = unitPrice,
                Total = quantity * unitPrice,
                Status = statuses[_random.Next(statuses.Length)],
                CreatedAt = DateTime.Now.AddDays(-_random.Next(1, 120))
            });
        }

        _context.PurchaseOrders.AddRange(purchaseOrders);
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Órdenes de Venta

    private async Task CreateSalesOrdersAsync(List<User> users, List<Product> products)
    {
        if (await _context.Orders.AnyAsync())
        {
            Console.WriteLine("   ⚠️  Las órdenes de venta ya existen, omitiendo...");
            return;
        }

        var orders = new List<Order>();
        var clients = users.Where(u => u.Role == "Client").ToList();
        var statuses = new[] { "Pendiente", "Preparando", "Despachado", "En Camino", "Entregado", "Cancelado" };
        var paymentMethods = new[] { "Tarjeta de Crédito", "Tarjeta de Débito", "Transferencia Bancaria", "Efectivo", "MercadoPago" };

        for (int i = 0; i < 40; i++)
        {
            var client = clients[_random.Next(clients.Count)];
            var numItems = _random.Next(1, 6);
            var orderItems = new List<OrderItem>();
            decimal total = 0;

            for (int j = 0; j < numItems; j++)
            {
                var product = products[_random.Next(products.Count)];
                var quantity = _random.Next(1, 4);
                var subtotal = product.Price * quantity;
                total += subtotal;

                orderItems.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = quantity,
                    UnitPrice = product.Price,
                    Subtotal = subtotal,
                    Status = "Procesado",
                    FechaActualizacion = DateTime.Now.AddDays(-_random.Next(1, 90))
                });
            }

            var status = statuses[_random.Next(statuses.Length)];
            var createdDate = DateTime.Now.AddDays(-_random.Next(1, 180));

            var order = new Order
            {
                UserId = client.Id,
                CustomerName = client.FullName,
                CustomerEmail = client.Email,
                ShippingAddress = client.Address ?? "Dirección no especificada",
                PaymentMethod = paymentMethods[_random.Next(paymentMethods.Length)],
                Total = total,
                OriginalTotal = total,
                Status = status,
                CreatedAt = createdDate,
                UpdatedAt = status != "Pendiente" ? createdDate.AddDays(_random.Next(1, 7)) : null,
                Items = orderItems,
                Calificacion = status == "Entregado" ? _random.Next(3, 6) : null
            };

            orders.Add(order);
        }

        _context.Orders.AddRange(orders);
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Reseñas de Productos

    private async Task CreateProductReviewsAsync(List<User> users, List<Product> products)
    {
        if (await _context.Set<ProductReview>().AnyAsync())
        {
            Console.WriteLine("   ⚠️  Las reseñas ya existen, omitiendo...");
            return;
        }

        var reviews = new List<ProductReview>();
        var clients = users.Where(u => u.Role == "Client").ToList();
        var comments = new[]
        {
            "Excelente producto, muy buena calidad",
            "Cumple con lo esperado, recomendado",
            "Buena relación precio-calidad",
            "Producto de calidad superior",
            "Llegó en perfectas condiciones",
            "No cumplió mis expectativas",
            "Regular, esperaba más por el precio",
            "Muy satisfecho con la compra",
            "Perfecto, justo lo que necesitaba",
            "Bueno pero podría mejorar"
        };

        // Crear reseñas para algunos productos
        var reviewableProducts = products.Take(15).ToList();

        foreach (var product in reviewableProducts)
        {
            var numReviews = _random.Next(2, 6);
            var reviewers = clients.OrderBy(x => _random.Next()).Take(numReviews);

            foreach (var reviewer in reviewers)
            {
                reviews.Add(new ProductReview
                {
                    ProductId = product.Id,
                    UserId = reviewer.Id,
                    Rating = _random.Next(3, 6),
                    Comment = comments[_random.Next(comments.Length)],
                    CreatedAt = DateTime.Now.AddDays(-_random.Next(1, 90)),
                    HelpfulCount = _random.Next(0, 15),
                    NotHelpfulCount = _random.Next(0, 5)
                });
            }
        }

        _context.Set<ProductReview>().AddRange(reviews);
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Cupones

    private async Task CreateCouponsAsync(List<User> users, List<Product> products)
    {
        if (await _context.Coupons.AnyAsync())
        {
            Console.WriteLine("   ⚠️  Los cupones ya existen, omitiendo...");
            return;
        }

        var coupons = new List<Coupon>();
        var clients = users.Where(u => u.Role == "Client").Take(3).ToList();

        foreach (var client in clients)
        {
            coupons.Add(new Coupon
            {
                Code = $"DESC{_random.Next(1000, 9999)}",
                DiscountPercentage = _random.Next(10, 31),
                CreatedAt = DateTime.Now.AddDays(-30),
                ExpiresAt = DateTime.Now.AddDays(30),
                IsUsed = false,
                IsActive = true,
                UserId = client.Id,
                Reason = "Cupón de bienvenida por registro"
            });
        }

        _context.Coupons.AddRange(coupons);
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Notificaciones

    private async Task CreateNotificationsAsync(List<User> users)
    {
        if (await _context.Notifications.AnyAsync())
        {
            Console.WriteLine("   ⚠️  Las notificaciones ya existen, omitiendo...");
            return;
        }

        var notifications = new List<Notification>();

        var messagesAdmin = new[]
        {
            ("Stock crítico detectado", "Los siguientes productos requieren reposición urgente: Sensor Oxígeno, Kit Empacaduras."),
            ("Nueva orden de compra", "Se ha recibido una nueva orden de compra por $350.000."),
            ("Pedido entregado con demora", "El pedido #1023 fue entregado con 2 días de demora."),
            ("Alerta de inventario", "3 productos están por debajo del stock mínimo."),
            ("Venta importante", "Cliente Taller Mecánico realizó compra por $450.000."),
        };

        var messagesEmployee = new[]
        {
            ("Asignación de pedido", "Se te ha asignado el pedido #2056 para preparación."),
            ("Recordatorio de inventario", "Realizar conteo físico de neumáticos antes del viernes."),
            ("Nueva cotización", "Cliente solicita cotización para kit de embrague completo."),
            ("Producto devuelto", "Cliente devolvió batería Moura 65Ah. Verificar estado."),
        };

        var messagesClient = new[]
        {
            ("Bienvenido a AutoPartes", "¡Gracias por registrarte! Encuentra los mejores repuestos."),
            ("Oferta especial", "25% OFF en pastillas y discos de freno Brembo."),
            ("Producto en oferta", "El Kit de Distribución está en oferta."),
            ("Tu pedido fue confirmado", "Recibimos tu pedido correctamente."),
        };

        foreach (var user in users)
        {
            var messages = user.Role == "Admin" ? messagesAdmin
                         : user.Role == "Employee" ? messagesEmployee
                         : messagesClient;

            var numNotifications = _random.Next(3, 7);

            for (int i = 0; i < numNotifications; i++)
            {
                var (title, message) = messages[_random.Next(messages.Length)];
                notifications.Add(new Notification
                {
                    UserId = user.Id,
                    Title = title,
                    Message = message,
                    CreatedAt = DateTime.Now.AddDays(-_random.Next(1, 60)),
                    IsRead = _random.Next(0, 100) < 60
                });
            }
        }

        _context.Notifications.AddRange(notifications);
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Reclamos

    private async Task CreateClaimsAsync(List<User> users)
    {
        if (await _context.Claims.AnyAsync())
        {
            Console.WriteLine("   ⚠️  Los reclamos ya existen, omitiendo...");
            return;
        }

        var claims = new List<Claim>();
        var clients = users.Where(u => u.Role == "Client").Take(4).ToList();
        var admins = users.Where(u => u.Role == "Admin").ToList();

        var asuntos = new[]
        {
            "Producto defectuoso",
            "Entrega con demora",
            "Producto no coincide con descripción",
            "Falta de stock informado",
            "Consulta sobre garantía"
        };

        foreach (var client in clients)
        {
            var numClaims = _random.Next(1, 3);

            for (int i = 0; i < numClaims; i++)
            {
                var estado = (StatusClaim)_random.Next(1, 6);
                var fechaCreacion = DateTime.Now.AddDays(-_random.Next(1, 60));

                claims.Add(new Claim
                {
                    NumeroTicket = $"TKT-{_random.Next(10000, 99999)}",
                    Asunto = asuntos[_random.Next(asuntos.Length)],
                    Descripcion = "Descripción detallada del reclamo realizado por el cliente.",
                    NivelUrgencia = (LevelUrgency)_random.Next(1, 5),
                    Estado = estado,
                    FechaCreacion = fechaCreacion,
                    FechaActualizacion = fechaCreacion.AddDays(_random.Next(1, 5)),
                    FechaCierre = estado == StatusClaim.Resuelto || estado == StatusClaim.Cerrado
                        ? fechaCreacion.AddDays(_random.Next(3, 10))
                        : null,
                    ClienteId = client.Id,
                    AdministradorAsignadoId = estado != StatusClaim.Nuevo
                        ? admins[_random.Next(admins.Count)].Id
                        : null
                });
            }
        }

        _context.Claims.AddRange(claims);
        await _context.SaveChangesAsync();
    }

    #endregion

    #region Movimientos de Stock

    private async Task CreateStockMovementsAsync(List<Product> products)
    {
        if (await _context.StockMovements.AnyAsync())
        {
            Console.WriteLine("   ⚠️  Los movimientos de stock ya existen, omitiendo...");
            return;
        }

        var movements = new List<StockMovement>();

        // Crear algunos movimientos para productos seleccionados
        var selectedProducts = products.Take(10).ToList();

        foreach (var product in selectedProducts)
        {
            var numMovements = _random.Next(2, 5);

            for (int i = 0; i < numMovements; i++)
            {
                var movementType = (StockMovementType)_random.Next(0, 5);
                var quantity = _random.Next(5, 30);
                var previousStock = product.Stock;
                var newStock = movementType == StockMovementType.PurchaseEntry ||
                              movementType == StockMovementType.AdjustmentIncrease
                    ? previousStock + quantity
                    : previousStock - quantity;

                movements.Add(new StockMovement
                {
                    ProductId = product.Id,
                    MovementType = movementType,
                    Quantity = quantity,
                    PreviousStock = previousStock,
                    NewStock = newStock,
                    Reason = GetStockMovementReason(movementType),
                    CreatedAt = DateTime.Now.AddDays(-_random.Next(1, 90)),
                    UserName = "Sistema"
                });
            }
        }

        _context.StockMovements.AddRange(movements);
        await _context.SaveChangesAsync();
    }

    private string GetStockMovementReason(StockMovementType type)
    {
        return type switch
        {
            StockMovementType.PurchaseEntry => "Recepción de mercadería de proveedor",
            StockMovementType.SaleExit => "Venta a cliente",
            StockMovementType.AdjustmentIncrease => "Ajuste por conteo físico",
            StockMovementType.AdjustmentDecrease => "Ajuste por diferencia de inventario",
            StockMovementType.Return => "Devolución de cliente",
            _ => "Movimiento de stock"
        };
    }

    #endregion

    #region Imprimir Credenciales

    private void PrintCredentials()
    {
        Console.WriteLine("╔═══════════════════════════════════════════════════════════╗");
        Console.WriteLine("║              CREDENCIALES DE ACCESO                       ║");
        Console.WriteLine("╠═══════════════════════════════════════════════════════════╣");
        Console.WriteLine("║                                                           ║");
        Console.WriteLine("║  👨‍💼 SUPER ADMINISTRADOR:                                  ║");
        Console.WriteLine("║     Email: superadmin@autopartes.com                      ║");
        Console.WriteLine("║     Password: SuperAdmin123!                              ║");
        Console.WriteLine("║                                                           ║");
        Console.WriteLine("║  👨‍💼 ADMINISTRADOR:                                        ║");
        Console.WriteLine("║     Email: admin@autopartes.com                           ║");
        Console.WriteLine("║     Password: Admin123!                                   ║");
        Console.WriteLine("║                                                           ║");
        Console.WriteLine("║  👔 EMPLEADOS:                                            ║");
        Console.WriteLine("║     Email: vendedor@autopartes.com                        ║");
        Console.WriteLine("║     Email: deposito@autopartes.com                        ║");
        Console.WriteLine("║     Email: atencion@autopartes.com                        ║");
        Console.WriteLine("║     Password (todos): Empleado123!                        ║");
        Console.WriteLine("║                                                           ║");
        Console.WriteLine("║  👥 CLIENTES:                                             ║");
        Console.WriteLine("║     Email: taller.mecanico@email.com                      ║");
        Console.WriteLine("║     Email: gomeria.central@email.com                      ║");
        Console.WriteLine("║     Email: juan.perez@email.com                           ║");
        Console.WriteLine("║     Email: maria.rodriguez@email.com                      ║");
        Console.WriteLine("║     Email: carlos.gomez@email.com                         ║");
        Console.WriteLine("║     Password (todos): Cliente123!                         ║");
        Console.WriteLine("║                                                           ║");
        Console.WriteLine("╚═══════════════════════════════════════════════════════════╝\n");
    }

    #endregion
}
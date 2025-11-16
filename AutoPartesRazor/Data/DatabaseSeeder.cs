using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using AutoPartesRazor.Models;
using AutoPartesRazor.Data;

namespace AutoPartesRazor.Data;

public class DatabaseSeeder
{
    private readonly AutoPartesRazorContext _context;
    private readonly UserManager<User> _userManager;
    private readonly Random _random = new Random();

    public DatabaseSeeder(AutoPartesRazorContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task SeedAsync()
    {
        try
        {
            Console.WriteLine("=== INICIANDO CARGA DE DATOS DE PRUEBA - AUTOPARTES ===");

            // 1. CREAR USUARIOS
            Console.WriteLine("📝 Creando usuarios...");
            var users = await CreateUsersAsync();
            Console.WriteLine($"✅ {users.Count} usuarios creados");

            // 2. CREAR CATEGORÍAS
            Console.WriteLine("📁 Creando categorías de autopartes...");
            var categories = CreateCategories();
            await _context.SaveChangesAsync();
            Console.WriteLine($"✅ {categories.Count} categorías creadas");

            // 3. CREAR MARCAS
            Console.WriteLine("🏷️ Creando marcas automotrices...");
            var brands = CreateBrands();
            await _context.SaveChangesAsync();
            Console.WriteLine($"✅ {brands.Count} marcas creadas");

            // 4. CREAR PROVEEDORES
            Console.WriteLine("🏭 Creando proveedores...");
            var suppliers = CreateSuppliers();
            await _context.SaveChangesAsync();
            Console.WriteLine($"✅ {suppliers.Count} proveedores creados");

            // 5. CREAR PRODUCTOS
            Console.WriteLine("📦 Creando repuestos y accesorios...");
            var products = CreateProducts(categories, brands);
            await _context.SaveChangesAsync();
            Console.WriteLine($"✅ {products.Count} productos creados");

            // 6. CREAR RELACIONES PRODUCTO-PROVEEDOR
            Console.WriteLine("🔗 Creando relaciones producto-proveedor...");
            CreateProductSuppliers(products, suppliers);
            await _context.SaveChangesAsync();
            Console.WriteLine("✅ Relaciones producto-proveedor creadas");

            // 7. CREAR ÓRDENES DE COMPRA
            Console.WriteLine("📋 Creando órdenes de compra...");
            CreatePurchaseOrders(products, suppliers);
            await _context.SaveChangesAsync();
            Console.WriteLine("✅ Órdenes de compra creadas");

            // 8. CREAR CARRITOS
            Console.WriteLine("🛒 Creando carritos...");
            CreateCarts(users, products);
            await _context.SaveChangesAsync();
            Console.WriteLine("✅ Carritos creados");

            // 9. CREAR ÓRDENES
            Console.WriteLine("📊 Creando órdenes de venta...");
            CreateOrders(users, products);
            await _context.SaveChangesAsync();
            Console.WriteLine("✅ Órdenes de venta creadas");

            // 10. CREAR NOTIFICACIONES
            Console.WriteLine("🔔 Creando notificaciones...");
            CreateNotifications(users);
            await _context.SaveChangesAsync();
            Console.WriteLine("✅ Notificaciones creadas");

            Console.WriteLine("\n=== ✅ DATOS DE PRUEBA CREADOS EXITOSAMENTE ===\n");
            PrintCredentials();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ ERROR: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            throw;
        }
    }

    private void PrintCredentials()
    {
        Console.WriteLine("═══════════════════════════════════════════");
        Console.WriteLine("        CREDENCIALES DE ACCESO");
        Console.WriteLine("═══════════════════════════════════════════");
        Console.WriteLine("\n👨‍💼 ADMINISTRADOR:");
        Console.WriteLine("   Email: admin@autopartes.com");
        Console.WriteLine("   Password: Admin123!");
        Console.WriteLine("\n👔 EMPLEADOS:");
        Console.WriteLine("   Email: vendedor@autopartes.com");
        Console.WriteLine("   Password: Empleado123!");
        Console.WriteLine("   Email: deposito@autopartes.com");
        Console.WriteLine("   Password: Empleado123!");
        Console.WriteLine("\n👥 CLIENTES:");
        Console.WriteLine("   Email: taller.mecanico@email.com");
        Console.WriteLine("   Email: gomeria.central@email.com");
        Console.WriteLine("   Email: juan.perez@email.com");
        Console.WriteLine("   Email: maria.rodriguez@email.com");
        Console.WriteLine("   Email: carlos.gomez@email.com");
        Console.WriteLine("   Password (todos): Cliente123!");
        Console.WriteLine("═══════════════════════════════════════════\n");
    }

    private async Task<List<User>> CreateUsersAsync()
    {
        var users = new List<User>();

        // Admin
        var admin = new User
        {
            UserName = "admin@autopartes.com",
            Email = "admin@autopartes.com",
            FullName = "Administrador Principal",
            Role = "Admin",
            Address = "Av. Córdoba 2500, Buenos Aires",
            PhoneNumber = "1145678900",
            EmailConfirmed = true,
            RegistrationDate = DateTime.Now.AddYears(-2)
        };
        var adminResult = await _userManager.CreateAsync(admin, "Admin123!");
        if (adminResult.Succeeded) users.Add(admin);

        // Empleados
        var vendedor = new User
        {
            UserName = "vendedor@autopartes.com",
            Email = "vendedor@autopartes.com",
            FullName = "Roberto Díaz",
            Role = "Employee",
            Address = "Av. Corrientes 1500, Buenos Aires",
            PhoneNumber = "1156789011",
            EmailConfirmed = true,
            RegistrationDate = DateTime.Now.AddMonths(-8)
        };
        var vendedorResult = await _userManager.CreateAsync(vendedor, "Empleado123!");
        if (vendedorResult.Succeeded) users.Add(vendedor);

        var deposito = new User
        {
            UserName = "deposito@autopartes.com",
            Email = "deposito@autopartes.com",
            FullName = "Miguel Torres",
            Role = "Employee",
            Address = "Av. Warnes 2200, Buenos Aires",
            PhoneNumber = "1167890122",
            EmailConfirmed = true,
            RegistrationDate = DateTime.Now.AddMonths(-10)
        };
        var depositoResult = await _userManager.CreateAsync(deposito, "Empleado123!");
        if (depositoResult.Succeeded) users.Add(deposito);

        // Clientes
        var clientNames = new[]
        {
            ("Taller Mecánico San Martín", "taller.mecanico@email.com", "Av. San Martín 3450, Buenos Aires", "Cliente comercial - Taller mecánico"),
            ("Gomería Central", "gomeria.central@email.com", "Av. Rivadavia 7890, Buenos Aires", "Cliente comercial - Gomería"),
            ("Juan Pérez", "juan.perez@email.com", "Calle Alsina 567, Buenos Aires", "Cliente particular"),
            ("María Rodríguez", "maria.rodriguez@email.com", "Av. Santa Fe 2100, Buenos Aires", "Cliente particular"),
            ("Carlos Gómez", "carlos.gomez@email.com", "Calle Lavalle 890, Buenos Aires", "Cliente particular")
        };

        foreach (var (name, email, address, description) in clientNames)
        {
            var client = new User
            {
                UserName = email,
                Email = email,
                FullName = name,
                Role = "Client",
                Address = address,
                PhoneNumber = $"11{_random.Next(40000000, 69999999)}",
                EmailConfirmed = true,
                RegistrationDate = DateTime.Now.AddDays(-_random.Next(30, 365))
            };
            var clientResult = await _userManager.CreateAsync(client, "Cliente123!");
            if (clientResult.Succeeded) users.Add(client);
        }

        return users;
    }

    private List<Category> CreateCategories()
    {
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
        return categories;
    }

    private List<Brand> CreateBrands()
    {
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
        return brands;
    }

    private List<Supplier> CreateSuppliers()
    {
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
        return suppliers;
    }

    private List<Product> CreateProducts(List<Category> categories, List<Brand> brands)
    {
        var products = new List<Product>
        {
            // MOTOR
            new Product
            {
                Name = "Bujías NGK Platino (Set x4)",
                Description = "Bujías de platino de alta duración. Compatible con motores nafteros. Mayor rendimiento y economía de combustible.",
                Price = 28000m,
                Stock = 45,
                MinimumStock = 15,
                CategoryId = categories[0].Id,
                BrandId = brands[2].Id
            },
            new Product
            {
                Name = "Kit de Distribución Completo",
                Description = "Kit completo con correa, tensor y rodillos. Compatible con múltiples modelos. Incluye instructivo de instalación.",
                Price = 85000m,
                Stock = 12,
                MinimumStock = 5,
                CategoryId = categories[0].Id,
                BrandId = brands[11].Id
            },
            new Product
            {
                Name = "Bomba de Agua Original",
                Description = "Bomba de agua de calidad OEM. Garantía de 12 meses. Alta durabilidad y rendimiento.",
                Price = 45000m,
                Stock = 20,
                MinimumStock = 8,
                CategoryId = categories[0].Id,
                BrandId = brands[0].Id
            },
            new Product
            {
                Name = "Filtro de Aire Deportivo",
                Description = "Filtro de aire de alto flujo, lavable y reutilizable. Aumenta potencia hasta 5HP.",
                Price = 32000m,
                Stock = 18,
                MinimumStock = 6,
                CategoryId = categories[7].Id,
                BrandId = brands[14].Id
            },

            // FRENOS
            new Product
            {
                Name = "Pastillas de Freno Delanteras Brembo",
                Description = "Pastillas cerámicas de alto rendimiento. Menor ruido y polvo. Excelente poder de frenado.",
                Price = 42000m,
                Stock = 35,
                MinimumStock = 12,
                CategoryId = categories[1].Id,
                BrandId = brands[1].Id
            },
            new Product
            {
                Name = "Discos de Freno Ventilados (Par)",
                Description = "Discos ventilados de hierro fundido. Mejor disipación de calor. Incluye tornillos de fijación.",
                Price = 68000m,
                Stock = 15,
                MinimumStock = 6,
                CategoryId = categories[1].Id,
                BrandId = brands[0].Id
            },
            new Product
            {
                Name = "Líquido de Frenos DOT 4 (500ml)",
                Description = "Líquido de frenos sintético DOT 4. Alto punto de ebullición. Para sistemas ABS.",
                Price = 4500m,
                Stock = 80,
                MinimumStock = 20,
                CategoryId = categories[1].Id,
                BrandId = brands[5].Id
            },
            new Product
            {
                Name = "Kit Reparación Cilindro Freno",
                Description = "Kit completo para reparación de cilindro maestro. Incluye retenes y pistones.",
                Price = 15000m,
                Stock = 25,
                MinimumStock = 8,
                CategoryId = categories[1].Id,
                BrandId = brands[14].Id
            },

            // SUSPENSIÓN
            new Product
            {
                Name = "Amortiguadores Monroe Delanteros (Par)",
                Description = "Amortiguadores hidráulicos de gas. Tecnología Monroe OESpectrum. Garantía 2 años.",
                Price = 95000m,
                Stock = 10,
                MinimumStock = 4,
                CategoryId = categories[2].Id,
                BrandId = brands[6].Id
            },
            new Product
            {
                Name = "Espirales Traseros Progresivos",
                Description = "Resortes helicoidales progresivos. Mayor estabilidad y confort. Acero templado.",
                Price = 38000m,
                Stock = 18,
                MinimumStock = 6,
                CategoryId = categories[2].Id,
                BrandId = brands[14].Id
            },
            new Product
            {
                Name = "Kit Rotulas Dirección Completo",
                Description = "Kit con rótulas internas y externas. Incluye terminales axiales. Para tren delantero completo.",
                Price = 52000m,
                Stock = 14,
                MinimumStock = 5,
                CategoryId = categories[2].Id,
                BrandId = brands[14].Id
            },

            // TRANSMISIÓN
            new Product
            {
                Name = "Embrague Kit Completo",
                Description = "Kit completo: disco, plato y collarin. Compatible múltiples modelos. Instalación profesional recomendada.",
                Price = 125000m,
                Stock = 8,
                MinimumStock = 3,
                CategoryId = categories[3].Id,
                BrandId = brands[12].Id
            },
            new Product
            {
                Name = "Aceite Transmisión ATF Dexron III",
                Description = "Aceite sintético para transmisión automática. 1 litro. Protección superior.",
                Price = 8500m,
                Stock = 60,
                MinimumStock = 20,
                CategoryId = categories[3].Id,
                BrandId = brands[5].Id
            },

            // SISTEMA ELÉCTRICO
            new Product
            {
                Name = "Batería Moura 12V 65Ah",
                Description = "Batería sellada libre de mantenimiento. 650A arranque en frío. Garantía 12 meses.",
                Price = 95000m,
                Stock = 22,
                MinimumStock = 8,
                CategoryId = categories[10].Id,
                BrandId = brands[13].Id
            },
            new Product
            {
                Name = "Alternador Bosch Remanufacturado",
                Description = "Alternador remanufacturado 90A. Garantía 6 meses. Incluye polea.",
                Price = 78000m,
                Stock = 6,
                MinimumStock = 3,
                CategoryId = categories[4].Id,
                BrandId = brands[0].Id
            },
            new Product
            {
                Name = "Burro de Arranque 12V",
                Description = "Motor de arranque remanufacturado. Potencia original. Garantía 6 meses.",
                Price = 65000m,
                Stock = 8,
                MinimumStock = 3,
                CategoryId = categories[4].Id,
                BrandId = brands[10].Id
            },
            new Product
            {
                Name = "Bobina de Encendido Individual",
                Description = "Bobina de encendido electrónica. Mayor voltaje y eficiencia. Para motores modernos.",
                Price = 18500m,
                Stock = 30,
                MinimumStock = 10,
                CategoryId = categories[4].Id,
                BrandId = brands[10].Id
            },

            // REFRIGERACIÓN
            new Product
            {
                Name = "Radiador de Aluminio Reforzado",
                Description = "Radiador completo de aluminio. Mayor eficiencia de enfriamiento. Incluye tapón.",
                Price = 115000m,
                Stock = 5,
                MinimumStock = 2,
                CategoryId = categories[5].Id,
                BrandId = brands[12].Id
            },
            new Product
            {
                Name = "Termostato con Junta",
                Description = "Termostato de apertura gradual. Temperatura 82°C. Incluye junta de sellado.",
                Price = 12000m,
                Stock = 40,
                MinimumStock = 15,
                CategoryId = categories[5].Id,
                BrandId = brands[0].Id
            },
            new Product
            {
                Name = "Líquido Refrigerante Concentrado 5L",
                Description = "Refrigerante orgánico de larga duración. Protección hasta -37°C. Color verde.",
                Price = 15000m,
                Stock = 50,
                MinimumStock = 15,
                CategoryId = categories[5].Id,
                BrandId = brands[5].Id
            },
            new Product
            {
                Name = "Electroventilador con Motoventilador",
                Description = "Electroventilador completo 12V. Incluye motoventilador y aspas. Alta eficiencia.",
                Price = 55000m,
                Stock = 10,
                MinimumStock = 4,
                CategoryId = categories[5].Id,
                BrandId = brands[14].Id
            },

            // ESCAPE
            new Product
            {
                Name = "Caño de Escape Completo",
                Description = "Sistema de escape completo acero inoxidable. Incluye silenciador y caño trasero.",
                Price = 145000m,
                Stock = 4,
                MinimumStock = 2,
                CategoryId = categories[6].Id,
                BrandId = brands[14].Id
            },
            new Product
            {
                Name = "Silenciador Deportivo",
                Description = "Silenciador de flujo libre. Sonido deportivo. Acero inoxidable 409.",
                Price = 85000m,
                Stock = 7,
                MinimumStock = 3,
                CategoryId = categories[6].Id,
                BrandId = brands[14].Id
            },

            // FILTROS
            new Product
            {
                Name = "Filtro de Aceite Mann Filter",
                Description = "Filtro de aceite de alta eficiencia. Compatible múltiples motores. Cambio cada 10.000km.",
                Price = 6500m,
                Stock = 120,
                MinimumStock = 40,
                CategoryId = categories[7].Id,
                BrandId = brands[3].Id
            },
            new Product
            {
                Name = "Filtro de Combustible Original",
                Description = "Filtro de nafta/gasoil alta calidad. Protección bomba combustible. Cambio 15.000km.",
                Price = 8500m,
                Stock = 80,
                MinimumStock = 25,
                CategoryId = categories[7].Id,
                BrandId = brands[3].Id
            },
            new Product
            {
                Name = "Filtro Habitáculo Carbón Activado",
                Description = "Filtro de aire acondicionado con carbón activado. Elimina olores y bacterias.",
                Price = 12000m,
                Stock = 55,
                MinimumStock = 20,
                CategoryId = categories[7].Id,
                BrandId = brands[3].Id
            },

            // ACEITES Y LUBRICANTES
            new Product
            {
                Name = "Aceite Motor Mobil Super 3000 5W-40 (4L)",
                Description = "Aceite sintético multivehículo. Protección superior motor. Normas API SN, ACEA A3/B4.",
                Price = 32000m,
                Stock = 45,
                MinimumStock = 15,
                CategoryId = categories[8].Id,
                BrandId = brands[4].Id
            },
            new Product
            {
                Name = "Aceite Castrol Edge 5W-30 (4L)",
                Description = "Aceite totalmente sintético. Tecnología Fluid Titanium. Máxima protección.",
                Price = 38000m,
                Stock = 35,
                MinimumStock = 12,
                CategoryId = categories[8].Id,
                BrandId = brands[5].Id
            },
            new Product
            {
                Name = "Grasa Multiuso Litio EP2 (400g)",
                Description = "Grasa lubricante multiuso. Resistente agua y temperatura. Para rodamientos y articulaciones.",
                Price = 3500m,
                Stock = 90,
                MinimumStock = 30,
                CategoryId = categories[8].Id,
                BrandId = brands[14].Id
            },
            new Product
            {
                Name = "Aceite Diferencial 80W-90 (1L)",
                Description = "Aceite para diferencial y caja transferencia. Alta presión extrema. API GL-5.",
                Price = 7500m,
                Stock = 40,
                MinimumStock = 15,
                CategoryId = categories[8].Id,
                BrandId = brands[4].Id
            },

            // NEUMÁTICOS
            new Product
            {
                Name = "Neumático Pirelli P7 195/65 R15",
                Description = "Neumático premium todo terreno. Excelente adherencia en mojado. Bajo ruido rodadura.",
                Price = 85000m,
                Stock = 24,
                MinimumStock = 8,
                CategoryId = categories[9].Id,
                BrandId = brands[8].Id
            },
            new Product
            {
                Name = "Neumático Michelin Primacy 205/55 R16",
                Description = "Neumático de alto rendimiento. Mayor durabilidad y confort. Eficiencia energética A.",
                Price = 95000m,
                Stock = 20,
                MinimumStock = 8,
                CategoryId = categories[9].Id,
                BrandId = brands[9].Id
            },
            new Product
            {
                Name = "Cubierta Fate AR-440 175/70 R13",
                Description = "Neumático económico para ciudad. Buena relación precio/calidad. Uso moderado.",
                Price = 45000m,
                Stock = 40,
                MinimumStock = 12,
                CategoryId = categories[9].Id,
                BrandId = brands[14].Id
            },

            // ILUMINACIÓN
            new Product
            {
                Name = "Kit Luces LED H7 6000K",
                Description = "Kit conversión LED para faros principales. 6000 lúmenes. Luz blanca brillante.",
                Price = 22000m,
                Stock = 30,
                MinimumStock = 10,
                CategoryId = categories[11].Id,
                BrandId = brands[7].Id
            },
            new Product
            {
                Name = "Foco Halógeno H4 12V 60/55W",
                Description = "Lámpara halógena estándar. Mayor luminosidad que original. Pack x2 unidades.",
                Price = 4500m,
                Stock = 100,
                MinimumStock = 30,
                CategoryId = categories[11].Id,
                BrandId = brands[7].Id
            },
            new Product
            {
                Name = "Barra LED Auxiliar 36W",
                Description = "Barra de LED para off-road. 3600 lúmenes. Montaje universal. IP68 resistente agua.",
                Price = 18000m,
                Stock = 15,
                MinimumStock = 5,
                CategoryId = categories[11].Id,
                BrandId = brands[14].Id
            },

            // ACCESORIOS
            new Product
            {
                Name = "Alfombras Goma Premium (Set x4)",
                Description = "Alfombras de goma universales. Fácil limpieza. Antideslizantes con bordes elevados.",
                Price = 12000m,
                Stock = 45,
                MinimumStock = 15,
                CategoryId = categories[12].Id,
                BrandId = brands[14].Id
            },
            new Product
            {
                Name = "Cubre Volante Cuero Premium",
                Description = "Funda de volante cuero ecológico. Mayor agarre y confort. Talle universal M.",
                Price = 8500m,
                Stock = 55,
                MinimumStock = 20,
                CategoryId = categories[12].Id,
                BrandId = brands[14].Id
            },
            new Product
            {
                Name = "Organizador Baúl Plegable",
                Description = "Organizador de baúl con compartimientos. Plegable y lavable. 3 divisiones ajustables.",
                Price = 6500m,
                Stock = 35,
                MinimumStock = 12,
                CategoryId = categories[12].Id,
                BrandId = brands[14].Id
            },
            new Product
            {
                Name = "Kit Herramientas Básicas Auto",
                Description = "Kit 23 piezas: llaves, destornilladores, pinzas. Estuche resistente. Ideal emergencias.",
                Price = 15000m,
                Stock = 28,
                MinimumStock = 10,
                CategoryId = categories[12].Id,
                BrandId = brands[14].Id
            },

            // CARROCERÍA
            new Product
            {
                Name = "Espejo Retrovisor Derecho Eléctrico",
                Description = "Espejo con regulación eléctrica. Calefaccionado. Compatible múltiples modelos.",
                Price = 35000m,
                Stock = 12,
                MinimumStock = 4,
                CategoryId = categories[13].Id,
                BrandId = brands[14].Id
            },
            new Product
            {
                Name = "Manija Exterior Puerta Cromada",
                Description = "Manija de puerta cromada. Calidad OEM. Incluye cilindro y llaves.",
                Price = 18000m,
                Stock = 20,
                MinimumStock = 8,
                CategoryId = categories[13].Id,
                BrandId = brands[14].Id
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
                BrandId = brands[0].Id
            },
            new Product
            {
                Name = "Correa Poli V 6 canales",
                Description = "Correa poly V para accesorios motor. Alta resistencia. 6 nervios.",
                Price = 8500m,
                Stock = 4,
                MinimumStock = 12,
                CategoryId = categories[0].Id,
                BrandId = brands[11].Id
            },
            new Product
            {
                Name = "Kit Empacaduras Motor Completo",
                Description = "Juego completo de juntas motor. Incluye retenes y o-rings. Calidad OEM.",
                Price = 55000m,
                Stock = 2,
                MinimumStock = 6,
                CategoryId = categories[0].Id,
                BrandId = brands[14].Id
            }
        };

        _context.Products.AddRange(products);
        return products;
    }

    private void CreateProductSuppliers(List<Product> products, List<Supplier> suppliers)
    {
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
    }

    private void CreatePurchaseOrders(List<Product> products, List<Supplier> suppliers)
    {
        var purchaseOrders = new List<PurchaseOrder>();
        var statuses = new[] { "Pending", "Approved", "Received", "Cancelled" };

        // Crear 25 órdenes de compra
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
    }

    private void CreateCarts(List<User> users, List<Product> products)
    {
        var carts = new List<Cart>();
        var clients = users.Where(u => u.Role == "Client").ToList();

        // 3 clientes tienen carritos activos
        for (int i = 0; i < Math.Min(3, clients.Count); i++)
        {
            var client = clients[i];
            var numItems = _random.Next(2, 5);

            for (int j = 0; j < numItems; j++)
            {
                var product = products[_random.Next(products.Count)];
                carts.Add(new Cart
                {
                    UserId = client.Id,
                    ProductId = product.Id,
                    Quantity = _random.Next(1, 4),
                    CreatedAt = DateTime.Now.AddDays(-_random.Next(1, 10))
                });
            }
        }

        _context.Carts.AddRange(carts);
    }

    private void CreateOrders(List<User> users, List<Product> products)
    {
        var orders = new List<Order>();
        var clients = users.Where(u => u.Role == "Client").ToList();
        var statuses = new[] { "Pending", "Processing", "Shipped", "Delivered", "Cancelled" };
        var paymentMethods = new[] { "Tarjeta de Crédito", "Tarjeta de Débito", "Transferencia Bancaria", "Efectivo", "MercadoPago" };

        // Crear 30 órdenes
        for (int i = 0; i < 30; i++)
        {
            var client = clients[_random.Next(clients.Count)];
            var numItems = _random.Next(1, 6);
            var orderItems = new List<OrderItem>();
            decimal total = 0;

            for (int j = 0; j < numItems; j++)
            {
                var product = products[_random.Next(products.Count)];
                var quantity = _random.Next(1, 3);
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
                Status = status,
                CreatedAt = createdDate,
                UpdatedAt = status != "Pending" ? createdDate.AddDays(_random.Next(1, 7)) : null,
                Items = orderItems,
                Calificacion = status == "Delivered" ? _random.Next(3, 6) : null
            };

            orders.Add(order);
        }

        _context.Orders.AddRange(orders);
    }

    private void CreateNotifications(List<User> users)
    {
        var notifications = new List<Notification>();
        var messagesAdmin = new[]
        {
            ("Stock crítico detectado", "Los siguientes productos requieren reposición urgente: Bujías NGK, Filtro aceite Mann, Líquido de frenos DOT 4."),
            ("Nueva orden de compra", "Se ha recibido una nueva orden de compra de Distribuidora Repuestos SA por $350.000."),
            ("Pedido entregado con demora", "El pedido #1023 fue entregado con 2 días de demora. Revisar logística."),
            ("Alerta de inventario", "15 productos están por debajo del stock mínimo. Generar órdenes de compra."),
            ("Nuevo proveedor registrado", "Se ha agregado un nuevo proveedor: Lubricantes del Sur. Revisar catálogo."),
            ("Venta importante", "Cliente Taller Mecánico San Martín realizó compra por $450.000. Coordinar envío."),
            ("Producto más vendido", "Las pastillas Brembo son el producto más vendido del mes con 45 unidades.")
        };

        var messagesEmployee = new[]
        {
            ("Asignación de pedido", "Se te ha asignado el pedido #2056 para preparación. Cliente: Gomería Central."),
            ("Recordatorio de inventario", "Realizar conteo físico de neumáticos en depósito antes del viernes."),
            ("Nueva cotización", "Cliente solicita cotización para kit de embrague completo. Responder en 24hs."),
            ("Producto devuelto", "Cliente devolvió batería Moura 65Ah. Verificar estado y procesar garantía."),
            ("Capacitación programada", "Capacitación sobre nuevos productos Bosch el próximo lunes 10:00hs.")
        };

        var messagesClient = new[]
        {
            ("Bienvenido a AutoPartes", "¡Gracias por registrarte! Encuentra los mejores repuestos para tu vehículo a precios increíbles."),
            ("Tu pedido fue confirmado", "Recibimos tu pedido correctamente. Lo estamos preparando para el envío."),
            ("Pedido en camino", "¡Tu pedido está en camino! Recibirás tus repuestos en 24-48hs hábiles."),
            ("Pedido entregado", "Tu pedido fue entregado con éxito. ¡Esperamos que disfrutes tus productos!"),
            ("Oferta especial en frenos", "🔥 25% OFF en pastillas y discos de freno Brembo. Stock limitado."),
            ("Llegaron los neumáticos Michelin", "Nueva línea de neumáticos Michelin disponible. Consulta por tu medida."),
            ("Promoción aceites sintéticos", "Llevá 4 litros de aceite sintético y llevate el filtro GRATIS."),
            ("Tu opinión nos importa", "¿Cómo fue tu experiencia? Calificá tu última compra y obtené 10% descuento."),
            ("Recordatorio de mantenimiento", "¿Hace más de 10.000km del último service? Revisá nuestros kits de mantenimiento."),
            ("Producto en tu lista de deseos", "El Kit de Distribución que miraste está en oferta. ¡No te lo pierdas!")
        };

        foreach (var user in users)
        {
            var messages = user.Role == "Admin" ? messagesAdmin
                         : user.Role == "Employee" ? messagesEmployee
                         : messagesClient;

            var numNotifications = user.Role == "Admin" ? _random.Next(5, 10)
                                 : user.Role == "Employee" ? _random.Next(3, 7)
                                 : _random.Next(3, 8);

            for (int i = 0; i < numNotifications; i++)
            {
                var (title, message) = messages[_random.Next(messages.Length)];
                notifications.Add(new Notification
                {
                    UserId = user.Id,
                    Title = title,
                    Message = message,
                    CreatedAt = DateTime.Now.AddDays(-_random.Next(1, 60)),
                    IsRead = _random.Next(0, 100) < 70 // 70% leídas
                });
            }
        }

        _context.Notifications.AddRange(notifications);
    }
}
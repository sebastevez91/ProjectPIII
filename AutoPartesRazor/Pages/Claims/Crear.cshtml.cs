using AutoPartesRazor.Interfaces;
using AutoPartesRazor.Models;
using AutoPartesRazor.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace AutoPartesRazor.Pages.Reclamos;

[Authorize]
public class CrearModel : PageModel
{
    private readonly IClaimService _reclamoService;
    private readonly UserManager<User> _userManager;
    private readonly Data.AutoPartesRazorContext _context;

    public CrearModel(IClaimService reclamoService, UserManager<User> userManager, Data.AutoPartesRazorContext context)
    {
        _reclamoService = reclamoService;
        _userManager = userManager;
        _context = context;
    }

    [BindProperty]
    public CrearReclamoViewModel Input { get; set; } = new CrearReclamoViewModel();

    // MODIFICAR ESTE MÉTODO
    public async Task OnGetAsync()
    {
        var usuario = await _userManager.GetUserAsync(User);
        if (usuario != null)
        {
            // ✅ CAMBIAR ESTO
            var pedidos = await _context.Orders
                .Where(o => o.UserId != null && o.UserId == usuario.Id && !o.IsDeleted)  // Agregar verificación de null y IsDeleted
                .OrderByDescending(o => o.CreatedAt)
                .Take(20)
                .ToListAsync();

            // Debug temporal
            Console.WriteLine($"=== DEBUG CREAR RECLAMO ===");
            Console.WriteLine($"Usuario ID: {usuario.Id}");
            Console.WriteLine($"Usuario Email: {usuario.Email}");
            Console.WriteLine($"Pedidos encontrados: {pedidos.Count}");

            if (pedidos.Any())
            {
                foreach (var p in pedidos)
                {
                    Console.WriteLine($"  - Pedido #{p.Id} | Fecha: {p.CreatedAt:dd/MM/yyyy} | Total: ${p.Total} | UserId: {p.UserId}");
                }
            }
            else
            {
                Console.WriteLine("  ⚠️ NO SE ENCONTRARON PEDIDOS");

                var totalPedidos = await _context.Orders.Where(o => !o.IsDeleted).CountAsync();
                Console.WriteLine($"  Total pedidos activos en BD: {totalPedidos}");

                var todosPedidos = await _context.Orders
                    .Where(o => !o.IsDeleted)
                    .Select(o => new { o.Id, o.UserId, o.CustomerEmail })
                    .Take(10)
                    .ToListAsync();

                Console.WriteLine("  Pedidos en la BD:");
                foreach (var p in todosPedidos)
                {
                    Console.WriteLine($"    Order #{p.Id} | UserId: '{p.UserId}' | Email: {p.CustomerEmail}");
                }
            }
            Console.WriteLine($"===========================");

            Input = new CrearReclamoViewModel
            {
                PedidosDisponibles = pedidos
            };
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            // RECARGAR PEDIDOS SI HAY ERROR
            var usuarioTemp = await _userManager.GetUserAsync(User);
            if (usuarioTemp != null)
            {
                Input.PedidosDisponibles = await _context.Orders
                    .Where(o => o.UserId == usuarioTemp.Id)
                    .OrderByDescending(o => o.CreatedAt)
                    .Take(20)
                    .ToListAsync();
            }
            return Page();
        }

        var usuario = await _userManager.GetUserAsync(User);
        if (usuario == null)
        {
            return RedirectToPage("/Account/Login");
        }

        try
        {
            // MODIFICAR PARA INCLUIR EL ORDERID
            var reclamo = await _reclamoService.CrearReclamoAsync(
                usuario.Id,
                Input.Asunto,
                Input.Descripcion,
                Input.NivelUrgencia,
                Input.OrderId 
            );

            TempData["MensajeExito"] = $"Reclamo creado exitosamente. Número de ticket: {reclamo.NumeroTicket}";
            return RedirectToPage("./Detalle", new { id = reclamo.Id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Error al crear el reclamo: {ex.Message}");

            // RECARGAR PEDIDOS SI HAY ERROR
            Input.PedidosDisponibles = await _context.Orders
                .Where(o => o.UserId == usuario.Id)
                .OrderByDescending(o => o.CreatedAt)
                .Take(20)
                .ToListAsync();

            return Page();
        }
    }
}

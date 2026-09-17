using Microsoft.AspNetCore.Mvc;
using Provider.Application.Provider.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Provider.Application.Provider.Requests;

namespace Provider.Api.Controllers;

[ApiController]
[Authorize]
[Route("providers")]
public class ProvidersController : ControllerBase
{
    private readonly IProviderService _providerService;

    public ProvidersController(IProviderService providerService)
    {
        _providerService = providerService;
    }

    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable()
    {
        var list = await _providerService.GetAvailableProvidersAsync();
        return Ok(list);
    }

    [HttpPost("optimize")]
    public async Task<IActionResult> Post([FromBody] AssignProviderRequest request)
    {
        if (request == null) return BadRequest(new { message = "Location is required" });

        var assigned = await _providerService.OptimizeAsync(request.UserLat ?? 0.0, request.UserLon ?? 0.0);
        if (assigned == null) return NotFound(new { message = "No providers available" });

        return Ok(assigned);
    }

    [HttpPost("assign")]
    public async Task<IActionResult> AssignProvider([FromBody] AssignProviderRequest request)
    {
        // Code review / errores del snippet original:
        // 1) Acceso directo a _db desde el controller: rompe la separación de responsabilidades (SRP) y acopla controller a la infra.
        // 2) Uso de .ToList() y FirstOrDefault(): trae toda la tabla a memoria y escoge el primero sin criterios (ineficiente y lógico pobre).
        // 3) No hay validación del request ni manejo de parámetros nulos.
        // 4) Se modifica la entidad directamente (selected.IsBusy = true) sin pasar por la regla de dominio ni repositorio (violación de encapsulación/SOLID).
        // 5) No hay manejo de concurrencia ni transacción: dos peticiones simultáneas podrían asignar el mismo proveedor.
        // 6) Uso de strings literales para mensajes sin estándar de respuesta.
        // 7) No se usa async/await correctamente en SaveChanges (posible bloqueo) y no se devuelve un DTO sancionado.
        // 8) El tipo 'Request' no está definido ni es explícito; falta DTO/contract.
        // 9) Controller contiene demasiada lógica: debería delegar en la capa de aplicación.

        if (request == null) return BadRequest(new { message = "Request is required" });

        // Delegar en la capa Application (ProviderService) que implementa la lógica de asignación y persistencia.
        var assigned = await _providerService.AssignProviderAsync(request);
        if (assigned == null) return NotFound(new { message = "No providers available" });

        return Ok(assigned);
    }
}
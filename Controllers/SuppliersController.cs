using Microsoft.AspNetCore.Mvc;
using MiseHub.Api.Interfaces;
using MiseHub.Api.Models;
using MiseHub.Api.DTOs;

namespace MiseHub.Api.Controllers;

// [Route("api/[controller]")] → "SuppliersController" blir "api/suppliers"
[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    // Bara interfacen, aldrig Fake*-klasserna direkt. ASP.NET Core matar in
    // rätt instans via konstruktorn (Dependency Injection).
    private readonly ISupplierService _suppliers;
    private readonly IProductService _products;

    public SuppliersController(ISupplierService suppliers, IProductService products)
    {
        _suppliers = suppliers;
        _products = products;
    }

    // GET /api/suppliers
    [HttpGet]
    public async Task<ActionResult<List<Supplier>>> GetAll() => await _suppliers.GetAllAsync();

    // GET /api/suppliers/{id} — {id} fångas upp som parametern nedan
    [HttpGet("{id}")]
    public async Task<ActionResult<Supplier>> GetById(string id)
    {
        var supplier = await _suppliers.GetByIdAsync(id);
        return supplier is null ? NotFound() : Ok(supplier);
    }

    // POST /api/suppliers — body mappas automatiskt till CreateSupplierRequest
    [HttpPost]
    public async Task<ActionResult<Supplier>> Create(CreateSupplierRequest request)
    {
        // Validera på servern, lita inte bara på frontend
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Company name is required.");

        var supplier = await _suppliers.AddAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = supplier.Id }, supplier);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(string id) =>
        await _suppliers.RemoveAsync(id) ? NoContent() : NotFound();

    // POST /api/suppliers/{id}/sync
    [HttpPost("{id}/sync")]
    public async Task<ActionResult<object>> SyncCatalog(string id)
    {
        var supplier = await _suppliers.GetByIdAsync(id);
        if (supplier is null) return NotFound();

        var added = await _products.SyncCatalogAsync(id);
        return Ok(new { added });
    }
}
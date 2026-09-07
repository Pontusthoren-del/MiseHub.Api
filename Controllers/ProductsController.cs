using Microsoft.AspNetCore.Mvc;
using MiseHub.Api.Interfaces;
using MiseHub.Api.Models;
using MiseHub.Api.DTOs;

namespace MiseHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")] // → "api/products"
public class ProductsController : ControllerBase
{
    private readonly IProductService _products;

    public ProductsController(IProductService products)
    {
        _products = products;
    }

    // [FromQuery] = query och supplierId hämtas från URL:ens query-string
    // (GET /api/products?query=olive&supplierId=altia)
    [HttpGet]
    public async Task<ActionResult<List<Product>>> GetAll([FromQuery] string? query, [FromQuery] string? supplierId) =>
        await _products.GetAllAsync(query, supplierId);
}
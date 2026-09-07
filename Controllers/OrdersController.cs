using Microsoft.AspNetCore.Mvc;
using MiseHub.Api.Interfaces;
using MiseHub.Api.Models;
using MiseHub.Api.DTOs;

namespace MiseHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orders;

    public OrdersController(IOrderService orders)
    {
        _orders = orders;
    }

    [HttpGet]
    public async Task<ActionResult<List<Order>>> GetAll() => await _orders.GetAllAsync();

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetById(string id)
    {
        var order = await _orders.GetByIdAsync(id);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    public async Task<ActionResult<Order>> Create(CreateOrderRequest request)
    {
        // Validera på servern, lita inte på att React redan kollat detta
        if (string.IsNullOrWhiteSpace(request.SupplierId) || request.LineItems.Count == 0)
            return BadRequest("SupplierId and at least one line item are required.");

        var order = await _orders.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
    }

    // POST /api/orders/pay-all
    [HttpPost("pay-all")]
    public async Task<IActionResult> PayAll()
    {
        await _orders.PayAllAsync();
        return NoContent();
    }

    // POST /api/orders/{id}/pay — betalar EN specifik order
    [HttpPost("{id}/pay")]
    public async Task<IActionResult> PayOne(string id) =>
        await _orders.PayOneAsync(id) ? NoContent() : NotFound();
}
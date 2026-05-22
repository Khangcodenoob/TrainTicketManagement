using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TrainTicketApi.Data;
using TrainTicketApi.DTOs.Customers;
using TrainTicketApi.Services.Interfaces;

namespace TrainTicketApi.Controllers;

[ApiController]
[Route("api/customers")]
[Authorize(Roles = "Admin,Staff")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly AppDbContext _context;

    public CustomersController(ICustomerService customerService, AppDbContext context)
    {
        _customerService = customerService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _customerService.GetAllAsync();
        return Ok(new { message = "Láº¥y danh sÃ¡ch khÃ¡ch hÃ ng thÃ nh cÃ´ng.", data });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _customerService.GetByIdAsync(id);
        return StatusCode(result.StatusCode, new { message = result.Message, data = result.Data });
    }

    [HttpGet("{id:int}/ticket-history")]
    public async Task<IActionResult> GetTicketHistory(int id)
    {
        var customerExists = await _context.Customers.AnyAsync(x => x.CustomerId == id);
        if (!customerExists)
        {
            return NotFound(new { message = "KhÃ´ng tÃ¬m tháº¥y khÃ¡ch hÃ ng." });
        }

        var data = await _context.Tickets
            .Where(x => x.CustomerId == id)
            .OrderByDescending(x => x.BookingDate)
            .Select(x => new
            {
                x.TicketId,
                x.TicketCode,
                x.TrainTripId,
                x.SeatNumber,
                x.Price,
                x.BookingDate,
                x.PaymentStatus,
                x.TicketStatus
            })
            .ToListAsync();

        return Ok(new { message = "Láº¥y lá»‹ch sá»­ mua vÃ© thÃ nh cÃ´ng.", data });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CustomerRequestDto request)
    {
        var result = await _customerService.CreateAsync(request);
        return StatusCode(result.StatusCode, new { message = result.Message, data = result.Data });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] CustomerRequestDto request)
    {
        var result = await _customerService.UpdateAsync(id, request);
        return StatusCode(result.StatusCode, new { message = result.Message, data = result.Data });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _customerService.DeleteAsync(id);
        return StatusCode(result.StatusCode, new { message = result.Message });
    }
}

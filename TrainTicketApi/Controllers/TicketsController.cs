using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TrainTicketApi.DTOs.Tickets;
using TrainTicketApi.Services.Interfaces;

namespace TrainTicketApi.Controllers;

[ApiController]
[Route("api/tickets")]
[Authorize(Roles = "Admin,Staff")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _ticketService.GetAllAsync();
        return Ok(new { message = "Lấy danh sách vé tàu thành công.", data });
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string? keyword, [FromQuery] string? ticketStatus, [FromQuery] string? paymentStatus)
    {
        var data = await _ticketService.SearchAsync(keyword, ticketStatus, paymentStatus);
        return Ok(new { message = "Tìm kiếm vé thành công.", data });
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _ticketService.GetByIdAsync(id);
        return StatusCode(result.StatusCode, new { message = result.Message, data = result.Data });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTicketRequestDto request)
    {
        var result = await _ticketService.CreateAsync(request);
        return StatusCode(result.StatusCode, new { message = result.Message, data = result.Data });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTicketRequestDto request)
    {
        var result = await _ticketService.UpdateAsync(id, request);
        return StatusCode(result.StatusCode, new { message = result.Message, data = result.Data });
    }

    [HttpPut("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, [FromBody] CancelTicketRequestDto request)
    {
        var result = await _ticketService.CancelAsync(id, request);
        return StatusCode(result.StatusCode, new { message = result.Message });
    }

    [HttpPost("{id:int}/pay")]
    public async Task<IActionResult> Pay(int id, [FromBody] PayTicketRequestDto request)
    {
        var result = await _ticketService.PayAsync(id, request);
        return StatusCode(result.StatusCode, new { message = result.Message });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _ticketService.DeleteAsync(id);
        return StatusCode(result.StatusCode, new { message = result.Message });
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TrainTicketApi.Data;
using TrainTicketApi.DTOs.TrainTrips;
using TrainTicketApi.Services.Interfaces;

namespace TrainTicketApi.Controllers;

[ApiController]
[Route("api/train-trips")]
[Authorize(Roles = "Admin,Staff")]
public class TrainTripsController : ControllerBase
{
    private readonly ITrainTripService _trainTripService;
    private readonly AppDbContext _context;

    public TrainTripsController(ITrainTripService trainTripService, AppDbContext context)
    {
        _trainTripService = trainTripService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _trainTripService.GetAllAsync();
        return Ok(new { message = "Láº¥y danh sÃ¡ch chuyáº¿n tÃ u thÃ nh cÃ´ng.", data });
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string? departureStation,
        [FromQuery] string? arrivalStation,
        [FromQuery] DateTime? departureDate)
    {
        var query = _context.TrainTrips.Include(x => x.Route).AsQueryable();

        if (!string.IsNullOrWhiteSpace(departureStation))
        {
            var value = departureStation.Trim();
            query = query.Where(x => x.Route != null && x.Route.DepartureStation.Contains(value));
        }

        if (!string.IsNullOrWhiteSpace(arrivalStation))
        {
            var value = arrivalStation.Trim();
            query = query.Where(x => x.Route != null && x.Route.ArrivalStation.Contains(value));
        }

        if (departureDate.HasValue)
        {
            var day = departureDate.Value.Date;
            query = query.Where(x => x.DepartureTime.Date == day);
        }

        var data = await query
            .OrderBy(x => x.DepartureTime)
            .Select(x => new
            {
                x.TrainTripId,
                x.TrainCode,
                x.RouteId,
                DepartureStation = x.Route != null ? x.Route.DepartureStation : string.Empty,
                ArrivalStation = x.Route != null ? x.Route.ArrivalStation : string.Empty,
                x.DepartureTime,
                x.ArrivalTime,
                x.TotalSeats,
                x.AvailableSeats,
                x.BaseTicketPrice,
                x.Status
            })
            .ToListAsync();

        return Ok(new { message = "TÃ¬m kiáº¿m chuyáº¿n tÃ u thÃ nh cÃ´ng.", data });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _trainTripService.GetByIdAsync(id);
        return StatusCode(result.StatusCode, new { message = result.Message, data = result.Data });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TrainTripRequestDto request)
    {
        var result = await _trainTripService.CreateAsync(request);
        return StatusCode(result.StatusCode, new { message = result.Message, data = result.Data });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TrainTripRequestDto request)
    {
        var result = await _trainTripService.UpdateAsync(id, request);
        return StatusCode(result.StatusCode, new { message = result.Message, data = result.Data });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _trainTripService.DeleteAsync(id);
        return StatusCode(result.StatusCode, new { message = result.Message });
    }

    [HttpGet("{id:int}/available-seats")]
    public async Task<IActionResult> GetAvailableSeats(int id)
    {
        var trip = await _context.TrainTrips.FirstOrDefaultAsync(t => t.TrainTripId == id);
        if (trip == null) return NotFound(new { message = "Chuyến tàu không tồn tại." });

        var bookedSeats = await _context.Tickets
            .Where(t => t.TrainTripId == id && t.TicketStatus != "Cancelled")
            .Select(t => t.SeatNumber)
            .ToListAsync();

        return Ok(new { message = "Lấy trạng thái ghế thành công.", data = new { totalSeats = trip.TotalSeats, bookedSeats } });
    }
}

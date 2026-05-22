using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrainTicketApi.Data;
using TrainTicketApi.DTOs.Dashboard;

namespace TrainTicketApi.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = "Admin,Staff")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _context;

    public DashboardController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var stats = new DashboardStatsDto
        {
            TotalRoutes = await _context.Routes.CountAsync(),
            TotalTrips = await _context.TrainTrips.CountAsync(),
            TotalCustomers = await _context.Customers.CountAsync(),
            TotalTickets = await _context.Tickets.CountAsync(),
            BookedTickets = await _context.Tickets.CountAsync(x => x.TicketStatus == "Booked"),
            CancelledTickets = await _context.Tickets.CountAsync(x => x.TicketStatus == "Cancelled"),
            TotalRevenue = await _context.Tickets.Where(x => x.TicketStatus == "Booked").SumAsync(x => x.Price),
            UpcomingTripsCount = await _context.TrainTrips.CountAsync(x => x.DepartureTime > DateTime.UtcNow)
        };

        return Ok(new { message = "Láº¥y thá»‘ng kÃª báº£ng Ä‘iá»u khiá»ƒn thÃ nh cÃ´ng.", data = stats });
    }

    [HttpGet("seat-map/{trainTripId:int}")]
    public async Task<IActionResult> GetSeatMap(int trainTripId)
    {
        var trainTrip = await _context.TrainTrips.FirstOrDefaultAsync(x => x.TrainTripId == trainTripId);
        if (trainTrip is null)
        {
            return NotFound(new { message = "KhÃ´ng tÃ¬m tháº¥y chuyáº¿n tÃ u." });
        }

        var tickets = await _context.Tickets
            .Where(x => x.TrainTripId == trainTripId && x.TicketStatus != "Cancelled")
            .Select(x => new { x.SeatNumber, x.TicketStatus })
            .ToListAsync();

        var bookedLookup = tickets.ToDictionary(x => x.SeatNumber, x => x.TicketStatus);
        var seats = new List<SeatStatusDto>();
        for (var i = 1; i <= trainTrip.TotalSeats; i++)
        {
            var seatNumber = $"A{i:D2}";
            var isBooked = bookedLookup.TryGetValue(seatNumber, out var status);
            seats.Add(new SeatStatusDto
            {
                SeatNumber = seatNumber,
                IsBooked = isBooked,
                TicketStatus = status
            });
        }

        return Ok(new
        {
            message = "Láº¥y sÆ¡ Ä‘á»“ gháº¿ thÃ nh cÃ´ng.",
            data = new SeatMapDto
            {
                TrainTripId = trainTripId,
                TotalSeats = trainTrip.TotalSeats,
                AvailableSeats = trainTrip.AvailableSeats,
                Seats = seats
            }
        });
    }

    [HttpGet("reports/export")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ExportCsv([FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
    {
        var query = _context.Tickets.AsQueryable();
        if (fromDate.HasValue)
        {
            query = query.Where(x => x.BookingDate >= fromDate.Value);
        }
        if (toDate.HasValue)
        {
            query = query.Where(x => x.BookingDate <= toDate.Value);
        }

        var tickets = await query.OrderByDescending(x => x.BookingDate).ToListAsync();
        var csv = new StringBuilder();
        csv.AppendLine("TicketId,TicketCode,TrainTripId,CustomerId,SeatNumber,Price,BookingDate,PaymentStatus,TicketStatus");
        foreach (var ticket in tickets)
        {
            csv.AppendLine($"{ticket.TicketId},{ticket.TicketCode},{ticket.TrainTripId},{ticket.CustomerId},{ticket.SeatNumber},{ticket.Price},{ticket.BookingDate:O},{ticket.PaymentStatus},{ticket.TicketStatus}");
        }

        return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"ticket-report-{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
    }

    [HttpGet("audit-logs")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAuditLogs()
    {
        var data = await _context.AuditLogs
            .OrderByDescending(x => x.PerformedAt)
            .Take(200)
            .ToListAsync();
        return Ok(new { message = "Láº¥y nháº­t kÃ½ há»‡ thá»‘ng thÃ nh cÃ´ng.", data });
    }
}

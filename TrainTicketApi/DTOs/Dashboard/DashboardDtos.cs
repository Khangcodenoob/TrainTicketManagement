namespace TrainTicketApi.DTOs.Dashboard;

public class DashboardStatsDto
{
    public int TotalRoutes { get; set; }
    public int TotalTrips { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalTickets { get; set; }
    public int BookedTickets { get; set; }
    public int CancelledTickets { get; set; }
    public decimal TotalRevenue { get; set; }
    public int UpcomingTripsCount { get; set; }
}

public class SeatMapDto
{
    public int TrainTripId { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public List<SeatStatusDto> Seats { get; set; } = new();
}

public class SeatStatusDto
{
    public string SeatNumber { get; set; } = string.Empty;
    public bool IsBooked { get; set; }
    public string? TicketStatus { get; set; }
}

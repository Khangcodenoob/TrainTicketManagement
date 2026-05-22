namespace TrainTicketWinForms.Models;

public class DashboardStats
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

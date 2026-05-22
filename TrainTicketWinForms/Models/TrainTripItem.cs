namespace TrainTicketWinForms.Models;

public class TrainTripItem
{
    public int TrainTripId { get; set; }
    public string TrainCode { get; set; } = string.Empty;
    public int RouteId { get; set; }
    public string DepartureStation { get; set; } = string.Empty;
    public string ArrivalStation { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public decimal BaseTicketPrice { get; set; }
    public string Status { get; set; } = string.Empty;
}

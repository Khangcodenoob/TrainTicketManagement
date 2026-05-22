namespace TrainTicketApi.Models;

public class TrainTrip
{
    public int TrainTripId { get; set; }
    public string TrainCode { get; set; } = string.Empty;
    public int RouteId { get; set; }
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public int TotalSeats { get; set; }
    public int AvailableSeats { get; set; }
    public decimal BaseTicketPrice { get; set; }
    public string Status { get; set; } = string.Empty;

    public TrainRoute? Route { get; set; }
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}

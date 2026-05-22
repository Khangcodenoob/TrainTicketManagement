namespace TrainTicketApi.Models;

public class TrainRoute
{
    public int RouteId { get; set; }
    public string DepartureStation { get; set; } = string.Empty;
    public string ArrivalStation { get; set; } = string.Empty;
    public decimal DistanceKm { get; set; }
    public string Status { get; set; } = string.Empty;

    public ICollection<TrainTrip> TrainTrips { get; set; } = new List<TrainTrip>();
}

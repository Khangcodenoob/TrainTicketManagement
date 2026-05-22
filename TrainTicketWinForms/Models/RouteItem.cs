namespace TrainTicketWinForms.Models;

public class RouteItem
{
    public int RouteId { get; set; }
    public string DepartureStation { get; set; } = string.Empty;
    public string ArrivalStation { get; set; } = string.Empty;
    public decimal DistanceKm { get; set; }
    public string Status { get; set; } = string.Empty;
}

using System.ComponentModel.DataAnnotations;

namespace TrainTicketApi.DTOs.Routes;

public class RouteRequestDto
{
    [Required, MaxLength(100)]
    public string DepartureStation { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string ArrivalStation { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal DistanceKm { get; set; }

    [Required, MaxLength(30)]
    public string Status { get; set; } = string.Empty;
}

public class RouteResponseDto
{
    public int RouteId { get; set; }
    public string DepartureStation { get; set; } = string.Empty;
    public string ArrivalStation { get; set; } = string.Empty;
    public decimal DistanceKm { get; set; }
    public string Status { get; set; } = string.Empty;
}

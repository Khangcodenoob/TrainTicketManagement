using System.ComponentModel.DataAnnotations;

namespace TrainTicketApi.DTOs.TrainTrips;

public class TrainTripRequestDto
{
    [Required, MaxLength(20)]
    public string TrainCode { get; set; } = string.Empty;

    [Range(1, int.MaxValue)]
    public int RouteId { get; set; }

    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }

    [Range(1, int.MaxValue)]
    public int TotalSeats { get; set; }

    [Range(0, int.MaxValue)]
    public int AvailableSeats { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal BaseTicketPrice { get; set; }

    [Required, MaxLength(30)]
    public string Status { get; set; } = string.Empty;
}

public class TrainTripResponseDto
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
}

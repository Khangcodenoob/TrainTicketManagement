using System.ComponentModel.DataAnnotations;

namespace TrainTicketApi.DTOs.Tickets;

public class CreateTicketRequestDto
{
    [Range(1, int.MaxValue)]
    public int TrainTripId { get; set; }

    [Range(1, int.MaxValue)]
    public int CustomerId { get; set; }

    [Required, MaxLength(10)]
    public string SeatNumber { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal? Price { get; set; }

    [MaxLength(50)]
    public string? PaymentMethod { get; set; }
}

public class CancelTicketRequestDto
{
    [MaxLength(500)]
    public string? CancelReason { get; set; }
}

public class PayTicketRequestDto
{
    [Required, MaxLength(50)]
    public string PaymentMethod { get; set; } = string.Empty;
}


public class UpdateTicketRequestDto
{
    [Required, MaxLength(30)]
    public string PaymentStatus { get; set; } = string.Empty;

    [Required, MaxLength(30)]
    public string TicketStatus { get; set; } = string.Empty;
}

public class TicketResponseDto
{
    public int TicketId { get; set; }
    public string TicketCode { get; set; } = string.Empty;
    public int TrainTripId { get; set; }
    public int CustomerId { get; set; }
    public string SeatNumber { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public DateTime BookingDate { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public string TicketStatus { get; set; } = string.Empty;
    public string? PaymentMethod { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancelReason { get; set; }
    public string? CreatedBy { get; set; }
}

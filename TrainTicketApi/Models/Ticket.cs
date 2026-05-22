namespace TrainTicketApi.Models;

public class Ticket
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


    public TrainTrip? TrainTrip { get; set; }
    public Customer? Customer { get; set; }
}

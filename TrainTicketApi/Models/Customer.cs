namespace TrainTicketApi.Models;

public class Customer
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string IdentityNumber { get; set; } = string.Empty;
    public string? Address { get; set; }

    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}

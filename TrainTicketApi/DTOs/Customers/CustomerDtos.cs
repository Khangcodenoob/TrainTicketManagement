using System.ComponentModel.DataAnnotations;

namespace TrainTicketApi.DTOs.Customers;

public class CustomerRequestDto
{
    [Required, MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, RegularExpression(@"^0\d{9,10}$")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required, RegularExpression(@"^\d{9,12}$")]
    public string IdentityNumber { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? Address { get; set; }
}

public class CustomerResponseDto
{
    public int CustomerId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string IdentityNumber { get; set; } = string.Empty;
    public string? Address { get; set; }
}

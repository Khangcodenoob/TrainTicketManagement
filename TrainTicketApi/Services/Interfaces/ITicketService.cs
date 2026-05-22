using TrainTicketApi.DTOs.Tickets;

namespace TrainTicketApi.Services.Interfaces;

public interface ITicketService
{
    Task<List<TicketResponseDto>> GetAllAsync();
    Task<List<TicketResponseDto>> SearchAsync(string? keyword, string? ticketStatus, string? paymentStatus);
    Task<ServiceResult<TicketResponseDto>> GetByIdAsync(int ticketId);
    Task<ServiceResult<TicketResponseDto>> CreateAsync(CreateTicketRequestDto request);
    Task<ServiceResult<TicketResponseDto>> UpdateAsync(int ticketId, UpdateTicketRequestDto request);
    Task<ServiceResult<object>> CancelAsync(int ticketId, CancelTicketRequestDto request);
    Task<ServiceResult<object>> PayAsync(int ticketId, PayTicketRequestDto request);
    Task<ServiceResult<object>> DeleteAsync(int ticketId);
}

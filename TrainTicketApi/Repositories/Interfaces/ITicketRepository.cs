using TrainTicketApi.Models;

namespace TrainTicketApi.Repositories.Interfaces;

public interface ITicketRepository
{
    Task<List<Ticket>> GetAllAsync();
    Task<Ticket?> GetByIdAsync(int ticketId);
    Task<bool> ExistsSeatInTripAsync(int trainTripId, string seatNumber, int? excludeTicketId = null);
    Task<Ticket> CreateAsync(Ticket ticket);
    Task<Ticket?> UpdateAsync(Ticket ticket);
    Task<bool> DeleteAsync(int ticketId);
}

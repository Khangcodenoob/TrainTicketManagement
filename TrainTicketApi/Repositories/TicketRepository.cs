using Microsoft.EntityFrameworkCore;
using TrainTicketApi.Data;
using TrainTicketApi.Models;
using TrainTicketApi.Repositories.Interfaces;

namespace TrainTicketApi.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly AppDbContext _context;

    public TicketRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Ticket>> GetAllAsync() =>
        _context.Tickets.AsNoTracking().OrderBy(x => x.TicketId).ToListAsync();

    public Task<Ticket?> GetByIdAsync(int ticketId) =>
        _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticketId);

    public Task<bool> ExistsSeatInTripAsync(int trainTripId, string seatNumber, int? excludeTicketId = null) =>
        _context.Tickets.AnyAsync(x =>
            x.TrainTripId == trainTripId &&
            x.SeatNumber == seatNumber &&
            (!excludeTicketId.HasValue || x.TicketId != excludeTicketId.Value));

    public async Task<Ticket> CreateAsync(Ticket ticket)
    {
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public async Task<Ticket?> UpdateAsync(Ticket ticket)
    {
        var existing = await _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticket.TicketId);
        if (existing is null)
        {
            return null;
        }

        existing.PaymentStatus = ticket.PaymentStatus;
        existing.TicketStatus = ticket.TicketStatus;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int ticketId)
    {
        var entity = await _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == ticketId);
        if (entity is null)
        {
            return false;
        }

        _context.Tickets.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}

using Microsoft.EntityFrameworkCore;
using TrainTicketApi.Data;
using TrainTicketApi.Models;
using TrainTicketApi.Repositories.Interfaces;

namespace TrainTicketApi.Repositories;

public class TrainTripRepository : ITrainTripRepository
{
    private readonly AppDbContext _context;

    public TrainTripRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<TrainTrip>> GetAllAsync() =>
        _context.TrainTrips.AsNoTracking().OrderBy(x => x.TrainTripId).ToListAsync();

    public Task<TrainTrip?> GetByIdAsync(int trainTripId) =>
        _context.TrainTrips.FirstOrDefaultAsync(x => x.TrainTripId == trainTripId);

    public Task<bool> ExistsByRouteIdAsync(int routeId) =>
        _context.Routes.AnyAsync(x => x.RouteId == routeId);

    public async Task<TrainTrip> CreateAsync(TrainTrip trip)
    {
        _context.TrainTrips.Add(trip);
        await _context.SaveChangesAsync();
        return trip;
    }

    public async Task<TrainTrip?> UpdateAsync(TrainTrip trip)
    {
        var existing = await _context.TrainTrips.FirstOrDefaultAsync(x => x.TrainTripId == trip.TrainTripId);
        if (existing is null)
        {
            return null;
        }

        existing.TrainCode = trip.TrainCode;
        existing.RouteId = trip.RouteId;
        existing.DepartureTime = trip.DepartureTime;
        existing.ArrivalTime = trip.ArrivalTime;
        existing.TotalSeats = trip.TotalSeats;
        existing.AvailableSeats = trip.AvailableSeats;
        existing.BaseTicketPrice = trip.BaseTicketPrice;
        existing.Status = trip.Status;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int trainTripId)
    {
        var entity = await _context.TrainTrips.FirstOrDefaultAsync(x => x.TrainTripId == trainTripId);
        if (entity is null)
        {
            return false;
        }

        _context.TrainTrips.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}

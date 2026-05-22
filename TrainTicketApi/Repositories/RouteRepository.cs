using Microsoft.EntityFrameworkCore;
using TrainTicketApi.Data;
using TrainTicketApi.Models;
using TrainTicketApi.Repositories.Interfaces;

namespace TrainTicketApi.Repositories;

public class RouteRepository : IRouteRepository
{
    private readonly AppDbContext _context;

    public RouteRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<TrainRoute>> GetAllAsync() =>
        _context.Routes.AsNoTracking().OrderBy(x => x.RouteId).ToListAsync();

    public Task<TrainRoute?> GetByIdAsync(int routeId) =>
        _context.Routes.FirstOrDefaultAsync(x => x.RouteId == routeId);

    public async Task<TrainRoute> CreateAsync(TrainRoute route)
    {
        _context.Routes.Add(route);
        await _context.SaveChangesAsync();
        return route;
    }

    public async Task<TrainRoute?> UpdateAsync(TrainRoute route)
    {
        var existing = await _context.Routes.FirstOrDefaultAsync(x => x.RouteId == route.RouteId);
        if (existing is null)
        {
            return null;
        }

        existing.DepartureStation = route.DepartureStation;
        existing.ArrivalStation = route.ArrivalStation;
        existing.DistanceKm = route.DistanceKm;
        existing.Status = route.Status;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int routeId)
    {
        var entity = await _context.Routes.FirstOrDefaultAsync(x => x.RouteId == routeId);
        if (entity is null)
        {
            return false;
        }

        _context.Routes.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}

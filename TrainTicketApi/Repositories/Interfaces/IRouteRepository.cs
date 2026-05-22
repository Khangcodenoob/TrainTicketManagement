using TrainTicketApi.Models;

namespace TrainTicketApi.Repositories.Interfaces;

public interface IRouteRepository
{
    Task<List<TrainRoute>> GetAllAsync();
    Task<TrainRoute?> GetByIdAsync(int routeId);
    Task<TrainRoute> CreateAsync(TrainRoute route);
    Task<TrainRoute?> UpdateAsync(TrainRoute route);
    Task<bool> DeleteAsync(int routeId);
}

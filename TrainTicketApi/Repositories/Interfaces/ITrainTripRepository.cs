using TrainTicketApi.Models;

namespace TrainTicketApi.Repositories.Interfaces;

public interface ITrainTripRepository
{
    Task<List<TrainTrip>> GetAllAsync();
    Task<TrainTrip?> GetByIdAsync(int trainTripId);
    Task<bool> ExistsByRouteIdAsync(int routeId);
    Task<TrainTrip> CreateAsync(TrainTrip trip);
    Task<TrainTrip?> UpdateAsync(TrainTrip trip);
    Task<bool> DeleteAsync(int trainTripId);
}

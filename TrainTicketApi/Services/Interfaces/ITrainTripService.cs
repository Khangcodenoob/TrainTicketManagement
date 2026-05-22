using TrainTicketApi.DTOs.TrainTrips;

namespace TrainTicketApi.Services.Interfaces;

public interface ITrainTripService
{
    Task<List<TrainTripResponseDto>> GetAllAsync();
    Task<ServiceResult<TrainTripResponseDto>> GetByIdAsync(int trainTripId);
    Task<ServiceResult<TrainTripResponseDto>> CreateAsync(TrainTripRequestDto request);
    Task<ServiceResult<TrainTripResponseDto>> UpdateAsync(int trainTripId, TrainTripRequestDto request);
    Task<ServiceResult<object>> DeleteAsync(int trainTripId);
}

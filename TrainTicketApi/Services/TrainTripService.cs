using TrainTicketApi.DTOs.TrainTrips;
using TrainTicketApi.Models;
using TrainTicketApi.Repositories.Interfaces;
using TrainTicketApi.Services.Interfaces;

namespace TrainTicketApi.Services;

public class TrainTripService : ITrainTripService
{
    private readonly ITrainTripRepository _trainTripRepository;

    public TrainTripService(ITrainTripRepository trainTripRepository)
    {
        _trainTripRepository = trainTripRepository;
    }

    public async Task<List<TrainTripResponseDto>> GetAllAsync() =>
        (await _trainTripRepository.GetAllAsync()).Select(MapResponse).ToList();

    public async Task<ServiceResult<TrainTripResponseDto>> GetByIdAsync(int trainTripId)
    {
        var trip = await _trainTripRepository.GetByIdAsync(trainTripId);
        return trip is null
            ? ServiceResult<TrainTripResponseDto>.Fail(StatusCodes.Status404NotFound, "KhÃ´ng tÃ¬m tháº¥y chuyáº¿n tÃ u.")
            : ServiceResult<TrainTripResponseDto>.Ok(MapResponse(trip), "Láº¥y thÃ´ng tin chuyáº¿n tÃ u thÃ nh cÃ´ng.");
    }

    public async Task<ServiceResult<TrainTripResponseDto>> CreateAsync(TrainTripRequestDto request)
    {
        var validation = await ValidateRequestAsync(request);
        if (validation is not null)
        {
            return validation;
        }

        var created = await _trainTripRepository.CreateAsync(MapEntity(0, request));
        return ServiceResult<TrainTripResponseDto>.Created(MapResponse(created), "Táº¡o chuyáº¿n tÃ u thÃ nh cÃ´ng.");
    }

    public async Task<ServiceResult<TrainTripResponseDto>> UpdateAsync(int trainTripId, TrainTripRequestDto request)
    {
        var validation = await ValidateRequestAsync(request);
        if (validation is not null)
        {
            return validation;
        }

        var updated = await _trainTripRepository.UpdateAsync(MapEntity(trainTripId, request));
        return updated is null
            ? ServiceResult<TrainTripResponseDto>.Fail(StatusCodes.Status404NotFound, "KhÃ´ng tÃ¬m tháº¥y chuyáº¿n tÃ u.")
            : ServiceResult<TrainTripResponseDto>.Ok(MapResponse(updated), "Cáº­p nháº­t chuyáº¿n tÃ u thÃ nh cÃ´ng.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(int trainTripId)
    {
        var deleted = await _trainTripRepository.DeleteAsync(trainTripId);
        return deleted
            ? ServiceResult<object>.Ok(null, "XÃ³a chuyáº¿n tÃ u thÃ nh cÃ´ng.")
            : ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "KhÃ´ng tÃ¬m tháº¥y chuyáº¿n tÃ u.");
    }

    private async Task<ServiceResult<TrainTripResponseDto>?> ValidateRequestAsync(TrainTripRequestDto request)
    {
        if (request.ArrivalTime <= request.DepartureTime)
        {
            return ServiceResult<TrainTripResponseDto>.Fail(StatusCodes.Status400BadRequest, "Giá» Ä‘áº¿n pháº£i lá»›n hÆ¡n giá» khá»Ÿi hÃ nh.");
        }

        if (request.AvailableSeats > request.TotalSeats)
        {
            return ServiceResult<TrainTripResponseDto>.Fail(StatusCodes.Status400BadRequest, "Sá»‘ gháº¿ cÃ²n trá»‘ng khÃ´ng Ä‘Æ°á»£c lá»›n hÆ¡n tá»•ng sá»‘ gháº¿.");
        }

        if (!await _trainTripRepository.ExistsByRouteIdAsync(request.RouteId))
        {
            return ServiceResult<TrainTripResponseDto>.Fail(StatusCodes.Status400BadRequest, "RouteId khÃ´ng tá»“n táº¡i.");
        }

        return null;
    }

    private static TrainTrip MapEntity(int id, TrainTripRequestDto request) =>
        new()
        {
            TrainTripId = id,
            TrainCode = request.TrainCode.Trim(),
            RouteId = request.RouteId,
            DepartureTime = request.DepartureTime,
            ArrivalTime = request.ArrivalTime,
            TotalSeats = request.TotalSeats,
            AvailableSeats = request.AvailableSeats,
            BaseTicketPrice = request.BaseTicketPrice,
            Status = request.Status.Trim()
        };

    private static TrainTripResponseDto MapResponse(TrainTrip trip) =>
        new()
        {
            TrainTripId = trip.TrainTripId,
            TrainCode = trip.TrainCode,
            RouteId = trip.RouteId,
            DepartureTime = trip.DepartureTime,
            ArrivalTime = trip.ArrivalTime,
            TotalSeats = trip.TotalSeats,
            AvailableSeats = trip.AvailableSeats,
            BaseTicketPrice = trip.BaseTicketPrice,
            Status = trip.Status
        };
}

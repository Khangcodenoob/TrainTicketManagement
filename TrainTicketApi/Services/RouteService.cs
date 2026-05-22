using TrainTicketApi.DTOs.Routes;
using TrainTicketApi.Models;
using TrainTicketApi.Repositories.Interfaces;
using TrainTicketApi.Services.Interfaces;

namespace TrainTicketApi.Services;

public class RouteService : IRouteService
{
    private readonly IRouteRepository _routeRepository;

    public RouteService(IRouteRepository routeRepository)
    {
        _routeRepository = routeRepository;
    }

    public async Task<List<RouteResponseDto>> GetAllAsync() =>
        (await _routeRepository.GetAllAsync()).Select(MapResponse).ToList();

    public async Task<ServiceResult<RouteResponseDto>> GetByIdAsync(int routeId)
    {
        var route = await _routeRepository.GetByIdAsync(routeId);
        return route is null
            ? ServiceResult<RouteResponseDto>.Fail(StatusCodes.Status404NotFound, "KhÃ´ng tÃ¬m tháº¥y tuyáº¿n tÃ u.")
            : ServiceResult<RouteResponseDto>.Ok(MapResponse(route), "Láº¥y thÃ´ng tin tuyáº¿n tÃ u thÃ nh cÃ´ng.");
    }

    public async Task<ServiceResult<RouteResponseDto>> CreateAsync(RouteRequestDto request)
    {
        if (request.DepartureStation.Trim().Equals(request.ArrivalStation.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<RouteResponseDto>.Fail(StatusCodes.Status400BadRequest, "Ga Ä‘i vÃ  ga Ä‘áº¿n khÃ´ng Ä‘Æ°á»£c trÃ¹ng nhau.");
        }

        var created = await _routeRepository.CreateAsync(new TrainRoute
        {
            DepartureStation = request.DepartureStation.Trim(),
            ArrivalStation = request.ArrivalStation.Trim(),
            DistanceKm = request.DistanceKm,
            Status = request.Status.Trim()
        });

        return ServiceResult<RouteResponseDto>.Created(MapResponse(created), "Táº¡o tuyáº¿n tÃ u thÃ nh cÃ´ng.");
    }

    public async Task<ServiceResult<RouteResponseDto>> UpdateAsync(int routeId, RouteRequestDto request)
    {
        if (request.DepartureStation.Trim().Equals(request.ArrivalStation.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            return ServiceResult<RouteResponseDto>.Fail(StatusCodes.Status400BadRequest, "Ga Ä‘i vÃ  ga Ä‘áº¿n khÃ´ng Ä‘Æ°á»£c trÃ¹ng nhau.");
        }

        var updated = await _routeRepository.UpdateAsync(new TrainRoute
        {
            RouteId = routeId,
            DepartureStation = request.DepartureStation.Trim(),
            ArrivalStation = request.ArrivalStation.Trim(),
            DistanceKm = request.DistanceKm,
            Status = request.Status.Trim()
        });

        return updated is null
            ? ServiceResult<RouteResponseDto>.Fail(StatusCodes.Status404NotFound, "KhÃ´ng tÃ¬m tháº¥y tuyáº¿n tÃ u.")
            : ServiceResult<RouteResponseDto>.Ok(MapResponse(updated), "Cáº­p nháº­t tuyáº¿n tÃ u thÃ nh cÃ´ng.");
    }

    public async Task<ServiceResult<object>> DeleteAsync(int routeId)
    {
        var deleted = await _routeRepository.DeleteAsync(routeId);
        return deleted
            ? ServiceResult<object>.Ok(null, "XÃ³a tuyáº¿n tÃ u thÃ nh cÃ´ng.")
            : ServiceResult<object>.Fail(StatusCodes.Status404NotFound, "KhÃ´ng tÃ¬m tháº¥y tuyáº¿n tÃ u.");
    }

    private static RouteResponseDto MapResponse(TrainRoute route) =>
        new()
        {
            RouteId = route.RouteId,
            DepartureStation = route.DepartureStation,
            ArrivalStation = route.ArrivalStation,
            DistanceKm = route.DistanceKm,
            Status = route.Status
        };
}

using TrainTicketApi.DTOs.Routes;

namespace TrainTicketApi.Services.Interfaces;

public interface IRouteService
{
    Task<List<RouteResponseDto>> GetAllAsync();
    Task<ServiceResult<RouteResponseDto>> GetByIdAsync(int routeId);
    Task<ServiceResult<RouteResponseDto>> CreateAsync(RouteRequestDto request);
    Task<ServiceResult<RouteResponseDto>> UpdateAsync(int routeId, RouteRequestDto request);
    Task<ServiceResult<object>> DeleteAsync(int routeId);
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TrainTicketApi.DTOs.Routes;
using TrainTicketApi.Services.Interfaces;

namespace TrainTicketApi.Controllers;

[ApiController]
[Route("api/routes")]
[Authorize(Roles = "Admin,Staff")]
public class RoutesController : ControllerBase
{
    private readonly IRouteService _routeService;

    public RoutesController(IRouteService routeService)
    {
        _routeService = routeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var data = await _routeService.GetAllAsync();
        return Ok(new { message = "Láº¥y danh sÃ¡ch tuyáº¿n tÃ u thÃ nh cÃ´ng.", data });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _routeService.GetByIdAsync(id);
        return StatusCode(result.StatusCode, new { message = result.Message, data = result.Data });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RouteRequestDto request)
    {
        var result = await _routeService.CreateAsync(request);
        return StatusCode(result.StatusCode, new { message = result.Message, data = result.Data });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] RouteRequestDto request)
    {
        var result = await _routeService.UpdateAsync(id, request);
        return StatusCode(result.StatusCode, new { message = result.Message, data = result.Data });
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _routeService.DeleteAsync(id);
        return StatusCode(result.StatusCode, new { message = result.Message });
    }
}

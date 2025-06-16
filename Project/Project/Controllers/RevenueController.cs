using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Exceptions;
using Project.Services;

namespace Project.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RevenueController : ControllerBase
{
    private readonly IRevenueService _revenueService;

    public RevenueController(IRevenueService revenueService)
    {
        _revenueService = revenueService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(decimal), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetTotalRevenueAsync()
    {
        try
        {
            var totalRevenue = await _revenueService.GetTotalRevenueAsync();
            return Ok(totalRevenue);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(decimal), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetRevenueForProductAsync(int id)
    {
        try
        {
            var totalRevenue = await _revenueService.GetRevenueForProductAsync(id);
            return Ok(totalRevenue);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    [HttpGet("predicted")]
    [ProducesResponseType(typeof(decimal), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetTotalPredictedRevenueAsync()
    {
        try
        {
            var totalRevenue = await _revenueService.GetTotalPredictedRevenueAsync();
            return Ok(totalRevenue);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    [HttpGet("predicted/{id}")]
    [ProducesResponseType(typeof(decimal), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetPredictedRevenueForProductAsync(int id)
    {
        try
        {
            var totalRevenue = await _revenueService.GetPredictedRevenueForProductAsync(id);
            return Ok(totalRevenue);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpGet("currency/{currencyCode}")]
    [ProducesResponseType(typeof(decimal), 200)]
    [ProducesResponseType(401)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetTotalRevenueForCurrencyAsync(string currencyCode)
    {
        try
        {
            var result = await _revenueService.GetRevenueInCurrencyAsync(currencyCode);
            return Ok(result);
        }
        catch (BadRequestException e)
        {
            return BadRequest(e.Message);
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
    
    
}
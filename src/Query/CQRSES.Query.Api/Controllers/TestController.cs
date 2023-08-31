using CQRSES.Query.Application.Common;
using CQRSES.Query.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CQRSES.Query.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TestController : ControllerBase
{
    private readonly IRepository<Ad> _repository;

    public TestController(IRepository<Ad> repository)
    {
        _repository = repository;
    }

    [HttpGet("Read")]
    public async Task<IActionResult> Read(string id)
    {
        var ad = await _repository.GetByIdAsync(id);

        return Ok(ad);
    }
}
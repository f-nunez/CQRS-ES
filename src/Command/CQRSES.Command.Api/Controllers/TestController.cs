using CQRSES.Command.Application.Common;
using CQRSES.Command.Domain.AdAggregate;
using Microsoft.AspNetCore.Mvc;

namespace CQRSES.Command.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class TestController : ControllerBase
{
    private readonly IEventStore<Ad> _eventStore;

    public TestController(IEventStore<Ad> eventStore)
    {
        _eventStore = eventStore;
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create(string title, string description)
    {
        var newAd = new Ad();

        newAd.Create(
            Guid.NewGuid().ToString(),
            title,
            description
        );

        await _eventStore.AppendEventsAsync(newAd, -1);

        return Ok(newAd);
    }

    [HttpGet("EditDescription")]
    public async Task<IActionResult> EditDescription(string id, string description, long expectedVersion)
    {
        var res = await _eventStore.ReadStreamEventsAsync(id);
        res.UpdateDescription(description);
        await _eventStore.AppendEventsAsync(res, expectedVersion);

        return Ok(res);
    }

    [HttpGet("EditTitle")]
    public async Task<IActionResult> EditTitle(string id, string title, long expectedVersion)
    {
        var res = await _eventStore.ReadStreamEventsAsync(id);
        res.UpdateTitle(title);
        await _eventStore.AppendEventsAsync(res, expectedVersion);

        return Ok(res);
    }

    [HttpGet("EditTitleAndDescription")]
    public async Task<IActionResult> EditTitleAndDescription(string id, string title, string description, long expectedVersion)
    {
        var res = await _eventStore.ReadStreamEventsAsync(id);
        res.UpdateTitle(title);
        res.UpdateDescription(description);
        await _eventStore.AppendEventsAsync(res, expectedVersion);

        return Ok(res);
    }

    [HttpGet("Read")]
    public async Task<IActionResult> Read(string id)
    {
        var res = await _eventStore.ReadStreamEventsAsync(id);

        return Ok(res);
    }
}
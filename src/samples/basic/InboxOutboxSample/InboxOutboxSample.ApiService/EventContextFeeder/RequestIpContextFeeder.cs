using FrameBox.Core.EventContexts.Interfaces;
using FrameBox.Core.EventContexts.Models;

namespace InboxOutboxSample.ApiService.EventContextFeeder;

internal class RequestIpContextFeeder : IEventContextFeeder
{
    private readonly HttpContext _context;

    public RequestIpContextFeeder(HttpContext context)
    {
        _context = context;
    }

    public Task FeedAsync(IEventContext context, CancellationToken cancellationToken)
    {
        var ipAddress = _context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        context.Push("RequestIp", ipAddress);

        return Task.CompletedTask;
    }
}

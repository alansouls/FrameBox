using FrameBox.Core.EventContexts.Interfaces;
using InboxOutboxSample.Shared.Utils;

namespace InboxOutboxSample.ApiService.EventContextFeeder;

internal class RequestIpContextFeeder : IEventContextFeeder, IEventContextRestorer
{
    public const string ContextType = "RequestIp";
    public string Type => ContextType;
    
    private readonly UsefulDataBag _bag;

    public RequestIpContextFeeder(UsefulDataBag bag)
    {
        _bag = bag;
    }

    public Task FeedAsync(IEventContext context, CancellationToken cancellationToken)
    {
        var ipAddress = _bag.IpAddress ?? "Unknown";

        context.Push("RequestIp", ipAddress);

        return Task.CompletedTask;
    }

    public Task RestoreAsync(IEventContext context, CancellationToken cancellationToken)
    {
        if (context.Data.TryGetValue("RequestIp", out var ipAddress))
        {
            _bag.IpAddress = ipAddress;
        }

        return Task.CompletedTask;
    }
}

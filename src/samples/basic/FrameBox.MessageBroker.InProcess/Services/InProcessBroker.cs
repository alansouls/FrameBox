using FrameBox.Core.Common.Exceptions;
using FrameBox.Core.Common.Interfaces;
using FrameBox.Core.Inbox.Models;
using FrameBox.Core.Outbox.Models;
using FrameBox.MessageBroker.AzureServiceBus.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Threading.Channels;

namespace FrameBox.MessageBroker.AzureServiceBus.Services;

public class InProcessBroker : IMessageBroker
{
    private readonly InProcessOptions _options;
    private readonly ChannelWriter<OutboxMessage> _outboxWriter;
    private readonly ChannelWriter<InboxMessage> _inboxWriter;

    public InProcessBroker(ChannelWriter<OutboxMessage> outboxChannel, ChannelWriter<InboxMessage> inboxChannel, IOptions<InProcessOptions> options)
    {
        _options = options.Value;
        _outboxWriter = outboxChannel;
        _inboxWriter = inboxChannel;
    }

    public async Task SendMessagesAsync<T>(IEnumerable<T> messages, CancellationToken cancellationToken) where T : class, IMessage
    {
        var messageList = messages.ToList();

        if (messageList.Count == 0)
        {
            return;
        }

        var failedMessages = new List<T>();
        foreach (var message in messageList)
        {
            try
            {
                if (message is OutboxMessage outboxMessage)
                {
                    await _outboxWriter.WriteAsync(outboxMessage, cancellationToken);
                }
                else if (message is InboxMessage inboxMessage)
                {
                    await _inboxWriter.WriteAsync(inboxMessage, cancellationToken);
                }
                else
                {
                    throw new InvalidOperationException("Message must be of type OutboxMessage or InboxMessage");
                }
            }
            catch (Exception)
            {
                failedMessages.Add(message);
            }
        }

        if (failedMessages.Count > 0)
        {
            throw new FailedToSendMessagesException<T>(failedMessages);
        }
    }
}

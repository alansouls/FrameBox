using Azure.Messaging.ServiceBus;
using FrameBox.Core.Common.Exceptions;
using FrameBox.Core.Common.Interfaces;
using FrameBox.MessageBroker.AzureServiceBus.Options;
using Microsoft.Extensions.Options;

namespace FrameBox.MessageBroker.AzureServiceBus.Services;

public class AzureServiceBusBroker : IMessageBroker
{
    private readonly ServiceBusClient _client;
    private readonly AzureServiceBusOptions _options;

    public AzureServiceBusBroker(ServiceBusClient client, IOptions<AzureServiceBusOptions> options)
    {
        _client = client;
        _options = options.Value;
    }

    public async Task SendMessagesAsync<T>(IEnumerable<T> messages, CancellationToken cancellationToken) where T : class, IMessage
    {
        var messageList = messages.ToList();

        if (messageList.Count == 0)
        {
            return;
        }

        var sender = _client.CreateSender(_options.GetTopicName<T>());
        var failedMessages = new List<T>();
        var batch = messageList;
        while (batch.Count > 0)
        {
            using var messageBatch = await sender.CreateMessageBatchAsync(cancellationToken);
            int i = 0;
            bool sendBatch = true;
            foreach (var message in batch)
            {
                var messageJson = message.ToJson();

                var serviceBusMessage = new ServiceBusMessage()
                {
                    MessageId = message.Id.ToString(),
                    Body = new BinaryData(messageJson),
                    ContentType = "application/json"
                };

                if (!messageBatch.TryAddMessage(serviceBusMessage))
                {
                    if (i == 0)
                    {
                        // Message size too large to fit in a batch
                        failedMessages.Add(message);
                        i = 1;
                        sendBatch = false;
                    }
                    break;
                }
                ++i;
            }

            batch = messageList.Skip(i).ToList();

            if (!sendBatch)
            {
                continue;
            }

            try
            {
                await sender.SendMessagesAsync(messageBatch, cancellationToken);
            }
            catch (Exception)
            {
                failedMessages.AddRange(batch);
            }
        }

        if (failedMessages.Count > 0)
        {
            throw new FailedToSendMessagesException<T>(failedMessages);
        }
    }
}

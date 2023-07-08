using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Entr.Azure.WebJobs.Dispatching.ServiceBus
{

    public class ServiceBusDispatcherHostedService : IHostedService
    {
        private readonly string _connectionString;
        private readonly string _topicName;
        private readonly string _subscriptionName;
        private readonly int _maxDeliveryCount;
        private readonly int _maxConcurrentCalls;
        private readonly IContentTypeConverter _contentTypeConverter;

        private readonly MessageDispatcher _dispatcher;
        private readonly ILogger<ServiceBusDispatcherHostedService> _logger;

        private readonly JsonSerializer _serializer;

        private MessageReceiver _messageReceiver;

        public ServiceBusDispatcherHostedService(
            string connectionString,
            string topic,
            string subscription,
            int maxDeliveryCount,
            int maxConcurrentCalls,
            IContentTypeConverter contentTypeConverter,
            MessageDispatcher messageDispatcher,
            ILogger<ServiceBusDispatcherHostedService> logger)
        {
            _contentTypeConverter = contentTypeConverter;
            _serializer = JsonSerializer.Create();

            _connectionString = connectionString;
            _topicName = topic;
            _subscriptionName = subscription;
            _maxDeliveryCount = maxDeliveryCount;
            _maxConcurrentCalls = maxConcurrentCalls;
            _dispatcher = messageDispatcher;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var entityPath = EntityNameHelper.FormatSubscriptionPath(_topicName, _subscriptionName);

            _messageReceiver = new MessageReceiver(_connectionString, entityPath, ReceiveMode.PeekLock);

            var messageHandlerOptions = new MessageHandlerOptions(HandleException)
            {
                AutoComplete = false,
                MaxConcurrentCalls = _maxConcurrentCalls,
            };

            _messageReceiver.RegisterMessageHandler(ProcessMessageAsync, messageHandlerOptions);

            _logger.LogInformation($"Azure Service Bus Message Dispatcher Host Started (Topic: \"{_topicName}\", Subscription: \"{_subscriptionName}\").");

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Azure Service Bus Message Dispatcher Host Stopping (Topic: \"{_topicName}\", Subscription: \"{_subscriptionName}\")...");

            if (_messageReceiver != null)
            {
                await _messageReceiver.CloseAsync();
            }

            _logger.LogInformation($"Azure Service Bus Message Dispatcher Host Stopped (Topic: \"{_topicName}\", Subscription: \"{_subscriptionName}\").");

        }

        private async Task ProcessMessageAsync(Message message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Processing message \"{message.MessageId}\" of type \"{message.ContentType}\".");

            var messageType = _contentTypeConverter.GetType(message.ContentType);

            if (messageType == null)
            {
                // We could not find a matching type to bind the message to in our referenced assemblies.
                await _messageReceiver.CompleteAsync(message.SystemProperties.LockToken);

                _logger.LogWarning($"Could not locate type for ContentType \"{message.ContentType}\".");

                return;
            }

            _logger.LogTrace($"Attempting to deserialize message content for \"{message.MessageId}\" of type \"{messageType.FullName}\".");

            object messageContent;

            try
            {
                using (var bodyStream = new MemoryStream(message.Body))
                using (var bodyReader = new StreamReader(bodyStream, Encoding.UTF8))
                {
                    messageContent = _serializer.Deserialize(bodyReader, messageType);
                }

                _logger.LogTrace($"Successfully deserialized message content for \"{message.MessageId}\" of type \"{messageType.FullName}\".");
            }
            catch (Exception ex)
            {
                var deserializationFailureMessage = $"Failed to deserialize message content for \"{message.MessageId}\" of type \"{messageType.FullName}\".";

                await _messageReceiver.DeadLetterAsync(message.SystemProperties.LockToken, deserializationFailureMessage);

                _logger.LogError(ex, deserializationFailureMessage);

                return;
            }

            try
            {
                await _dispatcher.Send(messageContent, cancellationToken);

                await _messageReceiver.CompleteAsync(message.SystemProperties.LockToken);

                _logger.LogInformation($"Successfully executed handler for message \"{message.MessageId}\" of type \"{messageType.FullName}\".");
            }
            catch (HandlerNotFoundException)
            {
                await _messageReceiver.CompleteAsync(message.SystemProperties.LockToken);

                _logger.LogInformation($"Could not locate handler for message \"{message.MessageId}\" of type \"{messageType.FullName}\".");
            }
            catch (Exception ex)
            {
                if (message.SystemProperties.DeliveryCount == _maxDeliveryCount)
                {
                    var deliveryCountExceededMessage = $"Failed to process message \"{message.MessageId}\" of type \"{messageType.FullName}\". Maximum delivery count exceeded.";

                    await _messageReceiver.DeadLetterAsync(message.SystemProperties.LockToken, deliveryCountExceededMessage);

                    _logger.LogError(ex, deliveryCountExceededMessage);
                }
                else
                {
                    // NOTE: we DO NOT Complete, Abandon or Defer here as we are currently relying on the lock timeout to expire
                    // causing the message to be re-processed.
                    _logger.LogWarning($"Unhandled exception while processing message \"{message.MessageId}\" of type \"{messageType.FullName}\". Delivery count: {message.SystemProperties.DeliveryCount}.");
                }
            }
        }

        private Task HandleException(ExceptionReceivedEventArgs args)
        {
            _logger.LogError(args.Exception, args.Exception.Message);

            return Task.CompletedTask;
        }
    }
}

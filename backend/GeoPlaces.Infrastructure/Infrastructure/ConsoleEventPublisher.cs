using GeoPlaces.Contracts.Events;
using Microsoft.Extensions.Logging;

namespace GeoPlaces.Infrastructure;

public class ConsoleEventPublisher : IEventPublisher
{
    private readonly ILogger<ConsoleEventPublisher> _logger;

    public ConsoleEventPublisher(ILogger<ConsoleEventPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Event published: {EventType} {@Event}", typeof(TEvent).Name, @event);
        return Task.CompletedTask;
    }
}

using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using CommandsService.Enums;
using System.Text.Json;
using CommandsService.Dtos;
using CommandsService.Data;
using CommandsService.Models;

namespace CommandsService.EventProcessing
{
  public class EventProcessor : IEventProcessor
  {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
      _scopeFactory = scopeFactory;
      _mapper = mapper;

    }

    public void ProcessEvent(string message)
    {
      var eventType = DetermineEvent(message);
      switch (eventType)
      {
        case EventType.PlatformPublished:
          addPlatform(message);
          break;
        default:
          break;
      }
    }

    private void addPlatform(string publishedMessage)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
        var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(publishedMessage);

        try
        {
          var plat = _mapper.Map<Platform>(platformPublishedDto);

          if (!repo.ExternalPlatformExists(plat.ExternalId))
          {
            repo.CreatePlatform(plat);
            repo.SaveChanges();

            Console.WriteLine("--> Platform added!");
          }
          else
          {
            Console.WriteLine("--> Platform already exisits!");
          }
        }
        catch (System.Exception ex)
        {
          Console.WriteLine($"--> Could not add Platform to DB {ex.Message}");
        }

      }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
      Console.WriteLine("--> Determining Event");

      var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

      switch (eventType?.Event)
      {
        case "Platform_Published":
          Console.WriteLine("--> Platform Published Event Detected");
          return EventType.PlatformPublished;

        default:
          Console.WriteLine("--> Could not determine the event type");
          return EventType.Undetermined;
      }

    }
  }
}


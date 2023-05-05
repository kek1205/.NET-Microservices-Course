using AutoMapper;
using CommandsService.Dtos;
using CommandsService.Models;
using PlatformService;

namespace CommandsService.Profiles
{
  public class Commandsprofile : Profile
  {
    public Commandsprofile()
    {

      // Source => Target
      CreateMap<Platform, PlatformReadDto>();
      CreateMap<CommandCreateDto, Command>();
      CreateMap<Command, CommandReadDto>();
      CreateMap<PlatformPublishedDto, Platform>()
        .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id));
      CreateMap<GrpcPlatformModel, Platform>()
        .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.PlatformId))
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name)) //Does not need to do it
        .ForMember(dest => dest.Commands, opt => opt.Ignore());
    }

  }
}
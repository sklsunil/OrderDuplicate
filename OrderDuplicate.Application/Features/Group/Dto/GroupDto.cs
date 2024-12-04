using AutoMapper;
using OrderDuplicate.Application.Common.Mappings;
using OrderDuplicate.Application.Features.Counter.DTOs;
using OrderDuplicate.Domain.Entities;
using System.ComponentModel;

namespace OrderDuplicate.Application.Features.Group.Dto
{
    public class GroupDto : IMapFrom<GroupEntity>
    {
        public void Mapping(Profile profile)
        {
            profile.CreateMap<GroupEntity, GroupDto>()
                   .ForMember(dest => dest.Counters, opt => opt.MapFrom(src => src.GroupCounters.Select(gc => gc.Counter)));
            profile.CreateMap<GroupDto, GroupEntity>();
        }

        [Description("Id")]
        public int Id { get; set; }
        public string GroupName { get; set; }
        public List<CounterDto> Counters { get; set; } = new List<CounterDto>();
    }
}

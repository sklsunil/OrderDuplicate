using AutoMapper;

using OrderDuplicate.Application.Common.Mappings;
using OrderDuplicate.Domain.Entities;

using System.ComponentModel;

namespace OrderDuplicate.Application.Features.Counter.DTOs;


public class CounterDto : IMapFrom<CounterEntity>
{
    public void Mapping(Profile profile)
    {
        profile.CreateMap<CounterEntity, CounterDto>();
        profile.CreateMap<CounterDto, CounterEntity>();
    }

    [Description("Id")]
    public int Id { get; set; }

    [Description("Counter Name")]
    public string CounterName { get; set; }

    [Description("Person Id")]
    public int PersonId { get; set; }

}


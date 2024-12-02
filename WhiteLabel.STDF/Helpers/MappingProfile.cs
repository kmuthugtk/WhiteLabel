using AutoMapper;
using LinqToStdf.Records.V4;
using WhiteLabel.STDF.Models;

namespace WhiteLabel.STDF.Helpers;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<Mir, MirRecord>();
	}
}

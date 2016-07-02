using AutoMapper;
using MrCullDevTools.Models.Users;

namespace MrCullDevTools
{
	public class AutomapperConfig
	{
		public static void CreateMappings()
		{
			Mapper.CreateMap<User, ViewModels.Account.Register>();
			Mapper.CreateMap<ViewModels.Account.Register, User>();
		}
	}
}
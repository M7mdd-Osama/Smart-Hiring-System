using AutoMapper;
using SmartHiring.APIs.DTOs;
using SmartHiring.Core.Entities;

namespace SmartHiring.APIs.Helpers
{
	public class PictureUrlResolver<TSource, TDestination> : IValueResolver<TSource, TDestination, string>
		where TSource : class
	{
		private readonly IConfiguration _configuration;

		public PictureUrlResolver(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string Resolve(TSource source, TDestination destination, string destMember, ResolutionContext context)
		{
			var logoUrlProperty = typeof(TSource).GetProperty("LogoUrl");
			if (logoUrlProperty != null)
			{
				var logoUrl = logoUrlProperty.GetValue(source) as string;
				if (!string.IsNullOrEmpty(logoUrl))
				{
					return $"{_configuration["ApiBaseUrl"]}{logoUrl}";
				}
			}
			return string.Empty;
		}
	}
}

using AutoMapper;
using System.Reflection;

namespace SmartHiring.APIs.Helpers
{
	public class PictureUrlResolver<TSource, TDestination> : IValueResolver<TSource, TDestination, string> where TSource : class
	{
		private readonly IConfiguration _configuration;
		private readonly HashSet<object> _visitedObjects = new();

		public PictureUrlResolver(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string Resolve(TSource source, TDestination destination, string destMember, ResolutionContext context)
		{
			_visitedObjects.Clear();
            if (source == null) return string.Empty;

			string logoUrl = FindLogoUrl(source);
			return !string.IsNullOrEmpty(logoUrl) ? $"{_configuration["ApiBaseUrl"]}{logoUrl}" : string.Empty;
		}

		private string FindLogoUrl(object obj)
		{
			if (obj == null || _visitedObjects.Contains(obj)) return null;
			_visitedObjects.Add(obj);

			var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (var prop in properties)
			{
				if (prop.Name == "LogoUrl" && prop.PropertyType == typeof(string))
				{
					var value = prop.GetValue(obj) as string;
					if (!string.IsNullOrEmpty(value))
						return value;
				}
				else if (!prop.PropertyType.IsPrimitive && !prop.PropertyType.IsEnum && prop.PropertyType != typeof(string))
				{
					var nestedValue = FindLogoUrl(prop.GetValue(obj));
					if (!string.IsNullOrEmpty(nestedValue))
						return nestedValue;
				}
			}
			return null;
		}
	}
}
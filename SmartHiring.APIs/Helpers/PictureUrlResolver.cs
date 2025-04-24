using AutoMapper;
using System.Reflection;
namespace SmartHiring.APIs.Helpers
{
    public class GenericUrlResolver<TSource, TDestination> : IValueResolver<TSource, TDestination, string> where TSource : class
    {
        private readonly IConfiguration _configuration;
        private readonly HashSet<object> _visitedObjects = new();
        private readonly string _propertyToFind;

        public GenericUrlResolver(IConfiguration configuration, string propertyToFind)
        {
            _configuration = configuration;
            _propertyToFind = propertyToFind;
        }

        public string Resolve(TSource source, TDestination destination, string destMember, ResolutionContext context)
        {
            _visitedObjects.Clear();
            if (source == null) return string.Empty;
            string url = FindUrlProperty(source);
            return !string.IsNullOrEmpty(url) ? $"{_configuration["ApiBaseUrl"]}{url}" : string.Empty;
        }

        private string FindUrlProperty(object obj)
        {
            if (obj == null || _visitedObjects.Contains(obj)) return null;
            _visitedObjects.Add(obj);
            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                if (prop.Name == _propertyToFind && prop.PropertyType == typeof(string))
                {
                    var value = prop.GetValue(obj) as string;
                    if (!string.IsNullOrEmpty(value))
                        return value;
                }
                else if (!prop.PropertyType.IsPrimitive && !prop.PropertyType.IsEnum && prop.PropertyType != typeof(string))
                {
                    var nestedValue = FindUrlProperty(prop.GetValue(obj));
                    if (!string.IsNullOrEmpty(nestedValue))
                        return nestedValue;
                }
            }
            return null;
        }
    }

    public class LogoUrlResolverFactory<TSource, TDestination> : IValueResolver<TSource, TDestination, string> where TSource : class
    {
        private readonly GenericUrlResolver<TSource, TDestination> _resolver;

        public LogoUrlResolverFactory(IConfiguration configuration)
        {
            _resolver = new GenericUrlResolver<TSource, TDestination>(configuration, "LogoUrl");
        }

        public string Resolve(TSource source, TDestination destination, string destMember, ResolutionContext context)
        {
            return _resolver.Resolve(source, destination, destMember, context);
        }
    }

    public class CVUrlResolverFactory<TSource, TDestination> : IValueResolver<TSource, TDestination, string> where TSource : class
    {
        private readonly GenericUrlResolver<TSource, TDestination> _resolver;

        public CVUrlResolverFactory(IConfiguration configuration)
        {
            _resolver = new GenericUrlResolver<TSource, TDestination>(configuration, "CV_Link");
        }

        public string Resolve(TSource source, TDestination destination, string destMember, ResolutionContext context)
        {
            return _resolver.Resolve(source, destination, destMember, context);
        }
    }
}
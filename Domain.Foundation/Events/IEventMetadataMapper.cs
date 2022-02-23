namespace Domain.Foundation.Events
{
    public interface IEventMetadataMapper
    {
        void AddType<T>(string name);
    }
    
    public static class EventMetadataMapperExtensions
    {
        public static IEventMetadataMapper Add<T>(this IEventMetadataMapper mapper, string name)
        {
            mapper.AddType<T>(name);
            return mapper;
        }
    }
}
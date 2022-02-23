namespace Domain.Foundation.SourceGenerator.Tests.TestAggregate
{
    public record OrderItem
    {
        public long ProductId { get; init; }
        public uint Count { get; set; }
    }
}
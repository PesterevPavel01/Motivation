using Motivation.Domain.Entities.Base;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Motivation.Domain.Entities
{
    public sealed class OutboxMessage : Auditable
    {
        public OutboxMessage(Guid id)
        {
            Id = id;
        }
        public static JsonSerializerOptions JsonSettings 
            => new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                };

        public Guid Id { get; }
        public required string Type { get; set; }

        public required string Content { get; set; }

        public DateTime? ProcessedAt { get; set; }

        public string? Error { get; set; }
    }
}

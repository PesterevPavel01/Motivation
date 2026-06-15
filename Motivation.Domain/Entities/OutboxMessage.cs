using Motivation.Domain.Entities.Base;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Motivation.Domain.Entities
{
    public class OutboxMessage : Auditable
    {
        public OutboxMessage(Guid id)
        {
            Id = id;
        }
        public static JsonSerializerOptions JsonSettings 
            => new()
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                };

        public Guid Id { get; }

        public bool Enabled { get; protected set; } = true;

        public void Disable()
        {
            Enabled = false;
        }
        
        public required string Type { get; set; }

        public required string Content { get; set; }

        public DateTime? ProcessedAt { get; set; }

        public string? Error { get; set; }
    }
}

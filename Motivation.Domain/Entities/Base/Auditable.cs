using Motivation.Domain.Interfaces;

namespace Motivation.Domain.Entities.Base
{
    public class Auditable : IAuditable
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}

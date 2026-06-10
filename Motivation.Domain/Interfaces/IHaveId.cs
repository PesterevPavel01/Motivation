namespace Motivation.Domain.Interfaces
{
    /// <summary>
    /// Identifier common interface
    /// </summary>
    public interface IHaveId
    {
        /// <summary>
        /// Identifier
        /// </summary>
        Guid Id { get; set; }
    }
}
using Motivation.Contracts.Interfaces;
using Motivation.Domain.Interfaces;

namespace Motivation.Contracts.ViewModels
{
    /// <summary>
    /// ViewModelBase for 
    /// </summary>
    public class ViewModelBase : IViewModel, IHaveId
    {
        /// <summary>
        /// Identifier
        /// </summary>
        public Guid Id { get; set; }
    }
}
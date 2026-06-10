namespace Motivation.IdentityServer.Domain.Interfaces
{
    public interface IIdentityClientConfiguration<T>
    {
        T GetDescriptor();
    }
}

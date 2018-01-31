namespace TNVS.SimpleInjector.Modularization.Abstractions
{
    public interface IModule
    {
        void RegisterServices(IServiceRegistry registry);
    }
}

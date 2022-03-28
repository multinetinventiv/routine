namespace Routine.Service.Configuration
{
    public class ServiceConfigurationBuilder
    {
        public ConventionBasedServiceConfiguration FromBasic() =>
            new ConventionBasedServiceConfiguration()
                .RootPath.Set("Service")
                .EnableTestApp.Set(true)
                .TestAppPath.Set("TestApp")
                .AllowGet.Set(false)
                .ExceptionResult.Set(c => c.By(ex => new ExceptionResult(ex.GetType().Name, ex.ToString())))
                .ResponseHeaderValue.SetDefault()

                .NextLayer();
    }
}

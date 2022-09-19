namespace Routine.Service.Configuration;

public class ServiceClientConfigurationBuilder
{
    public ConventionBasedServiceClientConfiguration FromBasic() =>
        new ConventionBasedServiceClientConfiguration()
            .Exception.Set(c => c.By(er => new Exception($"{er.Type} - {er.Message}")).When(er => er is not null))
            .Exception.Set(new Exception())
            .RequestHeaderValue.Set(string.Empty)

            .NextLayer();
}

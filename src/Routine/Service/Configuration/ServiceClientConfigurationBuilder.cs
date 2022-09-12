using System;

namespace Routine.Service.Configuration;

public class ServiceClientConfigurationBuilder
{
    public ConventionBasedServiceClientConfiguration FromBasic() =>
        new ConventionBasedServiceClientConfiguration()
            .Exception.Set(new Exception())
            .RequestHeaderValue.Set(string.Empty)

            .NextLayer();
}

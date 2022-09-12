using Routine.Core.Configuration;
using Routine.Service.Configuration;
using System;

namespace Routine;

public static class ServiceClientPatterns
{
    public static ConventionBasedServiceClientConfiguration FromEmpty(this PatternBuilder<ConventionBasedServiceClientConfiguration> source) => new();
    public static ConventionBasedServiceClientConfiguration FormattedExceptionPattern(this PatternBuilder<ConventionBasedServiceClientConfiguration> source, string formatWithTypeAndMessageAndIsHandled) =>
        source.FromEmpty()
            .Exception.Set(e => e.By(ex => new Exception(string.Format(formatWithTypeAndMessageAndIsHandled, ex.Type, ex.Message, ex.IsHandled))));
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Routine.Core;
using Routine.Test.Core.Interceptor.Domain;

namespace Routine.Test.Core.Interceptor.Domain
{
	public class TestContext<T> : InterceptionContext
	{
		public T Value { get; set; }
	}
}

namespace Routine.Test.Core.Interceptor
{
	public abstract class InterceptorTestBase : CoreTestBase
	{
		protected class TestConfiguration { }

		protected TestContext<string> String() { return String(null); }
		protected TestContext<string> String(string value) { return Ctx<string>(value); }
		protected TestContext<T> Ctx<T>() { return Ctx<T>(default(T)); }
		protected TestContext<T> Ctx<T>(T value) { return new TestContext<T> { Value = value }; }
		protected TestConfiguration DummyConfiguration() { return new TestConfiguration(); }
	}
}

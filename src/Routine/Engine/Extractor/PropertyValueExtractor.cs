using System;

namespace Routine.Engine.Extractor
{
	public class PropertyValueExtractor : ExtractorBase
	{
		private readonly IProperty property;

		private Func<object, object, string> converterDelegate;

		public PropertyValueExtractor(IProperty property)
		{
			if (property == null) { throw new ArgumentNullException("property"); }

			this.property = property;

			Return(result => result == null ? null : result.ToString());
		}

		public PropertyValueExtractor Return(Func<object, string> converterDelegate) { return Return((o, f) => converterDelegate(o)); }
		public PropertyValueExtractor Return(Func<object, object, string> converterDelegate) { this.converterDelegate = converterDelegate; return this; }

		protected override string Extract(object obj)
		{
			if (obj == null)
			{
				return null;
			}

			var result = property.FetchFrom(obj);

			return converterDelegate(result, obj);
		}
	}
}

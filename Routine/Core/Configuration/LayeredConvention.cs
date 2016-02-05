using Routine.Core.Configuration.Convention;

namespace Routine.Core.Configuration
{
	internal class LayeredConvention<TFrom, TResult>
	{
		public IConvention<TFrom, TResult> Convention { get; private set; }
		public Layer Layer { get; private set; }

		public LayeredConvention(IConvention<TFrom, TResult> convention, Layer layer)
		{
			Convention = convention;
			Layer = layer;
		}
	}
}
using System;

namespace Routine.Core.Configuration
{
	public class LayeredBase<TConcrete> : ILayered
		where TConcrete : LayeredBase<TConcrete>
	{
		private Layer currentLayer;

		protected LayeredBase()
		{
			currentLayer = Layer.LeastSpecific;
		}

		public TConcrete Override(Func<TConcrete, TConcrete> @override)
		{
			var oldLayer = currentLayer;

			currentLayer = Layer.MostSpecific;

			@override((TConcrete)this);

			currentLayer = oldLayer;

			return (TConcrete)this;
		}

		public TConcrete NextLayer()
		{
			currentLayer = currentLayer.MoreSpecific();

			return (TConcrete)this;
		}

		#region ILayered implementation

		Layer ILayered.CurrentLayer { get { return currentLayer; } }

		#endregion
	}
}
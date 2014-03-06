namespace Routine.Core.Extractor
{
	public class StaticExtractor<TFrom, TResult> : BaseOptionalExtractor<StaticExtractor<TFrom, TResult>, TFrom, TResult>
	{
		private readonly TResult staticResult;

		public StaticExtractor(TResult staticResult)
		{
			this.staticResult = staticResult;
		}

		protected override TResult Extract(TFrom obj)
		{
			return staticResult;
		}
	}
}

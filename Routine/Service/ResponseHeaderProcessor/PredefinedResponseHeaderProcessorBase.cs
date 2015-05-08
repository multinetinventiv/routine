using System;
using System.Collections.Generic;

namespace Routine.Service.ResponseHeaderProcessor
{
	public abstract class PredefinedResponseHeaderProcessorBase<TConcrete> : IResponseHeaderProcessor
		where TConcrete : PredefinedResponseHeaderProcessorBase<TConcrete>
	{
		private readonly string[] headerKeys;
		private Action<List<string>> processorDelegate;

		protected PredefinedResponseHeaderProcessorBase(params string[] headerKeys)
		{
			this.headerKeys = headerKeys;

			processorDelegate = list => { };
		}

		protected void Process(Dictionary<string, string> responseHeaders)
		{
			var headers = new List<string>();
			foreach (var headerKey in headerKeys)
			{
				string header;
				if (!responseHeaders.TryGetValue(headerKey, out header))
				{
					header = string.Empty;
				}

				headers.Add(header);
			}

			processorDelegate(headers);
		}

		protected TConcrete Do(Action<List<string>> processorDelegate) { this.processorDelegate = processorDelegate; return (TConcrete)this; }

		#region IResponseHeaderProcessor implementation

		void IResponseHeaderProcessor.Process(Dictionary<string, string> responseHeaders) { Process(responseHeaders); }

		#endregion
	}
}
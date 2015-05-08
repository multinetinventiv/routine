using System;

namespace Routine.Service.ResponseHeaderProcessor
{
    
	public class Predefined1ResponseHeaderProcessor : PredefinedResponseHeaderProcessorBase<Predefined1ResponseHeaderProcessor>
	{
		public Predefined1ResponseHeaderProcessor(string headerKey1) 
			: base(headerKey1) { }

		public Predefined1ResponseHeaderProcessor Do(Action<string> processorDelegate)
		{
			return Do(headers => processorDelegate(headers[0]));
		}
	}
    
	public class Predefined2ResponseHeaderProcessor : PredefinedResponseHeaderProcessorBase<Predefined2ResponseHeaderProcessor>
	{
		public Predefined2ResponseHeaderProcessor(string headerKey1, string headerKey2) 
			: base(headerKey1, headerKey2) { }

		public Predefined2ResponseHeaderProcessor Do(Action<string, string> processorDelegate)
		{
			return Do(headers => processorDelegate(headers[0], headers[1]));
		}
	}
    
	public class Predefined3ResponseHeaderProcessor : PredefinedResponseHeaderProcessorBase<Predefined3ResponseHeaderProcessor>
	{
		public Predefined3ResponseHeaderProcessor(string headerKey1, string headerKey2, string headerKey3) 
			: base(headerKey1, headerKey2, headerKey3) { }

		public Predefined3ResponseHeaderProcessor Do(Action<string, string, string> processorDelegate)
		{
			return Do(headers => processorDelegate(headers[0], headers[1], headers[2]));
		}
	}
    
	public class Predefined4ResponseHeaderProcessor : PredefinedResponseHeaderProcessorBase<Predefined4ResponseHeaderProcessor>
	{
		public Predefined4ResponseHeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4) 
			: base(headerKey1, headerKey2, headerKey3, headerKey4) { }

		public Predefined4ResponseHeaderProcessor Do(Action<string, string, string, string> processorDelegate)
		{
			return Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3]));
		}
	}
    
	public class Predefined5ResponseHeaderProcessor : PredefinedResponseHeaderProcessorBase<Predefined5ResponseHeaderProcessor>
	{
		public Predefined5ResponseHeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5) 
			: base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5) { }

		public Predefined5ResponseHeaderProcessor Do(Action<string, string, string, string, string> processorDelegate)
		{
			return Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4]));
		}
	}
    
	public class Predefined6ResponseHeaderProcessor : PredefinedResponseHeaderProcessorBase<Predefined6ResponseHeaderProcessor>
	{
		public Predefined6ResponseHeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6) 
			: base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6) { }

		public Predefined6ResponseHeaderProcessor Do(Action<string, string, string, string, string, string> processorDelegate)
		{
			return Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5]));
		}
	}
    
	public class Predefined7ResponseHeaderProcessor : PredefinedResponseHeaderProcessorBase<Predefined7ResponseHeaderProcessor>
	{
		public Predefined7ResponseHeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7) 
			: base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7) { }

		public Predefined7ResponseHeaderProcessor Do(Action<string, string, string, string, string, string, string> processorDelegate)
		{
			return Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6]));
		}
	}
    
	public class Predefined8ResponseHeaderProcessor : PredefinedResponseHeaderProcessorBase<Predefined8ResponseHeaderProcessor>
	{
		public Predefined8ResponseHeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8) 
			: base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8) { }

		public Predefined8ResponseHeaderProcessor Do(Action<string, string, string, string, string, string, string, string> processorDelegate)
		{
			return Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6], headers[7]));
		}
	}
    
	public class Predefined9ResponseHeaderProcessor : PredefinedResponseHeaderProcessorBase<Predefined9ResponseHeaderProcessor>
	{
		public Predefined9ResponseHeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9) 
			: base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9) { }

		public Predefined9ResponseHeaderProcessor Do(Action<string, string, string, string, string, string, string, string, string> processorDelegate)
		{
			return Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6], headers[7], headers[8]));
		}
	}
    
	public class Predefined10ResponseHeaderProcessor : PredefinedResponseHeaderProcessorBase<Predefined10ResponseHeaderProcessor>
	{
		public Predefined10ResponseHeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9, string headerKey10) 
			: base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9, headerKey10) { }

		public Predefined10ResponseHeaderProcessor Do(Action<string, string, string, string, string, string, string, string, string, string> processorDelegate)
		{
			return Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6], headers[7], headers[8], headers[9]));
		}
	}
    
	public class Predefined11ResponseHeaderProcessor : PredefinedResponseHeaderProcessorBase<Predefined11ResponseHeaderProcessor>
	{
		public Predefined11ResponseHeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9, string headerKey10, string headerKey11) 
			: base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9, headerKey10, headerKey11) { }

		public Predefined11ResponseHeaderProcessor Do(Action<string, string, string, string, string, string, string, string, string, string, string> processorDelegate)
		{
			return Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6], headers[7], headers[8], headers[9], headers[10]));
		}
	}
    
	public class Predefined12ResponseHeaderProcessor : PredefinedResponseHeaderProcessorBase<Predefined12ResponseHeaderProcessor>
	{
		public Predefined12ResponseHeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9, string headerKey10, string headerKey11, string headerKey12) 
			: base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9, headerKey10, headerKey11, headerKey12) { }

		public Predefined12ResponseHeaderProcessor Do(Action<string, string, string, string, string, string, string, string, string, string, string, string> processorDelegate)
		{
			return Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6], headers[7], headers[8], headers[9], headers[10], headers[11]));
		}
	}
    
	public class Predefined13ResponseHeaderProcessor : PredefinedResponseHeaderProcessorBase<Predefined13ResponseHeaderProcessor>
	{
		public Predefined13ResponseHeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9, string headerKey10, string headerKey11, string headerKey12, string headerKey13) 
			: base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9, headerKey10, headerKey11, headerKey12, headerKey13) { }

		public Predefined13ResponseHeaderProcessor Do(Action<string, string, string, string, string, string, string, string, string, string, string, string, string> processorDelegate)
		{
			return Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6], headers[7], headers[8], headers[9], headers[10], headers[11], headers[12]));
		}
	}
    
	public class Predefined14ResponseHeaderProcessor : PredefinedResponseHeaderProcessorBase<Predefined14ResponseHeaderProcessor>
	{
		public Predefined14ResponseHeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9, string headerKey10, string headerKey11, string headerKey12, string headerKey13, string headerKey14) 
			: base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9, headerKey10, headerKey11, headerKey12, headerKey13, headerKey14) { }

		public Predefined14ResponseHeaderProcessor Do(Action<string, string, string, string, string, string, string, string, string, string, string, string, string, string> processorDelegate)
		{
			return Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6], headers[7], headers[8], headers[9], headers[10], headers[11], headers[12], headers[13]));
		}
	}
    
	public class Predefined15ResponseHeaderProcessor : PredefinedResponseHeaderProcessorBase<Predefined15ResponseHeaderProcessor>
	{
		public Predefined15ResponseHeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9, string headerKey10, string headerKey11, string headerKey12, string headerKey13, string headerKey14, string headerKey15) 
			: base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9, headerKey10, headerKey11, headerKey12, headerKey13, headerKey14, headerKey15) { }

		public Predefined15ResponseHeaderProcessor Do(Action<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string> processorDelegate)
		{
			return Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6], headers[7], headers[8], headers[9], headers[10], headers[11], headers[12], headers[13], headers[14]));
		}
	}

}
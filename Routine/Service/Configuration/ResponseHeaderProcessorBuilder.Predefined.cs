using Routine.Service.ResponseHeaderProcessor;

namespace Routine.Service.Configuration
{
    public partial class ResponseHeaderProcessorBuilder
    {
        	
	    public Predefined1ResponseHeaderProcessor For(string headerKey1)
        {
		    return new Predefined1ResponseHeaderProcessor(headerKey1);
	    }
        	
	    public Predefined2ResponseHeaderProcessor For(string headerKey1, string headerKey2)
        {
		    return new Predefined2ResponseHeaderProcessor(headerKey1, headerKey2);
	    }
        	
	    public Predefined3ResponseHeaderProcessor For(string headerKey1, string headerKey2, string headerKey3)
        {
		    return new Predefined3ResponseHeaderProcessor(headerKey1, headerKey2, headerKey3);
	    }
        	
	    public Predefined4ResponseHeaderProcessor For(string headerKey1, string headerKey2, string headerKey3, string headerKey4)
        {
		    return new Predefined4ResponseHeaderProcessor(headerKey1, headerKey2, headerKey3, headerKey4);
	    }
        	
	    public Predefined5ResponseHeaderProcessor For(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5)
        {
		    return new Predefined5ResponseHeaderProcessor(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5);
	    }
        	
	    public Predefined6ResponseHeaderProcessor For(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6)
        {
		    return new Predefined6ResponseHeaderProcessor(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6);
	    }
        	
	    public Predefined7ResponseHeaderProcessor For(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7)
        {
		    return new Predefined7ResponseHeaderProcessor(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7);
	    }
        	
	    public Predefined8ResponseHeaderProcessor For(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8)
        {
		    return new Predefined8ResponseHeaderProcessor(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8);
	    }
        	
	    public Predefined9ResponseHeaderProcessor For(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9)
        {
		    return new Predefined9ResponseHeaderProcessor(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9);
	    }
        	
	    public Predefined10ResponseHeaderProcessor For(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9, string headerKey10)
        {
		    return new Predefined10ResponseHeaderProcessor(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9, headerKey10);
	    }
        	
	    public Predefined11ResponseHeaderProcessor For(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9, string headerKey10, string headerKey11)
        {
		    return new Predefined11ResponseHeaderProcessor(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9, headerKey10, headerKey11);
	    }
        	
	    public Predefined12ResponseHeaderProcessor For(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9, string headerKey10, string headerKey11, string headerKey12)
        {
		    return new Predefined12ResponseHeaderProcessor(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9, headerKey10, headerKey11, headerKey12);
	    }
        	
	    public Predefined13ResponseHeaderProcessor For(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9, string headerKey10, string headerKey11, string headerKey12, string headerKey13)
        {
		    return new Predefined13ResponseHeaderProcessor(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9, headerKey10, headerKey11, headerKey12, headerKey13);
	    }
        	
	    public Predefined14ResponseHeaderProcessor For(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9, string headerKey10, string headerKey11, string headerKey12, string headerKey13, string headerKey14)
        {
		    return new Predefined14ResponseHeaderProcessor(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9, headerKey10, headerKey11, headerKey12, headerKey13, headerKey14);
	    }
        	
	    public Predefined15ResponseHeaderProcessor For(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9, string headerKey10, string headerKey11, string headerKey12, string headerKey13, string headerKey14, string headerKey15)
        {
		    return new Predefined15ResponseHeaderProcessor(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9, headerKey10, headerKey11, headerKey12, headerKey13, headerKey14, headerKey15);
	    }
    
    }
}
using System;

namespace Routine.Service.HeaderProcessor
{

    public class Predefined1HeaderProcessor : PredefinedHeaderProcessorBase<Predefined1HeaderProcessor>
    {
        public Predefined1HeaderProcessor(string headerKey1)
            : base(headerKey1) { }

        public Predefined1HeaderProcessor Do(Action<string> processorDelegate) =>
            Do(headers => processorDelegate(headers[0]));
    }

    public class Predefined2HeaderProcessor : PredefinedHeaderProcessorBase<Predefined2HeaderProcessor>
    {
        public Predefined2HeaderProcessor(string headerKey1, string headerKey2)
            : base(headerKey1, headerKey2) { }

        public Predefined2HeaderProcessor Do(Action<string, string> processorDelegate) =>
            Do(headers => processorDelegate(headers[0], headers[1]));
    }

    public class Predefined3HeaderProcessor : PredefinedHeaderProcessorBase<Predefined3HeaderProcessor>
    {
        public Predefined3HeaderProcessor(string headerKey1, string headerKey2, string headerKey3)
            : base(headerKey1, headerKey2, headerKey3) { }

        public Predefined3HeaderProcessor Do(Action<string, string, string> processorDelegate) =>
            Do(headers => processorDelegate(headers[0], headers[1], headers[2]));
    }

    public class Predefined4HeaderProcessor : PredefinedHeaderProcessorBase<Predefined4HeaderProcessor>
    {
        public Predefined4HeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4)
            : base(headerKey1, headerKey2, headerKey3, headerKey4) { }

        public Predefined4HeaderProcessor Do(Action<string, string, string, string> processorDelegate) =>
            Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3]));
    }

    public class Predefined5HeaderProcessor : PredefinedHeaderProcessorBase<Predefined5HeaderProcessor>
    {
        public Predefined5HeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5)
            : base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5) { }

        public Predefined5HeaderProcessor Do(Action<string, string, string, string, string> processorDelegate) =>
            Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4]));
    }

    public class Predefined6HeaderProcessor : PredefinedHeaderProcessorBase<Predefined6HeaderProcessor>
    {
        public Predefined6HeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6)
            : base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6) { }

        public Predefined6HeaderProcessor Do(Action<string, string, string, string, string, string> processorDelegate) =>
            Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5]));
    }

    public class Predefined7HeaderProcessor : PredefinedHeaderProcessorBase<Predefined7HeaderProcessor>
    {
        public Predefined7HeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7)
            : base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7) { }

        public Predefined7HeaderProcessor Do(Action<string, string, string, string, string, string, string> processorDelegate) =>
            Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6]));
    }

    public class Predefined8HeaderProcessor : PredefinedHeaderProcessorBase<Predefined8HeaderProcessor>
    {
        public Predefined8HeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8)
            : base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8) { }

        public Predefined8HeaderProcessor Do(Action<string, string, string, string, string, string, string, string> processorDelegate) =>
            Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6], headers[7]));
    }

    public class Predefined9HeaderProcessor : PredefinedHeaderProcessorBase<Predefined9HeaderProcessor>
    {
        public Predefined9HeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9)
            : base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9) { }

        public Predefined9HeaderProcessor Do(Action<string, string, string, string, string, string, string, string, string> processorDelegate) =>
            Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6], headers[7], headers[8]));
    }

    public class Predefined10HeaderProcessor : PredefinedHeaderProcessorBase<Predefined10HeaderProcessor>
    {
        public Predefined10HeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9, string headerKey10)
            : base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9, headerKey10) { }

        public Predefined10HeaderProcessor Do(Action<string, string, string, string, string, string, string, string, string, string> processorDelegate) =>
            Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6], headers[7], headers[8], headers[9]));
    }

    public class Predefined11HeaderProcessor : PredefinedHeaderProcessorBase<Predefined11HeaderProcessor>
    {
        public Predefined11HeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9, string headerKey10, string headerKey11)
            : base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9, headerKey10, headerKey11) { }

        public Predefined11HeaderProcessor Do(Action<string, string, string, string, string, string, string, string, string, string, string> processorDelegate) =>
            Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6], headers[7], headers[8], headers[9], headers[10]));
    }

    public class Predefined12HeaderProcessor : PredefinedHeaderProcessorBase<Predefined12HeaderProcessor>
    {
        public Predefined12HeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9, string headerKey10, string headerKey11, string headerKey12)
            : base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9, headerKey10, headerKey11, headerKey12) { }

        public Predefined12HeaderProcessor Do(Action<string, string, string, string, string, string, string, string, string, string, string, string> processorDelegate) =>
            Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6], headers[7], headers[8], headers[9], headers[10], headers[11]));
    }

    public class Predefined13HeaderProcessor : PredefinedHeaderProcessorBase<Predefined13HeaderProcessor>
    {
        public Predefined13HeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9, string headerKey10, string headerKey11, string headerKey12, string headerKey13)
            : base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9, headerKey10, headerKey11, headerKey12, headerKey13) { }

        public Predefined13HeaderProcessor Do(Action<string, string, string, string, string, string, string, string, string, string, string, string, string> processorDelegate) =>
            Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6], headers[7], headers[8], headers[9], headers[10], headers[11], headers[12]));
    }

    public class Predefined14HeaderProcessor : PredefinedHeaderProcessorBase<Predefined14HeaderProcessor>
    {
        public Predefined14HeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9, string headerKey10, string headerKey11, string headerKey12, string headerKey13, string headerKey14)
            : base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9, headerKey10, headerKey11, headerKey12, headerKey13, headerKey14) { }

        public Predefined14HeaderProcessor Do(Action<string, string, string, string, string, string, string, string, string, string, string, string, string, string> processorDelegate) =>
            Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6], headers[7], headers[8], headers[9], headers[10], headers[11], headers[12], headers[13]));
    }

    public class Predefined15HeaderProcessor : PredefinedHeaderProcessorBase<Predefined15HeaderProcessor>
    {
        public Predefined15HeaderProcessor(string headerKey1, string headerKey2, string headerKey3, string headerKey4, string headerKey5, string headerKey6, string headerKey7, string headerKey8, string headerKey9, string headerKey10, string headerKey11, string headerKey12, string headerKey13, string headerKey14, string headerKey15)
            : base(headerKey1, headerKey2, headerKey3, headerKey4, headerKey5, headerKey6, headerKey7, headerKey8, headerKey9, headerKey10, headerKey11, headerKey12, headerKey13, headerKey14, headerKey15) { }

        public Predefined15HeaderProcessor Do(Action<string, string, string, string, string, string, string, string, string, string, string, string, string, string, string> processorDelegate) =>
            Do(headers => processorDelegate(headers[0], headers[1], headers[2], headers[3], headers[4], headers[5], headers[6], headers[7], headers[8], headers[9], headers[10], headers[11], headers[12], headers[13], headers[14]));
    }

}
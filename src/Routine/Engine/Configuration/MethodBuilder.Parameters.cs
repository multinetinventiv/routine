using Routine.Engine.Virtual;
using System;

namespace Routine.Engine.Configuration
{
    public partial class MethodBuilder
    {
        public VirtualMethod Virtual<TParam1>(string name, Action<TParam1> body) =>
            Virtual(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Body.Set((_, parameters) =>
                {
                    body((TParam1)parameters[0]);
                    return null;
                });

        public VirtualMethod Virtual<TParam1, TReturn>(string name, Func<TParam1, TReturn> body) =>
            Virtual<TReturn>(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Body.Set((_, parameters) => body((TParam1)parameters[0]));

        public VirtualMethod Virtual<TParam1, TParam2>(string name, Action<TParam1, TParam2> body) =>
            Virtual(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Body.Set((_, parameters) =>
                {
                    body((TParam1)parameters[0], (TParam2)parameters[1]);
                    return null;
                });

        public VirtualMethod Virtual<TParam1, TParam2, TReturn>(string name, Func<TParam1, TParam2, TReturn> body) =>
            Virtual<TReturn>(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Body.Set((_, parameters) => body((TParam1)parameters[0], (TParam2)parameters[1]));

        public VirtualMethod Virtual<TParam1, TParam2, TParam3>(string name, Action<TParam1, TParam2, TParam3> body) =>
            Virtual(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Body.Set((_, parameters) =>
                {
                    body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2]);
                    return null;
                });

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TReturn>(string name, Func<TParam1, TParam2, TParam3, TReturn> body) =>
            Virtual<TReturn>(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Body.Set((_, parameters) => body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2]));

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4>(string name, Action<TParam1, TParam2, TParam3, TParam4> body) =>
            Virtual(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Body.Set((_, parameters) =>
                {
                    body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3]);
                    return null;
                });

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TReturn>(string name, Func<TParam1, TParam2, TParam3, TParam4, TReturn> body) =>
            Virtual<TReturn>(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Body.Set((_, parameters) => body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3]));

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5>(string name, Action<TParam1, TParam2, TParam3, TParam4, TParam5> body) =>
            Virtual(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Body.Set((_, parameters) =>
                {
                    body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4]);
                    return null;
                });

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TReturn>(string name, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TReturn> body) =>
            Virtual<TReturn>(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Body.Set((_, parameters) => body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4]));

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(string name, Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> body) =>
            Virtual(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[5]))
                .Body.Set((_, parameters) =>
                {
                    body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4], (TParam6)parameters[5]);
                    return null;
                });

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TReturn>(string name, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TReturn> body) =>
            Virtual<TReturn>(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[5]))
                .Body.Set((_, parameters) => body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4], (TParam6)parameters[5]));

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>(string name, Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7> body) =>
            Virtual(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[5]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[6]))
                .Body.Set((_, parameters) =>
                {
                    body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4], (TParam6)parameters[5], (TParam7)parameters[6]);
                    return null;
                });

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TReturn>(string name, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TReturn> body) =>
            Virtual<TReturn>(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[5]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[6]))
                .Body.Set((_, parameters) => body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4], (TParam6)parameters[5], (TParam7)parameters[6]))
            ;


        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8>(string name, Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8> body) =>
            Virtual(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[5]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[6]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[7]))
                .Body.Set((_, parameters) =>
                {
                    body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4], (TParam6)parameters[5], (TParam7)parameters[6], (TParam8)parameters[7]);
                    return null;
                })
            ;

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TReturn>(string name, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TReturn> body) =>
            Virtual<TReturn>(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[5]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[6]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[7]))
                .Body.Set((_, parameters) => body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4], (TParam6)parameters[5], (TParam7)parameters[6], (TParam8)parameters[7]));

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9>(string name, Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9> body) =>
            Virtual(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[5]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[6]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[7]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[8]))
                .Body.Set((_, parameters) =>
                {
                    body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4], (TParam6)parameters[5], (TParam7)parameters[6], (TParam8)parameters[7], (TParam9)parameters[8]);
                    return null;
                });

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TReturn>(string name, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TReturn> body) =>
            Virtual<TReturn>(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[5]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[6]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[7]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[8]))
                .Body.Set((_, parameters) => body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4], (TParam6)parameters[5], (TParam7)parameters[6], (TParam8)parameters[7], (TParam9)parameters[8]));

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10>(string name, Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10> body) =>
            Virtual(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[5]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[6]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[7]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[8]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[9]))
                .Body.Set((_, parameters) =>
                {
                    body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4], (TParam6)parameters[5], (TParam7)parameters[6], (TParam8)parameters[7], (TParam9)parameters[8], (TParam10)parameters[9]);
                    return null;
                });

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TReturn>(string name, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TReturn> body) =>
            Virtual<TReturn>(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[5]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[6]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[7]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[8]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[9]))
                .Body.Set((_, parameters) => body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4], (TParam6)parameters[5], (TParam7)parameters[6], (TParam8)parameters[7], (TParam9)parameters[8], (TParam10)parameters[9]));

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11>(string name, Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11> body) =>
            Virtual(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[5]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[6]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[7]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[8]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[9]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[10]))
                .Body.Set((_, parameters) =>
                {
                    body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4], (TParam6)parameters[5], (TParam7)parameters[6], (TParam8)parameters[7], (TParam9)parameters[8], (TParam10)parameters[9], (TParam11)parameters[10]);
                    return null;
                });

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TReturn>(string name, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TReturn> body) =>
            Virtual<TReturn>(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[5]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[6]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[7]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[8]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[9]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[10]))
                .Body.Set((_, parameters) => body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4], (TParam6)parameters[5], (TParam7)parameters[6], (TParam8)parameters[7], (TParam9)parameters[8], (TParam10)parameters[9], (TParam11)parameters[10]));

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12>(string name, Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12> body) =>
            Virtual(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[5]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[6]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[7]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[8]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[9]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[10]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[11]))
                .Body.Set((_, parameters) =>
                {
                    body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4], (TParam6)parameters[5], (TParam7)parameters[6], (TParam8)parameters[7], (TParam9)parameters[8], (TParam10)parameters[9], (TParam11)parameters[10], (TParam12)parameters[11]);
                    return null;
                });

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TReturn>(string name, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TReturn> body) =>
            Virtual<TReturn>(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[5]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[6]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[7]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[8]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[9]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[10]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[11]))
                .Body.Set((_, parameters) => body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4], (TParam6)parameters[5], (TParam7)parameters[6], (TParam8)parameters[7], (TParam9)parameters[8], (TParam10)parameters[9], (TParam11)parameters[10], (TParam12)parameters[11]));

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13>(string name, Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13> body) =>
            Virtual(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[5]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[6]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[7]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[8]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[9]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[10]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[11]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[12]))
                .Body.Set((_, parameters) =>
                {
                    body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4], (TParam6)parameters[5], (TParam7)parameters[6], (TParam8)parameters[7], (TParam9)parameters[8], (TParam10)parameters[9], (TParam11)parameters[10], (TParam12)parameters[11], (TParam13)parameters[12]);
                    return null;
                });

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TReturn>(string name, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TReturn> body) =>
            Virtual<TReturn>(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[5]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[6]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[7]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[8]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[9]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[10]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[11]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[12]))
                .Body.Set((_, parameters) => body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4], (TParam6)parameters[5], (TParam7)parameters[6], (TParam8)parameters[7], (TParam9)parameters[8], (TParam10)parameters[9], (TParam11)parameters[10], (TParam12)parameters[11], (TParam13)parameters[12]));

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TParam14>(string name, Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TParam14> body) =>
            Virtual(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[5]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[6]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[7]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[8]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[9]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[10]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[11]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[12]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[13]))
                .Body.Set((_, parameters) =>
                {
                    body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4], (TParam6)parameters[5], (TParam7)parameters[6], (TParam8)parameters[7], (TParam9)parameters[8], (TParam10)parameters[9], (TParam11)parameters[10], (TParam12)parameters[11], (TParam13)parameters[12], (TParam14)parameters[13]);
                    return null;
                });

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TParam14, TReturn>(string name, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TParam14, TReturn> body) =>
            Virtual<TReturn>(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[5]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[6]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[7]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[8]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[9]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[10]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[11]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[12]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[13]))
                .Body.Set((_, parameters) => body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4], (TParam6)parameters[5], (TParam7)parameters[6], (TParam8)parameters[7], (TParam9)parameters[8], (TParam10)parameters[9], (TParam11)parameters[10], (TParam12)parameters[11], (TParam13)parameters[12], (TParam14)parameters[13]));

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TParam14, TParam15>(string name, Action<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TParam14, TParam15> body) =>
            Virtual(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[5]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[6]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[7]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[8]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[9]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[10]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[11]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[12]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[13]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[14]))
                .Body.Set((_, parameters) =>
                {
                    body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4], (TParam6)parameters[5], (TParam7)parameters[6], (TParam8)parameters[7], (TParam9)parameters[8], (TParam10)parameters[9], (TParam11)parameters[10], (TParam12)parameters[11], (TParam13)parameters[12], (TParam14)parameters[13], (TParam15)parameters[14]);
                    return null;
                });

        public VirtualMethod Virtual<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TParam14, TParam15, TReturn>(string name, Func<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9, TParam10, TParam11, TParam12, TParam13, TParam14, TParam15, TReturn> body) =>
            Virtual<TReturn>(name)
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[0]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[1]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[2]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[3]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[4]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[5]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[6]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[7]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[8]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[9]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[10]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[11]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[12]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[13]))
                .Parameters.Add(p => p.Virtual(body.Method.GetParameters()[14]))
                .Body.Set((_, parameters) => body((TParam1)parameters[0], (TParam2)parameters[1], (TParam3)parameters[2], (TParam4)parameters[3], (TParam5)parameters[4], (TParam6)parameters[5], (TParam7)parameters[6], (TParam8)parameters[7], (TParam9)parameters[8], (TParam10)parameters[9], (TParam11)parameters[10], (TParam12)parameters[11], (TParam13)parameters[12], (TParam14)parameters[13], (TParam15)parameters[14]));
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspectCore.DynamicProxy;

namespace WebApplication2
{
    public interface ICustomService
    {
        object Call(string str);
        object CallA(string str);
    }
    public class CustomService: ICustomService
    { 
        public object Call(string str)
        {
            return "Hello World";
        }
        public object CallA(string str)
        {
            return "Hello World";
        }
    }
}

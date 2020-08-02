using AspectCore.DynamicProxy;
using AspectCore.Injector;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication2
{
    public class LogRecordInterceptorAttribute:AbstractInterceptorAttribute
    {
        [FromContainer]
        public ILogger<LogRecordInterceptorAttribute> Logger { get; set; }
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            //Console.WriteLine("开始记录日志");
            //await next.Invoke(context);
            //Console.WriteLine("结束记录日志");

            MethodInfo method = context.ImplementationMethod;
          
            object[] p =context.Parameters;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < p.Length; i++)
            {
                sb.Append(p[i]);
            }

            string invokeMsg = $"{method.DeclaringType.Name}.{method.Name}.{sb}";
            Logger.LogTrace($"{invokeMsg}...");
            object obj = context.ReturnValue;
            await context.Invoke(next);

            Logger.LogTrace(invokeMsg);
            string str = "";
        }
    }

}

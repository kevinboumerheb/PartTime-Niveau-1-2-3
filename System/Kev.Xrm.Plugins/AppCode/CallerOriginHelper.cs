using Microsoft.Xrm.Sdk;

namespace Kev.Xrm.Plugins.AppCode
{
    internal enum CallerOrigin
    {
        Undefind = 0,
        Application = 1,
        WebService = 2,
        AsyncService = 3
    }

    internal class CallerOriginHelper
    {
        internal static CallerOrigin GetCallerOrigin(
            IPluginExecutionContext context)
        {
            if (context.GetType().Name == "SandboxPluginExecutionContext")
                return CallerOrigin.Undefind;

            object callerorigin =
                context.GetType().GetProperty("CallerOrigin").GetValue(context, null);

            switch (callerorigin.GetType().Name)
            {
                case "ApplicationOrigin":
                    return CallerOrigin.Application;
                case "AsyncServiceOrigin":
                    return CallerOrigin.AsyncService;
                case "WebServiceApiOrigin":
                    return CallerOrigin.WebService;
            }

            return CallerOrigin.Undefind;
        }
    }
}
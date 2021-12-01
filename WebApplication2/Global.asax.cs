using OpenTelemetry;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace WebApplication2
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private TracerProvider tracerProvider;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            this.tracerProvider = Sdk.CreateTracerProviderBuilder()
                .AddAspNetInstrumentation((options) => options.Enrich
                    = (activity, eventName, rawObject) =>
                {
                    if (eventName.Equals("OnStartActivity"))
                    {
                        if (rawObject is HttpRequest httpRequest)
                        {
                            activity.SetTag("physicalPath", httpRequest.PhysicalPath);
                        }
                    }
                    else if (eventName.Equals("OnStopActivity"))
                    {
                        if (rawObject is HttpResponse httpResponse)
                        {
                            activity.SetTag("responseType", httpResponse.ContentType);
                        }
                    }
                })
                    .AddJaegerExporter()
                .Build();
        }


        protected void Application_End()
        {
            this.tracerProvider?.Dispose();
        }
    }
}

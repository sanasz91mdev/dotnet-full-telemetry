using Newtonsoft.Json;
using OpenTelemetry;
using OpenTelemetry.Context;
using OpenTelemetry.Trace;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace WebApplication2.Controllers
{
    public class ValuesController : ApiController
    {
        //set a valid instrumentation source
        ActivitySource activitySource = new ActivitySource(
"OpenTelemetry.Instrumentation.AspNet.Telemetry",
"1.0.0.0");
        // GET api/values
        public IHttpActionResult Get()
        {
            var s1 = new OpenTelemetry.Context.Propagation.TraceContextPropagator().Fields;
            var current = Tracer.CurrentSpan;

            string s3 = current.Context.TraceId.ToString();
            string s2 = current.Context.SpanId.ToString();

            var parentContext = new ActivityContext(
                        ActivityTraceId.CreateFromString(s3.AsSpan()),
                        ActivitySpanId.CreateFromString(s2.AsSpan()),
                        ActivityTraceFlags.Recorded);

            var activity = activitySource.StartActivity("Test span", ActivityKind.Server, parentContext);
            // var activity = activitySource.StartActivity("Test span"); //Use if want to start span irrespective of parent
            activity?.SetTag("http.method", "GET");
            if (activity != null && activity.IsAllDataRequested == true)
            {
                activity.SetTag("http.url", "/api/values");
            }


            activity?.SetTag("otel.status_code", "ERROR");
            activity?.SetTag("otel.status_description", "error status description");

            activity?.Stop();


            var s21 = activity?.Duration;

            var dict = new Dictionary<string, string>
            {
                { "name", "Foobar" },
                { "url", "admin@foobar.com" }
            };

            var json = new JsonResult()
            {
                Data = dict
            };


            return Ok(json);
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }



    public class UserModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Company { get; set; }



        /// <summary>  
        /// Get the Users  
        /// </summary>  
        /// <returns></returns>  
        public List<UserModel> GetUsers()
        {
            var usersList = new List<UserModel>
            {
                new UserModel
                {
                    UserId = 1,
                    UserName = "Ram",
                    Company = "Mindfire Solutions"
                },
                new UserModel
                {
                    UserId = 1,
                    UserName = "chand",
                    Company = "Mindfire Solutions"
                },
                new UserModel
                {
                    UserId = 1,
                    UserName = "Abc",
                    Company = "Abc Solutions"
                }
            };

            return usersList;
        }
    }


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace CountingKs.Services
{
    public class CountingKsControllerSelector : DefaultHttpControllerSelector
    {
        private readonly HttpConfiguration _configuration;

        public CountingKsControllerSelector(HttpConfiguration configuration) 
            : base(configuration)
        {
            _configuration = configuration;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            var controllers = GetControllerMapping();

            var routeData = request.GetRouteData();

            var controllerName = (string)routeData.Values["controller"];

            if (controllers.TryGetValue(controllerName, out var descriptor))
            {
                //var version = GetVersionFromQueryString(request);
                //var version = GetVersionFromHeader(request);
                //var version = GetVersionFromAcceptHeader(request);
                var version = GetVersionFromMediaType(request);

                var newName = $"{controllerName}V{version}";

                if (controllers.TryGetValue(newName, out var versionedDescriptor))
                {
                    return versionedDescriptor;
                }

                return descriptor;
            }

            return null;
        }

        private string GetVersionFromMediaType(HttpRequestMessage request)
        {
            var accept = request.Headers.Accept;
            var ex = new Regex(@"application\/vnd\.countingks\.([a-z]+)\.v(\d+)\+json", RegexOptions.IgnoreCase);

            foreach (var mime in accept)
            {
                var match = ex.Match(mime.MediaType);
                if (match.Success)
                {
                    return match.Groups[2].Value;
                }
            }
            return "1";
        }

        private string GetVersionFromAcceptHeader(HttpRequestMessage request)
        {
            var accept = request.Headers.Accept;

            foreach (var mime in accept)
            {
                if (mime.MediaType == "application/json")
                {
                    var value = mime.Parameters
                        .FirstOrDefault(p => p.Name.Equals("version", StringComparison.OrdinalIgnoreCase));

                    if (value != null)
                    {
                        return value.Value;
                    }
                }
            }

            return "1";
        }

        private string GetVersionFromHeader(HttpRequestMessage request)
        {
            const string HEADER_NAME = "X-CountingKs-Version";

            if (request.Headers.Contains(HEADER_NAME))
            {
                var header = request.Headers.GetValues(HEADER_NAME).FirstOrDefault();
                if (header != null)
                {
                    return header;
                }
            }

            return "1";
        }

        private string GetVersionFromQueryString(HttpRequestMessage request)
        {
            var query = request.RequestUri.ParseQueryString();
            var version = query["v"] ?? "1";
            return version;
        }
    }
}
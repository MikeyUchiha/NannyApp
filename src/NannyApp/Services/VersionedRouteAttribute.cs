using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace NannyApp.Services
{
    public class VersionedRouteAttribute : RouteAttribute
    {
        public VersionedRouteAttribute(string template, int minVersion) : base(template)
        {
            MinVersion = minVersion;
        }

        public VersionedRouteAttribute(string template) : this(template, 1)
        {

        }

        public VersionedRouteAttribute(string template, int minVersion, int maxVersion) : this(template, minVersion)
        {
            MaxVersion = maxVersion;
        }

        public int MinVersion { get; set; }
        public int? MaxVersion { get; set; }
    }
}

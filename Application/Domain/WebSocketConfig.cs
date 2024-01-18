using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class WebSocketConfig
    {
        public string? BaseUri { get; set; }
        public Dictionary<string, string>? Services { get; set; }

        private Uri CompletedUri { get; set; }

        public WebSocketConfig()
        {
            //var servicesPath =  string.Join("/", Services?.Select(s => s.Value));         
            //CompletedUri = new Uri($"{BaseUri}{servicesPath}");
        }
        public Uri GetCompletedUri()
        {
            var servicesPath = string.Join("/", Services?.Select(s => s.Value));
            CompletedUri = new Uri($"{BaseUri}{servicesPath}");
            return CompletedUri;
        }
    }
}

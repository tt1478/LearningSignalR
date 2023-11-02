using Microsoft.AspNetCore.TestHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRServerUnitTest
{
    public class UnitTestHostServiceModel
    {
        public TestServer TestServer { get; set; }
        public string AccessToken { get; set; }
    }
}

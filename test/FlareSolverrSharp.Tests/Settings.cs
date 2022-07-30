﻿using System;

namespace FlareSolverrSharp.Tests
{
    internal static class Settings
    {
        internal const string FlareSolverrApiUrl = "http://localhost:8191/";
        internal const string ProxyUrl = "http://127.0.0.1:8888/";
        internal static readonly Uri ProtectedUri = new Uri("https://nowsecure.nl");
        internal static readonly Uri ProtectedDdgUri = new Uri("https://anidex.info/");
        internal static readonly Uri ProtectedCcfUri = new Uri("https://www.muziekfabriek.org");

        /*
        To configure TinyProxy in local:
           * sudo vim /etc/tinyproxy/tinyproxy.conf
              * edit => LogFile "/tmp/tinyproxy.log"
              * edit => Syslog Off
           * sudo tinyproxy -d
           * sudo tail -f /tmp/tinyproxy.log
        */
    }
}

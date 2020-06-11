using System;
using System.Collections.Generic;
using System.Text;

namespace AwsCSLibrary
{
    public class DBRecord // dynamoDB record
    {
        public string Id { get; set; }
        public string Source { get; set; }
        public string Target { get; set; }
    }
}

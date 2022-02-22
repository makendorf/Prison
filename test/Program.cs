using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace test
{
    class Program
    {
        
        static void Main(string[] args)
        {
            var SC = new ServiceController("CaptureService_a44a2");

            string ComputerName = "localhost";
            ManagementScope Scope;

            if (!ComputerName.Equals("localhost", StringComparison.OrdinalIgnoreCase))
            {
                ConnectionOptions Conn = new ConnectionOptions();
                Conn.Username = "";
                Conn.Password = "";
                Conn.Authority = "ntlmdomain:DOMAIN";
                Scope = new ManagementScope(String.Format("\\\\{0}\\root\\CIMV2", ComputerName), Conn);
            }
            else
                Scope = new ManagementScope(String.Format("\\\\{0}\\root\\CIMV2", ComputerName), null);

            Scope.Connect();
            ObjectQuery Query = new ObjectQuery("SELECT * FROM Win32_Service");
            ManagementObjectSearcher Searcher = new ManagementObjectSearcher(Scope, Query);

            foreach (ManagementObject WmiObject in Searcher.Get())
            {
                if((string)WmiObject["Name"] == "CaptureService_a44a2")
                {
                    Console.WriteLine("{0,-35} {1,-40}", "Name", WmiObject["Name"]);// String
                    Console.WriteLine("{0,-35} {1,-40}", "PathName", WmiObject["PathName"]);// String

                    var fullPath = ((string)WmiObject["PathName"]).Split(@"\".ToCharArray());
                    var name = fullPath[fullPath.Length-1];
                    Console.WriteLine("{0,-35} {1,-40}", "Name", name);
                }
            }
        }
    }
}

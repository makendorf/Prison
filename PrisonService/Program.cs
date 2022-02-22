using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace PrisonService
{
    public class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>

        public static Service1 service1 { get; set; } = new Service1();
        static void Main()
        {
            try
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                    service1
                };
                ServiceBase.Run(ServicesToRun);
            }
            catch (Exception ex)
            {
                using (StreamWriter q = new StreamWriter("D:\\Записки с зоны.txt", true))
                {
                    q.WriteLine(ex);
                }
            }

            
        }
    }
}

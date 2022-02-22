using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace PrisonService
{
    [RunInstaller(true)]
    public partial class Installer1 : Installer
    {
        readonly ServiceInstaller serviceInstaller;
        readonly ServiceProcessInstaller processInstaller;
        public Installer1()
        {
            InitializeComponent();
            serviceInstaller = new ServiceInstaller
            {
                // Режим запуска (в данном случае Manual (вручную)).
                // Если служба не является драйвером устройства допустимы только значения Manual, Authomatic (автоматический запуск при загрузке системы) и Disabled (отключено).
                StartType = ServiceStartMode.Automatic,
                // Имя службы (должно совпадать с именем класса службы).
                ServiceName = "Prison_Service",
                // Отображаемое имя службы. Под ним служба будет отображаться в различных утилитах для работы со службами Windows.
                // Это необязательные параметр. При его отсутствии будет отображаться ServiceName.
                DisplayName = "_Prison"
            };
            processInstaller = new ServiceProcessInstaller
            {
                // Учётная запись для службы
                Account = ServiceAccount.LocalSystem
            };
            
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}

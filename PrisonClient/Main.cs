using ENet.Managed;
using MetroFramework.Forms;
using Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PL
{
    public partial class Main : MetroForm
    {
        public PrisonClient client;
        public BindingList<ClientID> ShopListData = new BindingList<ClientID>();
        public BindingList<ServiceInfo> ServiceListData = new BindingList<ServiceInfo>();
        Autorization autorizationForm;
        public bool debug = false;
        public Main()
        {
            InitializeComponent();
            
        }

        private void Main_Load(object sender, EventArgs e)
        {
            Log.OnLog += OnLogMessage;
            Connect();
            
            Invoke(new Action(() =>
            {
                ShopListBox.DisplayMember = "DisplayName";
                ShopListBox.ValueMember = "ID";
                ShopListBox.DataSource = ShopListData;
            }));
            Invoke(new Action(() =>
            {
                ServicesListBox.DisplayMember = "Name";
                ServicesListBox.ValueMember = "Status";
                ServicesListBox.DataSource = ServiceListData;
            }));

        }
        private void Connect()
        {
            client = new PrisonClient("127.0.0.1", 2307);
            ClientCnf();
            Task.Factory.StartNew(() =>
            {
                try
                {
                    client.Connect();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.StackTrace);
                }
            });
        }
        private void ClientCnf()
        {
            client.ID.Name = Dns.GetHostName() + "/" + Environment.UserName;
            client.ID.Type = ClientType.Client;
            client.OnConnected += Connected;
            client.OnDisconnect += Disconnected;
            client.OnReceive += Receive;
        }
        private void OnLogMessage(LogLevel Level, string Message)
        {
            Invoke(new Action(() =>
            {
                LogListBox.Items.Add(Message);
                LogListBox.SelectedIndex = LogListBox.Items.Count - 1;
            }));
        }

        private void Receive(NetworkPayload Packet)
        {
            Log.Info($"Тип данных: {Packet.Type} Источник: ");
            switch (Packet.Type)
            {
                case PacketType.ClientID:
                    {
                        var ClID = (ClientID)NetworkSerialization.Deserialize(Packet.Data);
                        client.ID.GUID = ClID.ToString();
                        Log.Success($"Успешная регистрация в системе");
                        Log.Info($"GUID: {client.ID.GUID}, {client.ID.Name}");
                        if (debug)
                        {
                            Invoke(new Action(() =>
                            {
                                Log.Warning("Авторизация пользователя");
                                autorizationForm = new Autorization(client);
                                if (autorizationForm.ShowDialog() != DialogResult.OK)
                                {
                                    Application.Exit();
                                }
                            }));
                            if (autorizationForm.Auth)
                            {
                                RequestShopList();
                            }
                        }
                        else
                        {
                            RequestShopList();
                        }
                        break;
                    }
                case PacketType.RequestUserList:
                    {
                        var userList = (UserList[])NetworkSerialization.Deserialize(Packet.Data);
                        autorizationForm.Username = userList;
                        break;
                    }
                case PacketType.ShopConnectionList:
                    {
                        var ShopList = (BindingList<ClientID>)NetworkSerialization.Deserialize(Packet.Data);
                        if (ShopListData.Count == 0)
                        {
                            for (int i = 0; i < ShopList.Count; i++)
                            {
                                ShopListData.Add(ShopList[i]);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < ShopListData.Count; i++)
                            {
                                ShopListData[i] = ShopList[i];
                            }
                        }
                        ShopListBox.SelectedIndex = 0;
                        break;
                    }
                case PacketType.ServiceStatus:
                    {
                        var svc = (ServiceInfo)NetworkSerialization.Deserialize(Packet.Data);
                        for(int i = 0; i < ShopListData.Count; i++)
                        {
                            if(ShopListData[i].Services != null)
                            {
                                for (int j = 0; j < ShopListData[i].Services.Length; j++)
                                {
                                    if (ShopListData[i].Services[j].Name == svc.Name)
                                    {
                                        ShopListData[i].Services[j].Status = svc.Status;
                                    }
                                }
                            }
                           
                        }
                        break;
                    }
            }
        }
        
        private void Disconnected()
        {
            Log.Warning("Клиент отключен");
        }
        private void SendID()
        {
            NetworkPayload payload = new NetworkPayload(PacketType.ClientID, NetworkSerialization.Serialize(client.ID));
            client.Send(payload);
        }
        private void RequestShopList()
        {
            NetworkPayload payload = new NetworkPayload(PacketType.ShopConnectionList);
            client.Send(payload);
        }
        private void Connected()
        {
            SendID();
        }

        private void ShopListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Invoke(new Action(() =>
            {
                var serviceData = new BindingList<ServiceInfo>(ShopListData[ShopListBox.SelectedIndex].Services);
                if (ServiceListData.Count == 0)
                {
                    for (int i = 0; i < serviceData.Count; i++)
                    {
                        ServiceListData.Add(serviceData[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < ServiceListData.Count; i++)
                    {
                        ServiceListData[i] = serviceData[i];
                    }
                }
            }));
        }

        private void ServicesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var serviceInfo = ServiceListData[ServicesListBox.SelectedIndex];
            ServiceNameLabel.Text = "Имя службы: " + serviceInfo.Name;
            ServiceStatusLabel.Text = "Статус службы: " + serviceInfo.Status.ToString();
            ServiceAutoStartCheckBox.CheckedChanged -= ServiceAutoStartComboBox_CheckedChanged;
            ServiceAutoStartCheckBox.Checked = serviceInfo.AutoStart;
            ServiceAutoStartCheckBox.CheckedChanged += ServiceAutoStartComboBox_CheckedChanged;
            ServiceCheckIntervalTextBox.Text = serviceInfo.CheckInterval.ToString();
            ServiceCheckIntervalTextBox.Enabled = serviceInfo.AutoStart;
            
            switch ((ServiceControllerStatus)ServicesListBox.SelectedValue)
            {
                case ServiceControllerStatus.Running:
                    {
                        ServiceStartButton.Enabled = false;
                        ServiceStopButton.Enabled = true;
                        ServiceRestartButton.Enabled = true;
                        break;
                    }
                case ServiceControllerStatus.Stopped:
                    {
                        ServiceStartButton.Enabled = true;
                        ServiceStopButton.Enabled = false;
                        ServiceRestartButton.Enabled = true;
                        break;
                    }
                default:
                    {
                        ServiceStartButton.Enabled = false;
                        ServiceStopButton.Enabled = false;
                        ServiceRestartButton.Enabled = true;
                        break;
                    }
            }
            

        }

        private void ServiceAutoStartComboBox_CheckedChanged(object sender, EventArgs e)
        {
            if(ShopListData[ShopListBox.SelectedIndex].Services[ServicesListBox.SelectedIndex].AutoStart != ServiceAutoStartCheckBox.Checked)
            {
                ShopListData[ShopListBox.SelectedIndex].Services[ServicesListBox.SelectedIndex].AutoStart = ServiceAutoStartCheckBox.Checked;
                SendChangeService();
            }
        }
        private void SendChangeService()
        {
            var clID = ShopListData[ShopListBox.SelectedIndex];
            
            NetworkPayload payload = new NetworkPayload()
            {
                Data = NetworkSerialization.Serialize(clID.Services[ServicesListBox.SelectedIndex]),
                Receiver = clID.GUID,
                Type = PacketType.ChangeServiceAutoStart
            };
            client.Send(payload);
            

        }

        private void ServiceCheckIntervalTextBox_TextChanged(object sender, EventArgs e)
        {
            ShopListData[ShopListBox.SelectedIndex].Services[ServicesListBox.SelectedIndex].CheckInterval = Convert.ToInt64(ServiceCheckIntervalTextBox.Text);
            SendChangeService();
        }

        private void ServiceStartButton_Click(object sender, EventArgs e)
        {
            var clID = ShopListData[ShopListBox.SelectedIndex];
            NetworkPayload payload = new NetworkPayload()
            {
                Data = NetworkSerialization.Serialize(clID.Services[ServicesListBox.SelectedIndex].Name),
                Receiver = clID.GUID,
                Type = PacketType.StartService
            };
            client.Send(payload);
        }

        private void ServiceStopButton_Click(object sender, EventArgs e)
        {
            var clID = ShopListData[ShopListBox.SelectedIndex];
            NetworkPayload payload = new NetworkPayload()
            {
                Data = NetworkSerialization.Serialize(clID.Services[ServicesListBox.SelectedIndex].Name),
                Receiver = clID.GUID,
                Type = PacketType.StopService
            };
            client.Send(payload);
        }

        private void ServiceRestartButton_Click(object sender, EventArgs e)
        {
            var clID = ShopListData[ShopListBox.SelectedIndex];
            NetworkPayload payload = new NetworkPayload()
            {
                Data = NetworkSerialization.Serialize(clID.Services[ServicesListBox.SelectedIndex].Name),
                Receiver = clID.GUID,
                Type = PacketType.RestartService
            };
            client.Send(payload);
        }

        private void UpdatePanel()
        {
            switch (tabControl1.SelectedIndex)
            {
                case 1:
                    {
                        PuttyForm puttyForm = new PuttyForm()
                        {
                            Dock = DockStyle.Fill,
                            TopLevel = false,
                            FormBorderStyle = FormBorderStyle.None
                        };
                        tabControl1.TabPages[1].Controls.Add(puttyForm);
                        puttyForm.Show();
                        break;
                    }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdatePanel();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    client.Disconnect();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.StackTrace);
                }
                finally
                {
                    client = null;
                    Environment.Exit(0);
                }
            });
            
        }
    }
}

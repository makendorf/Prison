using ENet.Managed;
using Network;
using System;
//using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using WPFToggleSwitch;
using System.Windows.Threading;
using System.Linq;
using System.Diagnostics;
using System.Threading;

namespace PrisonClient
{
    public struct IsChecked
    {
        public bool Check { get; set; }
        private string path;
        public string Path
        {
            get => path.ToString();
            set
            {
                if (Check == true)
                {
                    path = PrisonClient.Properties.Resources.watchServOn.ToString();
                    path = Environment.CurrentDirectory + "/data/image/button/watchServOn.png";
                    //path = "pack://application:,,,/Resources/watchServOn.png";
                }
                else
                {
                    path = Environment.CurrentDirectory + "/data/image/button/watchServOff.png";
                    //path = "pack://application:,,,/Resources/watchServOff.png";
                }
            }
        }
    }
    public struct ListItem
    {
        public string Text { get; set; }
        public bool IsEnable { get; set; }
        public IsChecked IsChecked { get; set; }
        public override string ToString()
        {
            return Text;
        }
    }
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public ObservableCollection<ListItem> source = new ObservableCollection<ListItem>();
        public ENetClient client = new ENetClient(8);
        public ClientID[] clients;
        public Window window = Application.Current.MainWindow;
        public ServiceInfo[] serviceInfos;
        public ToggleSwitch ServiceSwitch = new ToggleSwitch();
        public Button ServiceButtonWatch = new Button();
        public MainWindow()
        {
            this.DataContext = this;
            InitializeComponent();
            //this.DataContext = new Data();
            window = Application.Current.MainWindow;
            serviceListBox.Dispatcher.Invoke(() => { serviceListBox.ItemsSource = source; });
            Log.OnLog += OnLogMessage;
            client.ID.Name = System.Net.Dns.GetHostName() + "/" + Environment.UserName;
            client.ID.Type = ClientType.Client;
            client.OnConnect += Connected;
            client.OnDisconnect += Disconnected;
            client.OnReceive += Receive;
            Task.Factory.StartNew(() =>
            {
                client.Connect("31.210.218.202", 2307);
            });
           
        }

        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as ScrollViewer).ScrollToBottom();
        }
        private void OnLogMessage(LogLevel Level, string Message) =>
            //logListBox.Items.Add(Message);
            Dispatcher.Invoke(new Action(() =>
            {
                ListBoxItem item = new ListBoxItem
                {
                    FontSize = 15,
                    Content = Message,
                    Foreground = ColoredListBoxItem.GetLevelColor(Level),
                    Height = 15
                };
                logListBox.Items.Add(item);
                logListBox.ScrollIntoView(item);
            }));
        private void Receive(int Channel, NetworkPayload Packet, ENetPeer Sender)
        {
            Log.Info($"Тип данных: {Packet.Type} Размер данных: {Packet.Data.Length}");
            switch (Packet.Type)
            {
                case PacketType.ConnectionList:
                    {
                        clients = (ClientID[])NetworkSerialization.Deserialize(Packet.Data);
                        foreach (var cl in clients)
                        {
                            Log.Info("Name: {0}, ID: {1} Type: {2} ({3})", cl.Name, cl.ID.ToString(), cl.Type, cl.ID.ToString() == client.ID.ToString() ? "Self" : "");
                        }
                        break;
                    }
                case PacketType.ShopConnectionList:
                    {
                        clients = (ClientID[])NetworkSerialization.Deserialize(Packet.Data);
                        Dispatcher.Invoke(new Action(() =>
                            {
                                int SelectIndex = shopListBox.SelectedIndex;
                                shopListBox.Items.Clear();
                                foreach (var cli in clients)
                                {
                                    shopListBox.Items.Add(cli.Name);
                                }
                                shopListBox.SelectedIndex = SelectIndex;
                            }));
                        break;
                    }
                case PacketType.Message:
                    {
                        Message msg = (Message)NetworkSerialization.Deserialize(Packet.Data);
                        Log.Info($"{msg.Name}: {msg.Text}");
                        break;
                    }
                case PacketType.ClientID:
                    {
                        var ClID = (ClientID)NetworkSerialization.Deserialize(Packet.Data);
                        client.ID.ID = ClID.ToString();
                        Log.Success($"Успешная регистрация в системе");
                        Log.Info($"GUID: {client.ID.ID}, {client.ID.Name}");
                        break;
                    }
                case PacketType.ServiceStatus:
                    {
                        ServiceInfo Service = (ServiceInfo)NetworkSerialization.Deserialize(Packet.Data);
                        serviceListBox.Dispatcher.Invoke(() =>
                        {

                            for (int i = 0; i < source.Count; i++)
                            {
                                if (source[i].Text == Service.Name && Packet.Sender == clients[shopListBox.SelectedIndex].ID)
                                {
                                    Log.Info("Статус службы {1}: {0}", Service.Status, Service.Name);
                                    ListItem item = source[i];
                                    var _item = item.IsChecked;
                                    _item.Check = Service.AutoStart;
                                    _item.Path = "";
                                    item.IsChecked = _item;
                                    ServiceSwitch.IsEnabled = true;
                                    Log.Info("Путь {1}: {0}", source[i].Text, source[i].IsChecked.Path);
                                    switch (Service.Status)
                                    {

                                        case ServiceControllerStatus.Running:
                                            {
                                                item.IsEnable = true;
                                                ServiceSwitch.IsEnabled = true;
                                                break;
                                            }
                                        case ServiceControllerStatus.Stopped:
                                            {
                                                item.IsEnable = false;
                                                ServiceSwitch.IsEnabled = true;
                                                break;
                                            }
                                        default:
                                            item.IsEnable = false;
                                            break;

                                    }
                                    source[i] = item;
                                }
                            }
                        });

                        break;
                    }
                case PacketType.ServiceListGet:
                    {
                        Log.Success("ПЕТУХИ ПРИЛЕТЕЛИ ЬЫЪ");
                        serviceInfos = (ServiceInfo[])NetworkSerialization.Deserialize(Packet.Data);
                        serviceListBox.Dispatcher.Invoke(() =>
                        {
                            source.Clear();
                            foreach (var srv in serviceInfos)
                            {
                                Log.Info("Статус службы {1}: {0}", srv.Status, srv.Name);
                                ListItem item = new ListItem
                                {
                                    Text = srv.Name,
                                    IsEnable = srv.Status == ServiceControllerStatus.Running
                                };

                                var _item = item.IsChecked;
                                _item.Check = srv.AutoStart;
                                _item.Path = "";
                                item.IsChecked = _item;
                                source.Add(item);

                                //serviceListBox.Dispatcher.Invoke(() => 
                                //serviceListBox.Items.Add(new ListItem() { IsChecked = srv.Status == ServiceControllerStatus.Running ? true : false , Text = srv.Name}));
                            }
                        });
                        break;
                    }
                case PacketType.ServiceAutoStart:
                    {
                        ServiceInfo Service = (ServiceInfo)NetworkSerialization.Deserialize(Packet.Data);
                        serviceListBox.Dispatcher.Invoke(() =>
                        {

                            for (int i = 0; i < source.Count; i++)
                            {
                                if (source[i].Text == Service.Name && Packet.Sender == clients[shopListBox.SelectedIndex].ID)
                                {
                                    Log.Info("Автостарт");
                                    Log.Info("Статус службы {1}: {0}", Service.Status, Service.AutoStart);
                                    var _item = source[i];
                                    var __item = _item.IsChecked;
                                    __item.Check = Service.AutoStart;
                                    __item.Path = "";
                                    _item.IsChecked = __item;
                                    source[i] = _item;
                                }
                            }
                        });
                        break;
                    }
            }
        }
        public void GetServiceList()
        {
            Log.Info($"Запрос списка служб от {shopListBox.Items[shopListBox.SelectedIndex]}");
            NetworkPayload payload = new NetworkPayload(PacketType.ServiceListGet, new byte[0], client.ID.ID, clients[shopListBox.SelectedIndex].ID);
            client.Send(payload);
        }
        private void SendID()
        {
            NetworkPayload payload = new NetworkPayload(PacketType.ClientID, NetworkSerialization.Serialize(client.ID));
            client.Send(payload);
        }
        private void RequestShopList()
        {
            NetworkPayload payload = new NetworkPayload(PacketType.ShopConnectionList, new byte[0]);
            client.Send(payload);
        }

        private void Disconnected()
        {
            Log.Info("Соединение с сервером разорвано");
        }

        private void Connected()
        {
            SendID();
            RequestShopList();
            //FillShopList();
           
            Log.Success("Соединение с сервером установлено");
        }


        private void ShopListBox_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            try
            {
                source.Clear();
                GetServiceList();
            }
            catch (Exception)
            {
                MessageBox.Show("Магазин не в сети");
            }
        }
        private void ToggleSwitch_Click(object sender, RoutedEventArgs e)
        {
            ServiceSwitch = (ToggleSwitch)sender;
            switch (ServiceSwitch.IsChecked)
            {
                case true:
                    {
                        ServiceSwitch.IsEnabled = false;
                        NetworkPayload payload = new NetworkPayload(PacketType.ServiceStart, NetworkSerialization.Serialize(ServiceSwitch.Tag.ToString()), client.ID.ToString(), clients[shopListBox.SelectedIndex].ID);
                        client.Send(payload);
                        Log.Warning($"Запускаем службу {((ToggleSwitch)sender).Tag}");
                        break;
                    }
                case false:
                    {
                        ServiceSwitch.IsEnabled = false;
                        NetworkPayload payload = new NetworkPayload(PacketType.ServiceStop, NetworkSerialization.Serialize(ServiceSwitch.Tag.ToString()), client.ID.ToString(), clients[shopListBox.SelectedIndex].ID);
                        client.Send(payload);
                        Log.Warning($"Останавливаем службу {ServiceSwitch.Tag}");
                        break;
                    }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach (var srv in serviceInfos)
            {
                if (srv.Name == ((Button)sender).Tag.ToString())
                {
                    srv.AutoStart = !srv.AutoStart;
                    NetworkPayload payload = new NetworkPayload(PacketType.ServiceAutoStart, NetworkSerialization.Serialize(srv), client.ID.ToString(), clients[shopListBox.SelectedIndex].ID);
                    client.Send(payload);
                    break;
                }
            }
        }
        private void connect(object ip)
        {
            ProcessStartInfo cmd = new ProcessStartInfo
            {
                FileName = @"C:\Program Files\PuTTY\putty.exe",
                UseShellExecute = false,
                RedirectStandardInput = false,
                RedirectStandardOutput = true,
                Arguments = $"-ssh root@{ip} -pw xxxxxx"
            };
            using (Process process = Process.Start(cmd))
            {
                process.WaitForExit();
            }
        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            Thread thread = new Thread(connect);
            thread.Start("95.71.124.94 221");
           
            //Message message = new Message(client.ID.Name, textBoxMessage.Text);
            //NetworkPayload payload = new NetworkPayload(PacketType.Message, NetworkSerialization.Serialize(message), client.ID.ToString(), clients[shopListBox.SelectedIndex].ID);
            //client.Send(payload);
        }
    }
}

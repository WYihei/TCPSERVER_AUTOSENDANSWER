using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using Newtonsoft.Json;
using PropertyChanged;
using SuperSimpleTcp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Threading;

namespace tcp_auto
{
    public enum MessageDirection
    {
        from,
        to
    }
    public class CommunicationLog
    {
        public string Message { get; set; }
        public string RelatedIPPort { get; set; }
        public DateTime Time { get; set; }
        public MessageDirection Direction { get; set; }
    }

    [AddINotifyPropertyChangedInterface]
    public class MainWindowViewModel : ObservableObject
    {
        //每次软件打开   自动获取本机ipaddress example——"10.198.75.60"
        public static string _ipaddress = null;

        public static string ipaddress()
        {
            if (_ipaddress != null)
            {
                return _ipaddress;
            }
            else
            {
                IPAddress[] AddressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList;
                for (int i = 0; i < AddressList.Length; i++)
                {
                    IPAddress _IPAddress = AddressList[i];

                    if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                    {
                        _ipaddress = _IPAddress.ToString();
                        return _ipaddress;
                    }
                }
                return _ipaddress;
            }
        }




        private ObservableCollection<EchoKeyValuePair> _dataDridKVs;

        public ObservableCollection<string> LocalIps { get; set; } = new ObservableCollection<string>();
        public string SelectedIp { get; set; }
        public int PortNum { get; set; }
        public ObservableCollection<CommunicationLog> DataLog { get; set; } = new ObservableCollection<CommunicationLog>();
        public string SendMessage { get; set; }
        public ObservableCollection<string> ListheningClients { get; set; } = new ObservableCollection<string>();

        public MainWindowViewModel()
        {
			initialSoftware();
		}
		SimpleTcpServer server;
		public ICommand StartKeyValueCommand { get => new RelayCommand(() =>
        {
            if (server?.IsListening ?? false)
            {
                server.Stop();
            }
            else
            {
                string serverAddress = SelectedIp.ToString() + ":" + this.PortNum.ToString();
                server = new SimpleTcpServer(serverAddress);
                server.Events.DataReceived += Events_DataReceived;
                server.Events.ClientConnected += ClientConnected;
                server.Events.ClientDisconnected += ClientDisConnected;
                server.Start();
            }

        });
        }

        private void ClientDisConnected(object? sender, ConnectionEventArgs e)
        {

            App.Current.Dispatcher.Invoke(() => ListheningClients.Remove(e.IpPort));
        }

        private void ClientConnected(object? sender, ConnectionEventArgs e)
        {
            App.Current.Dispatcher.Invoke(()=> ListheningClients.Add(e.IpPort));
        }
        public string SelectedClient { get; set; }
        public ICommand SendDataCommand { get => new RelayCommand(() =>
        {
            if (server == null)
            {
                MessageBox.Show("请先连接客户端再发送数据");
                return;
            }
            var allClients = server.GetClients();
            List<string> clientsList = allClients.ToList();
            var first = clientsList[0];
            if (SendMessage.Length > 0 && SelectedClient != null)
            {
                SendWithLog(SelectedClient, SendMessage);
            }
        });
        }
        public bool AlwaysScrollToEnd { get; set; }

        public void readPort()
		{
			if (!File.Exists(@"IpPort.txt"))
			{
				PortNum = 8888;
				return;
			}
			else
			{
				string[] defLines = File.ReadAllLines(@"IpPort.txt");
				for (int i = 0; i < defLines.Length; i++)
				{
					string item = defLines[i];
					string[] values = item.Split(',');
					PortNum = Convert.ToInt32(values[1]);
				}
			}
		}


        
        public int DelayTaskTime { get; set; } = 0;
		private void Events_DataReceived(object? sender, DataReceivedEventArgs e)
		{
            var clientData = e.Data;
			var byteArray = clientData.ToArray();
			string getContent = System.Text.Encoding.ASCII.GetString(byteArray);

            CommunicationLog newItem = new CommunicationLog()
            {
                RelatedIPPort = e.IpPort,
                Message = System.Text.Encoding.ASCII.GetString(byteArray),
                Direction = MessageDirection.from,
                Time = DateTime.Now
            };
            App.Current.Dispatcher.Invoke(()=> DataLog.Add(newItem));
            var allReply = DataDridKVs.Where(a => a.Key == getContent && a.IsEnable).ToList();
				if (allReply.Count > 0)
				//if (dictionary.ContainsKey(getContent))
				{
                foreach (var item in allReply)
                {
					string serverSend = item.Value;
						Thread.Sleep(DelayTaskTime);
                    SendWithLog(e.IpPort, serverSend);
				}
				}
		}
        void SendWithLog(string targetIPPort,string message)
        {
            server.Send(targetIPPort, message);
            CommunicationLog newItem = new CommunicationLog()
            {
                RelatedIPPort = targetIPPort,
                Message= message,
                Direction=MessageDirection.to,
                Time = DateTime.Now
            };
            App.Current.Dispatcher.Invoke(() => DataLog.Add(newItem));
        }
		private void initialSoftware()
		{
            foreach (var item in Config.Instance.EchoKeyValuePairs)
            {
                DataDridKVs.Add(item);
            }
            comboDataShow();
            string[] lastIpPort = Config.Instance.LastIpPort.Split(',');
            if (lastIpPort.Length == 2)
            {
                SelectedIp = lastIpPort[0];
                PortNum = Convert.ToInt32(lastIpPort[1]);
            }
        }



        public void comboDataShow()
        {
            IPAddress[] ip = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (var item in ip)
            {
                if (item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    LocalIps.Add(item.ToString());
                }
            }
            LocalIps.Add("127.0.0.1");
            LocalIps.Add("0.0.0.0");
        }



        public ObservableCollection<EchoKeyValuePair> DataDridKVs { get; set; } = new ObservableCollection<EchoKeyValuePair>();
      

        public EchoKeyValuePair SelectedItem { get; set; }

        public bool EchoEnable { get; set; }


        public string KeyMess { get; set; }


        public string ValueMess { get; set; }


        public ICommand AddCommand
        {
            get
            {
                return new RelayCommand<object>((obj) =>
                {
                    if (!string.IsNullOrEmpty(KeyMess))
                    {
                        DataDridKVs.Add(new EchoKeyValuePair()
                        {
                            Key = KeyMess,
                            Value = ValueMess,
                            IsEnable = true
                        });
                        SaveReplyData();
					}
                    else
                    {
                        MessageBox.Show("键值为空，请输入");
                        return;
                    }
                });
            }
        }

        /// <summary>
        /// 删除键值对
        /// </summary>
        public ICommand DeleteCommand
        {
            get
            {
                return new RelayCommand<object>((obj) =>
                {

                        if (SelectedItem==null)
                        {
                            MessageBox.Show("请先选择需要删除的任务");
                            return;
                        }
                        else
                        {

                            DataDridKVs.Remove(SelectedItem);

                            SaveReplyData();

						}
                   
                });
            }
        }
        void SaveReplyData()
        {
            Config.Instance.LastIpPort = $"{SelectedIp},{PortNum}";
            Config.Instance.EchoKeyValuePairs = new List<EchoKeyValuePair>(DataDridKVs);
            Config.Instance.Save();

        }

		public ICommand ExportCommand
        {
            get
            {
                return new RelayCommand<object>((obj) =>
                {
                    //SaveFileDialog saveFileDialog = new SaveFileDialog();
                    //saveFileDialog.Filter = "键值匹配|*.txt";
                    //if (saveFileDialog.ShowDialog() == true)
                    //{
                    //    //存储的path
                    //    var filePaths = saveFileDialog.FileName;
                    //    //path和       content的全部数据                      
                    //    string allContents = "";
                    //    for (int i = 0; i < DataDridKVs.Count; i++)
                    //    {
                    //        EchoKeyValuePair item = DataDridKVs[i];
                    //        string line = "";
                    //        line += $"{item.Key},{item.Value}\n";
                    //        allContents += line;
                    //    }
                    //    File.WriteAllText(filePaths, allContents);
                    //}
                });
            }
        }

        public ICommand ImportCommand
        {
            get
            {
                return new RelayCommand<object>((obj) =>
                {
                    OpenFileDialog fileDialog = new OpenFileDialog();
                    fileDialog.Filter = "配置文件|*.config";
                    if (fileDialog.ShowDialog() == true)
                    {
                        var filePaths = fileDialog.FileName;
                        string json = File.ReadAllText(filePaths);
                        Config importedConfig = JsonConvert.DeserializeObject<Config>(json);
                        
                        if (importedConfig != null)
                        {
                            foreach (var item in importedConfig.EchoKeyValuePairs)
                            {
                                DataDridKVs.Add(item);
                            }
                        }
                        
                    }
                });
            }
        }
    }
}

using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
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
    public class MainWindowViewModel : BaseClass
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




        private ObservableCollection<MainModel> _dataDridKVs;

        private ObservableCollection<string> comboIp;

        public ObservableCollection<string> ComboIps
        {
            get { return comboIp; }
            set
            {
                comboIp = value;
                NotifyChanged();
            }
        }
        public string SelectedIp { get; set; }
        public int PortNum { get; set; }
        public string DataLog { get; set; }
		public MainWindowViewModel()
        {
            InitData();
            //index
            ipaddress();
            //readDatagrid();
            comboDataShow();
			readPort();
			readIp();
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
                server.Start();
            }

        });
        }
        public ICommand SendDataCommand { get => new RelayCommand(() =>
        {
            if (server == null)
            {
                MessageBox.Show("请先连接客户端再发送数据");
                return;
            }
            //要发送的内容     一个一个发

            //获取client的IpPort   再发送
            var allClients = server.GetClients();
            List<string> clientsList = allClients.ToList();
            var first = clientsList[0];

            server.Send(first, DataLog);

            //接收到    client发送的数据    时间显示
            //string currentTime = DateTime.Now.ToString();
        });
        }
		public void readPort()
		{
			if (!File.Exists(@"IpPort.txt"))
			{
				//不存在文件
				PortNum = 8888;

				return;
			}
			else
			{
				//读取
				//return;
				string[] defLines = File.ReadAllLines(@"IpPort.txt");
				for (int i = 0; i < defLines.Length; i++)
				{
					//split
					string item = defLines[i];
					string[] values = item.Split(',');

					//this.IpAddress.Text = values[0].ToString();
					PortNum = Convert.ToInt32(values[1]);
					//this.IpCombo.Text = _ipaddress;
					//this.IpCombo.Text = values[0].ToString();
				}
			}
		}

		public void readIp()
		{
			if (!File.Exists(@"ipComboInitial.txt"))
			{
				//不存在文件
				return;
			}
			else
			{

				string[] defLines = File.ReadAllLines(@"ipComboInitial.txt");
				for (int i = 0; i < defLines.Length; i++)
				{
					//split
					string item = defLines[i];
					//string[] values = item.Split(',');
					SelectedIp = item;
				}
			}
		}
        public int DelayTaskTime { get; set; } = 0;
		private void Events_DataReceived(object? sender, DataReceivedEventArgs e)
		{
			//check的true false
			var clientData = e.Data;
			var byteArray = clientData.ToArray();
			string getContent = System.Text.Encoding.Default.GetString(byteArray);

			//e  client的IpPort
			var clientIpPort = e.IpPort.ToString();
            string this_time_log = "";
			this_time_log = DateTime.Now.ToShortDateString()+ " " + DateTime.Now.ToLongTimeString() +":"+ DateTime.Now.Millisecond + " from " + clientIpPort + "\r\n" + getContent + "\r\n";
				//接收到    client发送的数据    时间显示

                //var aa=DataDridKVs[SelectedIndex].Use;
                var allReply = DataDridKVs.Where(a => a.DataKey == getContent && a.IsEnable).ToList();
				if (allReply.Count > 0)
				//if (dictionary.ContainsKey(getContent))
				{
                foreach (var item in allReply)
                {

					//server 发送数据   返回给client（不要忘记发送）
					string serverSend = item.DataValue;

						Thread.Sleep(DelayTaskTime);
						server.Send(clientIpPort, serverSend);
					this_time_log += DateTime.Now.ToLongDateString() + DateTime.Now.ToLongTimeString() + " to " + clientIpPort + "\r\n" + serverSend + "\r\n";

				}
				}
            DataLog = this_time_log;
		}
		private void initialSoftware()
		{

			string[] defLines = File.ReadAllLines("Data.txt");
			for (int i = 0; i < defLines.Length; i++)
			{
				//split
				string item = defLines[i];
				string[] values = item.Split("<-->");
				DataDridKVs.Add(new MainModel() { DataKey = values[0], DataValue = values[1] });
			}

			//存储     path和       content的全部数据
			//string line = $"{this.IpAddress.Text},{this.PortNum.Text}\n";
			//System.IO.File.WriteAllText(@"IpPort.txt", line);

			//再存储一个 放入集合中
			//string fromTextBox = $"{this.IpAddress.Text},{this.PortNum.Text}";
			string fromTextBox = $"{SelectedIp},{this.PortNum}";
			List<string> ipCombo = new List<string>();
			ipCombo.Add("127.0.0.1,8888");
			ipCombo.Add("0.0.0.0,8888");
			ipCombo.Add(fromTextBox);
			System.IO.File.WriteAllLines(@"IpPortCom.txt", ipCombo);
		}
		private void InitData()
        {
            DataDridKVs = new ObservableCollection<MainModel>();
            ComboIps = new ObservableCollection<string>();
            //便于查看  之后删除
            //DataDridKVs.Add(new MainModel()
            //{
            //    DataKey = "111",
            //    DataValue = "222"
            //});
        }


        public void comboDataShow()
        {

            ComboIps.Add(_ipaddress);
            ComboIps.Add("127.0.0.1");
            ComboIps.Add("0.0.0.0");
            //存储 初始化打开界面不用选   再存储一个 放入集合中
            List<string> ipComboInitial = new List<string>();
            ipComboInitial.Add(_ipaddress);
            System.IO.File.WriteAllLines(@"ipComboInitial.txt", ipComboInitial);

            //back
            //string[] allIp = File.ReadAllLines(@"IpPort.txt");
            //for (int i = 0; i < allIp.Length; i++)
            //{
            //    //split
            //    string item = allIp[i];
            //    string[] values = item.Split(',');
            //    ComboIps.Add(new ComboIp()
            //    {
            //        ComboIpAdd = values[0]
            //    });
            //}
        }

        public void readDatagrid()
		{
            if (!File.Exists("Data.txt"))
            {
                File.Create("Data.txt");

			}
            string[] defLines = File.ReadAllLines(@"Data.txt");
            for (int i = 0; i < defLines.Length; i++)
            {
                //split
                string item = defLines[i];
                string[] values = item.Split("<-->");

                if (values.Length > 1)
                {
                    DataDridKVs.Add(new MainModel()
                    {
                        DataKey = values[0],
                        DataValue = values[1],
                    });
                }
            }
        }

    




        public ObservableCollection<MainModel> DataDridKVs
        {
            //DataDridKVs.Add(new MainModel()
            //        {
            //DataKey = KeyMess,
            //            DataValue = ValueMess
            //        });

            //get { return _dataDridKVs; }
            //set
            //{
            //    _dataDridKVs = value;
            //    NotifyChanged();
            //}

            get { return _dataDridKVs; }
            set
            {
                _dataDridKVs = value;
                NotifyChanged();
            }
        }

      
       






        //index
        private int _selectedIndex = -1;

        public int SelectedIndex
        {
            get { return _selectedIndex; }
            set
            {
                _selectedIndex = value;
                NotifyChanged();
            }
        }

        //Item
        private MainModel _selectedItem;

        public MainModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                NotifyChanged();
            }
        }
        
        //是否启用
        private bool _useKV;

        public bool UseKV
        {
            get { return _useKV; }
            set
            {
                _useKV = value;
                NotifyChanged();
            }
        }


        //键
        private string _keyMess;

        public string KeyMess
        {
            get { return _keyMess; }
            set
            {
                _keyMess = value;
                NotifyChanged();
            }
        }

        //值
        private string _valueMess;

        public string ValueMess
        {
            get { return _valueMess; }
            set
            {
                _valueMess = value;
                NotifyChanged();
            }
        }


        public ICommand UpTaskCommand
        {
            get
            {
                return new RelayCommand<object>((obj) =>
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        if (SelectedIndex == -1)
                        {
                            MessageBox.Show("请先选择需要上移的任务");
                            return;
                        }
                        else if (SelectedIndex <= 0)
                        {
                            MessageBox.Show("当前任务已经是第一个任务");
                            return;
                        }

                        DataDridKVs.Move(SelectedIndex, SelectedIndex - 1);

                        //存储     path和       content的全部数据
                        string allContents = "";
                        for (int i = 0; i < DataDridKVs.Count; i++)
                        {
                            MainModel item = DataDridKVs[i];
                            string line = "";
                            line += $"{item.DataKey},{item.DataValue}\n";
                            allContents += line;
                        }

                        System.IO.File.WriteAllText(@"Data.txt", allContents);
                    });
                });
            }
        }

        //public ICommand DelayTaskCommand
        //{
        //    get
        //    {
        //        return new RelayCommand<object>((obj) =>
        //        {
        //            App.Current.Dispatcher.Invoke(() =>
        //            {
        //                //InputWindow isw = new InputWindow();
        //                //isw.Title = "给新页面命名";
        //                //isw.ShowDialog();
        //                Window newWin = new Window();
        //                //newWin.Show();// 两个窗口都可用
        //                newWin.Title = "DelayWindow";
        //                newWin.ShowDialog();

        //                //DelayWindow  delayWindow= new DelayWindow();
        //                //delayWindow.InitializeComponent();
        //                ////delayWindow

        //            });
        //        });
        //    }
        //}



        public ICommand DownTaskCommand
        {
            get
            {
                return new RelayCommand<object>((obj) =>
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        var lastIndex = DataDridKVs.Count;
                        if (SelectedIndex == -1)
                        {
                            MessageBox.Show("请先选择需要下移的任务");
                            return;
                        }
                        else if (SelectedIndex == lastIndex - 1)
                        {
                            MessageBox.Show("当前任务已经是最后一个任务");
                            return;
                        }
                        DataDridKVs.Move(SelectedIndex, SelectedIndex + 1);
                        
                        //存储     path和       content的全部数据
                        string allContents = "";
                        for (int i = 0; i < DataDridKVs.Count; i++)
                        {
                            MainModel item = DataDridKVs[i];
                            string line = "";
                            line += $"{item.DataKey},{item.DataValue}\n";
                            allContents += line;
                        }

                        System.IO.File.WriteAllText(@"Data.txt", allContents);
                    });
                });
            }
        }
        //private void DownTask()
        //{
        //    var lastIndex = DataDridKVs.Count;
        //    if (SelectedIndex == -1)
        //    {
        //        MessageBox.Show("请先选择需要操作的任务");
        //        return;
        //    }
        //    else if (SelectedIndex == lastIndex - 1)
        //    {
        //        MessageBox.Show("当前任务已经是最后一个任务");
        //        return;
        //    }
        //    DataDridKVs.Move(SelectedIndex, SelectedIndex + 1);
        //}

        /// <summary>
        /// 增加键值对
        /// </summary>
        public ICommand AddCommand
        {
            get
            {
                return new RelayCommand<object>((obj) =>
                {
                    //if (!string.IsNullOrEmpty(KeyMess) && !string.IsNullOrEmpty(ValueMess))
                    if (!string.IsNullOrEmpty(KeyMess))
                    {

                        DataDridKVs.Add(new MainModel()
                        {
                            DataKey = KeyMess,
                            DataValue = ValueMess,
                            IsEnable = true
                        });


                        //存储     path和       content的全部数据
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
			string allContents = "";
			for (int i = 0; i < DataDridKVs.Count; i++)
			{
				MainModel item = DataDridKVs[i];
				string line = "";
				line += $"{item.DataKey}<-->{item.DataValue}\n";
				allContents += line;
			}

			System.IO.File.WriteAllText(@"Data.txt", allContents);
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
                    //        MainModel item = DataDridKVs[i];
                    //        string line = "";
                    //        line += $"{item.DataKey},{item.DataValue}\n";
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
                    fileDialog.Filter = "键值匹配|*.txt";
                    if (fileDialog.ShowDialog() == true)
                    {
                        var filePaths = fileDialog.FileName;
                        string[] defLines = File.ReadAllLines(filePaths);
                        for (int i = 0; i < defLines.Length; i++)
                        {
                            //split
                            string item = defLines[i];
                            string[] values = item.Split(',');

                            DataDridKVs.Add(new MainModel()
                            {
                                DataKey = double.Parse(values[0].Replace(" ", "")).ToString(),
                                DataValue = double.Parse(values[1].Replace(" ", "")).ToString(),
                            });
                            //Messenger.Default.Send<Tuple<string, string>>(new Tuple<string, string>(KeyMess, ValueMess), "SendMess");
                        }
                        
                    }
                });
            }
        }
    }
}

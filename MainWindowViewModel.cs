using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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

        private ObservableCollection<ComboIp> comboIp;

        public ObservableCollection<ComboIp> ComboIps
        {
            get { return comboIp; }
            set
            {
                comboIp = value;
                NotifyChanged();
            }
        }

        public MainWindowViewModel()
        {
            InitData();
            //index
            ipaddress();
            readDatagrid();
            comboDataShow();
        }

        private void InitData()
        {
            DataDridKVs = new ObservableCollection<MainModel>();
            ComboIps = new ObservableCollection<ComboIp>();
            //便于查看  之后删除
            //DataDridKVs.Add(new MainModel()
            //{
            //    DataKey = "111",
            //    DataValue = "222"
            //});
        }

        Dictionary<string, string> dictionary = new Dictionary<string, string>();

        public void comboDataShow()
        {
            //之前的一种逻辑   现阶段不用
            //string[] allIp = File.ReadAllLines(@"IpPortCom.txt");
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
            ComboIps.Add(new ComboIp()
            {
                ComboIpAdd = _ipaddress
            });
            ComboIps.Add(new ComboIp()
            {
                ComboIpAdd = "127.0.0.1"
            });
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
            string[] defLines = File.ReadAllLines(@"Data.txt");
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
                        //提示弹窗即可
                        for (int i = 0; i < DataDridKVs.Count; i++)
                        {
                            //if (DataDridKVs[i].DataKey.Contains(KeyMess))
                            if (DataDridKVs[i].DataKey.Contains(KeyMess)&& DataDridKVs[i].DataKey.Length== KeyMess.Length)
                            {
                                MessageBox.Show("存在重复的键");
                                return;
                            }

                            //启用
                            //if (DataDridKVs[i].Use == true)
                            //{
                            //    MessageBox.Show("存在重复的键");
                            //    return;
                            //}

                            //else
                            //{
                            //    DataDridKVs.Add(new MainModel()
                            //    {
                            //        DataKey = KeyMess,
                            //        DataValue = ValueMess
                            //    });
                            //}
                        }

                        //if (DataDridKVs.Contains(KeyMess))
                        //{
                        //    MessageBox.Show("存在重复的键，请删除");
                        //}

                        DataDridKVs.Add(new MainModel()
                        {
                            //Use = true,
                            DataKey = KeyMess,
                            DataValue = ValueMess
                        });

                        Messenger.Default.Send<Tuple<string, string>>(new Tuple<string, string>(KeyMess, ValueMess), "SendMess");

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

                        //随后删除
                        //for (int i = 0; i < DataDridKVs.Count; i++)
                        //{
                        //    if (DataDridKVs[i].DataKey.Contains(KeyMess))
                        //    {
                        //        return;
                        //    }
                        //    else
                        //    {
                        //        DataDridKVs.Add(new MainModel()
                        //        {
                        //            DataKey = KeyMess,
                        //            DataValue = ValueMess
                        //        });
                        //    }
                        //}

                        //DataDridKVs.Add(new MainModel()
                        //{
                        //    DataKey = KeyMess,
                        //    DataValue = ValueMess
                        //});

                        //if (DataDridKVs.Count > 0)
                        //{
                        //    for (int i = 0; i < DataDridKVs.Count; i++)
                        //    {
                        //        //if (!DataDridKVs[i].DataKey.Contains(KeyMess))
                        //        if (DataDridKVs[i].DataKey.Contains(KeyMess))
                        //        {
                        //            return;
                        //        }
                        //        else
                        //        {
                        //            DataDridKVs.Add(new MainModel()
                        //            {
                        //                DataKey = KeyMess,
                        //                DataValue = ValueMess
                        //            });
                        //        }
                        //    }        
                        //    Messenger.Default.Send<Tuple<string, string>>(new Tuple<string, string>(KeyMess, ValueMess), "SendMess");
                        //}
                        ////datagrid为空时   直接添加
                        //else
                        //{
                        //    DataDridKVs.Add(new MainModel()
                        //    {
                        //        DataKey = KeyMess,
                        //        DataValue = ValueMess
                        //    });
                        //    Messenger.Default.Send<Tuple<string, string>>(new Tuple<string, string>(KeyMess, ValueMess), "SendMess");
                        //}
                    }
                    else
                    {
                        MessageBox.Show("键值为空，请输入");
                        return;
                    }

                    ////Messenger Send
                    //Messenger.Default.Send<Tuple<string, string>>(new Tuple<string, string>(KeyMess, ValueMess), "SendMess");
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
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        //if (SelectedItem is MainModel mainModel)
                        //{
                        //    //Messenger Send
                        //    Messenger.Default.Send<MainModel>(mainModel, "RemoveMess");
                        //}
                        //DataDridKVs.RemoveAt(SelectedIndex);

                        //datagrid不选择时候 不报错
                        if (SelectedItem==null)
                        {
                            MessageBox.Show("请先选择需要删除的任务");
                            return;
                        }
                        else
                        {
                            if (SelectedItem is MainModel mainModel)
                            {
                                //Messenger Send
                                Messenger.Default.Send<MainModel>(mainModel, "RemoveMess");
                            }
                            DataDridKVs.RemoveAt(SelectedIndex);



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
                        }
                    });
                });
            }
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

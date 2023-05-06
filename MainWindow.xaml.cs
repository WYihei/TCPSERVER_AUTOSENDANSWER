using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using SuperSimpleTcp;
//using SimpleTCP;

namespace tcp_auto
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel();
            //数据收 发
            Messenger.Default.Register<Tuple<string, string>>(this, "SendMess", GetSendMess);
            Messenger.Default.Register<MainModel>(this, "RemoveMess", GetRemoveMess);
            readIpPort();
        }

        public void readIpPort()
        {
            if (!File.Exists(@"IpPort.txt"))
            {
                //不存在文件
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

                    this.IpAddress.Text = values[0].ToString();
                    this.PortNum.Text = values[1].ToString();
                }
            }
        }

        SimpleTcpServer server;
        //连接web
        private void LinkCommand(object sender, RoutedEventArgs e)
        {
            this.Resources["serverStr"] = new TextBlock() { Text = "关闭SERVER" };
            //一打开 就开启server
            //测试的时候先写死   之后变成灵活输入端    192.168.10.23:8080 ip4
            //string serverAddress = "10.198.75.60:8080";
            //string serverAddress = "192.168.10.23:8080";
            string serverAddress = this.IpAddress.Text.ToString() + ":" + this.PortNum.Text.ToString();
            server = new SimpleTcpServer(serverAddress);
            server.Start();
            //bool flag = true;
            //if (flag)
            //{
            //    this.linkButton.Background = new SolidColorBrush(Colors.LightGray);
            //}
            MessageBox.Show("已连接client，client可以发送消息");
            //client发送的数据    server数据接收区得到      server数据发送区发送
            server.Events.DataReceived += Events_DataReceived;

            string[] defLines = File.ReadAllLines(@"Data.txt");
            for (int i = 0; i < defLines.Length; i++)
            {
                //split
                string item = defLines[i];
                string[] values = item.Split(',');
                dictionary.Add(values[0], values[1]);
            }

            //存储     path和       content的全部数据
            string line = $"{this.IpAddress.Text},{this.PortNum.Text}\n";
            System.IO.File.WriteAllText(@"IpPort.txt", line);
        }

        //private void AgainCommand(object sender, RoutedEventArgs e)
        //{
        //    this.Resources["serverStr"] = new TextBlock() { Text = "关闭SERVER" };
        //    string serverAddress = this.IpAddress.Text.ToString() + ":" + this.PortNum.Text.ToString();
        //    server = new SimpleTcpServer(serverAddress);
        //    server.Stop();

        //    bool flag = true;
        //    if (flag)
        //    {
        //        this.linkButton.Background = new SolidColorBrush(Colors.LightGray);
        //    }
        //    MessageBox.Show("已关闭Server");
        //}

        

        //private bool Use = true;
        //MainModel mainM = new MainModel();
        //bool sss = mainM.Use;
        

        //接收到    client发送的数据
        private void Events_DataReceived(object? sender, DataReceivedEventArgs e)
        {
            //check的true false
            var clientData = e.Data;
            var byteArray = clientData.ToArray();
            string getContent = System.Text.Encoding.Default.GetString(byteArray);

            //e  client的IpPort
            var clientIpPort = e.IpPort.ToString();

            Dispatcher.Invoke(() =>
            {
                //接收到    client发送的数据    时间显示
                string currentTime = DateTime.Now.ToString();
                //var aa=DataDridKVs[SelectedIndex].Use;

                //if (dictionary.ContainsKey(getContent)&& sss is true)
                if (dictionary.ContainsKey(getContent))
                {
                    ////SolidBrush sb = new SolidBrush(colorDialog.Color);
                    ////SolidColorBrush solidColorBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(sb.Color.A, sb.Color.R, sb.Color.G, sb.Color.B));
                    ////DataShow.Foreground = solidColorBrush;//改变字体颜色

                    ////UI界面上显示      
                    ////back  DataShow.Text += currentTime+ "  " + clientIpPort+ "\r\n" + "ACPT " + "\r\n"+getContent + "\r\n"+ "SEND " + "\r\n"+dictionary[getContent] + "\r\n";
                    //DataShow.Text += currentTime+ "  " + clientIpPort+ "\r\n" + "ACPT " + "\r\n"+getContent + "\r\n"+ "SEND " + "\r\n"+dictionary[getContent] + "\r\n";

                    ////server传送出
                    //string serverSend = dictionary[getContent];
                    ////server 发送数据   返回给client
                    //server.Send(clientIpPort, serverSend);







                    FlowDocument mcFlowDoc = new FlowDocument();
                    // Create a paragraph with text  
                    Paragraph para = new Paragraph();
                    //para.Inlines.Add(new Run(currentTime +"  " + clientIpPort + "\r\n"));                   
                    //para.Inlines.Add(new Bold(new Run("ACPT" + "\r\n")));
                    para.Inlines.Add(new Run(currentTime + "  " + clientIpPort + "  " + "ACPT" + "\r\n") { Foreground = Brushes.LightGray });
                    para.Inlines.Add(new Run(getContent + "\r\n") { Foreground = Brushes.Blue });
                    para.Inlines.Add(new Run(currentTime + "  " + clientIpPort + "  " + "SEND" + "\r\n") { Foreground = Brushes.LightGray });
                    //para.Inlines.Add(new Bold(new Run("SEND" + "\r\n")));
                    para.Inlines.Add(new Run(dictionary[getContent] + "\r\n") { Foreground = Brushes.Green });
                    // Add the paragraph to blocks of paragraph  
                    ParaText.Blocks.Add(para);
                }
                else
                {
                    //back     DataShow.DataContext += currentTime + "  " + clientIpPort + "\r\n" + "ACPT " + "\r\n" + getContent + "\r\n";                 
                    //var textGreen = new SolidColorBrush(Colors.Green);
                    //DataShow.Foreground = textGreen;//改变字体颜色 

                    //DataGet.Text = getContent + "  IpPort是:" + clientIpPort;
                    //DataSend.Text = "该键没有对应值";
                    
                    FlowDocument mcFlowDoc = new FlowDocument();
                    // Create a paragraph with text  
                    Paragraph para = new Paragraph();
                    para.Inlines.Add(new Run(currentTime + "  " + clientIpPort + "  " + "ACPT" + "\r\n") { Foreground = Brushes.LightGray });
                    //para.Inlines.Add(new Run(currentTime + "  " + clientIpPort + "\r\n") { Foreground = Brushes.Blue });
                    //para.Inlines.Add(new Bold(new Run("ACPT" + "\r\n")));
                    para.Inlines.Add(new Run(getContent + "\r\n") { Foreground = Brushes.Blue });    
                    // Add the paragraph to blocks of paragraph  
                    ParaText.Blocks.Add(para);
                    return;
                }
            });
        }

        //键值对 输入特定的键  输出特定的值
        Dictionary<string, string> dictionary = new Dictionary<string, string>();

        private void GetSendMess(Tuple<string, string> tuple)
        {
            //back
            dictionary.Add(tuple.Item1, tuple.Item2);

            //如果key重复的问题
            //if (dictionary.Keys.Count > 0)
            //{
            //    for (int i = 0; i < dictionary.Keys.Count; i++)
            //    {
            //        if (dictionary.Keys.Contains(tuple.Item1))
            //        {
            //            //MessageBox.Show("存在相同的键，请检查");
            //            //不添加
            //            return;
            //        }
            //        else
            //        {
            //            dictionary.Add(tuple.Item1, tuple.Item2);
            //        }
            //    }
            //}
            ////datagrid为空时   直接添加
            //else
            //{
            //    dictionary.Add(tuple.Item1, tuple.Item2);
            //}
        }

        private void GetRemoveMess(MainModel mainModel)
        {
            if (dictionary.Keys.Count > 0)
            {
                for (int i = 0; i < dictionary.Keys.Count; i++)
                {
                    if (dictionary.Keys.Contains(mainModel.DataKey))
                    {
                        dictionary.Remove(mainModel.DataKey);
                    }
                }
            }
        }


        //back
        //private void Events_DataReceived(object? sender, DataReceivedEventArgs e)
        //{
        //    //接收到    client发送的数据
        //    var clientData = e.Data;
        //    var byteArray = clientData.ToArray();
        //    string getContent = System.Text.Encoding.Default.GetString(byteArray);
        //    //e已经变成了server的     DataReceived
        //    var clientIpPort = e.IpPort.ToString();
        //    List<string> CmbBind = new List<string>();//数据源
        //    CmbBind.Add("111");
        //    CmbBind.Add("222");
        //    CmbBind.Add("333");
        //    Dispatcher.Invoke(() =>
        //    {
        //        if (CmbBind.Contains(getContent))
        //        {
        //            if (getContent=="111")
        //            {
        //                //UI界面上显示
        //                DataGet.Text = getContent;
        //                DataSend.Text = getContent + "把get数据转成send数据";                     
        //                string serverSend = getContent + "把get数据转成send数据";
        //                //server 发送数据  
        //                string serverToClient = "192.168.10.23:8080";
        //                SimpleTcpServer server = new SimpleTcpServer(serverToClient);
        //                server.Send(clientIpPort, serverSend);
        //            }
        //        }
        //        else
        //        {
        //            return;
        //        }
        //    });
        //    //back   一部分
        //    //Dispatcher.Invoke(() =>
        //    //{
        //    //    DataGet.Text = getContent;
        //    //    DataSend.Text = getContent + "把get数据转成send数据";
        //    //    string serverGet = this.DataGet.Text;
        //    //    string serverSend = serverGet + "把get数据转成send数据";
        //    //    this.DataSend.Text = serverSend;
        //    //    //server 发送数据  
        //    //    string serverToClient = "192.168.10.23:8080";
        //    //    SimpleTcpServer server = new SimpleTcpServer(serverToClient);
        //    //    server.Send(clientIpPort, serverSend);
        //    //});
        //    //back   分成两部分
        //    //Dispatcher.Invoke(() => 
        //    //{ 
        //    //    DataGet.Text = getContent; 
        //    //});
        //    //Dispatcher.Invoke(() =>
        //    //{
        //    //    DataSend.Text = getContent + "把get数据转成send数据";
        //    //    string serverGet = this.DataGet.Text;
        //    //    string serverSend = serverGet + "把get数据转成send数据";
        //    //    this.DataSend.Text = serverSend;
        //    //    //server 发送数据  
        //    //    string serverToClient = "192.168.10.23:8080";
        //    //    SimpleTcpServer server = new SimpleTcpServer(serverToClient);
        //    //    server.Send(clientIpPort, serverSend);
        //    //});
        //}

        private void Send_Data(object sender, RoutedEventArgs e)
        {
            //string serverGet = this.DataGet.Text;
            //string serverSend = serverGet + "把get数据转成send数据";
            //this.DataSend.Text = serverSend;
            ////server 发送数据  
            //string serverToClient = "192.168.10.23:8080";
            //SimpleTcpServer server = new SimpleTcpServer(serverToClient);
            //server.Send("clientIpPort", serverSend);
            ////server.Send(clientIpPort, serverSend);
            //MessageBox.Show("导出数据");
        }



        bool firstClick = true;
        private void One_Click(object sender, RoutedEventArgs e)
        {
            if (firstClick)
            {
                this.Resources["serverStr"] = new TextBlock() { Text = "已开启SERVER" };
                string serverAddress = this.IpAddress.Text.ToString() + ":" + this.PortNum.Text.ToString();
                server = new SimpleTcpServer(serverAddress);
                server.Start();

                MessageBox.Show("已连接client，client可以发送消息");
                //client发送的数据    server数据接收区得到      server数据发送区发送
                server.Events.DataReceived += Events_DataReceived;

                string[] defLines = File.ReadAllLines(@"Data.txt");
                for (int i = 0; i < defLines.Length; i++)
                {
                    //split
                    string item = defLines[i];
                    string[] values = item.Split(',');
                    dictionary.Add(values[0], values[1]);
                }

                //存储     path和       content的全部数据
                string line = $"{this.IpAddress.Text},{this.PortNum.Text}\n";
                System.IO.File.WriteAllText(@"IpPort.txt", line);

                firstClick = false;
       
            }
            else
            {
                this.Resources["serverStr"] = new TextBlock() { Text = "已关闭SERVER" };
                //Dispatcher.Invoke(() =>
                //{
                //    server.Stop();
                //});
          
                this.linkButton.Background = new SolidColorBrush(Colors.LightGray);
       
                MessageBox.Show("已关闭Server");
                return;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        }
        SimpleTcpServer server;
        //连接web
        private void LinkCommand(object sender, RoutedEventArgs e)
        {
            this.Resources["serverStr"] = new TextBlock() { Text = "SERVER已开启" };
            //一打开 就开启server
            //测试的时候先写死   之后变成灵活输入端    192.168.10.23:8080 ip4
            //string serverAddress = "10.198.75.60:8080";
            //string serverAddress = "192.168.10.23:8080";
            string serverAddress = this.IpAddress.Text.ToString() + ":" + this.PortNum.Text.ToString();
            server = new SimpleTcpServer(serverAddress);
            server.Start();
            bool flag = true;
            if (flag)
            {
                this.linkButton.Background = new SolidColorBrush(Colors.LightGray);
            }
            MessageBox.Show("已连接client，client可以发送消息");
            //client发送的数据    server数据接收区得到      server数据发送区发送
            server.Events.DataReceived += Events_DataReceived;           
            
                     
        }
        //接收到    client发送的数据
        private void Events_DataReceived(object? sender, DataReceivedEventArgs e)
        {
            
            var clientData = e.Data;
            var byteArray = clientData.ToArray();
            string getContent = System.Text.Encoding.Default.GetString(byteArray);

            //e  client的IpPort
            var clientIpPort = e.IpPort.ToString();

            Dispatcher.Invoke(() =>
            {
                if (dictionary.ContainsKey(getContent))
                {
                    //UI界面上显示
                    DataGet.Text = getContent;
                    DataSend.Text = dictionary[getContent];
                    //server传送出
                    string serverSend = dictionary[getContent];

                    //server 发送数据   返回给client
                    //string serverToClient = "192.168.10.23:8080";
                    //SimpleTcpServer server = new SimpleTcpServer(serverToClient);
                    server.Send(clientIpPort, serverSend);
                }
                else
                { 
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
    }
}
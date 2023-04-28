using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace tcp_auto
{
    public class MainWindowViewModel : BaseClass
    {
        private ObservableCollection<MainModel> _dataDridKVs;

        public MainWindowViewModel()
        {
            InitData();
        }

        private void InitData()
        {
            DataDridKVs = new ObservableCollection<MainModel>();
            
            //DataDridKVs.Add(new MainModel()
            //{
            //    DataKey = "111",
            //    DataValue = "222"
            //});



            //便于查看  之后注
            //DataDridKVs.Add(new MainModel()
            //{
            //    DataKey = "111",
            //    DataValue = "222"
            //});
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
                            DataKey = KeyMess,
                            DataValue = ValueMess
                        });

                        Messenger.Default.Send<Tuple<string, string>>(new Tuple<string, string>(KeyMess, ValueMess), "SendMess");

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
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "键值匹配|*.txt";
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        //存储的path
                        var filePaths = saveFileDialog.FileName;
                        //path和       content的全部数据                      
                        string allContents = "";
                        for (int i = 0; i < DataDridKVs.Count; i++)
                        {
                            MainModel item = DataDridKVs[i];
                            string line = "";
                            line += $"{item.DataKey},{item.DataValue}\n";
                            allContents += line;
                        }
                        File.WriteAllText(filePaths, allContents);
                    }
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

                        }
                    }
                });
            }
        }
    }
}

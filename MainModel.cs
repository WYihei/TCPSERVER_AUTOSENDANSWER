using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace tcp_auto
{
    public class MainModel : BaseClass
    {
        private string _dataKey;

        public string DataKey
        {
            get { return _dataKey; }
            set
            {
                _dataKey = value;
                NotifyChanged();
            }
        }

        private string _dataValue;

        public string DataValue
        {
            get { return _dataValue; }
            set
            {
                _dataValue = value;
                NotifyChanged();
            }
        }

        //是否禁用
        //private bool _use;
        //private bool _use;

        //public bool Use
        //{
        //    get { return _use; }
        //    set
        //    {
        //        _use = value;
        //        NotifyChanged();
        //    }
        //}
    }
}

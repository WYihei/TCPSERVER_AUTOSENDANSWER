using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace tcp_auto
{
    [AddINotifyPropertyChangedInterface]
    public class MainModel
    {
        public string DataKey { get; set; }
        public string DataValue { get; set; }
        public bool IsEnable { get; set; }
    }
}

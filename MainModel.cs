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
    public class EchoKeyValuePair
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public bool IsEnable { get; set; }
    }
}

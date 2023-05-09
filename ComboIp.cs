using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tcp_auto
{
    public class ComboIp : BaseClass
    {
        private string comboIpAdd;

        public string ComboIpAdd
        {
            get { return comboIpAdd; }
            set
            {
                comboIpAdd = value;
                NotifyChanged();
            }
        }
    }
}

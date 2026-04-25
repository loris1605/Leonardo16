using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SysNet
{
    public class StringPropertyMD : FrameworkPropertyMetadata
    {
        public StringPropertyMD() : base(string.Empty)
        {
            BindsTwoWayByDefault = true;
            DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }

        public StringPropertyMD(string defaultvalue) : base(defaultvalue)
        {
            BindsTwoWayByDefault = true;
            DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }

        public StringPropertyMD(PropertyChangedCallback handler) : base(handler)
        {
            DefaultValue = string.Empty;
            BindsTwoWayByDefault = true;
            DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }

        public StringPropertyMD(string defaultvalue, PropertyChangedCallback handler) : base(defaultvalue, handler)
        {
            BindsTwoWayByDefault = true;
            DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }
    }
}

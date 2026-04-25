using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SysNet
{
    public class DateTimePropertyMD : FrameworkPropertyMetadata
    {
        public DateTimePropertyMD() : base(0)
        {
            BindsTwoWayByDefault = true;
            DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }
        public DateTimePropertyMD(DateTime defaultvalue) : base(defaultvalue)
        {
            BindsTwoWayByDefault = true;
            DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }

        public DateTimePropertyMD(PropertyChangedCallback handler) : base(handler)
        {
            DefaultValue = DateTime.Now;
            BindsTwoWayByDefault = true;
            DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }

        public DateTimePropertyMD(DateTime defaultvalue, PropertyChangedCallback handler) : base(defaultvalue, handler)
        {
            BindsTwoWayByDefault = true;
            DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }


    }
}

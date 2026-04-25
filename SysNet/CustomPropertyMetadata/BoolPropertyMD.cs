using System.Windows;

namespace SysNet
{
    public class BoolPropertyMD : FrameworkPropertyMetadata 
    {
        public BoolPropertyMD() : base(false)
        {
            BindsTwoWayByDefault = true;
            DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }

        public BoolPropertyMD(bool defaultvalue) : base(defaultvalue)
        {
            BindsTwoWayByDefault = true;
            DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }

        public BoolPropertyMD(PropertyChangedCallback handler) : base(handler)
        {
            DefaultValue = false;
            BindsTwoWayByDefault = true;
            DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }

        public BoolPropertyMD(bool defaultvalue, PropertyChangedCallback handler) : base(defaultvalue,handler)
        {
            BindsTwoWayByDefault = true;
            DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }
    }

    


}

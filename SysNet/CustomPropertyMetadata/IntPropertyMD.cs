using System.Windows;

namespace SysNet
{
    public class IntPropertyMD : FrameworkPropertyMetadata
    {
        public IntPropertyMD() : base(0)
        {
            BindsTwoWayByDefault = true;
            DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }
        public IntPropertyMD(int defaultvalue) : base(defaultvalue)
        {
            BindsTwoWayByDefault = true;
            DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }

        public IntPropertyMD(PropertyChangedCallback handler) : base(handler)
        {
            DefaultValue = 0;
            BindsTwoWayByDefault = true;
            DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }

        public IntPropertyMD(int defaultvalue, PropertyChangedCallback handler) : base(defaultvalue, handler)
        {
            BindsTwoWayByDefault = true;
            DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }


    }
}

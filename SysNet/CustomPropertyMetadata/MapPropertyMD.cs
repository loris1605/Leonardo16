using System.Windows;

namespace SysNet
{
    public class MapPropertyMD<T> : FrameworkPropertyMetadata where T : class
    {
        public MapPropertyMD() : base(Create<T>.Instance())
        {
            BindsTwoWayByDefault = true;
            DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }

        

        public MapPropertyMD(T defaultvalue, PropertyChangedCallback handler) : base(defaultvalue, handler)
        {
            BindsTwoWayByDefault = true;
            DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }

        public MapPropertyMD(PropertyChangedCallback handler) : base(handler)
        {
            DefaultValue = Create<T>.Instance();
            BindsTwoWayByDefault = true;
            DefaultUpdateSourceTrigger = System.Windows.Data.UpdateSourceTrigger.PropertyChanged;
        }

        
    }
}

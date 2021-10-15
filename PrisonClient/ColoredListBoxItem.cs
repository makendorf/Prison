using Network;
using System.Windows.Media;
//using System.Drawing;

namespace PrisonClient
{
    class ColoredListBoxItem
    {
        public static Brush GetLevelColor(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.None:
                    return Brushes.Black;
                case LogLevel.Info:
                    return Brushes.White;
                case LogLevel.Success:
                    return Brushes.Lime;
                case LogLevel.Warning:
                    return Brushes.Yellow;
                case LogLevel.Error:
                    return Brushes.Red;
            }
            return Brushes.Black;
        }
        public string Value = "";
        public Brush Color = Brushes.Black;
        public override string ToString() { return Value; }
    }
}

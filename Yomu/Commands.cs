using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Yomu
{
    public static class Commands
    {
        public static RoutedCommand OpenArchive = new RoutedCommand();
        public static RoutedCommand OpenFolder = new RoutedCommand();
        public static RoutedCommand Quit = new RoutedCommand();
        public static RoutedCommand Config = new RoutedCommand();
        public static RoutedCommand Maximize = new RoutedCommand();
        public static RoutedCommand Minimize = new RoutedCommand();
    }
}

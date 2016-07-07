using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace UWP_Translator.Classes
{
    [Bindable]
    public class MainPageUI
    {
        private static string _TextMessageBox { get; set; }
        private static string _TextContentBox { get; set; }

        public static event EventHandler TextMessageBoxChanged;
        public static event EventHandler TextContentBoxChanged;

        public static string TextMessageBox
        {
            get { return _TextMessageBox; }
            set
            {
                if (value != _TextMessageBox)
                {
                    _TextMessageBox = value;

                    if (TextMessageBoxChanged != null)
                    {
                        TextMessageBoxChanged(null, EventArgs.Empty);
                    }

                }
            }
        }

        public static string TextContentBox
        {
            get { return _TextContentBox; }
            set
            {
                if (value != _TextContentBox)
                {
                    _TextContentBox = value;

                    if (TextContentBoxChanged != null)
                    {
                        TextContentBoxChanged(null, EventArgs.Empty);
                    }
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace UWP_Translator
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        public static bool cleanupDataBeforePrinting = true; //{ get; set; }
        public static bool translateToMITRE = true; //{ get; set; }
        public static bool translateToFRIES = true; //{ get; set; }
        public static bool translateToPitt = true; //{ get; set; }
        public static bool optimizeTranslations = false; //{ get; set; }
        public static bool hidePreviews = false; //{ get; set; }
        public static bool removePittEntriesWithMultiWordElementNames = true; //{ get; set; }
        public static bool removePittEntriesWithNoRegulator = true; //{ get; set; }
        public static int defaultDisplayFormatAfterTranslation = 2; //{ get; set; } // 0 = FRIES, 1 = MITRE, 2 = Pitt
        public static bool mitreFileNameMatchesFolder = true; //{ get; set; }
        public static string mitreSaveFileBaseName = "mitre"; //{ get; set; }
        public static int maxChars = 2000;
        public static bool testModeEnabled = true; //{ get; set; }

        public static string fileContents { get; set; }

        public static myTextBoxes MyTextBoxes = new myTextBoxes();

        public class myTextBoxes : INotifyPropertyChanged
        {
            //public myTextBoxes MyTextBoxes = new myTextBoxes();

            private string nextButtonText;
            private string _TextMessageBox;
            private string _TextContentBox;

            public event PropertyChangedEventHandler PropertyChanged = delegate { };

            public void HostViewModel()
            {
                this.NextButtonText = "Next";
            }

            public string NextButtonText
            {
                get { return this.nextButtonText; }
                set
                {
                    this.nextButtonText = value;
                    this.OnPropertyChanged();
                }
            }

            public string TextMessageBox
            {
                get { return this._TextMessageBox; }
                set
                {
                    this._TextMessageBox = value;
                    this.OnPropertyChanged();
                }
            }

            public string TextContentBox
            {
                get { return this._TextContentBox; }
                set
                {
                    this._TextContentBox = value;
                    this.OnPropertyChanged();
                }
            }

            public void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                // Raise the PropertyChanged event, passing the name of the property whose value has changed.
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }




        //private static string _TextMessageBox { get; set; }
        //private static string _TextContentBox { get; set; }

        //public static event EventHandler TextMessageBoxChanged;
        //public static event EventHandler TextContentBoxChanged;

        //public static string TextMessageBox
        //{
        //    get { return _TextMessageBox; }
        //    set
        //    {
        //        if (value != _TextMessageBox)
        //        {
        //            _TextMessageBox = value;

        //            if (TextMessageBoxChanged != null)
        //            {
        //                TextMessageBoxChanged(null, EventArgs.Empty);
        //            }

        //        }
        //    }
        //}

        //public static string TextContentBox
        //{
        //    get { return _TextContentBox; }
        //    set
        //    {
        //        if (value != _TextContentBox)
        //        {
        //            _TextContentBox = value;

        //            if (TextContentBoxChanged != null)
        //            {
        //                TextContentBoxChanged(null, EventArgs.Empty);
        //            }
        //        }
        //    }
        //}


        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
//#if DEBUG
//            if (System.Diagnostics.Debugger.IsAttached)
//            {
//                this.DebugSettings.EnableFrameRateCounter = true;
//            }
//#endif
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}

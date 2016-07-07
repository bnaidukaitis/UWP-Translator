using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP_Translator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(SettingsPage_Loaded);
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs args)
        {
            spChkbxCleanupBeforePrinting.IsChecked = App.cleanupDataBeforePrinting;
            spChkbxTranslateMITRE.IsChecked = App.translateToMITRE;
            spChkbxTranslateFRIES.IsChecked = App.translateToFRIES;
            spChkbxTranslatePitt.IsChecked = App.translateToPitt;
            spChkbxTranslatePitt.IsEnabled = false;
            spChkbxOptimizeTranslation.IsChecked = App.optimizeTranslations;
            spChkbxHidePreviews.IsChecked = App.hidePreviews;
            spChkbxRemovePittEntriesWithMultipleWords.IsChecked = App.removePittEntriesWithMultiWordElementNames;
            spChkbxRemovePittEntriesWithNoRegulator.IsChecked = App.removePittEntriesWithNoRegulator;
            spCmbbxDefaultFormatAfterTranslation.SelectedIndex = App.defaultDisplayFormatAfterTranslation; // 0 = FRIES, 1 = MITRE, 2 = Pitt
            spChkbxMITREFileNameSameAsFolder.IsChecked = App.mitreFileNameMatchesFolder;
            spTxtbxMITREBaseFileName.Text = App.mitreSaveFileBaseName;
            spChkbxTestModeEnabled.IsChecked = App.testModeEnabled;
            spChkbxTestModeEnabled.IsChecked = App.testModeEnabled;
        }


        private void spRadioButtonHome_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void spChkbxTranslateMITRE_Checked(object sender, RoutedEventArgs e)
        {
            App.translateToMITRE = true;
        }

        private void spChkbxTranslateMITRE_Unchecked(object sender, RoutedEventArgs e)
        {
            App.translateToMITRE = false;
        }

        private void spChkbxTranslateFRIES_Checked(object sender, RoutedEventArgs e)
        {
            App.translateToFRIES = true;
        }

        private void spChkbxTranslateFRIES_Unchecked(object sender, RoutedEventArgs e)
        {
            App.translateToFRIES = false;
        }

        private void spChkbxTranslatePitt_Checked(object sender, RoutedEventArgs e)
        {
            App.translateToPitt = true;
        }

        private void spChkbxTranslatePitt_Unchecked(object sender, RoutedEventArgs e)
        {
            App.translateToPitt = false;
        }

        private void spChkbxCleanupBeforePrinting_Checked(object sender, RoutedEventArgs e)
        {
            App.cleanupDataBeforePrinting = true;
        }

        private void spChkbxCleanupBeforePrinting_Unchecked(object sender, RoutedEventArgs e)
        {
            App.cleanupDataBeforePrinting = false;
        }

        private void spChkbxHidePreviews_Checked(object sender, RoutedEventArgs e)
        {
            App.hidePreviews = true;
        }

        private void spChkbxHidePreviews_Unchecked(object sender, RoutedEventArgs e)
        {
            App.hidePreviews = false;
        }

        private void spChkbxOptimizeTranslation_Checked(object sender, RoutedEventArgs e)
        {
            App.optimizeTranslations = true;
            spChkbxHidePreviews.IsChecked = true;
        }

        private void spChkbxOptimizeTranslation_Unchecked(object sender, RoutedEventArgs e)
        {
            App.optimizeTranslations = false;
            spChkbxHidePreviews.IsChecked = false;
        }

        private void spChkbxRemovePittEntriesWithMultipleWords_Checked(object sender, RoutedEventArgs e)
        {
            App.removePittEntriesWithMultiWordElementNames = true;
        }

        private void spChkbxRemovePittEntriesWithMultipleWords_Unchecked(object sender, RoutedEventArgs e)
        {
            App.removePittEntriesWithMultiWordElementNames = false;
        }

        private void spCmbbxDefaultFormatAfterTranslation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (spCmbbxDefaultFormatAfterTranslation.SelectedIndex)
            {
                case 0:
                    App.defaultDisplayFormatAfterTranslation = 0;
                    break;
                case 1:
                    App.defaultDisplayFormatAfterTranslation = 1;
                    break;
                case 2:
                    App.defaultDisplayFormatAfterTranslation = 2;
                    break;
                case 3:
                    App.defaultDisplayFormatAfterTranslation = 3;
                    break;
            }
        }

        private void spChkbxMITREFileNameSameAsFolder_Checked(object sender, RoutedEventArgs e)
        {
            App.mitreFileNameMatchesFolder = true;
            spTxtbxMITREBaseFileName.IsEnabled = false;
        }

        private void spChkbxMITREFileNameSameAsFolder_Unchecked(object sender, RoutedEventArgs e)
        {
            App.mitreFileNameMatchesFolder = false;
            spTxtbxMITREBaseFileName.IsEnabled = true;
        }

        private void spTxtbxMITREBaseFileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            //if(spTxtbxMITREBaseFileName.Text.Contains("/") || spTxtbxMITREBaseFileName.Text.Contains("\\"))
            //{
            //    spTxtbxMITREBaseFileName.Background = new SolidColorBrush(Windows.UI.Colors.Red);
            //    return;
            //}
            if(spTxtbxMITREBaseFileName.Text.Split(new char[] { '/', '\\', '|', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '=', '+', '`', '~', '>', '<', '?', '\'', '"', ':', ';', '\n', '\r', '\t' }).Length > 1)
            {
                spTxtbxMITREBaseFileName.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                return;
            }

            else
            {
                spTxtbxMITREBaseFileName.Background = new SolidColorBrush(Windows.UI.Colors.LightGreen);
                App.mitreSaveFileBaseName = spTxtbxMITREBaseFileName.Text;
            }
        }

        private void spSplitViewToggle_Click(object sender, RoutedEventArgs e)
        {
            this.spSplitView.IsPaneOpen = this.spSplitView.IsPaneOpen ? false : true;
        }

        private void spChkbxRemovePittEntriesWithNoRegulator_Checked(object sender, RoutedEventArgs e)
        {
            App.removePittEntriesWithNoRegulator = true;
        }

        private void spChkbxRemovePittEntriesWithNoRegulator_Unchecked(object sender, RoutedEventArgs e)
        {
            App.removePittEntriesWithNoRegulator = false;
        }

        private void spChkbxTestModeEnabled_Checked(object sender, RoutedEventArgs e)
        {
            App.testModeEnabled = true;
        }

        private void spChkbxTestModeEnabled_Unchecked(object sender, RoutedEventArgs e)
        {
            App.testModeEnabled = false;
        }
    }
}

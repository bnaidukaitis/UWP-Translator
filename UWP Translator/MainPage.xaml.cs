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
using UWP_Translator.Classes;
using System.Diagnostics;
using System.Threading.Tasks;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWP_Translator
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public Stopwatch timer = new Stopwatch();

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
            this.KeyDown += new KeyEventHandler(mpContentBox_KeyUp);
            //this.TextContentBox = mpContentBox.Text;
            //this.TextMessageBox = mpMessageBox.Text;
        }

        //public string TextContentBox { get; set; }
        //public string TextMessageBox { get; set; }

        public void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            mpMessageBox.Text = "Select a file or folder for translation using the buttons below.";
            if (App.hidePreviews)
            {
                mpContentBox.Text = "(Previews disabled)";
            }
            else
            {
                mpContentBox.Text = "File contents will appear here.";
            }
            

            //Binding myBinding = new Binding();
            ////myBinding.Path = new PropertyPath(mpMessageBox.Text);
            //myBinding.Source = App.MyTextBoxes.TextMessageBox;
            //myBinding.Mode = BindingMode.TwoWay;
            //myBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //BindingOperations.SetBinding(mpMessageBox, TextBox.TextProperty, myBinding);


            //Binding contentBoxBinding = new Binding();
            //contentBoxBinding.Mode = BindingMode.TwoWay;
            //contentBoxBinding.Source = App.MyTextBoxes.TextMessageBox;
            //contentBoxBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            //BindingOperations.SetBinding(mpContentBox, TextBox.TextProperty, contentBoxBinding);
        }

        private async void mpButtonLoadFile_Click(object sender, RoutedEventArgs e)
        {
            //LoadedFileInfo.getOpenFilePath();

            var dlg = new Windows.Storage.Pickers.FileOpenPicker();
            dlg.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            dlg.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            dlg.FileTypeFilter.Add(".json");
            dlg.FileTypeFilter.Add(".txt");
            dlg.FileTypeFilter.Add(".csv");

            Windows.Storage.StorageFile file = await dlg.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                LoadedFileInfo.fileName = file.Path;
                LoadedFileInfo.file = file;
                LoadedFileInfo.folder = null;
                mpMessageBox.Text = LoadedFileInfo.fileName;
            }
            else
            {
                mpMessageBox.Text = "Folder selection canceled. Please try again.";
                return;
            }

            if (LoadedFileInfo.fileName == "" || LoadedFileInfo.file == null)
            {
                mpMessageBox.Text = "Selection canceled. Please try again.";
                return;
            }
            
            mpMessageBox.Text = LoadedFileInfo.fileName;

            timer.Start();
            LoadedFileInfo.format = await LoadedFileInfo.checkDataFormat(LoadedFileInfo.file);
            //mpContentBox.Text = await LoadedFileInfo.readTextFile(LoadedFileInfo.file);

            mpComboBoxPanel.Visibility = Visibility.Collapsed;
            mpButtonAddContextsAndEntities.Visibility = Visibility.Collapsed;

            switch (LoadedFileInfo.format)
            {
                case "MITRE":
                    LoadedFileInfo.modelDataAsMITRE = new string[1];
                    LoadedFileInfo.modelDataAsMITRE[0] = await Windows.Storage.FileIO.ReadTextAsync(LoadedFileInfo.file);
                    if (!App.hidePreviews)
                    {
                        displayPreview("MITRE");
                    }
                    timer.Stop();
                    double loadTime = timer.ElapsedMilliseconds;
                    loadTime = loadTime / 1000;
                    timer.Reset();
                    mpMessageBox.Text = "MITRE data loaded successfully in " + loadTime.ToString() + " seconds.";
                    mpButtonTranslate.Visibility = Visibility.Visible;
                    mpButtonSave.Visibility = Visibility.Collapsed;
                    mpButtonAddContextsAndEntities.Visibility = Visibility.Collapsed;
                    mpButtonPrepareToPrint.Visibility = Visibility.Collapsed;
                    break;
                case "FRIES":
                    LoadedFileInfo.modelDataAsFRIES = await LoadedFileInfo.readTextFile(LoadedFileInfo.file);
                    if (!App.hidePreviews)
                    {
                        displayPreview("FRIES");
                    }
                    //dynamic jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject(LoadedFileInfo.modelDataAsFRIES);
                    //jsonData = jsonData.frames;
                    //jsonData = jsonData[0];
                    //string jsonDataString = Newtonsoft.Json.JsonConvert.SerializeObject(jsonData, Newtonsoft.Json.Formatting.Indented);
                    //mpContentBox.Text = jsonDataString;
                    timer.Stop();
                    loadTime = timer.ElapsedMilliseconds;
                    loadTime = loadTime / 1000;
                    timer.Reset();
                    mpMessageBox.Text = "FRIES data loaded successfully in " + loadTime.ToString() + " seconds.";
                    mpButtonTranslate.Visibility = Visibility.Visible;
                    mpButtonSave.Visibility = Visibility.Collapsed;
                    mpButtonAddContextsAndEntities.Visibility = Visibility.Collapsed;
                    mpButtonPrepareToPrint.Visibility = Visibility.Collapsed;
                    break;
                case "Pitt":
                    if (!App.hidePreviews)
                    {
                        displayPreview("Pitt");
                    }
                    timer.Stop();
                    loadTime = timer.ElapsedMilliseconds;
                    loadTime = loadTime / 1000;
                    timer.Reset();
                    mpMessageBox.Text = "Pitt data loaded successfully in " + loadTime.ToString() + " seconds.";
                    mpButtonTranslate.Visibility = Visibility.Visible;
                    mpButtonSave.Visibility = Visibility.Collapsed;
                    mpButtonAddContextsAndEntities.Visibility = Visibility.Collapsed;
                    mpButtonPrepareToPrint.Visibility = Visibility.Collapsed;
                    break;
                case "entityFRIES":
                    //mpContentBox.Text = await LoadedFileInfo.readTextFile(LoadedFileInfo.file);
                    mpMessageBox.Text = "FRIES entity-mention files cannot be translated. Please select another file.";
                    mpButtonTranslate.Visibility = Visibility.Collapsed;
                    mpButtonSave.Visibility = Visibility.Collapsed;
                    mpButtonAddContextsAndEntities.Visibility = Visibility.Collapsed;
                    mpButtonPrepareToPrint.Visibility = Visibility.Collapsed;
                    break;
                case "medscanFRIES":
                    //mpContentBox.Text = await LoadedFileInfo.readTextFile(LoadedFileInfo.file);
                    mpMessageBox.Text = "Medscan FRIES files are not currently supported. Please select another file.";
                    mpButtonTranslate.Visibility = Visibility.Collapsed;
                    mpButtonSave.Visibility = Visibility.Collapsed;
                    mpButtonAddContextsAndEntities.Visibility = Visibility.Collapsed;
                    mpButtonPrepareToPrint.Visibility = Visibility.Collapsed;
                    break;
                case "incompleteFRIES":
                    //mpContentBox.Text = await LoadedFileInfo.readTextFile(LoadedFileInfo.file);
                    mpMessageBox.Text = "File contains FRIES data with insufficient fields. Please select another file.";
                    mpButtonTranslate.Visibility = Visibility.Collapsed;
                    mpButtonSave.Visibility = Visibility.Collapsed;
                    mpButtonAddContextsAndEntities.Visibility = Visibility.Collapsed;
                    mpButtonPrepareToPrint.Visibility = Visibility.Collapsed;
                    break;
                case "incompleteMITRE":
                    //mpContentBox.Text = await LoadedFileInfo.readTextFile(LoadedFileInfo.file);
                    mpMessageBox.Text = "File contains MITRE data with insufficient fields. Please select another file.";
                    mpButtonTranslate.Visibility = Visibility.Collapsed;
                    mpButtonSave.Visibility = Visibility.Collapsed;
                    mpButtonAddContextsAndEntities.Visibility = Visibility.Collapsed;
                    mpButtonPrepareToPrint.Visibility = Visibility.Collapsed;
                    break;
                case "unknownCSV":
                    mpMessageBox.Text = "CSV file contains unfamiliar contents. Please select another file.";
                    mpButtonTranslate.Visibility = Visibility.Collapsed;
                    mpButtonSave.Visibility = Visibility.Collapsed;
                    mpButtonAddContextsAndEntities.Visibility = Visibility.Collapsed;
                    mpButtonPrepareToPrint.Visibility = Visibility.Collapsed;
                    break;
                case "unknownJSON":
                    mpMessageBox.Text = "JSON file contains unfamiliar contents. Please select another file.";
                    //mpContentBox.Text = await LoadedFileInfo.readTextFile(LoadedFileInfo.file);
                    mpButtonTranslate.Visibility = Visibility.Collapsed;
                    mpButtonSave.Visibility = Visibility.Collapsed;
                    mpButtonAddContextsAndEntities.Visibility = Visibility.Collapsed;
                    mpButtonPrepareToPrint.Visibility = Visibility.Collapsed;
                    break;
                case "invalidJSON":
                    mpMessageBox.Text = "File contains invalid JSON formatting. Please select another file.";
                    //mpContentBox.Text = await LoadedFileInfo.readTextFile(LoadedFileInfo.file);
                    mpButtonTranslate.Visibility = Visibility.Collapsed;
                    mpButtonSave.Visibility = Visibility.Collapsed;
                    mpButtonAddContextsAndEntities.Visibility = Visibility.Collapsed;
                    mpButtonPrepareToPrint.Visibility = Visibility.Collapsed;
                    break;
                case "unknownTXT":
                    mpMessageBox.Text = "Text file contains unfamiliar contents. Please select another file.";
                    mpButtonTranslate.Visibility = Visibility.Collapsed;
                    mpButtonSave.Visibility = Visibility.Collapsed;
                    mpButtonAddContextsAndEntities.Visibility = Visibility.Collapsed;
                    mpButtonPrepareToPrint.Visibility = Visibility.Collapsed;
                    break;
                case "unknown":
                    mpMessageBox.Text = "Unfamiliar file type. Please select another file (*.json, *.csv, or *.txt).";
                    //mpContentBox.Text = await LoadedFileInfo.readTextFile(LoadedFileInfo.file);
                    mpButtonTranslate.Visibility = Visibility.Collapsed;
                    mpButtonSave.Visibility = Visibility.Collapsed;
                    mpButtonAddContextsAndEntities.Visibility = Visibility.Collapsed;
                    mpButtonPrepareToPrint.Visibility = Visibility.Collapsed;
                    break;
                default:
                    break;
            }

            timer.Stop();
            timer.Reset();
        }

        private async void mpButtonLoadFolder_Click(object sender, RoutedEventArgs e)
        {
            await LoadedFileInfo.getFolderPath();

            timer.Start();

            if(LoadedFileInfo.folder != null)
            {
                IReadOnlyList<Windows.Storage.IStorageFile> fileList = await LoadedFileInfo.folder.GetFilesAsync();

                LoadedFileInfo.fileList = new List<Windows.Storage.IStorageFile>();
                 
                foreach (Windows.Storage.IStorageFile file in fileList)
                {
                    if (file.FileType == ".json" || file.FileType == ".txt")
                    {
                        string format = await LoadedFileInfo.checkDataFormat(file);
                        if(format == "MITRE")
                        {
                            LoadedFileInfo.fileList.Add(file);
                        }
                    }
                }

                LoadedFileInfo.modelDataAsMITRE = new string[LoadedFileInfo.fileList.Count];

                for (int i = 0; i < LoadedFileInfo.fileList.Count; i++)
                {
                    LoadedFileInfo.modelDataAsMITRE[i] = await LoadedFileInfo.readTextFile(LoadedFileInfo.fileList[i]);
                }

                LoadedFileInfo.format = "MITREFolder";

                if (!App.hidePreviews)
                {
                    displayPreview("MITREFolder");
                }
                
                mpButtonTranslate.Visibility = Visibility.Visible;
                mpButtonSave.Visibility = Visibility.Collapsed;
                mpButtonAddContextsAndEntities.Visibility = Visibility.Collapsed;
                mpButtonPrepareToPrint.Visibility = Visibility.Collapsed;

                timer.Stop();
                double loadTime = timer.ElapsedMilliseconds;
                loadTime = loadTime / 1000;
                timer.Reset();

                if (LoadedFileInfo.fileList.Count > 0)
                {
                    mpMessageBox.Text = LoadedFileInfo.fileList.Count.ToString() + " MITRE files loaded in " + loadTime.ToString() + " seconds.";
                    //mpContentBox.Text = LoadedFileInfo.modelDataAsMITRE[0];
                }
                else
                {
                    mpMessageBox.Text = "Chosen directory contains no valid MITRE files. Search completed in " + loadTime.ToString() + " seconds.";
                }
            }
            else
            {
                mpMessageBox.Text = "Folder selection calceled. Please try again.";
            }

        }

        private async void mpButtonTranslate_Click(object sender, RoutedEventArgs e)
        {
            timer.Start();
            double loadTime;

            switch (LoadedFileInfo.format)
            {
                case "MITRE":
                    LoadedFileInfo.modelDataAsMITRE = new string[1];
                    LoadedFileInfo.modelDataAsPittArray = new string[1, 57];
                    if (App.testModeEnabled)
                    {
                        Testing.translateFromMITRE(0);
                    }
                    else
                    {
                        Translators.translateFromMITRE(0);
                    }

                    Translators.removeUndesirableRows();

                    if (App.translateToFRIES)
                    {
                        Translators.translateToFRIES();
                    }
                    mpButtonTranslate.Visibility = Visibility.Collapsed;
                    mpComboBoxPanel.Visibility = Visibility.Visible;
                    mpButtonSave.Visibility = Visibility.Visible;
                    mpComboBox.SelectedIndex = (App.defaultDisplayFormatAfterTranslation + 1) % 2;
                    mpComboBox.SelectedIndex = App.defaultDisplayFormatAfterTranslation;
                    timer.Stop();
                    loadTime = timer.ElapsedMilliseconds;
                    loadTime = loadTime / 1000;
                    mpMessageBox.Text = "Translation complete in " + loadTime.ToString() + " seconds.";
                    break;
                case "MITREFolder":
                    if (App.testModeEnabled)
                    {
                        Testing.translateMultipleMITRE();
                    }
                    else
                    {
                        await Translators.translateMultipleMITRE();
                    }

                    Translators.removeUndesirableRows();

                    if (App.translateToFRIES)
                    {
                        Translators.translateToFRIES();
                    }
                    mpButtonTranslate.Visibility = Visibility.Collapsed;
                    mpComboBoxPanel.Visibility = Visibility.Visible;
                    mpButtonSave.Visibility = Visibility.Visible;
                    mpComboBox.SelectedIndex = (App.defaultDisplayFormatAfterTranslation + 1) % 2;
                    mpComboBox.SelectedIndex = App.defaultDisplayFormatAfterTranslation;
                    timer.Stop();
                    loadTime = timer.ElapsedMilliseconds;
                    loadTime = loadTime / 1000;
                    mpMessageBox.Text = "Translation complete in " + loadTime.ToString() + " seconds.";
                    break;
                case "FRIES":
                    if (App.testModeEnabled)
                    {
                        await Testing.translateFromFRIES(LoadedFileInfo.file);
                        mpComboBox.SelectedIndex = 2;
                        mpComboBox.SelectedIndex = App.defaultDisplayFormatAfterTranslation;
                    }
                    else
                    {
                        await Translators.translateFromFRIES(LoadedFileInfo.file);
                    }

                    Translators.removeUndesirableRows();
                    LoadedFileInfo.format = "Pitt";
                    LoadedFileInfo.modelDataAsMITRE = new string[LoadedFileInfo.modelDataAsPittArray.GetLength(0)];
                    if (App.translateToMITRE)
                    {
                        Translators.translatePitt_ToMitre();
                    }
                    mpComboBox.SelectedIndex = (App.defaultDisplayFormatAfterTranslation + 1) % 2;
                    mpComboBox.SelectedIndex = App.defaultDisplayFormatAfterTranslation;
                    
                                       
                    mpButtonTranslate.Visibility = Visibility.Collapsed;
                    mpComboBoxPanel.Visibility = Visibility.Visible;
                    mpButtonSave.Visibility = Visibility.Visible;
                    mpButtonAddContextsAndEntities.Visibility = Visibility.Visible;
                    timer.Stop();
                    loadTime = timer.ElapsedMilliseconds;
                    loadTime = loadTime / 1000;
                    mpMessageBox.Text = "Translation complete in " + loadTime.ToString() + " seconds.";
                    break;
                case "Pitt":
                    Translators.removeUndesirableRows();
                    if (App.translateToFRIES)
                    {
                        Translators.translatePitt_ToMitre();
                    }
                    if (App.translateToMITRE)
                    {
                        Translators.translateToFRIES();
                    }
                    mpButtonTranslate.Visibility = Visibility.Collapsed;
                    mpComboBoxPanel.Visibility = Visibility.Visible;
                    mpButtonSave.Visibility = Visibility.Visible;
                    mpComboBox.SelectedIndex = (App.defaultDisplayFormatAfterTranslation + 1) % 2;
                    mpComboBox.SelectedIndex = App.defaultDisplayFormatAfterTranslation;
                    timer.Stop();
                    loadTime = timer.ElapsedMilliseconds;
                    loadTime = loadTime / 1000;
                    mpMessageBox.Text = "Translation complete in " + loadTime.ToString() + " seconds.";
                    break;
                default:
                    mpMessageBox.Text = "Loaded data is not in a translatable format.";
                    break;
            }
            timer.Stop();
            timer.Reset();
        }

        private void mpButtonPrepareToPrint_Click(object sender, RoutedEventArgs e)
        {
            Translators.consolidateDuplicateRows();
            Translators.removeExcessColumnsFromPittData();
        }

        private async void mpButtonSave_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Windows.Storage.Pickers.FileSavePicker();
            dlg.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            switch (LoadedFileInfo.currentlyDisplayedFormat)
            {
                case "FRIES":
                    dlg.DefaultFileExtension = ".json";
                    dlg.FileTypeChoices.Add("FRIES (.json)", new List<string>() { ".json" });
                    Windows.Storage.StorageFile file = await dlg.PickSaveFileAsync();
                    if (file != null)
                    {
                        // Application now has read/write access to the file
                        await Windows.Storage.FileIO.WriteTextAsync(file, LoadedFileInfo.modelDataAsFRIES);
                        mpMessageBox.Text = "File save successful.";
                    }
                    else
                    {
                        mpMessageBox.Text = "File save canceled. Please try again.";
                    }
                    break;

                case "MITRE":
                    var dlg2 = new Windows.Storage.Pickers.FolderPicker();
                    //dlg.DefaultFileExtension = ".json";
                    dlg2.FileTypeFilter.Add(".");
                    //Windows.Storage.StorageFile file2 = await dlg.PickSaveFileAsync();
                    Windows.Storage.StorageFolder folder = await dlg2.PickSingleFolderAsync();
                    if (folder != null)
                    {
                        //await Task.Run(() =>
                        //{
                            for (int i = 0; i < LoadedFileInfo.modelDataAsMITRE.Length; i++)
                            {
                                // Assign file names based on settings provided in SettingsPage
                                string fileName = App.mitreSaveFileBaseName;
                                if (App.mitreFileNameMatchesFolder)
                                {
                                    fileName = folder.Name;
                                }

                                // Create file
                                Windows.Storage.StorageFile mitreFile = await folder.CreateFileAsync(fileName + (i + 1).ToString() + ".json", Windows.Storage.CreationCollisionOption.GenerateUniqueName);
                                await Windows.Storage.FileIO.WriteTextAsync(mitreFile, LoadedFileInfo.modelDataAsMITRE[i]);
                            }

                        mpMessageBox.Text = "File save successful.";
                    }
                    else
                    {
                        mpMessageBox.Text = "File save canceled. Please try again.";
                    }
                    break;

                case "Pitt":
                    dlg.DefaultFileExtension = ".csv";
                    dlg.FileTypeChoices.Add("Pitt (.csv)", new List<string>() { ".csv" });
                    Windows.Storage.StorageFile file3 = await dlg.PickSaveFileAsync();
                    if (file3 != null)
                    {
                        // Application now has read/write access to the file
                        System.Text.StringBuilder newOutput = new System.Text.StringBuilder();
                        newOutput.AppendLine(Pitt.Header.header);

                        if (App.cleanupDataBeforePrinting)
                        {
                            Translators.removeExcessColumnsFromPittData();
                        }

                        newOutput.AppendLine(LoadedFileInfo.modelDataAsPitt);
                        LoadedFileInfo.modelDataAsPitt = newOutput.ToString();

                        await Windows.Storage.FileIO.WriteTextAsync(file3, LoadedFileInfo.modelDataAsPitt);
                        mpMessageBox.Text = "File save successful.";
                    }
                    else
                    {
                        mpMessageBox.Text = "File save canceled. Please try again.";
                    }
                    break;
            }


        }

        private async void mpButtonAddContextsAndEntities_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.Pickers.FileOpenPicker picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.FileTypeFilter.Add(".json");
            picker.FileTypeFilter.Add(".txt");

            Windows.Storage.IStorageFile entityFile = await picker.PickSingleFileAsync();

            if(entityFile != null)
            {
                await Translators.addContextAndEntityInfo(entityFile);
                Translators.translatePitt_ToMitre();
                mpMessageBox.Text = "Context and entity information added successfully.";
                int oldSelectionIndex = mpComboBox.SelectedIndex;
                mpComboBox.SelectedIndex = (oldSelectionIndex + 1) % 2; // Change the selected format and then change it back to refresh the preview pane
                mpComboBox.SelectedIndex = oldSelectionIndex;
            }
            else
            {
                mpMessageBox.Text = "File selection canceled. Please try again.";
            }
            
        }

        private void mpSplitViewToggle_Click(object sender, RoutedEventArgs e)
        {
            this.mpSplitView.IsPaneOpen = this.mpSplitView.IsPaneOpen ? false : true;
        }

        // This triggers when the user selects a display format from the dropdown menu post-translation
        private void mpComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (App.hidePreviews)
            {
                return;
            }

            string selection = mpComboBox.SelectedItem.ToString();

            switch (selection)
            {
                case "FRIES":
                    if (App.translateToFRIES || LoadedFileInfo.format == "FRIES")
                    {
                        displayPreview("FRIES");
                        LoadedFileInfo.currentlyDisplayedFormat = "FRIES";
                    }
                    else
                    {
                        mpContentBox.Text = "Translation to the selected format has been disabled. Please select another format.";
                    }                
                    break;

                case "MITRE":
                    if (App.translateToMITRE || LoadedFileInfo.format == "MITRE" || LoadedFileInfo.format == "MITREFolder")
                    {
                        displayPreview("MITRE");
                        LoadedFileInfo.currentlyDisplayedFormat = "MITRE";
                    }
                    else
                    {
                        mpContentBox.Text = "Translation to the selected format has been disabled. Please select another format.";
                    }
                    break;

                case "Pitt":
                    if (App.translateToPitt || LoadedFileInfo.format == "Pitt")
                    {
                        displayPreview("Pitt");
                        LoadedFileInfo.currentlyDisplayedFormat = "Pitt";
                    }
                    else
                    {
                        mpContentBox.Text = "Translation to the selected format has been disabled. Please select another format.";
                    }
                    break;
                case "Summary":
                    if (App.testModeEnabled)
                    {
                        // Display test results
                        mpContentBox.Text = Testing.results(LoadedFileInfo.format);
                    }
                    else
                    {
                        mpContentBox.Text = "Testing mode has not been enabled in the Settings pane.\n\nPlease enable testing mode to view data related to the current translation.";
                    }
                    break;
            }
        }

        private void displayPreview(string format)
        {
            switch (format)
            {
                case "FRIES":
                    // Calculate the total number of pages in a FRIES file. xxx.Length - 1 will prevent a multiple of 2000 from being marked as having an extra page
                    int friesPages = ((LoadedFileInfo.modelDataAsFRIES.Length - 1) / App.maxChars) + 1;
                    int numChars = LoadedFileInfo.modelDataAsFRIES.Length;
                    if (numChars < App.maxChars)
                    {
                        LoadedFileInfo.FRIESPage = 0;
                        mpContentBox.Text = "(page " + (LoadedFileInfo.FRIESPage + 1).ToString() + " of " + friesPages.ToString() + ")\n" + LoadedFileInfo.modelDataAsFRIES.Substring(App.maxChars * LoadedFileInfo.FRIESPage, App.maxChars);
                    }
                    else
                    {
                        mpContentBox.Text = LoadedFileInfo.modelDataAsFRIES.Substring(App.maxChars * LoadedFileInfo.FRIESPage, App.maxChars);
                    }
                    LoadedFileInfo.currentlyDisplayedFormat = "FRIES";
                    break;
                case "MITRE":
                    LoadedFileInfo.MITREPage = 0;
                    mpContentBox.Text = LoadedFileInfo.modelDataAsMITRE[0];
                    LoadedFileInfo.currentlyDisplayedFormat = "MITRE";
                    break;
                case "MITREFolder":
                    LoadedFileInfo.MITREPage = 0;
                    mpContentBox.Text = "(page " + (LoadedFileInfo.MITREPage + 1).ToString() + " of " + (LoadedFileInfo.modelDataAsMITRE.Length).ToString() + ")\n" + LoadedFileInfo.modelDataAsMITRE[LoadedFileInfo.MITREPage];
                    LoadedFileInfo.currentlyDisplayedFormat = "MITRE";
                    break;
                case "Pitt":
                    mpContentBox.Text = LoadedFileInfo.modelDataAsPitt;
                    LoadedFileInfo.currentlyDisplayedFormat = "Pitt";
                    break;
                default:
                    break;
            }
        }

        private void previewNextPage(string format)
        {
            switch (format)
            {
                case "FRIES":
                    // Calculate the total number of pages in a FRIES file. xxx.Length - 1 will prevent a multiple of App.maxChars from being marked as having an extra page
                    int friesPages = ((LoadedFileInfo.modelDataAsFRIES.Length - 1) / App.maxChars) + 1;

                    if (friesPages == 1)
                    {
                        mpContentBox.Text = LoadedFileInfo.modelDataAsFRIES;
                        return;
                    }
                    if (LoadedFileInfo.modelDataAsFRIES.Length > (LoadedFileInfo.FRIESPage + 2) * App.maxChars) // Go to the next page if there is a full page of characters after the end of the current page
                    {
                        LoadedFileInfo.FRIESPage += 1;
                        mpContentBox.Text = "(page " + (LoadedFileInfo.FRIESPage + 1).ToString() + " of " + friesPages.ToString() + ")\n" + LoadedFileInfo.modelDataAsFRIES.Substring(App.maxChars * LoadedFileInfo.FRIESPage, App.maxChars);
                        return;
                    }
                    if (mpComboBox.SelectedIndex == 0 && (LoadedFileInfo.FRIESPage + 1) == (friesPages - 1)) // Final page reached - display all that is left
                    {
                        LoadedFileInfo.FRIESPage += 1;
                        mpContentBox.Text = "(page " + (LoadedFileInfo.FRIESPage + 1).ToString() + " of " + friesPages.ToString() + ")\n" + LoadedFileInfo.modelDataAsFRIES.Substring(App.maxChars * LoadedFileInfo.FRIESPage);
                        return;
                    }
                    if (mpComboBox.SelectedIndex == 0 && (LoadedFileInfo.FRIESPage + 1) == friesPages && LoadedFileInfo.modelDataAsFRIES.Length > App.maxChars) // Final page exceeded - Start over at page 1
                    {
                        LoadedFileInfo.FRIESPage = 0;
                        mpContentBox.Text = "(page " + (LoadedFileInfo.FRIESPage + 1).ToString() + " of " + friesPages.ToString() + ")\n" + LoadedFileInfo.modelDataAsFRIES.Substring(0, App.maxChars);
                        return;
                    }
                    break;

                case "MITRE":
                    if (LoadedFileInfo.modelDataAsMITRE.Length > 1)
                    {
                        if (LoadedFileInfo.MITREPage < LoadedFileInfo.modelDataAsMITRE.Length - 1)
                        {
                            LoadedFileInfo.MITREPage += 1;
                            mpContentBox.Text = "(page " + (LoadedFileInfo.MITREPage + 1).ToString() + " of " + (LoadedFileInfo.modelDataAsMITRE.Length).ToString() + ")\n" + LoadedFileInfo.modelDataAsMITRE[LoadedFileInfo.MITREPage];
                            return;
                        }
                        else
                        {
                            LoadedFileInfo.MITREPage = 0;
                            mpContentBox.Text = "(page " + (LoadedFileInfo.MITREPage + 1).ToString() + " of " + (LoadedFileInfo.modelDataAsMITRE.Length).ToString() + ")\n" + LoadedFileInfo.modelDataAsMITRE[LoadedFileInfo.MITREPage];
                        }
                        return;
                    }
                    break;
                case "Pitt":
                    break;
            }
        } // end previewNextPage

        private void previewPreviousPage(string format)
        {
            switch (format)
            {
                case "FRIES":
                    // Calculate the total number of pages in a FRIES file. xxx.Length - 1 will prevent a multiple of App.maxChars from being marked as having an extra page
                    int friesPages = ((LoadedFileInfo.modelDataAsFRIES.Length - 1) / App.maxChars) + 1;
                    if (mpComboBox.SelectedIndex == 0 && LoadedFileInfo.FRIESPage > 0)
                    {
                        LoadedFileInfo.FRIESPage -= 1;
                        mpContentBox.Text = "(page " + (LoadedFileInfo.FRIESPage + 1).ToString() + " of " + friesPages.ToString() + ")\n" + LoadedFileInfo.modelDataAsFRIES.Substring(App.maxChars * LoadedFileInfo.FRIESPage, App.maxChars);
                        return;
                    }
                    if (mpComboBox.SelectedIndex == 0 && LoadedFileInfo.FRIESPage == 0 && LoadedFileInfo.modelDataAsFRIES.Length > App.maxChars)
                    {
                        LoadedFileInfo.FRIESPage = friesPages - 1; // The computational version says 0 is the first page and n-1 is the last page
                        mpContentBox.Text = "(page " + (LoadedFileInfo.FRIESPage + 1).ToString() + " of " + friesPages.ToString() + ")\n" + LoadedFileInfo.modelDataAsFRIES.Substring(App.maxChars * LoadedFileInfo.FRIESPage);
                        return;
                    }
                    break;
                case "MITRE":
                    if (LoadedFileInfo.modelDataAsMITRE.Length > 1)
                    {
                        if (LoadedFileInfo.MITREPage > 0)
                        {
                            LoadedFileInfo.MITREPage -= 1;
                            mpContentBox.Text = "(page " + (LoadedFileInfo.MITREPage + 1).ToString() + " of " + (LoadedFileInfo.modelDataAsMITRE.Length).ToString() + ")\n" + LoadedFileInfo.modelDataAsMITRE[LoadedFileInfo.MITREPage];
                            return;
                        }
                        else
                        {
                            LoadedFileInfo.MITREPage = LoadedFileInfo.modelDataAsMITRE.Length - 1;
                            mpContentBox.Text = "(page " + (LoadedFileInfo.MITREPage + 1).ToString() + " of " + (LoadedFileInfo.modelDataAsMITRE.Length).ToString() + ")\n" + LoadedFileInfo.modelDataAsMITRE[LoadedFileInfo.MITREPage];
                            return;
                        }
                    }
                    break;
                case "Pitt":
                    break;
            }
        } // end previewPreviousPage


        private void mpComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            List<string> choices = new List<string>();
            choices.Add("FRIES");
            choices.Add("MITRE");
            choices.Add("Pitt");
            choices.Add("Summary");

            var comboBox = sender as ComboBox;
            comboBox.ItemsSource = choices;

            switch (LoadedFileInfo.format)
            {
                case "FRIES":
                    comboBox.SelectedIndex = 0;
                    break;
                case "MITRE":
                    comboBox.SelectedIndex = 1;
                    break;
                case "Pitt":
                    comboBox.SelectedIndex = 2;
                    break;
            }

            comboBox.SelectionChanged += new SelectionChangedEventHandler(mpComboBox_SelectionChanged);
        }

        private void mpContentBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            //return; // Remove when functionality is truly implemented...

            if(mpComboBoxPanel.Visibility == Visibility.Collapsed)
            {
                return; // Only change pages if the ComboBoxPanel is visible, i.e., MITRE data could be displayed.
            }

            switch (e.Key)
            {
                case Windows.System.VirtualKey.Right:
                    if (mpComboBox.SelectedIndex == 0 || (mpComboBox.Visibility == Visibility.Collapsed && LoadedFileInfo.modelDataAsFRIES.Length > App.maxChars))
                    {
                        previewNextPage("FRIES");
                        return;
                    }
                    if (mpComboBox.SelectedIndex == 1 || (mpComboBox.Visibility == Visibility.Collapsed && LoadedFileInfo.modelDataAsMITRE.Length > 1))
                    {
                        previewNextPage("MITRE");
                        return;
                    }
                    break;

                case Windows.System.VirtualKey.Left:
                    if (mpComboBox.SelectedIndex == 0 || (mpComboBox.Visibility == Visibility.Collapsed && LoadedFileInfo.modelDataAsFRIES.Length > App.maxChars))
                    {
                        previewPreviousPage("FRIES");
                        return;
                    }
                    if (mpComboBox.SelectedIndex == 1 || (mpComboBox.Visibility == Visibility.Collapsed && LoadedFileInfo.modelDataAsMITRE.Length > 1))
                    {
                        previewPreviousPage("MITRE");
                        return;
                    }
                    break;

                default:
                    break;
            }
            
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsPage));
        }

    }
}

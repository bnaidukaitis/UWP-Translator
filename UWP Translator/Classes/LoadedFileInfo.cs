using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace UWP_Translator.Classes
{
    public static class LoadedFileInfo
    {
        public static string fileName { get; set; }
        public static string fileExt { get; set; }
        public static Windows.Storage.IStorageFile file { get; set; }
        public static Windows.Storage.IStorageFolder folder { get; set; }
        public static List<Windows.Storage.IStorageFile> fileList { get; set; }
        public static string format { get; set; }
        public static string[,] modelDataAsPittArray { get; set; }
        public static string modelDataAsPitt { get; set; }
        public static string modelDataAsFRIES { get; set; }
        public static int FRIESPage { get; set; }
        public static string[] modelDataAsMITRE { get; set; }
        public static int MITREPage { get; set; }
        public static string currentlyDisplayedFormat { get; set; }
        //public static int DefaultFilterIndexForNextSave = 3;  // 1: FRIES. 2: MITRE. 3: Pitt

        // Writes to a CSV at the given path, accepts a string modelData that comes from a 
        // StringBuilder object after  passing to the .ToString() method
        public static void writeToCSV(string path, string modelData)
        {
            StringBuilder printableData = new StringBuilder();

            printableData.AppendLine(Pitt.Header.header);
            printableData.AppendLine(modelData);

            File.WriteAllText(@path, printableData.ToString());

        }

        public static async void getOpenFilePath()
        {
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
                LoadedFileInfo.fileName = file.Name;
                LoadedFileInfo.file = file;
                LoadedFileInfo.folder = null;
            }
            else
            {
                App.MyTextBoxes.TextMessageBox = "File selection canceled. Please try again.";
            }
        }

        public static async void getSaveFilePath(string defaultExt = ".csv", string filter = "Comma-delimited (.csv)|*.csv")
        {
            var dlg = new Windows.Storage.Pickers.FileSavePicker();
            //dlg.DefaultFileExtension = LoadedFileInfo.DefaultExtForNextSave;
            dlg.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            dlg.FileTypeChoices.Add("FRIES (*.json)", new List<string>() { ".json" });
            dlg.FileTypeChoices.Add("MITRE (*.json)", new List<string>() { ".json" });
            dlg.FileTypeChoices.Add("Pitt (*.json)", new List<string>() { ".json" });

            Windows.Storage.StorageFile file = await dlg.PickSaveFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                LoadedFileInfo.fileName = file.Name;
                LoadedFileInfo.file = file;
                LoadedFileInfo.folder = null;
            }
            else
            {
                App.MyTextBoxes.TextMessageBox = "File save canceled. Please try again.";
            }
        }

        public static async Task getFolderPath()
        {
            LoadedFileInfo.fileName = null;
            LoadedFileInfo.folder = null;
            LoadedFileInfo.file = null;

            var dlg = new Windows.Storage.Pickers.FolderPicker();
            dlg.FileTypeFilter.Add(".");
            dlg.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            dlg.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            Windows.Storage.StorageFolder folder = await dlg.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to the picked file
                LoadedFileInfo.fileName = folder.Name;
                LoadedFileInfo.folder = folder;
                LoadedFileInfo.file = null;
            }
            else
            {
                App.MyTextBoxes.TextMessageBox = "Folder selection canceled. Please try again.";
            }

            //return LoadedFileInfo.folder;
        }

        public static async Task<string> checkDataFormat(Windows.Storage.IStorageFile file)
        {
            string format = "unknown";  // Format can be any of the following: 
                                        // Pitt, FRIES, MITRE, entityFRIES, medscanFRIES, incompleteFRIES, 
                                        // incompleteMITRE, unknown, unknownJSON, unknownCSV

            if (file == null)
            {
                return format;
            }

            string fileExt = file.FileType; //filePath.Split(new char[] { '.' }).Last(); // Splits the path into substrings at '.' and takes the last one (e.g., 'csv'). 
            string fileContents;

            switch (fileExt)
            {
                case ".csv":
                    // Check to see if file matches the Pitt formalism...
                    format = "Pitt";
                    break;

                case ".json":
                    // Check to see if the file matches either the MITRE or FRIES formalisms
                    fileContents = await Windows.Storage.FileIO.ReadTextAsync(file);

                    try
                    {
                        dynamic jsonData = JsonConvert.DeserializeObject(fileContents);

                        // Check to see if the file matches the FRIES formalism
                        try
                        {
                            dynamic frames = jsonData.frames;

                            foreach (dynamic frame in frames)
                            {
                                try
                                {
                                    dynamic argument = frame.arguments;
                                    dynamic frameID = frame["frame-id"];
                                    dynamic frameType = frame["frame-type"];
                                    if (frameType == "event-mention")
                                    {
                                        format = "FRIES";
                                        return format;
                                    }
                                    if (frameType == "entity-mention")
                                    {
                                        format = "entityFRIES";
                                        return format;
                                    }
                                    if (frameType == "relation-mention")
                                    {
                                        format = "medscanFRIES";
                                        return format;
                                    }
                                    format = "incompleteFRIES";
                                    return format;
                                }
                                catch
                                {
                                    format = "incompleteFRIES";
                                    return format;
                                }
                            }

                        }
                        catch { format = "unknownJSON"; }

                        // Check to see if the file matches the MITRE formalism
                        try
                        {
                            dynamic interaction = jsonData.interaction;
                            try
                            {
                                dynamic partA = interaction.participant_a;
                                format = "MITRE";
                                return format;
                            }
                            catch
                            {
                                format = "incompleteMITRE";
                                return format;
                            }
                        }
                        catch { format = "unknownJSON"; }

                    }
                    catch (Newtonsoft.Json.JsonReaderException)
                    {
                        format = "invalidJSON";
                        return format;
                    }


                    break;

                case ".txt":
                    // Check to see if the file contains JSON data matching either the MITRE or FRIES formalisms
                    fileContents = await Windows.Storage.FileIO.ReadTextAsync(file);

                    try
                    {
                        dynamic jsonData2 = JsonConvert.DeserializeObject(fileContents);

                        // Check to see if the file matches the FRIES formalism
                        try
                        {
                            dynamic frames = jsonData2.frames;

                            foreach (dynamic frame in frames)
                            {
                                try
                                {
                                    dynamic argument = frame.arguments;
                                    dynamic frameID = frame["frame-id"];
                                    format = "FRIES";
                                    return format;
                                }
                                catch
                                {
                                    format = "incompleteFRIES";
                                    return format;
                                }
                            }

                        }
                        catch { format = "unknownTXT"; }

                        // Check to see if the file matches the MITRE formalism
                        try
                        {
                            dynamic interaction = jsonData2.interaction;
                            try
                            {
                                dynamic partA = interaction.participant_a;
                                format = "MITRE";
                                return format;
                            }
                            catch
                            {
                                format = "incompleteMITRE";
                                return format;
                            }
                        }
                        catch { format = "unknownTXT"; }
                    }
                    catch (Newtonsoft.Json.JsonReaderException)
                    {
                        format = "invalidJSON";
                        return format;
                    }

                    break;

                default:
                    break;
            }

            return format;
        }

        public static async Task<string> readTextFile(Windows.Storage.IStorageFile file)
        {
            string fileContents = await Windows.Storage.FileIO.ReadTextAsync(file);
            return fileContents;
        }


    }

}

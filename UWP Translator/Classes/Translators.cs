using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;

namespace UWP_Translator.Classes
{
    public class Translators
    {
        public static Stopwatch timer = new Stopwatch();

        #region ToPitt

        public static void translateFromMITRE(int Index)
        {
            //LoadedFileInfo.modelDataAsMITRE[Index] = await LoadedFileInfo.readTextFile(file);
            dynamic data = JsonConvert.DeserializeObject(LoadedFileInfo.modelDataAsMITRE[Index]);

            LoadedFileInfo.modelDataAsPittArray[Index, 56] = LoadedFileInfo.fileList[Index].Name;
            // Extract reference article
            LoadedFileInfo.modelDataAsPittArray[Index, 42] = data.pmc_id.ToString();
            // For convenience, limit to interaction data
            data = data.interaction;

            try
            {
                // Works if participant_b is a complex
                LoadedFileInfo.modelDataAsPittArray[Index, 4] = "{" + data.participant_b.entities[0].entity_text[0].ToString();
                LoadedFileInfo.modelDataAsPittArray[Index, 5] = "{" + data.participant_b.entities[0].entity_type.ToString();

                for (int i = 1; i < data.participant_b.entities.GetLength(); i++)
                {
                    LoadedFileInfo.modelDataAsPittArray[Index, 4] += data.participant_b.entities[i].entity_text[0].ToString();
                    LoadedFileInfo.modelDataAsPittArray[Index, 5] += data.participant_b.entities[i].entity_type.ToString();
                    if (i < data.participant_b.entities.GetLength() - 1)
                    {
                        LoadedFileInfo.modelDataAsPittArray[Index, 4] += ",";
                        LoadedFileInfo.modelDataAsPittArray[Index, 4] += ",";
                    }
                    else
                    {
                        LoadedFileInfo.modelDataAsPittArray[Index, 4] += "}";
                        LoadedFileInfo.modelDataAsPittArray[Index, 4] += "}";
                    }
                }
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
            {
                try
                {
                    // participant_b is the element under study - set its name
                    LoadedFileInfo.modelDataAsPittArray[Index, 4] = data.participant_b.entity_text[0].ToString();
                    LoadedFileInfo.modelDataAsPittArray[Index, 5] = data.participant_b.entity_type.ToString();
                    LoadedFileInfo.modelDataAsPittArray[Index, 45] = data.participant_a.entity_type.ToString();

                    string ID_b = data.participant_b.identifier.ToString();
                    string ID_a = data.participant_b.identifier.ToString();

                    // Set database ID. If format is DatabaseName:ID, sort as such. 
                    if (ID_b.Contains(":"))
                    {
                        LoadedFileInfo.modelDataAsPittArray[Index, 6] = ID_b.Substring(0, ID_b.IndexOf(":"));
                        LoadedFileInfo.modelDataAsPittArray[Index, 7] = ID_b.Substring(ID_b.IndexOf(":") + 1);
                    }
                    else  // If ID = "ungrounded", set database name and ID as ungrounded
                    {
                        LoadedFileInfo.modelDataAsPittArray[Index, 6] = ID_b;
                        LoadedFileInfo.modelDataAsPittArray[Index, 7] = ID_b;
                    }
                    // Do the same for participant_a, the regulator; This information is unncessessary for Pitt models but included in FRIES data
                    if (ID_a.Contains(":"))
                    {
                        LoadedFileInfo.modelDataAsPittArray[Index, 46] = ID_a.Substring(0, ID_a.IndexOf(":"));
                        LoadedFileInfo.modelDataAsPittArray[Index, 47] = ID_a.Substring(ID_a.IndexOf(":") + 1);
                    }
                    else  // If ID = "ungrounded", set database name and ID as ungrounded
                    {
                        LoadedFileInfo.modelDataAsPittArray[Index, 46] = ID_a;
                        LoadedFileInfo.modelDataAsPittArray[Index, 47] = ID_a;
                    }

                }
                catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException) { /* Do nothing */ }
            }

            try
            {
                if (data.interaction_type.ToString() == "increases")
                {
                    LoadedFileInfo.modelDataAsPittArray[Index, 32] = data.participant_a.entity_text[0];
                }
                else
                {
                    if (data.interaction_type.ToString() == "decreases")
                    {
                        LoadedFileInfo.modelDataAsPittArray[Index, 33] = data.participant_a.entity_text[0].ToString();
                    }
                }
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException) { /* Do nothing */ }
        }

        public static async Task translateMultipleMITRE()
        {
            LoadedFileInfo.modelDataAsPittArray = new string[LoadedFileInfo.fileList.Count, 57];
            //LoadedFileInfo.modelDataAsMITRE = new string[LoadedFileInfo.fileList.Count];

            for (int i = 0; i < LoadedFileInfo.fileList.Count; i++)
            {
                translateFromMITRE(i);
            }

            // Convert FileInfo.modelDataAsPittArray into a string for printing
            pittDataArrayToString();
        }

        public static List<frames> FRIES_JsonToObject(dynamic framesList)
        {
            int count = framesList.Count;
            List<frames> allFrames = new List<frames>();

            for (int i = 0; i < count; i++)
            {
                frames thisFrame = new frames();
                thisFrame.frame_id = framesList[i]["frame-id"];
                thisFrame.subtype = framesList[i].subtype;
                thisFrame.type = framesList[i].type;
                if (framesList[i].context != null)
                {
                    thisFrame.context.Add(framesList[i].context[0].ToString());
                }

                for (int j = 0; j < framesList[i].arguments.Count; j++)
                {
                    frames.args thisArg = new frames.args();
                    string argString = JsonConvert.SerializeObject(framesList[i].arguments[j]);
                    thisArg = JsonConvert.DeserializeObject<frames.args>(argString);
                    thisFrame.arguments.Add(thisArg);
                }
                allFrames.Add(thisFrame);
            }
            return allFrames;
        }

        public static async Task translateFromFRIES(Windows.Storage.IStorageFile file)
        {
            string jsonData = await LoadedFileInfo.readTextFile(file);
            dynamic data = JsonConvert.DeserializeObject(jsonData);

            // For simplicity, limit data to frames and not meta infomation
            //data = data.frames;

            List<frames> allFrames = FRIES_JsonToObject(data.frames);

            int count = allFrames.Count;
            LoadedFileInfo.modelDataAsPittArray = new string[count, 57];

            // Make a row for each FRIES frame (JSON object) in the file
            for (int i = 0; i < count; i++)
            {
                //dynamic myData = allFrames[i];
                //dynamic args = myData.arguments;
                LoadedFileInfo.modelDataAsPittArray[i, 56] = file.Name;
                if (allFrames[i].arguments.Count == 1)
                {
                    // Make this the element under study
                    LoadedFileInfo.modelDataAsPittArray[i, 4] = allFrames[i].arguments[0].text;
                    LoadedFileInfo.modelDataAsPittArray[i, 35] = allFrames[i].subtype;
                    // Extract "entity-mention" ID for later analysis
                    LoadedFileInfo.modelDataAsPittArray[i, 44] = allFrames[i].arguments[0].arg;
                }
                else
                {
                    int argCount = allFrames[i].arguments.Count;
                    // There is more than one argument in this case.
                    for (int j = 0; j < argCount; j++)
                    {
                        string text = allFrames[i].arguments[j].text.ToString();
                        string argType = allFrames[i].arguments[j].type.ToString();
                        switch (argType)
                        {
                            case "controlled":
                                // Make this the element under study
                                LoadedFileInfo.modelDataAsPittArray[i, 4] = text;
                                // Store entity mention ID for potential future processing
                                LoadedFileInfo.modelDataAsPittArray[i, 44] = allFrames[i].arguments[j].arg;
                                break;
                            case "controller":
                                // Make this the regulator, depending on its value
                                string subType = allFrames[i].subtype.ToString();
                                switch (subType)
                                {
                                    case "positive-regulation":
                                        LoadedFileInfo.modelDataAsPittArray[i, 32] = text;
                                        LoadedFileInfo.modelDataAsPittArray[i, 35] = "positive activation";
                                        break;
                                    case "positive-activation":
                                        LoadedFileInfo.modelDataAsPittArray[i, 32] = text;
                                        LoadedFileInfo.modelDataAsPittArray[i, 35] = "positive activation";
                                        break;
                                    case "negative-regulation":
                                        LoadedFileInfo.modelDataAsPittArray[i, 33] = text;
                                        LoadedFileInfo.modelDataAsPittArray[i, 35] = "negative activation";
                                        break;
                                    case "negative-activation":
                                        LoadedFileInfo.modelDataAsPittArray[i, 33] = text;
                                        LoadedFileInfo.modelDataAsPittArray[i, 35] = "negative activation";
                                        break;
                                    default:
                                        LoadedFileInfo.modelDataAsPittArray[i, 32] = text;
                                        LoadedFileInfo.modelDataAsPittArray[i, 35] = subType;
                                        break;
                                }

                                // Store this regulator's entity mention ID for potential future processing - useful for translation to the MITRE format
                                LoadedFileInfo.modelDataAsPittArray[i, 55] = allFrames[i].arguments[j].arg;
                                break;
                            default:
                                break;
                        }
                    }
                }

                // Extract PubMed references
                string frameID = allFrames[i].frame_id;
                try
                {
                    LoadedFileInfo.modelDataAsPittArray[i, 42] = frameID.Substring(frameID.IndexOf("PMC"), frameID.Substring(frameID.IndexOf("PMC")).IndexOf("-"));
                }
                catch { }

                // Extract FRIES context information for later analysis
                if (allFrames[i].context.Count != 0)
                {
                    LoadedFileInfo.modelDataAsPittArray[i, 43] = allFrames[i].context[0];
                }
            }

            // Place the data in the string FileInfo.ModelDataAsPitt;
            pittDataArrayToString();
        } // end translateFromFRIES

        public static async Task addContextAndEntityInfo(Windows.Storage.IStorageFile contextFile)
        {
            //timer.Start();
            string fileContents = await Windows.Storage.FileIO.ReadTextAsync(contextFile);
            dynamic jsonData = JsonConvert.DeserializeObject(fileContents);

            try
            {
                dynamic frames = jsonData.frames;

                string[] frameTypes = new string[frames.Count];

                // Extract all entity IDs and context IDs and put them into dictionaries with their index in "frames" as the value
                // Later, loop through elements in modelDataAsPitt, see if context and entity IDs are in the dictionaries created
                // If so, get the corresponding index i, look up in frames[i] and add information

                Dictionary<string, int> allContexts = new Dictionary<string, int>();
                Dictionary<string, int> allEntities = new Dictionary<string, int>();
                int count = frames.Count;
                int[] contextIndices = new int[count];
                int[] entityIndices = new int[count]; 

                for (int i = 0; i < count; i++)
                {
                    string frameType = frames[i]["frame-type"].ToString();
                    switch (frameType)
                    {
                        case "context":
                            allContexts.Add(frames[i]["frame-id"].ToString(), i);
                            break;
                        case "entity-mention":
                            allEntities.Add(frames[i]["frame-id"].ToString(), i);
                            break;
                        default:
                            break;
                    } // end switch
                } // end for loop, adds context and entity-mention IDs to dictionaries

                // See if context and entity-mention IDs from FileInfo.modelDataAsPitt are in the dictionaries created
                for (int i = 0; i < LoadedFileInfo.modelDataAsPittArray.GetLength(0); i++)
                {
                    if (LoadedFileInfo.modelDataAsPittArray[i, 44] != null) // A null entity reference will throw a null exception
                    {
                        if (allEntities.ContainsKey(LoadedFileInfo.modelDataAsPittArray[i, 44]))
                        {
                            // Add entity information to array
                            dynamic thisEntity = frames[allEntities[LoadedFileInfo.modelDataAsPittArray[i, 44]]];

                            // Extract information
                            LoadedFileInfo.modelDataAsPittArray[i, 5] = thisEntity.type.ToString(); // Element type
                            LoadedFileInfo.modelDataAsPittArray[i, 6] = thisEntity.xrefs[0]["namespace"].ToString(); // Database name
                            LoadedFileInfo.modelDataAsPittArray[i, 7] = thisEntity.xrefs[0]["id"].ToString(); // Database ID

                            // Use entity's context ID if one is not stored in the array already
                            if (LoadedFileInfo.modelDataAsPittArray[i, 43] == null)
                            {
                                try { LoadedFileInfo.modelDataAsPittArray[i, 43] = thisEntity.context.ToString(); }
                                catch { /* No context available... Do nothing */ }
                            }
                        }
                    }

                    // Do the same for regulators with available entity-mention IDs
                    if (LoadedFileInfo.modelDataAsPittArray[i, 55] != null) // A null entity reference will throw a null exception
                    {
                        if (allEntities.ContainsKey(LoadedFileInfo.modelDataAsPittArray[i, 55]))
                        {
                            // Add entity information to array
                            dynamic thisEntity = frames[allEntities[LoadedFileInfo.modelDataAsPittArray[i, 55]]];

                            // Extract information
                            LoadedFileInfo.modelDataAsPittArray[i, 45] = thisEntity.type.ToString(); // Element type
                            LoadedFileInfo.modelDataAsPittArray[i, 46] = thisEntity.xrefs[0]["namespace"].ToString(); // Database name
                            LoadedFileInfo.modelDataAsPittArray[i, 47] = thisEntity.xrefs[0]["id"].ToString(); // Database ID

                            // Use entity's context ID if one is not stored in the array already
                            if (LoadedFileInfo.modelDataAsPittArray[i, 43] == null)
                            {
                                try { LoadedFileInfo.modelDataAsPittArray[i, 43] = thisEntity.context.ToString(); }
                                catch { /* No context available... Do nothing */ }
                            }
                        }
                    }

                    if (LoadedFileInfo.modelDataAsPittArray[i, 43] != null) // A null context ID will throw a null exception
                    {
                        // Place context information into model
                        if (allContexts.ContainsKey(LoadedFileInfo.modelDataAsPittArray[i, 43]))
                        {
                            dynamic thisContext = frames[allContexts[LoadedFileInfo.modelDataAsPittArray[i, 43]]];

                            // Add cell line information
                            try
                            {
                                string cellLine = thisContext["facets"]["cell-line"][0].ToString();
                                if (cellLine.Split(':').Length > 2) // If the number of colons is greater than 1 in the string...
                                {
                                    cellLine = cellLine.Substring(cellLine.IndexOf(':') + 1); // Remove superfluous ':' characters
                                }
                                LoadedFileInfo.modelDataAsPittArray[i, 8] = cellLine;
                                LoadedFileInfo.modelDataAsPittArray[i, 48] = cellLine; // Assume the regulator is of the same cell line
                            }
                            catch { /* No information available... Do nothing */ }

                            // Add cell type information
                            try
                            {
                                string cellType = thisContext["facets"]["cell-type"][0].ToString();
                                if (cellType.Split(':').Length > 2) // If the number of colons is greater than 1 in the string...
                                {
                                    cellType = cellType.Substring(cellType.IndexOf(':') + 1); // Remove superfluous ':' characters
                                }
                                LoadedFileInfo.modelDataAsPittArray[i, 9] = cellType;
                                LoadedFileInfo.modelDataAsPittArray[i, 49] = cellType; // Assume the regulator is of the same cell type
                            }
                            catch { /* No information available... Do nothing */ }

                            // Add organism information
                            try
                            {
                                string organism = thisContext["facets"]["organism"][0].ToString();
                                if (organism.Split(':').Length > 2) // If the number of colons is greater than 1 in the string...
                                {
                                    organism = organism.Substring(organism.IndexOf(':') + 1); // Remove superfluous ':' characters
                                }
                                LoadedFileInfo.modelDataAsPittArray[i, 10] = organism;
                                LoadedFileInfo.modelDataAsPittArray[i, 50] = organism; // Assume the regulator is in the same organism
                            }
                            catch { /* No information available... Do nothing */ }

                            // Add tissue type information
                            try
                            {
                                string tissueType = thisContext["facets"]["tissue-type"][0].ToString();
                                if (tissueType.Split(':').Length > 2) // If the number of colons is greater than 1 in the string...
                                {
                                    tissueType = tissueType.Substring(tissueType.IndexOf(':') + 1); // Remove superfluous ':' characters
                                }
                                LoadedFileInfo.modelDataAsPittArray[i, 11] = tissueType;
                                LoadedFileInfo.modelDataAsPittArray[i, 51] = tissueType; // Assume the regulator is of the same tissue type
                            }
                            catch { /* No information available... Do nothing */ }
                        }
                    }
                }
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
            {
                MainPageUI.TextMessageBox = "No frame information available in selected entity file.";
                return;
            }

            // Update ModelDataAsPitt string
            pittDataArrayToString();
            //timer.Stop();
            //double loadTime = timer.ElapsedMilliseconds;
            //loadTime = loadTime / 1000;
            //MainPageUI.TextMessageBox = "Context and entity information linked successfully in " + loadTime.ToString() + " seconds.";
            //timer.Reset();
            //MainPageUI.TextContentBox = LoadedFileInfo.modelDataAsPitt;
        } // end addContextAndEntityInfo

        public static void pittDataArrayToString()
        {
            StringBuilder modelData = new StringBuilder();

            for (int i = 0; i < LoadedFileInfo.modelDataAsPittArray.GetLength(0); i++)
            {
                string nextLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42},{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53},{54},{55},{56}",
                    LoadedFileInfo.modelDataAsPittArray[i, 0], LoadedFileInfo.modelDataAsPittArray[i, 1], LoadedFileInfo.modelDataAsPittArray[i, 2], LoadedFileInfo.modelDataAsPittArray[i, 3], LoadedFileInfo.modelDataAsPittArray[i, 4], LoadedFileInfo.modelDataAsPittArray[i, 5], LoadedFileInfo.modelDataAsPittArray[i, 6], LoadedFileInfo.modelDataAsPittArray[i, 7],
                    LoadedFileInfo.modelDataAsPittArray[i, 8], LoadedFileInfo.modelDataAsPittArray[i, 9], LoadedFileInfo.modelDataAsPittArray[i, 10], LoadedFileInfo.modelDataAsPittArray[i, 11], LoadedFileInfo.modelDataAsPittArray[i, 12], LoadedFileInfo.modelDataAsPittArray[i, 13], LoadedFileInfo.modelDataAsPittArray[i, 14], LoadedFileInfo.modelDataAsPittArray[i, 15],
                    LoadedFileInfo.modelDataAsPittArray[i, 16], LoadedFileInfo.modelDataAsPittArray[i, 17], LoadedFileInfo.modelDataAsPittArray[i, 18], LoadedFileInfo.modelDataAsPittArray[i, 19], LoadedFileInfo.modelDataAsPittArray[i, 20], LoadedFileInfo.modelDataAsPittArray[i, 21], LoadedFileInfo.modelDataAsPittArray[i, 22], LoadedFileInfo.modelDataAsPittArray[i, 23],
                    LoadedFileInfo.modelDataAsPittArray[i, 24], LoadedFileInfo.modelDataAsPittArray[i, 25], LoadedFileInfo.modelDataAsPittArray[i, 26], LoadedFileInfo.modelDataAsPittArray[i, 27], LoadedFileInfo.modelDataAsPittArray[i, 28], LoadedFileInfo.modelDataAsPittArray[i, 29], LoadedFileInfo.modelDataAsPittArray[i, 30], LoadedFileInfo.modelDataAsPittArray[i, 31],
                    LoadedFileInfo.modelDataAsPittArray[i, 32], LoadedFileInfo.modelDataAsPittArray[i, 33], LoadedFileInfo.modelDataAsPittArray[i, 34], LoadedFileInfo.modelDataAsPittArray[i, 35], LoadedFileInfo.modelDataAsPittArray[i, 36], LoadedFileInfo.modelDataAsPittArray[i, 37], LoadedFileInfo.modelDataAsPittArray[i, 38], LoadedFileInfo.modelDataAsPittArray[i, 39],
                    LoadedFileInfo.modelDataAsPittArray[i, 40], LoadedFileInfo.modelDataAsPittArray[i, 41], LoadedFileInfo.modelDataAsPittArray[i, 42], LoadedFileInfo.modelDataAsPittArray[i, 43], LoadedFileInfo.modelDataAsPittArray[i, 44], LoadedFileInfo.modelDataAsPittArray[i, 45],  LoadedFileInfo.modelDataAsPittArray[i, 46], LoadedFileInfo.modelDataAsPittArray[i, 47], 
                    LoadedFileInfo.modelDataAsPittArray[i, 48], LoadedFileInfo.modelDataAsPittArray[i, 49], LoadedFileInfo.modelDataAsPittArray[i, 50], LoadedFileInfo.modelDataAsPittArray[i, 51], LoadedFileInfo.modelDataAsPittArray[i, 52], LoadedFileInfo.modelDataAsPittArray[i, 53], LoadedFileInfo.modelDataAsPittArray[i, 54], LoadedFileInfo.modelDataAsPittArray[i, 55], 
                    LoadedFileInfo.modelDataAsPittArray[i, 56]);

                modelData.AppendLine(nextLine);
            }

            LoadedFileInfo.modelDataAsPitt = modelData.ToString();
        }

        // consolidateDuplicateRows merges rows that contain the same element in the primary element column
        public static void consolidateDuplicateRows()
        {
            List<int> duplicateRows = new List<int>(); // Keeps track of all duplicate rows for later deletion
            // Look for subsequent rows that contain the same element and mark them and their index, stop looking when we get to the last row
            for (int i = 0; i < LoadedFileInfo.modelDataAsPittArray.GetLength(0) - 1; i++)
            {
                if (duplicateRows.Contains(i)) // If this is a duplicate let's not bother to find its own duplicates. We found them all previously.
                {
                    continue;
                }

                // Also mark this row for deletion if the element is equal to null
                if (LoadedFileInfo.modelDataAsPittArray[i, 4] == null)
                {
                    duplicateRows.Add(i);
                    continue;
                }

                List<int> duplicateIndex = new List<int>(); // Keeps track of this row's duplicates for consolidation

                // Loop through each row that is not this one and find duplicates
                for (int j = i + 1; j < LoadedFileInfo.modelDataAsPittArray.GetLength(0); j++)
                {
                    if (LoadedFileInfo.modelDataAsPittArray[i, 4] == LoadedFileInfo.modelDataAsPittArray[j, 4])
                    {
                        // If the element is the same, add this index to the lists of duplicates
                        duplicateRows.Add(j);
                        duplicateIndex.Add(j);
                    }
                }

                if (duplicateIndex.Count == 0)
                {
                    continue; // If there are no duplicates of this row, move on to the next row 
                }
                else
                {
                    // Consolidate information
                    for (int j = 0; j < duplicateIndex.Count; j++)
                    {
                        // If an element is in one row and not the other, add it
                        for (int k = 0; k < 57; k++)
                        {
                            // In the case of positive and negative regulators, make lists of each...
                            if (k == 32 || k == 33)
                            {
                                // ...
                            }
                            else
                            {
                                if (LoadedFileInfo.modelDataAsPittArray[i, k] == null || LoadedFileInfo.modelDataAsPittArray[duplicateIndex[j], k] == null)
                                {
                                    continue; // Continue if both values are null
                                }
                                if (LoadedFileInfo.modelDataAsPittArray[i, k] == null || LoadedFileInfo.modelDataAsPittArray[duplicateIndex[j], k] != null)
                                {
                                    // If the original is null and the duplicate has a value, set the original's value to the duplicate's
                                    LoadedFileInfo.modelDataAsPittArray[i, k] = LoadedFileInfo.modelDataAsPittArray[duplicateIndex[j], k];
                                }
                            }
                        }
                    }
                }

            }

            if (duplicateRows.Count > 0)
            {
                string[,] newArray = new string[LoadedFileInfo.modelDataAsPittArray.GetLength(0) - duplicateRows.Count, 57];
                int indexOffset = 0;

                for (int i = 0; i < LoadedFileInfo.modelDataAsPittArray.GetLength(0); i++)
                {
                    if (duplicateRows.Contains(i))
                    {
                        indexOffset++;
                        continue;
                    }
                    else
                    {
                        for (int j = 0; j < 57; j++)
                        {
                            newArray[i - indexOffset, j] = LoadedFileInfo.modelDataAsPittArray[i, j];
                        }
                    }
                }

                LoadedFileInfo.modelDataAsPittArray = newArray;

                // Update modelDataAsPitt string for printing
                pittDataArrayToString();
            }

        }

        // Removes rows with no primary element or according to other specified settings
        public static void removeUndesirableRows()
        {
            int length = LoadedFileInfo.modelDataAsPittArray.GetLength(0);
            int dimension = LoadedFileInfo.modelDataAsPittArray.GetLength(1);
            List<int> emptyRowIndices = new List<int>();
            
            // Mark rows with no primary element for deletion
            for (int i = 0; i < length; i++)
            {
                if (LoadedFileInfo.modelDataAsPittArray[i, 4] == null)
                {
                    emptyRowIndices.Add(i);
                }
                else
                {
                    // If set to do so, also mark entries where the element name is a sentence (more than 1 word)
                    if (App.removePittEntriesWithMultiWordElementNames)
                    {
                        if (LoadedFileInfo.modelDataAsPittArray[i, 4].Split(new char[] { ' ' }).Length > 1)
                        {
                            emptyRowIndices.Add(i);
                            continue; // Prevents this index from being added twice
                        }
                    }

                    // If set to do so, also mark entries without a regulator
                    if (App.removePittEntriesWithNoRegulator)
                    {
                        if (LoadedFileInfo.modelDataAsPittArray[i, 32] == null && LoadedFileInfo.modelDataAsPittArray[i, 33] == null)
                        {
                            emptyRowIndices.Add(i);
                        }
                    }
                }
            }

            // Make a new data array and fill it with rows that were not marked for deletion
            string[,] newPittArray = new string[length - emptyRowIndices.Count, dimension];
            int numSkipped = 0;

            for (int i = 0; i < length; i++)
            {
                if (emptyRowIndices.Contains(i))
                {
                    // This is a marked row, so skip it (do not add it to the new Pitt array)
                    numSkipped++;
                    continue; 
                }
                for (int j = 0; j < dimension; j++)
                {
                    newPittArray[i - numSkipped, j] = LoadedFileInfo.modelDataAsPittArray[i, j];
                }
            }

            // Set modelDataAsPittArray to equal the new data array
            LoadedFileInfo.modelDataAsPittArray = newPittArray;
            pittDataArrayToString();
        } // end removeUndesirableRows

        // removeExcessColumnsFromPittData removes columns 43+ from Pitt data for printing
        public static void removeExcessColumnsFromPittData()
        {
            StringBuilder modelData = new StringBuilder();

            for (int i = 0; i < LoadedFileInfo.modelDataAsPittArray.GetLength(0); i++)
            {
                string nextLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42}",
                    LoadedFileInfo.modelDataAsPittArray[i, 0], LoadedFileInfo.modelDataAsPittArray[i, 1], LoadedFileInfo.modelDataAsPittArray[i, 2], LoadedFileInfo.modelDataAsPittArray[i, 3], LoadedFileInfo.modelDataAsPittArray[i, 4], LoadedFileInfo.modelDataAsPittArray[i, 5], LoadedFileInfo.modelDataAsPittArray[i, 6], LoadedFileInfo.modelDataAsPittArray[i, 7],
                    LoadedFileInfo.modelDataAsPittArray[i, 8], LoadedFileInfo.modelDataAsPittArray[i, 9], LoadedFileInfo.modelDataAsPittArray[i, 10], LoadedFileInfo.modelDataAsPittArray[i, 11], LoadedFileInfo.modelDataAsPittArray[i, 12], LoadedFileInfo.modelDataAsPittArray[i, 13], LoadedFileInfo.modelDataAsPittArray[i, 14], LoadedFileInfo.modelDataAsPittArray[i, 15],
                    LoadedFileInfo.modelDataAsPittArray[i, 16], LoadedFileInfo.modelDataAsPittArray[i, 17], LoadedFileInfo.modelDataAsPittArray[i, 18], LoadedFileInfo.modelDataAsPittArray[i, 19], LoadedFileInfo.modelDataAsPittArray[i, 20], LoadedFileInfo.modelDataAsPittArray[i, 21], LoadedFileInfo.modelDataAsPittArray[i, 22], LoadedFileInfo.modelDataAsPittArray[i, 23],
                    LoadedFileInfo.modelDataAsPittArray[i, 24], LoadedFileInfo.modelDataAsPittArray[i, 25], LoadedFileInfo.modelDataAsPittArray[i, 26], LoadedFileInfo.modelDataAsPittArray[i, 27], LoadedFileInfo.modelDataAsPittArray[i, 28], LoadedFileInfo.modelDataAsPittArray[i, 29], LoadedFileInfo.modelDataAsPittArray[i, 30], LoadedFileInfo.modelDataAsPittArray[i, 31],
                    LoadedFileInfo.modelDataAsPittArray[i, 32], LoadedFileInfo.modelDataAsPittArray[i, 33], LoadedFileInfo.modelDataAsPittArray[i, 34], LoadedFileInfo.modelDataAsPittArray[i, 35], LoadedFileInfo.modelDataAsPittArray[i, 36], LoadedFileInfo.modelDataAsPittArray[i, 37], LoadedFileInfo.modelDataAsPittArray[i, 38], LoadedFileInfo.modelDataAsPittArray[i, 39],
                    LoadedFileInfo.modelDataAsPittArray[i, 40], LoadedFileInfo.modelDataAsPittArray[i, 41], LoadedFileInfo.modelDataAsPittArray[i, 42]);

                modelData.AppendLine(nextLine);
            }

            LoadedFileInfo.modelDataAsPitt = modelData.ToString();
        }


        #endregion

        #region ToMITRE

        public static void translateFRIES_ToMITRE()
        {

        }

        public static void translatePitt_ToMitre()
        {
            // Loop through each row of the array of Pitt data, transferring to the  MITRE array
            for (int i = 0; i < LoadedFileInfo.modelDataAsPittArray.GetLength(0); i++)
            {
                MITRE m = new MITRE();

                //string part_b_text = LoadedFileInfo.modelDataAsPittArray[i, 4].Trim(new char[] { '{', '}' });
                //string[] part_b_entities = part_b_text.Split(new char[] { ',' });
                //if(part_b_entities.Length == 1)
                //{
                //    m.interaction.participant_b.entity_text.Add(LoadedFileInfo.modelDataAsPittArray[i, 4]);
                //    m.interaction.participant_b.entity_type = LoadedFileInfo.modelDataAsPittArray[i, 5];
                //}
                //else
                //{
                //    for(int j = 0; i < part_b_entities.Length; j++)
                //    {
                //        m.interaction.participant_b.entity_text.Add(part_b_entities[j]);
                //    }
                //    m.interaction.participant_b.entity_type = "protein complex";
                //}

                m.interaction.participant_b.entity_type = LoadedFileInfo.modelDataAsPittArray[i, 5];

                if(m.interaction.participant_b.entity_type == "complex")
                {
                    string part_b_text = LoadedFileInfo.modelDataAsPittArray[i, 4].Trim(new char[] { '{', '}' });
                    string[] part_b_entities = part_b_text.Split(new char[] { ',' });
                    m.interaction.participant_b.entity_text.Add(part_b_entities[0]);

                    for (int j = 0; j < part_b_entities.Length; j++)
                    {
                        MITRE.interaction_.partB.entity_ thisEntity = new MITRE.interaction_.partB.entity_();
                        thisEntity.entity_text = part_b_entities[j];
                        m.interaction.participant_b.entities.Add(thisEntity);
                    }
                }
                else
                {
                    m.interaction.participant_b.entity_text.Add(LoadedFileInfo.modelDataAsPittArray[i, 4]);
                    m.interaction.participant_b.entities = null; // Set the list of entities in a complex to null so it is not serialized for a non-complex entity
                }

                // If database ID fields are blank or marked as "ungrounded", handle the merging of them differently
                if (LoadedFileInfo.modelDataAsPittArray[i, 6] == "ungrounded" || LoadedFileInfo.modelDataAsPittArray[i, 6] == null)
                {
                    m.interaction.participant_b.identifier = LoadedFileInfo.modelDataAsPittArray[i, 6];
                }
                else
                {
                    // Database name + ":" + database ID yields something along the lines of "Uniprot:23521".
                    m.interaction.participant_b.identifier = LoadedFileInfo.modelDataAsPittArray[i, 6] + ":" + LoadedFileInfo.modelDataAsPittArray[i, 7];
                }

                // If there is only one regulator and it is positive...
                if (LoadedFileInfo.modelDataAsPittArray[i, 32] != null && LoadedFileInfo.modelDataAsPittArray[i, 33] == null)
                {
                    m.interaction.participant_a.entity_text.Add(LoadedFileInfo.modelDataAsPittArray[i, 32]);
                    m.interaction.interaction_type = "increases";
                }
                // If there is only one regulator and it is negative... 
                if (LoadedFileInfo.modelDataAsPittArray[i, 32] == null && LoadedFileInfo.modelDataAsPittArray[i, 33] != null)
                {
                    m.interaction.participant_a.entity_text.Add(LoadedFileInfo.modelDataAsPittArray[i, 33]);
                    m.interaction.interaction_type = "decreases";
                }

                m.interaction.participant_a.entity_type = LoadedFileInfo.modelDataAsPittArray[i, 45];
                m.pmc_id = LoadedFileInfo.modelDataAsPittArray[i, 42];
                // If database ID fields for participant_a are blank or marked as "ungrounded", handle the merging of them differently
                if (LoadedFileInfo.modelDataAsPittArray[i, 46] == "ungrounded" || LoadedFileInfo.modelDataAsPittArray[i, 46] == null)
                {
                    m.interaction.participant_a.identifier = LoadedFileInfo.modelDataAsPittArray[i, 46];
                }
                else
                {
                    // Database name + ":" + database ID yields something along the lines of "Uniprot:23521".
                    m.interaction.participant_a.identifier = LoadedFileInfo.modelDataAsPittArray[i, 46] + ":" + LoadedFileInfo.modelDataAsPittArray[i, 47];
                }

                LoadedFileInfo.modelDataAsMITRE[i] = JsonConvert.SerializeObject(m, Formatting.Indented, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
            }
        } // end translatePitt_ToMitre

        // Returns the sum of all MITRE files for display in the Contents text box
        public static string displayMITRE()
        {
            StringBuilder MITREContent = new StringBuilder();

            for (int i = 0; i < LoadedFileInfo.modelDataAsMITRE.Length; i++)
            {
                MITREContent.AppendLine(LoadedFileInfo.modelDataAsMITRE[i]);
            }

            //App.MyTextBoxes.TextContentBox = MITREContent.ToString();
            return MITREContent.ToString();

        } // end displayMITRE

        // Writes a series of MITRE files, with one reaction described in each file.
        public static void printMITRE(string path)
        {
            System.IO.Directory.CreateDirectory(@path);

            for (int i = 0; i < LoadedFileInfo.modelDataAsMITRE.Length; i++)
            {
                string subPath = path + "\\" + i.ToString();
                System.IO.File.WriteAllText(@subPath, LoadedFileInfo.modelDataAsMITRE[i]);
            }
        } // end printMITRE

        #endregion

        #region ToFRIES
        public static void translateToFRIES()
        {
            FRIES friesData = new FRIES();
            friesData.object_meta.doc_id = LoadedFileInfo.modelDataAsPittArray[0, 42]; // Get PMC ID (reference)
            
            for(int i = 0; i < LoadedFileInfo.modelDataAsPittArray.GetLength(0); i++)
            {
                if(LoadedFileInfo.modelDataAsPittArray[i, 4] == null)
                {
                    continue;
                }
                else
                {
                    FRIES.frame frame = new FRIES.frame();
                    frame.subtype = LoadedFileInfo.modelDataAsPittArray[i, 35];

                    // Attach context information, if available
                    if (LoadedFileInfo.modelDataAsPittArray[i, 43] != null)
                    {
                        frame.context.Add(LoadedFileInfo.modelDataAsPittArray[i, 43]);
                    }
                    else
                    {
                        frame.context = null; // Set to null so that it is ignored in serialization
                    }

                    FRIES.args argument1 = new FRIES.args();
                    argument1.text = LoadedFileInfo.modelDataAsPittArray[i, 4]; // Set element name
                    argument1.arg = LoadedFileInfo.modelDataAsPittArray[i, 44]; // Set entity mention ID, if available

                    // Check to see if this element is positively regulated, negatively regulated, or neither
                    if (LoadedFileInfo.modelDataAsPittArray[i, 32] != null)
                    {
                        // This frame contains a positive regulator. Set the frame's reaction type and the regulator's name
                        FRIES.args argument2 = new FRIES.args();
                        frame.subtype = "positive-activation"; // Reaction type
                        argument2.text = LoadedFileInfo.modelDataAsPittArray[i, 32]; // Regulator's name
                        argument2.type = "controller"; // Argument type
                        argument1.type = "controlled"; // Argument type
                        argument2.arg = LoadedFileInfo.modelDataAsPittArray[i, 55]; // Set entity mention ID, if available

                        frame.arguments.Add(argument2); // Add the argument to the frame
                    }
                    if(LoadedFileInfo.modelDataAsPittArray[i, 33] != null)
                    {
                        FRIES.args argument2 = new FRIES.args();
                        frame.subtype = "negative-activation";
                        argument2.text = LoadedFileInfo.modelDataAsPittArray[i, 33]; // Regulator's name
                        argument2.type = "controller"; // Argument type
                        argument1.type = "controlled"; // Argument type
                        argument2.arg = LoadedFileInfo.modelDataAsPittArray[i, 55]; // Set entity mention ID, if available

                        frame.arguments.Add(argument2); // Add the argument to the frame
                    }
                    if (LoadedFileInfo.modelDataAsPittArray[i, 32] == null && LoadedFileInfo.modelDataAsPittArray[i, 33] == null)
                    {
                        frame.subtype = LoadedFileInfo.modelDataAsPittArray[i, 35]; // Sets reaction type to the 
                        // stored value, likely "phorphorylation" for a single-argument reaction
                        argument1.type = "theme"; // Sets special argument type for single-argument reactions
                    }

                    frame.arguments.Add(argument1); // Add argument1 to the frame
                    friesData.frames.Add(frame); // Add the frame to the framelist
                }
            }

            // Serialize the data to a FRIES string, indenting line and ignoring null values
            LoadedFileInfo.modelDataAsFRIES = JsonConvert.SerializeObject(friesData, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            // Replace underscores with hyphens (C# does not allow FRIES class variable creation using hyphens)
            LoadedFileInfo.modelDataAsFRIES = LoadedFileInfo.modelDataAsFRIES.Replace('_', '-');
            //LoadedFileInfo.modelDataAsFRIES = LoadedFileInfo.modelDataAsFRIES.Replace(null, "");
        } // end translateToFRIES

        #endregion



    }
}

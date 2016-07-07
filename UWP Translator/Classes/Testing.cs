using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace UWP_Translator.Classes
{
    public static class Testing
    {
        public static string results(string test_format)
        {
            if (!App.testModeEnabled)
            {
                return "Testing mode has not been enabled in the settings pane.";
            }

            StringBuilder resultsText = new StringBuilder();

            if (test_format == "MITREFolder")
            {
                test_format = "MITRE";
            }

            switch (test_format)
            {
                case "FRIES":
                    resultsText.AppendLine("Translation successful? " + (!FRIES_test.translationFailed).ToString() + "." );
                    resultsText.AppendLine("Number of frames in file: " + FRIES_test.numFrames );
                    resultsText.AppendLine("Number of frames with the same primary element: " + FRIES_test.numDuplicateFrames);
                    resultsText.AppendLine("Number of frames without regulators: " + FRIES_test.missing.regulator_preTrans);
                    //resultsText.AppendLine("Frame upon which failure occured: " + FRIES_test.failedFrame);
                    resultsText.AppendLine("Total untranslated fields: " + FRIES_test.untranslatedFields);
                    resultsText.AppendLine("Total empty fields in original format: " + FRIES_test.emptyFieldsPreTranslation);
                    resultsText.AppendLine("Total empty fields in translated format: " + FRIES_test.emptyFieldsPostTranslation);
                    resultsText.AppendLine("Number of exceptions thrown: " + FRIES_test.numExceptionsThrown);

                    //resultsText.AppendLine("\nMissing FRIES argument types before translation: " + FRIES_test.missing.FRIESArgumentType_preTrans);

                    // Calculate translation completeness (%) as number of populated fields post-translation divided by number of populated fields pre-translation
                    double translationCompletenessFRIES = 100.0 * (double)(7 * FRIES_test.numFrames - FRIES_test.emptyFieldsPostTranslation) / (double)(7 * FRIES_test.numFrames - FRIES_test.emptyFieldsPreTranslation);

                    resultsText.AppendLine("\nTranslation completeness: " + translationCompletenessFRIES + "%");

                    resultsText.AppendLine("\nMissing element names before translation: " + FRIES_test.missing.elementName_preTrans);
                    resultsText.AppendLine("Missing regulators before translation: " + FRIES_test.missing.regulator_preTrans);
                    resultsText.AppendLine("Missing reaction types before translation: " + FRIES_test.missing.mechanismType_preTrans);
                    resultsText.AppendLine("Missing context IDs before translation: " + FRIES_test.missing.contextID_preTrans);
                    resultsText.AppendLine("Missing element entity-mention IDs before translation: " + FRIES_test.missing.elementEntityID_preTrans);
                    resultsText.AppendLine("Missing regulator entity-mention IDs before translation: " + FRIES_test.missing.regulatorEntityID_preTrans);
                    resultsText.AppendLine("Missing PMC IDs before translation: " + FRIES_test.missing.PMCID_preTrans);

                    resultsText.AppendLine("\nMissing element names after translation: " + FRIES_test.missing.elementName_postTrans);
                    resultsText.AppendLine("Missing regulator names after translation: " + FRIES_test.missing.regulator_postTrans);
                    resultsText.AppendLine("Missing reaction types after translation: " + FRIES_test.missing.mechanismType_postTrans);
                    resultsText.AppendLine("Missing context IDs after translation: " + FRIES_test.missing.contextID_postTrans);
                    resultsText.AppendLine("Missing element entity-mention IDs after translation: " + FRIES_test.missing.elementEntityID_postTrans);
                    resultsText.AppendLine("Missing regulator entity-mention IDs after translation: " + FRIES_test.missing.regulatorEntityID_postTrans);
                    resultsText.AppendLine("Missing PMC IDs after translation: " + FRIES_test.missing.PMCID_postTrans);

                    resultsText.AppendLine("\nFailed frame contents, if any: \n" + FRIES_test.failedFrameContents);
                    break;

                case "MITRE":
                    resultsText.AppendLine("Translation successful? " + (!MITRE_test.translationFailed).ToString() + ".");
                    resultsText.AppendLine("Number of files: " + MITRE_test.numFiles);
                    resultsText.AppendLine("Number of files with the same primary element: " + MITRE_test.numDuplicateFiles);
                    resultsText.AppendLine("Number of files without regulators: " + MITRE_test.numFilesWithoutRegulators);
                    resultsText.AppendLine("Files upon which failure occured: " + MITRE_test.failedFileName);
                    resultsText.AppendLine("Total untranslated fields: " + MITRE_test.untranslatedFields);
                    resultsText.AppendLine("Total empty fields in original files: " + MITRE_test.emptyFieldsPreTranslation);
                    resultsText.AppendLine("Total empty fields in translated file(s): " + MITRE_test.emptyFieldsPostTranslation);
                    resultsText.AppendLine("Number of exceptions thrown: " + MITRE_test.numExceptionsThrown);

                    double translationCompletenessMITRE = 100.0 * (double)(7 * MITRE_test.numFiles - FRIES_test.emptyFieldsPostTranslation) / (double)(7 * MITRE_test.numFiles - MITRE_test.emptyFieldsPreTranslation);
                    resultsText.AppendLine("\nTranslation completeness: " + translationCompletenessMITRE + "%");

                    resultsText.AppendLine("\nMissing element names before translation: " + MITRE_test.missing.elementName_preTrans);
                    resultsText.AppendLine("Missing regulators before translation: " + MITRE_test.missing.regulator_preTrans);
                    resultsText.AppendLine("Missing reaction types before translation: " + MITRE_test.missing.mechanismType_preTrans);
                    resultsText.AppendLine("Missing PMC IDs before translation: " + MITRE_test.missing.PMCID_preTrans);

                    resultsText.AppendLine("\nMissing element names after translation: " + MITRE_test.missing.elementName_postTrans);
                    resultsText.AppendLine("Missing regulator names after translation: " + MITRE_test.missing.regulator_postTrans);
                    resultsText.AppendLine("Missing reaction types after translation: " + MITRE_test.missing.mechanismType_postTrans);
                    resultsText.AppendLine("Missing PMC IDs after translation: " + MITRE_test.missing.PMCID_postTrans);

                    resultsText.AppendLine("\nFailed frame contents, if any: \n" + MITRE_test.failedFileContents);
                    break;
                case "Pitt":
                    break;
            }

            return resultsText.ToString();
        }

        // FRIES tests
        public static class FRIES_test
        {
            public static _missing missing = new _missing();
            public static int numFrames { get; set; }
            public static int numDuplicateFrames { get; set; }
            public static int numFramesWithoutRegulators { get; set; }
            public static bool translationFailed { get; set; }
            public static int failedFrame { get; set; }
            public static string failedFrameContents { get; set; }
            public static int untranslatedFields { get; set; }
            public static int emptyFieldsPreTranslation { get; set; }
            public static int emptyFieldsPostTranslation { get; set; }
            public static int numExceptionsThrown { get; set; }

            public class _missing
            {
                public int elementName_preTrans { get; set; }
                public int regulator_preTrans { get; set; }
                public int mechanismType_preTrans { get; set; }
                public int contextID_preTrans { get; set; }
                public int elementEntityID_preTrans { get; set; }
                public int regulatorEntityID_preTrans { get; set; }
                public int PMCID_preTrans { get; set; }
                public int FRIESArgumentType_preTrans { get; set; }

                public int elementName_postTrans { get; set; }
                public int regulator_postTrans { get; set; }
                public int mechanismType_postTrans { get; set; }
                public int contextID_postTrans { get; set; }
                public int elementEntityID_postTrans { get; set; }
                public int regulatorEntityID_postTrans { get; set; }
                public int PMCID_postTrans { get; set; }
            }

        }

        // MITRE tests
        public static class MITRE_test
        {
            public static _missing missing = new _missing();
            public static int numFiles;
            public static int numComplexes;
            public static int numDuplicateFiles { get; set; }
            public static int numFilesWithoutRegulators { get; set; }
            public static int untranslatedFields { get; set; }
            public static int emptyFieldsPreTranslation { get; set; }
            public static int emptyFieldsPostTranslation { get; set; }
            public static bool translationFailed = false;
            public static string failedFileName { get; set; }
            public static string failedFileContents { get; set; }
            public static int numExceptionsThrown { get; set; }

            public class _missing
            {
                public int elementName_preTrans { get; set; }
                public int regulator_preTrans { get; set; }
                public int mechanismType_preTrans { get; set; }
                public int PMCID_preTrans { get; set; }

                public int elementName_postTrans { get; set; }
                public int regulator_postTrans { get; set; }
                public int mechanismType_postTrans { get; set; }
                public int PMCID_postTrans { get; set; }
            }

        }

        // Pitt tests
        public static class Pitt_test
        {

        }

        public static List<frames> FRIES_JsonToObject(dynamic framesList)
        {
            FRIES_test.numFrames = framesList.Count;
            
            List<frames> allFrames = new List<frames>();

            for (int i = 0; i < FRIES_test.numFrames; i++)
            {
                frames thisFrame = new frames();
                try { thisFrame.frame_id = framesList[i]["frame-id"]; }
                catch
                {
                    FRIES_test.numExceptionsThrown++;
                }

                try { thisFrame.subtype = framesList[i].subtype; }
                catch
                {
                    FRIES_test.numExceptionsThrown++;
                }

                try { thisFrame.type = framesList[i].type; }
                catch
                {
                    FRIES_test.numExceptionsThrown++;
                }

                try
                {
                    if (framesList[i].context != null)
                    {
                        thisFrame.context.Add(framesList[i].context[0].ToString());
                    }
                }
                catch
                {
                    FRIES_test.numExceptionsThrown++;
                }

                try
                {
                    for (int j = 0; j < framesList[i].arguments.Count; j++)
                    {
                        frames.args thisArg = new frames.args();
                        string argString = JsonConvert.SerializeObject(framesList[i].arguments[j]);
                        thisArg = JsonConvert.DeserializeObject<frames.args>(argString);
                        thisFrame.arguments.Add(thisArg);
                    }
                }
                catch
                {
                    FRIES_test.numExceptionsThrown++;
                }
                

                allFrames.Add(thisFrame);
            }
            return allFrames;
        }

        public static async Task translateFromFRIES(Windows.Storage.IStorageFile file)
        {
            // DECLARE DIAGNOSTIC VARIABLES
            FRIES_test.numFrames = 0;
            FRIES_test.numDuplicateFrames = 0;
            FRIES_test.numFramesWithoutRegulators = 0;
            FRIES_test.translationFailed = false;
            FRIES_test.failedFrame = 0;
            FRIES_test.failedFrameContents = null;
            FRIES_test.untranslatedFields = 0;
            FRIES_test.emptyFieldsPreTranslation = 0;
            FRIES_test.emptyFieldsPostTranslation = 0;
            FRIES_test.numExceptionsThrown = 0;

            FRIES_test.missing.FRIESArgumentType_preTrans = 0;
            FRIES_test.missing.contextID_postTrans = 0;
            FRIES_test.missing.contextID_preTrans = 0;
            FRIES_test.missing.elementEntityID_postTrans = 0;
            FRIES_test.missing.elementEntityID_preTrans = 0;
            FRIES_test.missing.elementName_postTrans = 0;
            FRIES_test.missing.elementName_preTrans = 0;
            FRIES_test.missing.mechanismType_postTrans = 0;
            FRIES_test.missing.mechanismType_preTrans = 0;
            FRIES_test.missing.PMCID_postTrans = 0;
            FRIES_test.missing.PMCID_preTrans = 0;
            FRIES_test.missing.regulatorEntityID_postTrans = 0;
            FRIES_test.missing.regulatorEntityID_preTrans = 0;
            FRIES_test.missing.regulator_postTrans = 0;
            FRIES_test.missing.regulator_preTrans = 0;
            // END DECLARE DIAGNOSTIC VARIABLES

            string jsonData = await LoadedFileInfo.readTextFile(file);
            dynamic data = JsonConvert.DeserializeObject(jsonData);

            List<frames> allFrames = FRIES_JsonToObject(data.frames);

            int count = allFrames.Count;
            LoadedFileInfo.modelDataAsPittArray = new string[count, 57];

            // Make a row for each FRIES frame (JSON object) in the file
            for (int i = 0; i < count; i++)
            {
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
                    // There is more than one argument in this case.
                    for (int j = 0; j < allFrames[i].arguments.Count; j++)
                    {
                        string text = allFrames[i].arguments[j].text;
                        switch (allFrames[i].arguments[j].type)
                        {
                            case "controlled":
                                // Make this the element under study
                                LoadedFileInfo.modelDataAsPittArray[i, 4] = text;
                                // Store entity mention ID for potential future processing
                                LoadedFileInfo.modelDataAsPittArray[i, 44] = allFrames[i].arguments[j].arg;
                                break;
                            case "controller":
                                // Make this the regulator, depending on its value
                                switch (allFrames[i].subtype)
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
                                        LoadedFileInfo.modelDataAsPittArray[i, 35] = allFrames[i].subtype;
                                        break;
                                }
                                // Store this regulator's entity mention ID for potential future processing - useful for translation to the MITRE format
                                LoadedFileInfo.modelDataAsPittArray[i, 55] = allFrames[i].arguments[j].arg;
                                break;

                            case "theme":
                                if (j == 0)
                                {
                                    LoadedFileInfo.modelDataAsPittArray[i, 4] = text;
                                    // Extract "entity-mention" ID for later analysis
                                    LoadedFileInfo.modelDataAsPittArray[i, 44] = allFrames[i].arguments[0].arg;
                                }
                                if (j ==1 && allFrames[i].arguments[0].type != "site")
                                {
                                    LoadedFileInfo.modelDataAsPittArray[i, 32] = text;
                                    LoadedFileInfo.modelDataAsPittArray[i, 35] = allFrames[i].type;
                                    // Store this regulator's entity mention ID for potential future processing - useful for translation to the MITRE format
                                    LoadedFileInfo.modelDataAsPittArray[i, 55] = allFrames[i].arguments[j].arg;
                                }
                                if (j == 1 && allFrames[i].arguments[0].type == "site")
                                {
                                    LoadedFileInfo.modelDataAsPittArray[i, 4] = text;
                                    // Extract "entity-mention" ID for later analysis
                                    LoadedFileInfo.modelDataAsPittArray[i, 44] = allFrames[i].arguments[0].arg;
                                }
                                break;

                            case "site":
                                if (allFrames[i].arguments.Count < 3)
                                {
                                    LoadedFileInfo.modelDataAsPittArray[i, 35] = allFrames[i].subtype;
                                    // Store this regulator's entity mention ID for potential future processing - useful for translation to the MITRE format
                                    LoadedFileInfo.modelDataAsPittArray[i, 55] = allFrames[i].arguments[j].arg;
                                }
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
            Translators.pittDataArrayToString();

            // DIAGNOSTIC
            // Pre-translation missing field calculations

            // Check to see if entity/regulator names exist
            for (int i = 0; i < allFrames.Count; i++)
            {
                if (allFrames[i].arguments.Count == 1)
                {
                    FRIES_test.missing.regulator_preTrans++;
                }
                if (allFrames[i].arguments.Count == 2)
                {
                    if (allFrames[i].arguments[0].type == "site" || allFrames[i].arguments[1].type == "site") // Count has not having a regulator if there are two arguments and one is a "site"
                    {
                        FRIES_test.missing.regulator_preTrans++;
                    }
                }


                for (int j = 0; j < allFrames[i].arguments.Count; j++)
                {
                    if (allFrames[i].arguments[j].type == "controlled" && allFrames[i].arguments[j].text == null)
                    {
                        FRIES_test.missing.elementName_preTrans++; // Is the primary element, has a regulator
                        continue;
                    }
                    if (allFrames[i].arguments[j].text == null && allFrames[i].arguments.Count == 1)
                    {
                        FRIES_test.missing.elementName_preTrans++; // Is the only reactant
                        continue;
                    }
                    if (allFrames[i].arguments[j].type == "theme" && allFrames[i].arguments[j].text == null && j == 0)
                    {
                        FRIES_test.missing.elementName_preTrans++; // At least one theme, this one is the primary element
                        continue;
                    }
                    if (allFrames[i].arguments[j].type == "controller" && allFrames[i].arguments[j].text == null)
                    {
                        FRIES_test.missing.regulator_preTrans++; //  Is the regulator
                        continue;
                    }
                    if (allFrames[i].arguments[j].type == "theme" && allFrames[i].arguments[j].text == null && j > 0 && allFrames[i].arguments[0].type == "theme")
                    {
                        FRIES_test.missing.regulator_preTrans++; // More than one theme, this one is the regulator
                        continue;
                    }
                }

                // Check to see if entity mention IDs exist
                for (int j = 0; j < allFrames[i].arguments.Count; j++)
                {
                    if (allFrames[i].arguments[j].type == "controlled" && allFrames[i].arguments[j].arg == null)
                    {
                        FRIES_test.missing.elementEntityID_preTrans++; // Is the primary element, has a regulator
                        continue;
                    }
                    if (allFrames[i].arguments[j].arg == null && allFrames[i].arguments.Count == 1)
                    {
                        FRIES_test.missing.elementEntityID_preTrans++; // Is the only reactant
                        continue;
                    }
                    if (allFrames[i].arguments[j].type == "theme" && allFrames[i].arguments[j].arg == null && j == 0)
                    {
                        FRIES_test.missing.elementEntityID_preTrans++; // At least one theme, this one is the primary element
                        continue;
                    }
                    if (allFrames[i].arguments[j].type == "controller" && allFrames[i].arguments[j].arg == null)
                    {
                        FRIES_test.missing.regulatorEntityID_preTrans++; //  Is the regulator
                        continue;
                    }
                    if (allFrames[i].arguments[j].type == "theme" && allFrames[i].arguments[j].arg == null && j > 0 && allFrames[i].arguments[0].type == "theme")
                    {
                        FRIES_test.missing.regulatorEntityID_preTrans++; // More than one theme, this one is the regulator
                        continue;
                    }
                }

                if (allFrames[i].type == null && allFrames[i].subtype == null) // Check to see if reaction type exists
                {
                    FRIES_test.missing.mechanismType_preTrans++;
                }

                if (allFrames[i].frame_id == null) // Check to see if PMC ID exists
                {
                    FRIES_test.missing.PMCID_preTrans++;
                }

                if (allFrames[i].context.Count == 0) // Check to see if context ID exists
                {
                    FRIES_test.missing.contextID_preTrans++;
                }
            }

            FRIES_test.emptyFieldsPreTranslation = FRIES_test.missing.contextID_preTrans + FRIES_test.missing.elementEntityID_preTrans 
                + FRIES_test.missing.elementName_preTrans + FRIES_test.missing.mechanismType_preTrans + FRIES_test.missing.PMCID_preTrans 
                + FRIES_test.missing.regulatorEntityID_preTrans + FRIES_test.missing.regulator_preTrans;


            List<string> allElements = new List<string>();

            // Post-translation missing field calculations
            for (int i = 0; i < LoadedFileInfo.modelDataAsPittArray.GetLength(0); i++)
            {
                if (LoadedFileInfo.modelDataAsPittArray[i, 4] == null) // Element name
                {
                    FRIES_test.missing.elementName_postTrans++;
                }
                else
                {
                    if (allElements.Contains(LoadedFileInfo.modelDataAsPittArray[i, 4]))
                    {
                        FRIES_test.numDuplicateFrames++;
                    }
                    else
                    {
                        allElements.Add(LoadedFileInfo.modelDataAsPittArray[i, 4]);
                    }
                }

                if (LoadedFileInfo.modelDataAsPittArray[i, 32] == null && LoadedFileInfo.modelDataAsPittArray[i, 33] == null) // Regulator names
                {
                    FRIES_test.missing.regulator_postTrans++;
                }

                if (LoadedFileInfo.modelDataAsPittArray[i, 35] == null) // Reaction mechanism
                {
                    FRIES_test.missing.mechanismType_postTrans++;
                }

                if (LoadedFileInfo.modelDataAsPittArray[i, 42] == null) // PMC ID
                {
                    FRIES_test.missing.PMCID_postTrans++;
                }

                if (LoadedFileInfo.modelDataAsPittArray[i, 43] == null) // Context ID
                {
                    FRIES_test.missing.contextID_postTrans++;
                }

                if (LoadedFileInfo.modelDataAsPittArray[i, 44] == null) // Primary element's entity-mention ID
                {
                    FRIES_test.missing.elementEntityID_postTrans++;
                }

                if (LoadedFileInfo.modelDataAsPittArray[i, 55] == null && (LoadedFileInfo.modelDataAsPittArray[i, 32] != null || LoadedFileInfo.modelDataAsPittArray[i, 33] != null)) // Regulator's entity-mention ID
                {
                    FRIES_test.missing.regulatorEntityID_postTrans++; // Regulator's entity-mention ID
                }
            }

            FRIES_test.emptyFieldsPostTranslation = FRIES_test.missing.contextID_postTrans + FRIES_test.missing.elementEntityID_postTrans
                + FRIES_test.missing.elementName_postTrans + FRIES_test.missing.mechanismType_postTrans + FRIES_test.missing.PMCID_postTrans
                + FRIES_test.missing.regulatorEntityID_postTrans + FRIES_test.missing.regulator_postTrans;
            // END DIAGNOSTIC

        } // end translateFromFRIES

        public static MITRE_data MITRE_JsonToObject(string fileContents)
        {
            MITRE_data mitreData = new MITRE_data();


            return mitreData;
        }


        public static void translateFromMITRE(int Index)
        {
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

                    MITRE_test.numComplexes++;
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
                catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
                {
                    // Report exception
                    MITRE_test.numExceptionsThrown++;
                    MITRE_test.failedFileName = LoadedFileInfo.fileList[Index].Name + " Not a Complex!";
                }
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
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
            {
                // Report exception
                MITRE_test.numExceptionsThrown++;
                MITRE_test.failedFileName = LoadedFileInfo.fileList[Index].Name;
            }
        } // end translateFromMITRE

        public static void translateMultipleMITRE()
        {
            // DECLARE DIAGNOSTIC VARIABLES
            MITRE_test.numFiles = 0;
            MITRE_test.emptyFieldsPostTranslation = 0;
            MITRE_test.emptyFieldsPreTranslation = 0;
            MITRE_test.failedFileContents = null;
            MITRE_test.failedFileName = null;
            MITRE_test.numComplexes = 0;
            MITRE_test.numDuplicateFiles = 0;
            MITRE_test.numExceptionsThrown = 0;
            MITRE_test.numFilesWithoutRegulators = 0;
            MITRE_test.translationFailed = false;
            MITRE_test.untranslatedFields = 0;

            MITRE_test.missing.elementName_postTrans = 0;
            MITRE_test.missing.elementName_preTrans = 0;
            MITRE_test.missing.mechanismType_postTrans = 0;
            MITRE_test.missing.mechanismType_preTrans = 0;
            MITRE_test.missing.PMCID_postTrans = 0;
            MITRE_test.missing.PMCID_preTrans = 0;
            MITRE_test.missing.regulator_postTrans = 0;
            MITRE_test.missing.regulator_preTrans = 0;
            // END DECLARE DIAGNOSTIC VARIABLES


            LoadedFileInfo.modelDataAsPittArray = new string[LoadedFileInfo.fileList.Count, 57];
            //LoadedFileInfo.modelDataAsMITRE = new string[LoadedFileInfo.fileList.Count];

            for (int i = 0; i < LoadedFileInfo.fileList.Count; i++)
            {
                translateFromMITRE(i);
            }

            // Convert FileInfo.modelDataAsPittArray into a string for printing
            Translators.pittDataArrayToString();
        } // end translateMultipleMITRE








    }
}

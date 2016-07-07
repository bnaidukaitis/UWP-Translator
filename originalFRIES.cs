using System;

public class Class1
{
	public Class1()
	{
        // Make a row for each FRIES frame (JSON object) in the file
        for (int i = 0; i < count; i++)
        {
            dynamic myData = data[i];
            dynamic args = myData.arguments;
            LoadedFileInfo.modelDataAsPittArray[i, 56] = file.Name;
            if (data[i].arguments.Count == 1)
            {
                // Make this the element under study
                try
                {
                    LoadedFileInfo.modelDataAsPittArray[i, 4] = args[0].text.ToString();
                }
                catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
                {
                    // If element name field does not exist, continue with the next object
                    continue;
                }

                // Name the event taking place (e.g., "phosphorylation")
                try
                {
                    LoadedFileInfo.modelDataAsPittArray[i, 35] = myData.subtype.ToString();
                    // Extract "entity-mention" ID for later analysis
                    LoadedFileInfo.modelDataAsPittArray[i, 44] = args[0].arg.ToString();
                }
                catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
                {
                    // Do nothing
                }
            }
            else
            {
                int argCount = args.Count;
                // There is more than one argument in this case.
                for (int j = 0; j < argCount; j++)
                {
                    string text = args[j].text.ToString();
                    string argType = args[j].type.ToString();
                    switch (argType)
                    {
                        case "controlled":
                            // Make this the element under study
                            LoadedFileInfo.modelDataAsPittArray[i, 4] = text;
                            // Store entity mention ID for potential future processing
                            try { LoadedFileInfo.modelDataAsPittArray[i, 44] = args[j].arg.ToString(); }
                            catch { /* Do Nothing */ }
                            break;
                        case "controller":
                            // Make this the regulator, depending on its value
                            string subType = myData.subtype.ToString();
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
                            try { LoadedFileInfo.modelDataAsPittArray[i, 55] = args[j].arg.ToString(); }
                            catch { /* Do Nothing */ }
                            break;
                        default:
                            break;
                    }
                }
            }

            // Extract PubMed references
            string frameID = myData["frame-id"].ToString();
            LoadedFileInfo.modelDataAsPittArray[i, 42] = frameID.Substring(frameID.IndexOf("PMC"), frameID.IndexOf("PMC") + frameID.Substring(0, frameID.IndexOf("PMC")).IndexOf("-"));
            // Extract FRIES context information for later analysis
            try { LoadedFileInfo.modelDataAsPittArray[i, 43] = myData.context[0].ToString(); }
            catch { /* Do nothing */ }
        }
    }
}

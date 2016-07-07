using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_Translator.Classes
{
    class Pitt
    {
        public string FullElementName { get; set; }
        public string Importance { get; set; }
        public string ElementName { get; set; }
        public string ElementType { get; set; }
        public string DatabaseID { get; set; }
        public string ElementID { get; set; }
        public string CellLine { get; set; }
        public string CellType { get; set; }
        public string Organism { get; set; }
        public string TissueType { get; set; }
        public string Location { get; set; }
        public string LocationIdentifier { get; set; }
        public string Notes1 { get; set; }
        public string VariableName { get; set; }
        public string ModelIO { get; set; }
        public string SpontRestDeg { get; set; }
        public string TypeOfValue { get; set; }
        public string Begin1 { get; set; }
        public string End1 { get; set; }
        public string SimEnd1 { get; set; }
        public string Begin2 { get; set; }
        public string End2 { get; set; }
        public string SimEnd2 { get; set; }
        public string Begin3 { get; set; }
        public string End3 { get; set; }
        public string SimEnd3 { get; set; }
        public string Notes2 { get; set; }
        public string NumReg { get; set; }
        public string Positive { get; set; }
        public string Negative { get; set; }
        public string InteractionDirIndir { get; set; }
        public string MechType { get; set; }
        public string Notes3 { get; set; }
        public string UniqueID { get; set; }
        public string ElementKind { get; set; }
        public string ElementSubtype { get; set; }
        public string Reference { get; set; }
        public string FRIESContextID { get; set; }
        public string FRIESEntityMentionID { get; set; }

        public static class Header
        {
            public static string[] modelData = new string[57] { "#", // 0
            "Full Element Name",// 1
            "Importance",       // 2
            "",                 // 3
            "Element name",     // 4
            "Element type",     // 5
            "Database ID",      // 6
            "Element ID",       // 7
            "Cell line",        // 8
            "Cell type",        // 9
            "Organism",         // 10
            "Tissue type",      // 11
            "Location",         // 12
            "Location identifier",  // 13
            "NOTES",            // 14
            "",                 // 15
            "Variable name",    // 16
            "Model Input (I) Output (O)",   // 17
            "Spontaneously Restores (R) Degrades(D)",   // 18
            "Type of value: Activity (A) Amount (C) Process (P)", // 19
            "Begin",    // 20
            "End",      // 21
            "Sim End",  // 22
            "Begin",    // 23
            "End",      // 24
            "Sim End",  // 25
            "Begin",    // 26
            "End",      // 27
            "Sim End",  // 28
            "NOTES",    // 29
            "",         // 30
            "No. of reg.",  // 31
            "Positive", // 32
            "Negative", // 33
            "Interaction Direct (D) Indirect (I)",  // 34
            "Mechanism type for direct (D)",        // 35
            "NOTES",                            // 36
            "",                                 // 37
            "Unique ID (text)",                 // 38
            "Element kind",                     // 39
            "Element sub-type",                 // 40
            "",                                 // 41
            "References for model connections", // 42 (PMC ID)
            "FRIES Context ID",                 // 43
            "FRIES Entity-mention ID",          // 44
            "MITRE participant_a entity_type",  // 45
            "MITRE participant_a database name",   // 46
            "MITRE participant_a database identifer", // 47
            "MITRE participant_a cell line", // 48
            "MITRE participant_a cell type", // 49
            "MITRE participant_a organism",  // 50
            "MITRE participant_a tissue type",  // 51
            "MITRE participant_a location", // 52
            "MITRE participant_a location identifier",   // 53
            "MITRE participant_a FRIES context ID", // 54
            "MITRE participant_a FRIES entity-mention ID",   // 55
            "File Name" // 56
            };

            public static string header = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33},{34},{35},{36},{37},{38},{39},{40},{41},{42}",//{43},{44},{45},{46},{47},{48},{49},{50},{51},{52},{53},{54},{55},{56}",
                modelData[0], modelData[1], modelData[2], modelData[3], modelData[4], modelData[5], modelData[6], modelData[7],
                modelData[8], modelData[9], modelData[10], modelData[11], modelData[12], modelData[13], modelData[14], modelData[15],
                modelData[16], modelData[17], modelData[18], modelData[19], modelData[20], modelData[21], modelData[22], modelData[23],
                modelData[24], modelData[25], modelData[26], modelData[27], modelData[28], modelData[29], modelData[30], modelData[31],
                modelData[32], modelData[33], modelData[34], modelData[35], modelData[36], modelData[37], modelData[38], modelData[39],
                modelData[40], modelData[41], modelData[42]);//, modelData[43], modelData[44], modelData[45], modelData[46], modelData[47],
                //modelData[48], modelData[49], modelData[50], modelData[51], modelData[52], modelData[53], modelData[54], modelData[55], modelData[56]);

        }
    }
}

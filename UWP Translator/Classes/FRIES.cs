using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_Translator.Classes
{
    public class FRIES
    {
        public List<frame> frames = new List<frame>();
        public string object_type = "frame-collection";
        public objMetaData object_meta = new objMetaData();

        public class frame
        {
            public string type { get; set; } // Broader mechanism, e.g. "protein-modificiation"
            public string frame_type = "event-mention"; // convert string to "frame-type" before final printing
            public string subtype { get; set; } // Mechanism, e.g. "phorphorylation"
            public List<args> arguments = new List<args>(); // Convert to array before final printing
            public List<string> context = new List<string>();
        }

        public class args
        {
            public string text { get; set; } // entity name
            public string argument_type = "entity"; 
            public string type { get; set; } // controlled, controller, or "theme" (if arguments.Count = 0)
            public string arg { get; set; } // entity-mention ID
        }

        public class objMetaData
        {
            public string doc_id { get; set; } // The PMC ID (reference article)
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_Translator.Classes
{
    public class MITRE
    {
        // MITRE translations consist of a string giving the PMC database reference and an "interaction" field
        public string pmc_id { get; set; }
        public interaction_ interaction = new interaction_();

        // Define the subclass "interaction"
        public class interaction_
        {
            // Members of the class "interation" instantiated above
            public string interaction_type { get; set; }
            public partA participant_a = new partA();
            public partB participant_b = new partB();

            // Define the subclasses (partA and partB) instantiated above
            public class partA
            {
                public List<string> entity_text = new List<string>();
                public string entity_type { get; set; }
                public string identifier { get; set; }
            }

            public class partB
            {
                public List<string> entity_text = new List<string>();
                public List<entity_> entities = new List<entity_>(); // for complexes
                public string entity_type { get; set; }
                public string identifier { get; set; }

                public class entity_
                {
                    public string entity_text { get; set; }
                    public string entity_type { get; set; }
                    public string identifier { get; set; }
                }

            }

        }

    }
}

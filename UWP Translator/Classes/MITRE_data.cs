using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_Translator.Classes
{
    public class MITRE_data
    {
        public string pmc_id { get; set; }
        public _interaction interaction { get; set; }

        public class _interaction
        {
            public string interaction_type { get; set; }
            
        }


    }
}

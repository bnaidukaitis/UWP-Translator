using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWP_Translator.Classes
{
    public class frames
    {
        public string frame_id { get; set; }
        public string type { get; set; }
        public string subtype { get; set; }
        public List<args> arguments = new List<args>();
        public List<string> context = new List<string>();

        public class args
        {
            public string text { get; set; }
            //public string argument_type { get; set; }
            public string type { get; set; }
            //public string object_type { get; set; }
            //public string index { get; set; }
            public string arg { get; set; }
        }
    }
}


/*
 * "arguments":[
        {
          "text":"Akt",
          "argument-type":"entity",
          "type":"controlled",
          "object-type":"argument",
          "index":0,
          "arg":"ment-PMC534114-UAZ-r1-6-1-28"
        },
        {
          "text":"EGF",
          "argument-type":"entity",
          "type":"controller",
          "object-type":"argument",
          "index":0,
          "arg":"ment-PMC534114-UAZ-r1-6-1-26"
        }
      ],
      "type":"activation",
      "frame-type":"event-mention",
      "subtype":"positive-activation",

    */
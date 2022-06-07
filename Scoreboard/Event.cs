using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoreboard
{
    class Event
    {
        public string id { get; set; }
        public string name { get; set; }
        public List<Phase> phases { get; set; }
    }
}

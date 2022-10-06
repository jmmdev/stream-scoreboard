using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoreboard
{
    class Tournament
    {
        public string name { get; set; }
        public string slug { get; set; }
        public int state { get; set; }
        public List<Event> events { get; set; }
    }
}

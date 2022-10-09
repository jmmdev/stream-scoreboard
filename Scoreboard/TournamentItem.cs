using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoreboard
{
    class TournamentItem
    {
        public string id { get; set; }
        public string name { get; set; }

        public TournamentItem(string id, string name)
        {
            this.id = id;
            this.name = name.Length > 40 ? name.Substring(0, 40) + "..." : name;
        }
    }
}

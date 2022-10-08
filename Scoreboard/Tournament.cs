using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoreboard
{
    public class Tournament
    {
        public string id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public int state { get; set; }
        public List<Event> events { get; set; }
        public List<Image> images { get; set; }

        public override int GetHashCode()
        {
            return int.Parse(id);
        }

        public override bool Equals(object other)
        {
            if (other is Tournament)
                return ((Tournament)other).id == id;
            return false;
        }
    }
}

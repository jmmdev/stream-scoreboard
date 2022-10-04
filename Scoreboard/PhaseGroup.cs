using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scoreboard
{
    class PhaseGroup
    {
        public string id { get; set; }
        public string displayIdentifier { get; set; }

        public SetConnection sets { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameJam.Models
{
    [Serializable]
    class GameData
    {
        public int Breads { get; set; }
        public int BreadsPerSecond { get; set; }
        public int BreadsPerClick { get; set; }
        public List<Upgrade> Upgrades { get; set; }
        public List<Achievement> Achievements { get; set; }

    }
}

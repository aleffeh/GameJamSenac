using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameJam.Models
{
    [Serializable]
    class Upgrade
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CostText { get; set; }
        public int Cost { get; set; }
        public int PPSUpgrade { get; set; }
        public bool Bought { get; set; }
        
    }
}

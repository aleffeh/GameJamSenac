using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameJam.Models
{
    [Serializable]
    class Achievement
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public bool Acquired { get; set; }

    }
}

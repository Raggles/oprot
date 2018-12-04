using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace oprot.database.core
{
    public class ProtectedPlant
    {
        public int ProtectedPlantId { get; set; }
        public string PlantId { get; set; }
        public string Substation { get; set; }
        public int? AssetNumber { get; set; }
        public virtual ICollection<ProtectionRelay> ProtectionRelays { get; set; } = new List<ProtectionRelay>();
    }
}

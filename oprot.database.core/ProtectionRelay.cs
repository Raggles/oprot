using System;
using System.Collections.Generic;

namespace oprot.database.core
{
    public class ProtectionRelay
    {
        public int ProtectionRelayId { get; set; }
        public ProtectedPlant Plant { get; set; }
        public int ProtectedPlantId { get; set; }
        public int? AssetNumber { get; set; }
        public string Prot { get; set; }
        public string RelayModel { get; set; }
        public string SettingsLocation { get; set; }
        public virtual ICollection<ProtectionElement> ProtectionElements { get; set; } = new List<ProtectionElement>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UF_Healthemia.Models.HealthemiaModels;

namespace UF_Healthemia.Serializable
{
    public class SerializableLimbsModel
    {
        public ulong SteamId { get; set; }
        public HealthemiaLimb Head { get; set; }
        public HealthemiaLimb Thorax { get; set; }
        public HealthemiaLimb Back { get; set; }
        public HealthemiaArmLimb LeftArm { get; set; }
        public HealthemiaArmLimb RightArm { get; set; }
        public HealthemiaLegLimb LeftLeg { get; set; }
        public HealthemiaLegLimb RightLeg { get; set; }

    }
}

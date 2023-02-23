using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SDG.Unturned.ItemConsumeableAsset;

namespace UF_Healthemia.Models
{
    public class HealthemiaHealContext
    {
        public HealthemiaHealData HealData => _healData;
        public KeyValuePair<bool, byte> HealAllLimbsData => _healAllLimbsData;
        private HealthemiaHealData _healData;
        private KeyValuePair<bool, byte> _healAllLimbsData;

        

        internal void ChangeHealData(ELimb limb, ItemMedicalAsset asset)
        {
            _healData = new HealthemiaHealData(limb, asset);
        }

        internal void ChangeHealAllLimbsData(float healingPoints)
        {
            _healAllLimbsData = new KeyValuePair<bool, float>(true, healingPoints);
        }
        
    }

    public class HealthemiaHealData
    {
        public ELimb Limb { get; private set; }
        public byte HealingPoints { get; private set; }

        public bool HealBleeding { get; private set; }

        public bool HealBroken { get; private set; }


        public HealthemiaHealData(ELimb limb, ItemMedicalAsset medicalAsset)
        {
            Limb = limb;
            HealingPoints = medicalAsset.health;
            HealBleeding = medicalAsset.bleedingModifier is Bleeding.Heal;
            HealBroken = medicalAsset.bonesModifier is Bones.Heal;
        }
    }
}

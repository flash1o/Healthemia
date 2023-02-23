using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SDG.Unturned;
using Rocket.Unturned;
using UF_Healthemia.Models.Medication;
using UF_Healthemia.Models;
using UF_Healthemia.Components;

namespace UF_Healthemia.Helpers
{
    internal class HealthemiaHealer
    {
        private List<DiseaseMedication> _diseaseMedications;
        private byte _stateRevivingHealingPoints;
        private Healthemia _instance => Healthemia.Instance;
        public HealthemiaHealer()
        {
            _diseaseMedications = new List<DiseaseMedication>();
        }

        internal bool IsDiseaseMedication(ItemMedicalAsset asset, out DiseaseMedication medication)
        {
            medication = _diseaseMedications.Find(x => x.ItemId == asset.id);

            return medication != null;
        }
        internal void ApplyDiseaseMedication(Player player, DiseaseMedication medication)
        {
            _instance.DiseasesService.ApplyDurationReduction(player, medication.MedicationPoints);
        }

        internal void ApplyManualMedication(Player player, ItemMedicalAsset asset, ELimb limb)
        {
            var playerComponent = player.GetComponent<HealthemiaPlayerComponent>();

            var healContext = new HealthemiaHealContext();
            healContext.ChangeHealData(limb, asset);
        }

        internal void ApplyStatusMedication(Player player)
        {
            var playerComponent = player.GetComponent<HealthemiaPlayerComponent>();

            var healContext = new HealthemiaHealContext();
            healContext.ChangeHealAllLimbsData(_stateRevivingHealingPoints);
            playerComponent.CauseHeal(healContext);
            playerComponent.CauseHealState();
        }
    }
}

using Rocket.Unturned.Events;
using System;
using System.Collections.Generic;
using Rocket.Unturned;
using SDG.Unturned;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UF_Healthemia.Providers;
using UF_Healthemia.Models.Diseases;
using UF_Healthemia.Models;
using UF_Healthemia.Components;

namespace UF_Healthemia.Helpers
{
    internal class HealthemiaConsumeEventHandler : MonoBehaviour
    {
        private HealthemiaDiseaseInfector _infector => Healthemia.Instance.DiseasesService.Infector;
        private HealthemiaConfiguration _configuration => Healthemia.Instance.Configuration.Instance;
        private HealthemiaDiseasesProvider _provider => Healthemia.Instance.DiseasesService.Provider;

        private HealthemiaHealer _healer => Healthemia.Instance.Healer;


        private void Start()
        {
            UseableConsumeable.onConsumePerformed += OnConsumedPerformed;
            UseableConsumeable.onPerformingAid += OnPerformingAid;
          

        }

        private void OnPerformingAid(Player instigator, Player target, ItemConsumeableAsset asset, ref bool shouldAllow)
        {
            if (target.GetComponent<HealthemiaPlayerComponent>().IsPlayerAlive)
                shouldAllow = false;

            if (asset.id != _configuration.HealUnconsniousOrDyingItem)
                shouldAllow = false;

            _healer.ApplyStatusMedication(Player player); 
        }

        private void OnConsumedPerformed(Player instigatingPlayer, ItemConsumeableAsset consumeableAsset)
        {
            if (consumeableAsset is ItemFoodAsset foodAsset)
            {
                if (foodAsset.name.Contains("Raw"))
                    _infector.TryInfect(instigatingPlayer, _provider.GetRandomDiseaseOfType<Poisoning>());
                return;
            }

            if (consumeableAsset is ItemMedicalAsset medicalAsset)
            {
                if (_healer.IsDiseaseMedication(medicalAsset, out var medication))
                    _healer.ApplyDiseaseMedication(instigatingPlayer, medication);
                else
                    _healer.ApplyAutoLimbsHealing(instigatingPlayer, medicalAsset);
                return;
            }
        }

        
    }
}

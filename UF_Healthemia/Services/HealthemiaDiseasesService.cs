using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UF_Healthemia.Helpers;
using UF_Healthemia.Models;
using UF_Healthemia.Providers;
using UnityEngine;
using SDG.Unturned;

namespace UF_Healthemia.Services
{
    internal class HealthemiaDiseasesService : MonoBehaviour
    {
        public HealthemiaDiseaseInfector Infector { get; private set; }
        public HealthemiaDiseasesProvider Provider { get; private set; }
        private List<HealthemiaActiveDisease> _activatedDiseases;
        private float _diseaseProcessRefreshDelay;
        private float _diseaseSymptomsChance;
        private bool _shouldShowSymptoms => UnityEngine.Random.value > _diseaseSymptomsChance / 100;

        private void Start()
        {
            _activatedDiseases = new List<HealthemiaActiveDisease>();
            Infector = new HealthemiaDiseaseInfector();
            Provider = new HealthemiaDiseasesProvider();
            StartCoroutine(DiseaseProcess());
        }

        private IEnumerator DiseaseProcess()
        {
            while (true)
            {
                yield return new WaitForSeconds(_diseaseProcessRefreshDelay);

                if (!_activatedDiseases.Any())
                    continue;

                foreach (var disease in _activatedDiseases)
                {
               
                    if (_shouldShowSymptoms)
                        disease.ShowSymptoms();

                    disease.Duration -= _diseaseProcessRefreshDelay;
                }
                
            }
            
        }

        internal void ActivateNewDisease(HealthemiaActiveDisease activeDisease) => _activatedDiseases.Add(activeDisease);

        internal void ApplyDurationReduction(Player player, float reduction)
        {
            var activeDisease = _activatedDiseases.FirstOrDefault(x => x.PlayerSteamId == player.channel.owner.playerID.steamID.m_SteamID);
            activeDisease.Duration -= reduction;
        }

        internal void DeactivateDisease(HealthemiaActiveDisease activeDisease) => _activatedDiseases.Remove(activeDisease);
        

        
    }
}

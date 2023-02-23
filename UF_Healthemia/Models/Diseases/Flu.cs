using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SDG.Unturned;
using Rocket.Unturned.Player;
using UF_Healthemia.Helpers;

namespace UF_Healthemia.Models.Diseases
{
    internal class Flu : HealthemiaDisease
    {
       
        private HealthemiaConfiguration _configuration => Healthemia.Instance.Configuration.Instance;
        private HealthemiaDiseaseInfector _infector => Healthemia.Instance.DiseasesService.Infector;
        private ushort _coughSound;
        private ushort _runnyNoseSound;
        private ushort _sneezingSound;
        private float _othersInfectionChance;
        private float _infectionRadius;



        public override void ShowSymptoms(Player player)
        {
            float random = UnityEngine.Random.Range(0, 2);

            switch (random)
            {
                case 0:
                    Cough(player);
                    break;
                case 1:
                    RunnyNose(player);
                    break;
                case 2:
                    Sneeze(player);
                    break;
                default:
                    break;
            }
        }

        private void Cough(Player player)
        {
            EffectManager.sendUIEffect(_coughSound, _configuration.SecondaryUIKey, player.channel.owner.transportConnection, true);
        }

        private void RunnyNose(Player player)
        {
            EffectManager.sendUIEffect(_runnyNoseSound, _configuration.SecondaryUIKey, player.channel.owner.transportConnection, true);
           
        }
 
        private void Sneeze(Player player)
        {
            EffectManager.sendUIEffect(_sneezingSound, _configuration.SecondaryUIKey, player.channel.owner.transportConnection, true);

            List<Player> nearbyPlayers = new List<Player>();
            PlayerTool.getPlayersInRadius(player.transform.position, _infectionRadius, nearbyPlayers);

            foreach  (var nearbyPlayer in nearbyPlayers)
            {
                if (UnityEngine.Random.value > _othersInfectionChance / 100)
                    _infector.TryInfect(nearbyPlayer, this);  
            }
        }



    }

}

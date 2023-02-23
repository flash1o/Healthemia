using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UF_Healthemia.Components;
using UF_Healthemia.Models;
using UF_Healthemia.Models.Diseases;
using Logger = Rocket.Core.Logging.Logger;

namespace UF_Healthemia.Helpers
{
    internal class HealthemiaDiseaseInfector
    {
    
        internal bool TryInfect(Player player, HealthemiaDisease disease)
        {
            if (HasDiseaseOfSameType(player, disease)) 
                return false;

            if (!(UnityEngine.Random.value > disease.InfectionChance / 100))
                return false;

            disease.Activate(player);
            return true;
        }

        internal void TryInfectAllPlayers(HealthemiaDisease disease)
        {
            int infected = 0;
            
            Task.Run(() => {
                foreach (var steamPlayer in Provider.clients)
                {
                    if (TryInfect(steamPlayer.player, disease))
                        infected++;
                }

                Logger.Log($"Infected {infected} players with {disease.Name}!");
            });
            
        }


        private bool HasDiseaseOfSameType(Player player, HealthemiaDisease disease)
        {
            return player.GetComponent<HealthemiaPlayerComponent>().HasDiseaseOfSameType(disease);
        }

    }
}

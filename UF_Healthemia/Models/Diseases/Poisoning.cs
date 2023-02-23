using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using SDG.Unturned;
using Rocket.Unturned.Player;

namespace UF_Healthemia.Models.Diseases
{
    internal class Poisoning : HealthemiaDisease
    {
       
        private HealthemiaConfiguration _configuration => Healthemia.Instance.Configuration.Instance;
        private ushort _rumblingSound;
       
   

        public override void ShowSymptoms(Player player)
        {
            RumblingStomach(player);
         
        }

        private void RumblingStomach(Player player)
        {
            EffectManager.sendUIEffect(_rumblingSound, _configuration.SecondaryUIKey, player.channel.owner.transportConnection, true);

        }

     
    }

}

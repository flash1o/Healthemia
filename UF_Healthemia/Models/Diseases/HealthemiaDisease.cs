using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UF_Healthemia.Components;
using UnityEngine;

namespace UF_Healthemia.Models
{
    internal abstract class HealthemiaDisease
    {
        public string Name { get; private set; }
        public float Duration { get; private set; }
        public float Damage { get; private set; }
        public float InfectionChance { get; private set; }

        public abstract void ShowSymptoms(Player player);
        public void Activate(Player player)
        {
            Healthemia.Instance.DiseasesService.ActivateNewDisease(new HealthemiaActiveDisease(player, this));
            player.GetComponent<HealthemiaPlayerComponent>().
        }
        

      

       

       
       
    }

}

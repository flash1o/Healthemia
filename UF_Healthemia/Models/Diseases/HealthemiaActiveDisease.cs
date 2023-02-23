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
    internal class HealthemiaActiveDisease
    {
        public ulong PlayerSteamId => _player.channel.owner.playerID.steamID.m_SteamID;
        private Player _player;
        private HealthemiaDisease _diseaseType;
        public float Duration
        {
            get { return _duration; }
            set 
            {
                if (value < 1)
                {
                    Deactivate();
                    return;
                }

                _duration = value;
         
            }
        }

        private float _duration;

        public HealthemiaActiveDisease(Player player, HealthemiaDisease disease)
        {
            _player = player;
            _diseaseType = disease;
            Duration = disease.Duration;
        }
        
        internal void ShowSymptoms()
        {
            _diseaseType.ShowSymptoms(_player);
        }
        private void Deactivate()
        {
            _player.GetComponent<HealthemiaPlayerComponent>().CauseHeal(_diseaseType);
            Healthemia.Instance.DiseasesService.DeactivateDisease(this);
        }
       
       
    }

}

using System;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UF_Healthemia.Models;
using UnityEngine;

namespace UF_Healthemia.Components
{
    public class HealthemiaPlayerComponent : MonoBehaviour
    {
        private Player _player;
      
        private HealthemiaPlayerHealth _playerHealth;

        private void Start()
        {
            _player = GetComponentInParent<Player>();
            _playerHealth = new HealthemiaPlayerHealth(_player);

            _player.life.onHurt += OnHurt;
        }

        private void OnHurt(Player player, byte damage, Vector3 force, EDeathCause cause, ELimb limb, CSteamID killer)
        {
            
        }
    }
}
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
        public bool IsPlayerAlive => _playerHealth.AbleToMove;
        public bool IsInDraggingAction;
        
        private Player _player;
      
        private HealthemiaPlayerHealth _playerHealth;

        private void Start()
        {
            _player = GetComponentInParent<Player>();
            _playerHealth = new HealthemiaPlayerHealth(_player);

            _player.stance.onStanceUpdated += OnStanceUpdated;
            _player.life.onHurt += OnHurt;
        }

        private void OnStanceUpdated()
        {
            if (!_playerHealth.AbleToMove)
                _player.stance.checkStance(EPlayerStance.PRONE, true);
        }

        private void OnHurt(Player player, byte damage, Vector3 force, EDeathCause cause, ELimb limb, CSteamID killer)
        {
            _playerHealth.SendDamage(limb, cause,  damage);
        }
    }
}
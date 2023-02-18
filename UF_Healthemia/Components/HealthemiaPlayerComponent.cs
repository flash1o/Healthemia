using System;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using UF_Healthemia.Helpers.Handlers;
using UF_Healthemia.Models;
using UF_Healthemia.Models.HealthemiaModels;
using UF_Healthemia.Tools;
using UF_Healthemia.UI.Context;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace UF_Healthemia.Components
{
    public class HealthemiaPlayerComponent : MonoBehaviour
    {
        public bool IsPlayerAlive => _playerHealth.AbleToMove; 
        public int BrokenArmsCount => _playerHealth.BrokenArmsCount;
        public bool IsInDraggingAction;

        private Player _player;
        private HealthemiaPlayerHealth _playerHealth;
        private HealthemiaPlayerEventHandler _eventHandler => Healthemia.Instance.PlayerHealthService.EventHandler;

        private void Start()
        {
            _player = GetComponentInParent<Player>();
            _playerHealth = new HealthemiaPlayerHealth(_player);
            _player.stance.onStanceUpdated += () => _eventHandler.HandleUpdatedStance(_player, _playerHealth);
            _player.equipment.onEquipRequested += (PlayerEquipment equipment, ItemJar jar, ItemAsset asset, ref bool allow) => _eventHandler.HandleEquipRequested(_playerHealth, equipment, jar, asset, ref allow);
        }
        

        public void CauseDamage(byte damage, EDeathCause cause, ELimb limb)
        {
            _playerHealth.SendDamage(limb, cause, damage);
        }

        public void CauseHeal(ItemConsumeableAsset asset, ELimb limb)
        {
            _playerHealth.SendHeal(asset, limb);
        }

        internal HealthemiaUIContext GetLimbsUIContext()
        {
            return _playerHealth.GetUIContextRepresentation();
        }
    }
}
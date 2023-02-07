using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Rocket.Unturned;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UF_Healthemia.Components;
using UF_Healthemia.Extensions;
using UF_Healthemia.Models;
using UnityEngine;

namespace UF_Healthemia.Services
{
    public class HealthemiaDraggingService : MonoBehaviour
    {
        private List<HealthemiaDraggingAction> _draggingActions;
        private Coroutine _draggingCoroutine;
        
        private void Awake()
        {
            _draggingActions = new List<HealthemiaDraggingAction>();
            _draggingCoroutine = StartCoroutine(DraggingCoroutine());
            UnturnedPlayerEvents.OnPlayerUpdateGesture += OnGestureChanged;
        }

        private void OnGestureChanged(UnturnedPlayer player, UnturnedPlayerEvents.PlayerGesture gesture)
        {
            if (gesture != UnturnedPlayerEvents.PlayerGesture.SurrenderStart && player.Player.stance.stance != EPlayerStance.CROUCH)
            {
                if (player.GetComponent<HealthemiaPlayerComponent>().IsInDraggingAction)
                    StopDraggingAction(Player.player);
                return;
            }

            if (!player.Player.TryGetPlayerInView(out Player target)) return;

            var targetHealthemiaPlayer = target.GetComponent<HealthemiaPlayerComponent>();
            if (targetHealthemiaPlayer.IsPlayerAlive || targetHealthemiaPlayer.IsInDraggingAction) return;
            
            _draggingActions.Add(new HealthemiaDraggingAction(target, player.Player));
        }

        private IEnumerator DraggingCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1.5f);
                if (!_draggingActions.Any()) continue;
                
                _draggingActions.ForEach(x => x.Drag());
            }
        }

        private void StopDraggingAction(Player player)
        {
            var action = _draggingActions.FirstOrDefault(x => x.Contains(player.channel.owner.playerID.steamID.m_SteamID));
            
            action.Stop();
            _draggingActions.Remove(action);
        }
    }
}
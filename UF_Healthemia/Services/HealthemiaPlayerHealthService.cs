using System;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using UF_Healthemia.Components;
using UF_Healthemia.Helpers;
using UF_Healthemia.Helpers.Handlers;
using UF_Healthemia.Tools;
using UF_Healthemia.UI;
using UnityEngine;

namespace UF_Healthemia.Services
{
    internal class HealthemiaPlayerHealthService : MonoBehaviour
    {
        public HealthemiaPlayerHealthHelper HealthHelper { get; private set; }
        public HealthemiaPlayerModifierTool ModifierTool { get; private set; }
        public HealthemiaHealthUIRepresenter UIRepresenter { get; private set; }
        public HealthemiaPlayerEventHandler EventHandler { get; private set; }
        private void Awake()
        {
            UnturnedEvents.OnPlayerDamaged += OnPlayerDamaged;
            HealthHelper = new HealthemiaPlayerHealthHelper();
            ModifierTool = new HealthemiaPlayerModifierTool();
            UIRepresenter = new HealthemiaHealthUIRepresenter();
            EventHandler = new HealthemiaPlayerEventHandler();
        }

        private void OnPlayerDamaged(UnturnedPlayer player, ref EDeathCause cause, ref ELimb limb, ref UnturnedPlayer killer, ref Vector3 direction, ref float damage, ref float times, ref bool candamage)
        {
            if (cause is EDeathCause.SUICIDE or EDeathCause.KILL)
                return;

            candamage = false;
            player.Player.GetComponent<HealthemiaPlayerComponent>().CauseDamage((byte)Math.Ceiling(damage), cause, limb);

        }
    }
}
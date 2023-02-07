using Rocket.Unturned.Player;
using SDG.Unturned;
using UnityEngine;

namespace UF_Healthemia.Extensions
{
    public static class HealthemiaPlayerExtensions
    {
        public static bool TryGetPlayerInView(this Player player, out Player target)
        {
            if (!Physics.Raycast(player.look.aim.position, player.look.aim.forward, out var hit, 100f, RayMasks.PLAYER))
            {
                target = null;
                return false;
            }
            
            target = hit.transform.GetComponentInParent<Player>();
            return true;
        }
    }
}
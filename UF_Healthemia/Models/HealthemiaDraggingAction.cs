using SDG.Unturned;
using UF_Healthemia.Components;

namespace UF_Healthemia.Models
{
    public class HealthemiaDraggingAction
    {
        private Player _draggedPlayer;
        private Player _draggerPlayer;
        private HealthemiaPlayerComponent _draggedPlayerComponent;
        private HealthemiaPlayerComponent _draggerPlayerComponent;

        public HealthemiaDraggingAction(Player injuredPlayer, Player draggerPlayer)
        {
            _draggedPlayer = injuredPlayer;
            _draggerPlayer = draggerPlayer;
            _draggedPlayerComponent = injuredPlayer.GetComponent<HealthemiaPlayerComponent>();
            _draggerPlayerComponent = draggerPlayer.GetComponent<HealthemiaPlayerComponent>();
            _draggedPlayerComponent.IsInDraggingAction = true;
            _draggerPlayerComponent.IsInDraggingAction = true;
        }

        public void Drag()
        {
            _draggedPlayer.teleportToPlayer(_draggerPlayer);
        }

        public bool Contains(ulong steamId)
        {
            return _draggedPlayer.channel.owner.playerID.steamID.m_SteamID == steamId || _draggerPlayer.channel.owner.playerID.steamID.m_SteamID == steamId;
        }

        public void Stop()
        {
            _draggedPlayerComponent.IsInDraggingAction = false;
            _draggerPlayerComponent.IsInDraggingAction = false;
        }
    }
}
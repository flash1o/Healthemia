using SDG.Unturned;

namespace UF_Healthemia.Models
{
    public class HealthemiaDraggingAction
    {
        private Player _draggedPlayer;
        private Player _draggerPlayer;

        public HealthemiaDraggingAction(Player injuredPlayer, Player draggerPlayer)
        {
            _draggedPlayer = injuredPlayer;
            _draggerPlayer = draggerPlayer;
        }

        public void Drag()
        {
            _draggedPlayer.teleportToPlayer(_draggerPlayer);
        }
    }
}
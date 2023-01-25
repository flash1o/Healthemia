using Rocket.Core.Plugins;
using Rocket.Unturned;
using SDG.Unturned;
using UF_Healthemia.Services;

namespace UF_Healthemia
{
    public class Healthemia : RocketPlugin<HealthemiaConfiguration>
    {
        public Healthemia Instance;
        private HealthemiaDraggingService _draggingService;
        
        protected override void Load()
        {
            Instance = this;
            
            if (Level.isLoaded)
                OnLevelLoaded(0);
            else
                Level.onLevelLoaded += OnLevelLoaded;
        }

        private void OnLevelLoaded(int level) => InitializeServices();

        protected override void Unload()
        {
            Instance = null;
        }

        private void InitializeServices()
        {
            _draggingService = gameObject.AddComponent<HealthemiaDraggingService>();
        }
    }
}
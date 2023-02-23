using Rocket.Core.Plugins;
using Rocket.Unturned;
using SDG.Unturned;
using UF_Healthemia.Helpers;
using UF_Healthemia.Patches;
using UF_Healthemia.Services;

namespace UF_Healthemia
{
    internal class Healthemia : RocketPlugin<HealthemiaConfiguration>
    {
        //TODO:
        //- Disease Healing (Duration) - DONE
        //- Disease EventHandling - DONE
        //- Healing - DONE, BUT UI SUPPORT
        //- Providers - ****
        //- Health Loading from Database (через Task.Run) - 50% DONE
        //- Event for Reviving / Making Consiosnius - DONE
        //- Refactoring
        public static Healthemia Instance;
        public HealthemiaDraggingService DraggingService { get; private set; }
        public HealthemiaPlayerHealthService PlayerHealthService { get; private set; }
        
        public HealthemiaDiseasesService DiseasesService { get; private set; }

        public HealthemiaHealer Healer { get; private set; }
        protected override void Load()
        {
            Instance = this;
            
            if (Level.isLoaded)
                OnLevelLoaded(0);
            else
                Level.onLevelLoaded += OnLevelLoaded;
            
            HealthemiaHarmony.HealthemiaPatchAll();
        }

        private void OnLevelLoaded(int level) => InitializeServices();

        protected override void Unload()
        {
            Instance = null;
        }

        private void InitializeServices()
        {
            DraggingService = gameObject.AddComponent<HealthemiaDraggingService>();
            PlayerHealthService = gameObject.AddComponent<HealthemiaPlayerHealthService>();
            DiseasesService = gameObject.AddComponent<HealthemiaDiseasesService>();
            Healer = new HealthemiaHealer();
        }
    }
}
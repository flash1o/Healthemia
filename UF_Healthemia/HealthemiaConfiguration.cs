using Rocket.API;

namespace UF_Healthemia
{
    public class HealthemiaConfiguration : IRocketPluginConfiguration
    {
        public int HeadHealth;
        public int TorsoHealth;
        public int BackHealth;
        public int LeftLegHealth;
        public int RightLegHealth;
        public int LeftArmHealth;
        public int RightArmHealth;

        public void LoadDefaults()
        {
            HeadHealth = 55;
            TorsoHealth = 95;
            BackHealth = 75;
            LeftLegHealth = 65;
            RightLegHealth = 65;
            LeftArmHealth = 40;
            RightArmHealth = 40;
        }
    }
}
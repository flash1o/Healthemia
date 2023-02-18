using Rocket.API;

namespace UF_Healthemia
{
    public class HealthemiaConfiguration : IRocketPluginConfiguration
    {
        public byte HeadHealth;
        public byte ThoraxHealth;
        public byte BackHealth;
        public byte LeftLegHealth;
        public byte RightLegHealth;
        public byte LeftArmHealth;
        public byte RightArmHealth;
        public short PrimaryUIKey;
        public short SecondaryUIKey;
        public byte MaxHealthbarElementsCount => 15;

        public void LoadDefaults()
        {
            HeadHealth = 55;
            ThoraxHealth = 95;
            BackHealth = 75;
            LeftLegHealth = 65;
            RightLegHealth = 65;
            LeftArmHealth = 40;
            RightArmHealth = 40;
            PrimaryUIKey = 2220;
            SecondaryUIKey = 2201;
        }

        public int GetMaxHealth()
        {
            return HeadHealth + ThoraxHealth + BackHealth + LeftArmHealth + RightArmHealth + LeftLegHealth + RightLegHealth;
        }
    }
}
using Unity.Collections;

namespace UF_Healthemia.Models.HealthemiaModels
{
    public class HealthemiaLimb
    {
        public int Health { get; set; }
        public bool IsBleeding { get; set; }
        
        public bool IsBlackOuted { get; set; }

        public readonly bool IsVital;

        public HealthemiaLimb(int health, bool isVital)
        {
            Health = health;
            IsVital = isVital;
        }
        
    }
}
namespace UF_Healthemia.Models.HealthemiaModels
{
    public class HealthemiaBreakableLimb : HealthemiaLimb
    {
        public bool IsBroken { get; set; }

        public HealthemiaBreakableLimb(byte health, bool isVital, string representation) : base(health, isVital, representation)
        {
            isVital = false;
        }

        public void PlayBreakEffect()
        {
            // send sound
        }
        
    }
}
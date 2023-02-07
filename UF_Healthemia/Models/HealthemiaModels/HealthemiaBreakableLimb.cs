namespace UF_Healthemia.Models.HealthemiaModels
{
    public class HealthemiaBreakableLimb : HealthemiaLimb
    {
        public bool IsBroken { get; set; }


        public HealthemiaBreakableLimb(int health, bool isVital) : base(health, isVital)
        {
            isVital = false;
        }
    }
}
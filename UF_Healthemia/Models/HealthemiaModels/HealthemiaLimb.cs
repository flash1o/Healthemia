using Unity.Collections;

namespace UF_Healthemia.Models.HealthemiaModels
{
    public class HealthemiaLimb
    {
        public byte Health 
        {
            get => _health;
            set { _health = value > MaxHealth ? MaxHealth : value; }
        }
        
        public bool IsBleeding { get; set; }
        public bool IsBlackOuted { get; set; }

        public readonly bool IsVital;

        public readonly byte MaxHealth;

        private byte _health;

        private readonly string _stringRepresentation;

        public HealthemiaLimb(byte health, bool isVital, string stringRepresentation)
        {
            MaxHealth = health;
            Health = health;
            IsVital = isVital;
            _stringRepresentation = stringRepresentation;
        }
        

        public override string ToString()
        {
            return _stringRepresentation;
        }
    }
}
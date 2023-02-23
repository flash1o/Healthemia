using SDG.Unturned;
using UF_Healthemia.Models.HealthemiaModels;
using UnityEngine;

namespace UF_Healthemia.Helpers
{
    public class HealthemiaDamageCalculator
    {
        private HealthemiaConfiguration _configuration => Healthemia.Instance.Configuration.Instance;
        public bool ShouldKnockout(EDeathCause cause)
        {
            float knockoutChance;
            if (cause is EDeathCause.MELEE)
                knockoutChance = _configuration.MeleeKnockoutChance;
            else
                knockoutChance = _configuration.PunchKnockoutChance;

            if (UnityEngine.Random.value > knockoutChance / 100)
                return true;

            return false;
        }
        public bool ShouldBreakBones(EDeathCause cause)
        {
            float breakingChance = 0;
            switch (cause)
            {
                case EDeathCause.GUN:
                    breakingChance = 35;
                    break;
                case EDeathCause.MELEE:
                    breakingChance = 25;
                    break;
                case EDeathCause.GRENADE:
                    breakingChance = 55;
                    break;
                case EDeathCause.MISSILE:
                case EDeathCause.LANDMINE:
                    breakingChance = 75;
                    break;
                case EDeathCause.PUNCH:
                    breakingChance = 8;
                    break;
                case EDeathCause.VEHICLE:
                    breakingChance = 85;
                    break;
                default:
                    breakingChance = 0;
                    break;
            }
            
            var random = Random.Range(0f, 100f);
            return breakingChance > random;
        }
        
        public bool ShouldCauseBleeding(EDeathCause cause)
        {
            float bleedingChance = 0;
            switch (cause)
            {
                case EDeathCause.GUN:
                    bleedingChance = 50;
                    break;
                case EDeathCause.MELEE:
                    bleedingChance = 35;
                    break;
                case EDeathCause.GRENADE:
                    bleedingChance = 65;
                    break;
                case EDeathCause.MISSILE:
                case EDeathCause.LANDMINE:
                    bleedingChance = 85;
                    break;
                case EDeathCause.PUNCH:
                    bleedingChance = 12;
                    break;
                case EDeathCause.VEHICLE:
                    bleedingChance = 65;
                    break;
                default:
                    bleedingChance = 0;
                    break;
            }
            
            var random = Random.Range(0f, 100f);
            return bleedingChance > random;
        }
        
        
    }
}
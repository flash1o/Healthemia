using SDG.Unturned;
using UF_Healthemia.Models.HealthemiaModels;
using UnityEngine;

namespace UF_Healthemia.Helpers
{
    public class HealthemiaPlayerHealthHelper
    {
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
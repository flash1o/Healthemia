using System;
using UnityEngine;

namespace UF_Healthemia.Models
{
    public class HealthemiaPlayerHealth
    {
        private float _totalHealth => _headHealth + _chestHealth + _stomachHealth + _leftArmHealth + _rightArmHealth + _leftLegHealth + _rightLegHealth;
        private float _headHealth;
        private float _chestHealth;
        private float _stomachHealth;
        private float _leftArmHealth;
        private float _rightArmHealth;
        private float _leftLegHealth;
        private float _rightLegHealth;

        private void ApplyFragmentation()
        {
            
        }

        private void ApplyDamage()
        {
            
        }
    }
}
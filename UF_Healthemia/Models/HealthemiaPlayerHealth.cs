using System;
using System.Runtime.InteropServices;
using SDG.Unturned;
using UF_Healthemia.Models.HealthemiaModels;
using UnityEngine;

namespace UF_Healthemia.Models
{
    public class HealthemiaPlayerHealth
    {
        private Player _player;
        private float _totalHealth => _head.Health + _thorax.Health + _back.Health + _leftArm.Health + _rightArm.Health + _leftLeg.Health + _rightLeg.Health;
        private HealthemiaLimb _head { get; set; }
        private HealthemiaLimb _thorax { get; set; }
        private HealthemiaLimb _back { get; set; }
        private HealthemiaBreakableLimb _leftArm { get; set; }
        private HealthemiaBreakableLimb _rightArm { get; set; }
        private HealthemiaBreakableLimb _leftLeg { get; set; }
        private HealthemiaBreakableLimb _rightLeg { get; set; }

        private HealthemiaPlayerState _healthemiaPlayerState { get; set; }

        private HealthemiaConfiguration _configuration => Healthemia.Instance.Configuration.Instance;

        public HealthemiaPlayerHealth(Player player)
        {
            _player = player;
            _healthemiaPlayerState = HealthemiaPlayerState.Alive;
            InitializeLimbs();
        }

        public void TakeDamage(int damage, HealthemiaLimb limb, bool causesBleeding, bool causesBreakingBones)
        {
            limb.Health -= damage;
            limb.IsBleeding = limb.IsBleeding || causesBleeding;

            if (limb is HealthemiaBreakableLimb breakableLimb)
                breakableLimb.IsBroken = breakableLimb.IsBroken || causesBreakingBones;
        }

        public void InitializeLimbs()
        {
            _head = new HealthemiaLimb(_configuration.HeadHealth, true);
            _thorax = new HealthemiaLimb(_configuration.TorsoHealth, true);
            _back = new HealthemiaLimb(_configuration.BackHealth, false);
            _rightArm = new HealthemiaBreakableLimb(_configuration.RightArmHealth, false);
            _leftArm = new HealthemiaBreakableLimb(_configuration.LeftArmHealth, false);
            _rightLeg = new HealthemiaBreakableLimb(_configuration.RightLegHealth, false);
            _leftLeg = new HealthemiaBreakableLimb(_configuration.LeftLegHealth, false);
        }
        
        public void SetDefaults()
        {
            _head.Health = _configuration.HeadHealth;
            _thorax.Health = _configuration.TorsoHealth;
            _back.Health = _configuration.BackHealth;
            _leftArm.Health = _configuration.LeftArmHealth;
            _rightArm.Health = _configuration.RightArmHealth;
            _leftLeg.Health = _configuration.LeftLegHealth;
            _rightLeg.Health = _configuration.RightLegHealth;
        }
        
        private void SetFeeblesness()
        {
            _player.movement.sendPluginGravityMultiplier(0);
            _player.movement.sendPluginJumpMultiplier(0);
            _player.movement.sendPluginSpeedMultiplier(0);
            _player.stance.checkStance(EPlayerStance.PRONE, true);
        }

        private void SetUnconsnious()
        {
            SetFeeblesness();
        }
        
        private void SetDying()
        {
            SetFeeblesness();
        }
    }

    
}
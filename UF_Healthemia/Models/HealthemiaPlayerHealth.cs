using System;
using System.Runtime.InteropServices;
using SDG.Unturned;
using UF_Healthemia.Helpers;
using UF_Healthemia.Models.HealthemiaModels;
using UnityEngine;

namespace UF_Healthemia.Models
{
    public class HealthemiaPlayerHealth
    {
        public bool AbleToMove => _healthemiaPlayerState is HealthemiaPlayerState.Alive;
        
        private Player _player;
        private float _totalHealth => _head.Health + _thorax.Health + _back.Health + _leftArm.Health + _rightArm.Health + _leftLeg.Health + _rightLeg.Health;
        private HealthemiaLimb _head { get; set; }
        private HealthemiaLimb _thorax { get; set; }
        private HealthemiaLimb _back { get; set; }
        private HealthemiaBreakableLimb _leftArm { get; set; }
        private HealthemiaBreakableLimb _rightArm { get; set; }
        private HealthemiaBreakableLimb _leftLeg { get; set; }
        private HealthemiaBreakableLimb _rightLeg { get; set; }
        private HealthemiaPlayerHealthHelper _helper => Healthemia.Instance.PlayerHealthService.HealthHelper;

        private HealthemiaPlayerState _healthemiaPlayerState
        {
            get => _healthemiaPlayerState;
            set
            {
                switch (value)
                {
                   case HealthemiaPlayerState.Dying:
                       SetDying();
                       break;
                   case HealthemiaPlayerState.Unconscious:
                       SetUnconsnious();
                       break;
                   case HealthemiaPlayerState.Alive:
                       break;
                   default:
                       break;
                }
               
            }
        }

        private HealthemiaConfiguration _configuration => Healthemia.Instance.Configuration.Instance;

        public HealthemiaPlayerHealth(Player player)
        {
            _player = player;
            _healthemiaPlayerState = HealthemiaPlayerState.Alive;
            InitializeLimbs();
        }

        public void SendDamage(ELimb limb, EDeathCause cause, int damage)
        {
            var healthemiaLimb = ConvertUnturnedLimbToHealthemiaLimb(limb);
            bool causeBleeding = _helper.ShouldCauseBleeding(cause);
            bool causeBreakingBones = _helper.ShouldBreakBones(cause);
            TakeDamage(damage, healthemiaLimb, causeBleeding, causeBreakingBones);
        }

        private void TakeDamage(int damage, HealthemiaLimb limb, bool causesBleeding, bool causesBreakingBones)
        {
            limb.Health -= damage;
            limb.IsBleeding = limb.IsBleeding || causesBleeding;

            if (limb is HealthemiaBreakableLimb breakableLimb)
                breakableLimb.IsBroken = breakableLimb.IsBroken || causesBreakingBones;

            if (limb.Health < 0)
            {
                if (limb.IsVital)
                    _healthemiaPlayerState = HealthemiaPlayerState.Dying;
                else
                    limb.IsBlackOuted = true;
            }
        }

        private void InitializeLimbs()
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
            _player.enablePluginWidgetFlag(EPluginWidgetFlags.Modal);
        }

        private void SetUnconsnious()
        {
            SetFeeblesness();
        }
        
        private void SetDying()
        {
            SetFeeblesness();
        }
        
        private HealthemiaLimb ConvertUnturnedLimbToHealthemiaLimb(ELimb limb)
        {
            switch (limb)
            {
                case ELimb.SKULL:
                    return _head;
                case ELimb.LEFT_FRONT:
                    return _thorax;
                case ELimb.RIGHT_FRONT:
                    return _thorax;
                case ELimb.SPINE:
                    return _back;
                case ELimb.LEFT_ARM:
                    return _leftArm;
                case ELimb.RIGHT_ARM:
                    return _rightArm;
                case ELimb.LEFT_HAND:
                    return _leftArm;
                case ELimb.RIGHT_HAND:
                    return _rightArm;
                case ELimb.LEFT_LEG:
                    return _leftLeg;
                case ELimb.RIGHT_LEG:
                    return _rightLeg;
                case ELimb.LEFT_FOOT:
                    return _leftLeg;
                case ELimb.RIGHT_FOOT:
                    return _rightLeg;
                case ELimb.LEFT_BACK:
                    return _back;
                case ELimb.RIGHT_BACK:
                    return _back;
                default:
                    return _thorax;
            }
        }
    }

    
}
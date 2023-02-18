using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using SDG.Unturned;
using Steamworks;
using UF_Healthemia.Extensions;
using UF_Healthemia.Helpers;
using UF_Healthemia.Models.HealthemiaModels;
using UF_Healthemia.Models.UI;
using UF_Healthemia.Tools;
using UF_Healthemia.UI;
using UF_Healthemia.UI.Context;
using UnityEngine;

namespace UF_Healthemia.Models
{
    public class HealthemiaPlayerHealth
    {
        public delegate void LimbDamageHandler(HealthemiaLimb limb);
        public event LimbDamageHandler OnLimbBroken;
        public event LimbDamageHandler OnLimbBlackouted;
        public bool AbleToMove => _healthemiaPlayerState is HealthemiaPlayerState.Alive;
        public int BrokenArmsCount { get; private set; }

        private Player _player;

        private IEnumerable<HealthemiaLimb> _limbs => new List<HealthemiaLimb>
        {
            _head, _thorax, _back, _leftArm, _rightArm, _rightLeg, _leftArm, _leftLeg
        };
        private float _totalHealth => _head.Health + _thorax.Health + _back.Health + _leftArm.Health + _rightArm.Health + _leftLeg.Health + _rightLeg.Health;
        private HealthemiaLimb _head { get; set; }
        private HealthemiaLimb _thorax { get; set; }
        private HealthemiaLimb _back { get; set; }
        private HealthemiaArmLimb _leftArm { get; set; }
        private HealthemiaArmLimb _rightArm { get; set; }
        private HealthemiaLegLimb _leftLeg { get; set; }
        private HealthemiaLegLimb _rightLeg { get; set; }
        private HealthemiaPlayerHealthHelper _helper => Healthemia.Instance.PlayerHealthService.HealthHelper;
        private HealthemiaPlayerModifierTool _modifier => Healthemia.Instance.PlayerHealthService.ModifierTool;
      
        private HealthemiaConfiguration _configuration => Healthemia.Instance.Configuration.Instance;

        private List<HealthemiaWoundInabilities> _inabilities;
        

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

        public bool HasInability(HealthemiaWoundInabilities inability)
        {
            return _inabilities.Contains(inability);
        }

        internal HealthemiaUIContext GetUIContextRepresentation()
        {
            var context = new HealthemiaUIContext();
            foreach (var limb in _limbs)
            {
                context.AddVisibility($"{limb}{GetLimbUIDamageName(limb)}", true);
                context.AddVisibility($"{limb}Bleeding", limb.IsBleeding);
                
                if (limb is HealthemiaBreakableLimb breakableLimb)
                    context.AddVisibility($"{breakableLimb}Broken", breakableLimb.IsBroken);
                
                context.AddBarData($"{limb}HealthBar", limb.Health, limb.MaxHealth);
            }

            context.AddInsertData("LimbsHealth_Current", ((int)Math.Round(_totalHealth)).ToString());
            context.AddInsertData("LimbsHealth_Max", _configuration.GetMaxHealth().ToString());
            
            return context;
        }
        
        private string GetLimbUIDamageName(HealthemiaLimb limb)
        {
            var percentage = 100 - (limb.Health / limb.MaxHealth * 100);

            return percentage > 20 ? "LowDamage" : percentage > 40 ? "MidDamage" : percentage > 70 ? "HighDamage" : "NoDamage";
        }

        public HealthemiaPlayerHealth(Player player)
        {
            _inabilities = new List<HealthemiaWoundInabilities>();
            _player = player;
            _healthemiaPlayerState = HealthemiaPlayerState.Alive;
            InitializeLimbs();
            OnLimbBroken += OnLimbBrokenHandler;
            OnLimbBlackouted += OnLimbBlackoutedHandler;
        }

        private void OnLimbBlackoutedHandler(HealthemiaLimb limb)
        {
            switch (limb)
            {
                case HealthemiaLegLimb:
                    _modifier.ApplySpeedModifier(_player.movement, TypeSpeedModifier.Blackout);
                    TryAddInability(HealthemiaWoundInabilities.SprintingInability);
                    break;
                case HealthemiaArmLimb:
                    TryAddInability(HealthemiaWoundInabilities.EquippingPrimaryInability);
                    break;
            }
        }

        private void OnLimbBrokenHandler(HealthemiaLimb limb)
        {
            switch (limb)
            {
                case HealthemiaLegLimb:
                    _modifier.ApplySpeedModifier(_player.movement, TypeSpeedModifier.Broken); 
                    break;
                case HealthemiaArmLimb:
                    BrokenArmsCount++;
                    break;
            }
        }

        private void TryAddInability(HealthemiaWoundInabilities inability)
        {
            if (!_inabilities.Contains(inability))
                _inabilities.Add(inability);
        }

        public void SendDamage(ELimb limb, EDeathCause cause, byte damage)
        {
            var healthemiaLimb = ConvertUnturnedLimbToHealthemiaLimb(limb);
            bool causeBleeding = _helper.ShouldCauseBleeding(cause);
            bool causeBreakingBones = _helper.ShouldBreakBones(cause);
            TakeDamage(damage, healthemiaLimb, causeBleeding, causeBreakingBones);
        }

        public void SendHeal(ItemConsumeableAsset asset, ELimb limb)
        {
            var healthemiaLimb = ConvertUnturnedLimbToHealthemiaLimb(limb);
            bool stopBleeding = asset.CanStopBleeding();
            bool healBrokenBone = asset.CanHealBrokenBone();
            TakeHeal(asset.health, healthemiaLimb, stopBleeding, healBrokenBone);
        }

        private void TakeDamage(byte damage, HealthemiaLimb limb, bool causesBleeding, bool causesBreakingBones)
        {
            limb.Health -= damage;
            limb.IsBleeding = limb.IsBleeding || causesBleeding;

            if (limb is HealthemiaBreakableLimb breakableLimb)
                breakableLimb.IsBroken = breakableLimb.IsBroken || causesBreakingBones;

            if (limb.Health >= 0) return;
            
            if (limb.IsVital)
                _healthemiaPlayerState = HealthemiaPlayerState.Dying;
            else
                limb.IsBlackOuted = true;
        }

        private void TakeHeal(byte healPoints, HealthemiaLimb limb, bool stopBleeding, bool healBrokenBone)
        {
            limb.Health += healPoints;

            if (stopBleeding)
                limb.IsBleeding = false;

            if (limb is HealthemiaBreakableLimb breakableLimb && healBrokenBone)
                breakableLimb.IsBroken = false;

            if (limb.IsBlackOuted)
                limb.IsBlackOuted = false;
        }

        private void InitializeLimbs()
        {
            _head = new HealthemiaLimb(_configuration.HeadHealth, true, "Head");
            _thorax = new HealthemiaLimb(_configuration.ThoraxHealth, true, "Thorax");
            _back = new HealthemiaLimb(_configuration.BackHealth, false, "Back");
            _rightArm = new HealthemiaArmLimb(_configuration.RightArmHealth, false, "RightArm");
            _leftArm = new HealthemiaArmLimb(_configuration.LeftArmHealth, false, "LeftArm");
            _rightLeg = new HealthemiaLegLimb(_configuration.RightLegHealth, false, "RightLeg");
            _leftLeg = new HealthemiaLegLimb(_configuration.LeftLegHealth, false, "LeftLeg");
        }
        
        public void SetDefaults()
        {
            _head.Health = _configuration.HeadHealth;
            _thorax.Health = _configuration.ThoraxHealth;
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

        public void SendDeath()
        {
            Die();
        }

        private void Die()
        {
            DamageTool.damage(_player, EDeathCause.KILL, ELimb.SKULL, CSteamID.Nil, Vector3.up, 200f, 1, out _, false, false, ERagdollEffect.NONE);
            SetDefaults();
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
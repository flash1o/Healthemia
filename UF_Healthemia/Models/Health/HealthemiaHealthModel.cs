using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UF_Healthemia.Components;
using UF_Healthemia.Helpers;
using UF_Healthemia.Models.Diseases;
using UF_Healthemia.Models.HealthemiaModels;
using UF_Healthemia.Serializable;
using UnityEngine;

namespace UF_Healthemia.Models
{
    public class HealthemiaHealthModel
    {
        public delegate void LimbDamageHandler(HealthemiaLimb limb);
        public event LimbDamageHandler OnLimbBroken;
        public event LimbDamageHandler OnLimbBlackouted;
        public bool AbleToMove => _healthemiaPlayerState is HealthemiaHealthState.Alive;
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
        private HealthemiaDamageCalculator _helper => Healthemia.Instance.PlayerHealthService.Calculator;
        private HealthemiaPlayerModifierTool _modifier => Healthemia.Instance.PlayerHealthService.ModifierTool;

        private HealthemiaConfiguration _configuration => Healthemia.Instance.Configuration.Instance;

        private List<HealthemiaWoundInabilities> _inabilities;

        private List<HealthemiaDisease> _diseases;

        private IEnumerable<HealthemiaLimb> _bleedingLimbs => _limbs.Where(x => x.IsBleeding);

        private bool _bleedingCoroutineIsStarted;

        private HealthemiaHealthState _healthemiaPlayerState
        {
            get => _healthemiaPlayerState;
            set
            {
                switch (value)
                {
                    case HealthemiaHealthState.Dying:
                        SetDying();
                        break;
                    case HealthemiaHealthState.Unconscious:
                        SetUnconsnious();
                        break;
                    case HealthemiaHealthState.Alive:
                        break;
                    default:
                        break;
                }

            }
        }

        internal void TryStartBleedingCoroutine(MonoBehaviour monoBehaviour)
        {
            if (!_bleedingCoroutineIsStarted)
               monoBehaviour.StartCoroutine(BleedingCoroutine());
        }

        internal IEnumerator BleedingCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(_configuration.BleedingDamageRefreshTime);

                if (_bleedingLimbs.Count() < 1)
                {
                    _bleedingCoroutineIsStarted = false;
                    yield break;
                }
             
                foreach (var bleedingLimb in _bleedingLimbs)
                    bleedingLimb.Health -= 2;  
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

        public HealthemiaHealthModel(Player player)
        {
            _inabilities = new List<HealthemiaWoundInabilities>();
            _diseases = new List<HealthemiaDisease>();
            _player = player;
            _healthemiaPlayerState = HealthemiaHealthState.Alive;
            InitializeLimbs();
            OnLimbBroken += OnLimbBrokenHandler;
            OnLimbBlackouted += OnLimbBlackoutedHandler;
        }


        private void OnLimbBlackoutedHandler(HealthemiaLimb limb)
        {
            if (limb.IsVital)
            {
                _healthemiaPlayerState = HealthemiaHealthState.Dying;
                return;
            }


            switch (limb)
            {
                case _leftLeg:
                case _rightLeg:
                    _modifier.ApplySpeedModifier(_player.movement, TypeSpeedModifier.Blackout);
                    TryAddInability(HealthemiaWoundInabilities.SprintingInability);
                    break;
                case _leftArm:
                case _rightArm:
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

        internal bool HasDiseaseOfSameType(HealthemiaDisease disease)
        {
            bool hasDiseaseOfSameType = false;

            if (disease is Flu)
                hasDiseaseOfSameType = _diseases.OfType<Flu>().Any();
            else
                hasDiseaseOfSameType = _diseases.OfType<Poisoning>().Any();

            return hasDiseaseOfSameType;
        }



        internal void SendDamage(ELimb limb, EDeathCause cause, byte damage)
        {
            var healthemiaLimb = ConvertUnturnedLimbToHealthemiaLimb(limb);
            bool causeBleeding = _helper.ShouldCauseBleeding(cause);
            bool causeBreakingBones = _helper.ShouldBreakBones(cause);
            TakeDamage(damage, healthemiaLimb, causeBleeding, causeBreakingBones);
        }

        internal void SendHeal(HealthemiaHealContext context)
        {
            if (context.HealAllLimbsData.Key)
                TakeHealToAllLimbs(context.HealAllLimbsData.Value);

            var healData = context.HealData; 
            var healthemiaLimb = ConvertUnturnedLimbToHealthemiaLimb(healData.Limb);
            TakeHeal(healthemiaLimb, healData.HealingPoints, healData.HealBleeding, healData.HealBroken);
            
        }

      

        internal void SendHealState() => _healthemiaPlayerState = HealthemiaHealthState.Alive;
        

        private void TakeHealToAllLimbs(byte healingPoints)
        {
            foreach (var limb in _limbs)
            {
                TakeHeal(limb, healingPoints, true, false);
            }
        }

        internal void SendHeal(HealthemiaDisease disease)
        {
            _diseases.Remove(disease);
        }

        private void TakeDamage(byte damage, HealthemiaLimb limb, bool causesBleeding, bool causesBreakingBones)
        {
            limb.Health -= damage;
            limb.IsBleeding = causesBleeding;

            if (causesBleeding)
                _player.GetComponent<HealthemiaPlayerComponent>().TryStartBleedingCoroutine();

            if (limb is HealthemiaBreakableLimb breakableLimb)
                breakableLimb.IsBroken = breakableLimb.IsBroken || causesBreakingBones;

            if (limb.Health >= 0) return;

            if (limb.IsVital)
                _healthemiaPlayerState = HealthemiaHealthState.Dying;
            else
                limb.IsBlackOuted = true;
        }

        private void TakeHeal(HealthemiaLimb limb, byte healingPoints, bool healBleeding, bool healBrokenBone)
        {
            limb.Health += healingPoints;

            if (healBleeding)
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

        public void Reset()
        {
            _head.Health = _configuration.HeadHealth;
            _thorax.Health = _configuration.ThoraxHealth;
            _back.Health = _configuration.BackHealth;
            _leftArm.Health = _configuration.LeftArmHealth;
            _rightArm.Health = _configuration.RightArmHealth;
            _leftLeg.Health = _configuration.LeftLegHealth;
            _rightLeg.Health = _configuration.RightLegHealth;

            foreach (var limb in _limbs)
            {
                limb.IsBlackOuted = false;
                limb.IsBleeding = false;

                if (limb is HealthemiaBreakableLimb breakableLimb)
                    breakableLimb.IsBroken = false;
            }
        }

        internal SerializableLimbsModel ToSerialized()
        {
            var limbsModel = new SerializableLimbsModel();
            limbsModel.Head = _head;
            limbsModel.Back = _back;
            limbsModel.Thorax = _thorax;
            limbsModel.LeftArm = _leftArm;
            limbsModel.RightArm = _rightArm;
            limbsModel.LeftLeg = _leftLeg;
            limbsModel.RightLeg = _rightLeg;
            return limbsModel;
        }

        internal void LoadFromSerialized(SerializableLimbsModel model)
        {
            _head = model.Head;
            _thorax = model.Thorax;
            _back = model.Back;
            _leftArm = model.LeftArm;
            _rightArm = model.RightArm;
            _leftLeg = model.LeftLeg;
            _rightLeg = model.RightLeg;
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
            Reset();
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
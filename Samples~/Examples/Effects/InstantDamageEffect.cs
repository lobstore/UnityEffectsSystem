using System;
using UnityEngine;
namespace LOGIYGames.EffectSystem.Examples
{
    [Serializable]
    public class InstantDamageEffect : RuntimeEffect
    {
        [SerializeField] protected float Damage;
        public InstantDamageEffect(DamageEffectDefinition effectData) : base(effectData)
        {
            Damage = effectData.BaseDamage;
        }

        public override EffectLifeType LifecicleType => EffectLifeType.Instant;

        public override void OnApply()
        {
            Owner.GetComponent<Health>()?.TakeDamage(Damage);
            isFinished = true;
        }
    }
}
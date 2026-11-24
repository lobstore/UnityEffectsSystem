using System;
using UnityEngine;

namespace LOGIYGames.EffectSystem.Examples
{
    /// <summary>Applies damage once and completes immediately.</summary>
    [Serializable]
    public class InstantDamageEffect : RuntimeEffect
    {
        [SerializeField]
        private float damage;

        /// <summary>Creates an instant damage effect from a damage definition.</summary>
        public InstantDamageEffect(DamageEffectDefinition effectData) : base(effectData)
        {
            if (effectData == null)
            {
                throw new ArgumentNullException(nameof(effectData));
            }

            damage = effectData.BaseDamage;
        }

        public override EffectLifeType LifecicleType => EffectLifeType.Instant;

        public override void OnApply()
        {
            ThrowIfDisposed();
            if (!IsInitialized || Owner == null)
            {
                throw new InvalidOperationException("Initialize must be called before applying InstantDamageEffect.");
            }

            Owner.GetComponent<Health>()?.TakeDamage(damage);
            isFinished = true;
        }
    }
}

using LOGIYGames.EffectSystem;
using LOGIYGames.Timers;
using System;

namespace LOGIYGames.EffectSystem.Examples
{
    /// <summary>Applies repeated damage to the owner until the configured duration expires.</summary>
    [Serializable]
    public class DamageOverTimeEffect : RuntimeEffect
    {
        private readonly float damage;

        /// <summary>Timer that emits OnInterval once per damage tick.</summary>
        public IntervalTimer Timer { get; private set; }

        public override EffectLifeType LifecicleType => EffectLifeType.Countdown;

        /// <summary>Creates a damage-over-time effect.</summary>
        public DamageOverTimeEffect(DamageEffectDefinition effectData, float interval, float duration) : base(effectData)
        {
            if (effectData == null)
            {
                throw new ArgumentNullException(nameof(effectData));
            }

            damage = effectData.BaseDamage;
            Timer = new IntervalTimer(duration, interval);
            Timer.OnInterval += OnInterval;
            Timer.OnTimerStop += () => isFinished = true;
        }

        public override void OnApply()
        {
            ThrowIfDisposed();
            if (!IsInitialized || Owner == null)
            {
                throw new InvalidOperationException("Initialize must be called before applying DamageOverTimeEffect.");
            }

            Timer.Start();
        }

        public override void OnUpdate(float delta)
        {
            if (Data != null && Timer != null)
            {
                Data.Value = Timer.CurrentTime;
            }
        }

        public override void OnRemove()
        {
            Timer?.Dispose();
        }

        private void OnInterval()
        {
            if (Owner == null)
            {
                return;
            }

            Owner.GetComponent<Health>()?.TakeDamage(damage);
        }
    }
}

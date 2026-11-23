using LOGIYGames.EffectSystem;
using LOGIYGames.Timers;
using System;

namespace LOGIYGames.EffectSystem.Examples
{
    [Serializable]
    public class DamageOverTimeEffect : RuntimeEffect
    {
        float Damage;
        public IntervalTimer Timer { get; private set; }
        public override EffectLifeType LifecicleType => EffectLifeType.Countdown;
        public DamageOverTimeEffect(DamageEffectDefinition effectData, float interval, float duration) : base(effectData)
        {
            Damage = effectData.BaseDamage;
            Timer = new IntervalTimer(duration, interval);
            Timer.OnInterval += OnInterval;
            Timer.OnTimerStop += () => isFinished = true;
        }

        public override void OnApply()
        {
            Timer.Start();
        }
        public override void OnUpdate(float delta)
        {
            Data.Value = Timer.CurrentTime;
        }
        public override void OnRemove()
        {
            Timer.Stop();
        }

        private void OnInterval()
        {

            Owner.GetComponent<Health>()?.TakeDamage(Damage);
        }

    }
}
using System;
using UnityEngine;

namespace LOGIYGames.EffectSystem.Examples
{
    [Serializable]
    public class DamageOverTimeEffectFactory : EffectFactory
    {
        public DamageEffectDefinition effectData;
        [SerializeField] private float interval;
        [SerializeField] private float duration;
        public override RuntimeEffect Create()
        {
            return new DamageOverTimeEffect(effectData, interval, duration);
        }
    }
}
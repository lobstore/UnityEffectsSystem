using System;

namespace LOGIYGames.EffectSystem.Examples
{
    [Serializable]
    public class InstantDamageEffectFactory : EffectFactory
    {
        public DamageEffectDefinition effectData;
        public override RuntimeEffect Create()
        {
            return new InstantDamageEffect(effectData);
        }
    }
}
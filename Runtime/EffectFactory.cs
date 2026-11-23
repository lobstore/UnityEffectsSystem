using System;
namespace LOGIYGames.EffectSystem
{
    [Serializable]
    public abstract class EffectFactory
    {
        public abstract RuntimeEffect Create();
    }
}
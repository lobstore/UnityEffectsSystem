using System;

namespace LOGIYGames.EffectSystem
{
    /// <summary>
    /// Serializable factory abstraction used to construct configured runtime effect instances.
    /// Derive from this type and implement <see cref="Create"/> for each concrete effect.
    /// </summary>
    [Serializable]
    public abstract class EffectFactory
    {
        /// <summary>Creates a new, unapplied runtime effect instance.</summary>
        /// <returns>A new effect instance. The caller owns its lifecycle and must remove/dispose it.</returns>
        public abstract RuntimeEffect Create();
    }
}

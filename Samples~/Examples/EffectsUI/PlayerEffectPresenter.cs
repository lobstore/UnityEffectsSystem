using System;
using UnityEngine;

namespace LOGIYGames.EffectSystem.Examples
{
    /// <summary>
    /// Binds one runtime effect to its visual representation.
    /// </summary>
    [Serializable]
    public class PlayerEffectPresenter
    {
        [SerializeField] private PlayerEffectView view;

        /// <summary>The runtime effect presented by this instance.</summary>
        public RuntimeEffect Effect { get; private set; }

        /// <summary>Creates a presenter and immediately binds the supplied view.</summary>
        public PlayerEffectPresenter(RuntimeEffect effect, PlayerEffectView view)
        {
            Effect = effect != null ? effect : throw new ArgumentNullException(nameof(effect));
            this.view = view != null ? view : throw new ArgumentNullException(nameof(view));
            this.view.Bind(this);
        }

        /// <summary>Unbinds the associated view.</summary>
        public void Unbind()
        {
            if (view != null)
            {
                view.Unbind();
            }

            view = null;
        }
    }
}

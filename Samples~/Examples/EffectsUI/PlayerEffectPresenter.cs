using System;
using UnityEngine;

namespace LOGIYGames.EffectSystem.Examples
{
    [Serializable]
    public class PlayerEffectPresenter
    {
        [SerializeField] PlayerEffectView view;
        public RuntimeEffect Effect { get; private set; }
        public PlayerEffectPresenter(RuntimeEffect effect, PlayerEffectView view)
        {
            Effect = effect;
            this.view = view;
            view.Bind(this);
        }
    }
}

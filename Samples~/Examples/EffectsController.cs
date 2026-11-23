using LOGIYGames.EffectSystem;
using R3;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace LOGIYGames.EffectSystem.Examples
{
    public class EffectsController : MonoBehaviour
    {
        [SerializeReference] private List<RuntimeEffect> _effects = new();
        public IReadOnlyList<RuntimeEffect> Effects => _effects;
        [NonSerialized] public UnityEvent<IReadOnlyList<RuntimeEffect>> OnContinuousEffectsChanged = new();


        private void Update()
        {
            float delta = Time.deltaTime;


            for (int i = _effects.Count - 1; i >= 0; i--)
            {
                RuntimeEffect effect = _effects[i];

                effect.OnUpdate(delta);


                if (effect.IsFinished)
                {
                    RemoveEffect(effect);
                }
            }
        }


        public void AddEffect(RuntimeEffect effect)
        {
            if (effect.LifecicleType != RuntimeEffect.EffectLifeType.Instant)
            {
                var existingSameEffect = _effects.FirstOrDefault(x => x.GetType() == effect.GetType());

                if (existingSameEffect != null)
                {
                    int existionEffectsCount = _effects.Count(x => x.GetType() == effect.GetType());
                    if (!effect.IsStackable || effect.Data.MaxStackSize <= existionEffectsCount)
                    {
                        RemoveEffect(existingSameEffect);
                    }
                }

                effect.Initialize(gameObject);


                _effects.Add(effect);
                OnContinuousEffectsChanged.Invoke(_effects);
            }

            effect.OnApply();
        }


        public void RemoveEffect(RuntimeEffect effect)
        {
            if (_effects.Remove(effect))
            {
                effect.OnRemove();
            }
            OnContinuousEffectsChanged.Invoke(_effects);
        }
    }
}
using LOGIYGames.EffectSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace LOGIYGames.EffectSystem.Examples
{
    /// <summary>
    /// Sample owner-side controller that applies, updates, stacks, and removes runtime effects.
    /// This controller is part of the optional Examples sample, not the core runtime package.
    /// </summary>
    public class EffectsController : MonoBehaviour
    {
        [SerializeReference]
        private List<RuntimeEffect> effects = new List<RuntimeEffect>();

        /// <summary>Read-only view of the currently active continuous effects.</summary>
        public IReadOnlyList<RuntimeEffect> Effects => effects;

        /// <summary>Raised after the active continuous-effects collection changes.</summary>
        [NonSerialized]
        public UnityEvent<IReadOnlyList<RuntimeEffect>> OnContinuousEffectsChanged = new UnityEvent<IReadOnlyList<RuntimeEffect>>();

        private void OnEnable()
        {
            if (OnContinuousEffectsChanged == null)
            {
                OnContinuousEffectsChanged = new UnityEvent<IReadOnlyList<RuntimeEffect>>();
            }
        }

        private void Update()
        {
            if (effects.Count == 0)
            {
                return;
            }

            // A snapshot prevents callbacks from invalidating the collection while it is being iterated.
            var snapshot = effects.ToArray();
            var delta = Time.deltaTime;

            foreach (var effect in snapshot)
            {
                if (effect == null || !effects.Contains(effect))
                {
                    continue;
                }

                effect.OnUpdate(delta);
                if (effect.IsFinished)
                {
                    RemoveEffect(effect);
                }
            }
        }

        /// <summary>
        /// Initializes and applies an effect. Instant effects are applied and disposed immediately;
        /// countdown and passive effects are stored and updated until removed.
        /// </summary>
        /// <param name="effect">New effect instance created by an EffectFactory.</param>
        public void AddEffect(RuntimeEffect effect)
        {
            if (effect == null)
            {
                Debug.LogWarning("Cannot add a null RuntimeEffect.", this);
                return;
            }

            var isContinuous = effect.LifecicleType != RuntimeEffect.EffectLifeType.Instant;
            var addedToCollection = false;

            try
            {
                effect.Initialize(gameObject);

                if (isContinuous)
                {
                    EnforceStackLimit(effect);
                    effects.Add(effect);
                    addedToCollection = true;
                    NotifyEffectsChanged();
                }

                effect.OnApply();

                // An effect may finish immediately during OnApply.
                if (effect.IsFinished)
                {
                    if (effects.Remove(effect))
                    {
                        addedToCollection = false;
                        NotifyEffectsChanged();
                    }

                    effect.Dispose();
                }
                else if (!isContinuous)
                {
                    effect.Dispose();
                }
            }
            catch
            {
                if (addedToCollection && effects.Remove(effect))
                {
                    try
                    {
                        NotifyEffectsChanged();
                    }
                    catch (Exception notificationException)
                    {
                        Debug.LogException(notificationException, this);
                    }
                }

                effect.Dispose();
                throw;
            }
        }

        /// <summary>Removes an active effect, invokes its cleanup hook, and releases its definition clone.</summary>
        /// <param name="effect">Effect instance to remove.</param>
        public void RemoveEffect(RuntimeEffect effect)
        {
            if (effect == null || !effects.Remove(effect))
            {
                return;
            }

            try
            {
                effect.Dispose();
            }
            finally
            {
                NotifyEffectsChanged();
            }
        }

        private void EnforceStackLimit(RuntimeEffect incomingEffect)
        {
            var maxStackSize = Mathf.Max(1, incomingEffect.Data != null ? incomingEffect.Data.MaxStackSize : 1);
            var matchingEffects = effects
                .Where(existing => existing != null && existing.GetType() == incomingEffect.GetType())
                .ToList();

            // Non-stackable effects have an effective stack limit of one.
            if (!incomingEffect.IsStackable)
            {
                maxStackSize = 1;
            }

            while (matchingEffects.Count >= maxStackSize)
            {
                var oldest = matchingEffects[0];
                matchingEffects.RemoveAt(0);
                RemoveEffect(oldest);
            }
        }

        private void NotifyEffectsChanged()
        {
            OnContinuousEffectsChanged?.Invoke(effects);
        }

        private void OnDestroy()
        {
            var snapshot = effects.ToArray();
            effects.Clear();

            foreach (var effect in snapshot)
            {
                if (effect == null)
                {
                    continue;
                }

                try
                {
                    effect.Dispose();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, this);
                }
            }

            NotifyEffectsChanged();
        }
    }
}

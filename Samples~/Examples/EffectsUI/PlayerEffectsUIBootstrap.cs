using System.Collections.Generic;
using UnityEngine;

namespace LOGIYGames.EffectSystem.Examples
{
    /// <summary>Creates and destroys effect UI views to mirror the controller's active effect list.</summary>
    public class PlayerEffectsUIBootstrap : MonoBehaviour
    {
        private readonly List<PlayerEffectPresenter> effectPresenters = new List<PlayerEffectPresenter>();
        private readonly List<PlayerEffectView> effectViews = new List<PlayerEffectView>();

        [SerializeField] private GameObject effectPrefab;
        [SerializeField] private RectTransform effectsContainer;
        [SerializeField] private EffectsController effects;

        private void OnEnable()
        {
            if (effects == null || effectsContainer == null || effectPrefab == null)
            {
                Debug.LogWarning("PlayerEffectsUIBootstrap requires an EffectsController, container, and effect prefab.", this);
                return;
            }

            effects.OnContinuousEffectsChanged.AddListener(UpdateEffectsViews);
            UpdateEffectsViews(effects.Effects);
        }

        private void OnDisable()
        {
            if (effects != null && effects.OnContinuousEffectsChanged != null)
            {
                effects.OnContinuousEffectsChanged.RemoveListener(UpdateEffectsViews);
            }

            ClearViews();
        }

        private void UpdateEffectsViews(IReadOnlyList<RuntimeEffect> activeEffects)
        {
            if (effectsContainer == null || effectPrefab == null)
            {
                return;
            }

            ClearViews();

            foreach (var effect in activeEffects)
            {
                if (effect == null || effect.Data == null || effect.Data.Icon == null)
                {
                    continue;
                }

                var instance = Instantiate(effectPrefab, effectsContainer, false);
                instance.transform.localScale = Vector3.one;

                var view = instance.GetComponent<PlayerEffectView>();
                if (view == null)
                {
                    Debug.LogWarning("Effect UI prefab is missing PlayerEffectView.", instance);
                    Destroy(instance);
                    continue;
                }

                effectViews.Add(view);
                effectPresenters.Add(new PlayerEffectPresenter(effect, view));
            }
        }

        private void ClearViews()
        {
            for (var i = 0; i < effectViews.Count; i++)
            {
                if (effectViews[i] != null)
                {
                    effectViews[i].Unbind();
                    Destroy(effectViews[i].gameObject);
                }
            }

            effectViews.Clear();
            effectPresenters.Clear();
        }
    }
}

using LOGIYGames.EffectSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LOGIYGames.EffectSystem.Examples
{
    /// <summary>UI view that presents the icon and current value of one runtime effect.</summary>
    public class PlayerEffectView : MonoBehaviour
    {
        [SerializeField] private Image effectIcon;
        [SerializeField] private TextMeshProUGUI displayText;

        private RuntimeEffect runtimeEffect;

        /// <summary>Binds the view to a presenter and refreshes its initial display.</summary>
        public void Bind(PlayerEffectPresenter presenter)
        {
            if (presenter == null || presenter.Effect == null)
            {
                Unbind();
                return;
            }

            runtimeEffect = presenter.Effect;
            if (effectIcon != null)
            {
                effectIcon.sprite = runtimeEffect.Data != null ? runtimeEffect.Data.Icon : null;
                effectIcon.enabled = effectIcon.sprite != null;
            }

            UpdateEffectDisplayText();
        }

        private void LateUpdate()
        {
            if (runtimeEffect != null)
            {
                UpdateEffectDisplayText();
            }
        }

        private void UpdateEffectDisplayText()
        {
            if (displayText != null && runtimeEffect != null && runtimeEffect.Data != null)
            {
                displayText.text = Mathf.RoundToInt(runtimeEffect.Data.Value).ToString();
            }
        }

        /// <summary>Clears the binding and removes stale UI references.</summary>
        public void Unbind()
        {
            runtimeEffect = null;

            if (effectIcon != null)
            {
                effectIcon.sprite = null;
                effectIcon.enabled = false;
            }

            if (displayText != null)
            {
                displayText.text = string.Empty;
            }
        }
    }
}

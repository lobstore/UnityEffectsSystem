using LOGIYGames.EffectSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LOGIYGames.EffectSystem.Examples
{
    public class PlayerEffectView : MonoBehaviour
    {
        [SerializeField] Image effectIcon;
        [SerializeField] TextMeshProUGUI displayText;
        RuntimeEffect RuntimeEffect;
        public void Bind(PlayerEffectPresenter presenter)
        {
            RuntimeEffect = presenter.Effect;
            UpdateEffectIcon(RuntimeEffect.Data.Icon);
            UpdateEffectDisplayText(RuntimeEffect.Data.Value);
        }
        private void LateUpdate()
        {
            UpdateEffectDisplayText(RuntimeEffect.Data.Value);

        }
        private void UpdateEffectDisplayText(float value)
        {
            displayText.text = Mathf.RoundToInt(RuntimeEffect.Data.Value).ToString();
        }

        private void UpdateEffectIcon(Sprite skillIcon)
        {
            this.effectIcon.sprite = skillIcon;
        }
        public void Unbind()
        {

        }

    }
}

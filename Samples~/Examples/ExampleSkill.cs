using LOGIYGames.EffectSystem;
using UnityEngine;

namespace LOGIYGames.EffectSystem.Examples
{
    public class ExampleSkill : MonoBehaviour
    {
        [SerializeField] EffectsController Controller;

        [SerializeReference] EffectFactory EffectFactory;

        public void ApplyEffect()
        {
            Controller.AddEffect(EffectFactory.Create());
        }
    }
}
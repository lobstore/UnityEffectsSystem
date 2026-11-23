using UnityEngine;
using UnityEngine.Events;

namespace LOGIYGames.EffectSystem.Examples
{
    public class Health : MonoBehaviour
    {
        public float currentHealth = 100f;
        public float maxHealth = 100f;
        public UnityEvent<float> OnCurrentHealthChanged;
        public UnityEvent<float> OnMaxHealthChanged;
        private void Start()
        {
            OnMaxHealthChanged.Invoke(maxHealth);
            OnCurrentHealthChanged.Invoke(currentHealth);
        }
        public void TakeDamage(float damage)
        {
            currentHealth -= damage;
            OnCurrentHealthChanged.Invoke(currentHealth);
        }

    }
}
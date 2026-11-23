using System;
using UnityEngine;

namespace LOGIYGames.EffectSystem
{

    [Serializable]
    public abstract class RuntimeEffect
    {
        public enum EffectLifeType
        {
            Instant,
            Countdown,
            Passive
        }
        protected GameObject Owner;
        public EffectDefinition Data;

        [SerializeField] protected bool isFinished;
        public abstract EffectLifeType LifecicleType { get;}
        public bool IsFinished => isFinished;
        public bool IsStackable => Data.MaxStackSize > 1;
        public int Level = 1;
        protected RuntimeEffect(EffectDefinition effectData)
        {
            Data = ScriptableObject.Instantiate( effectData);
        }

        public virtual void Initialize(GameObject owner)
        {
            Owner = owner;
        }


        public abstract void OnApply();


        public virtual void OnUpdate(float delta)
        {
        }


        public virtual void OnRemove()
        {
        }
    }
}

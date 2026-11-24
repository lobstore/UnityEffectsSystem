using System;
using UnityEngine;

namespace LOGIYGames.EffectSystem
{
    /// <summary>
    /// Base class for a single runtime instance of an ability, status, or gameplay effect.
    /// </summary>
    /// <remarks>
    /// The constructor clones the supplied <see cref="EffectDefinition"/>. Derived classes should
    /// capture immutable tuning values from the definition in their constructor, use <see cref="Owner"/>
    /// after <see cref="Initialize"/>, and release resources in <see cref="OnRemove"/>.
    ///
    /// RuntimeEffect does not manage its own updates. An effects controller or gameplay system must
    /// call <see cref="OnUpdate"/> and remove instances after <see cref="IsFinished"/> becomes true.
    /// Call <see cref="Dispose"/> when the instance is removed to destroy its runtime definition clone.
    /// </remarks>
    [Serializable]
    public abstract class RuntimeEffect : IDisposable
    {
        /// <summary>How the consumer should manage the lifetime of the effect.</summary>
        public enum EffectLifeType
        {
            /// <summary>Applied immediately and not kept in the active effects collection.</summary>
            Instant,
            /// <summary>Has a finite lifetime and is removed when finished.</summary>
            Countdown,
            /// <summary>Remains active until explicitly removed.</summary>
            Passive
        }

        /// <summary>GameObject affected by this effect after initialization.</summary>
        protected GameObject Owner;

        /// <summary>
        /// Per-instance clone of the source definition. Kept as a public field for Unity
        /// SerializeReference compatibility; application code should treat it as read-only.
        /// The clone is destroyed when this effect is disposed.
        /// </summary>
        public EffectDefinition Data;

        [SerializeField]
        protected bool isFinished;

        /// <summary>Lifetime category used by an effects controller.</summary>
        public abstract EffectLifeType LifecicleType { get; }

        /// <summary>Correctly-spelled alias for <see cref="LifecicleType"/>.</summary>
        public EffectLifeType LifecycleType => LifecicleType;

        /// <summary>Whether the effect has completed its gameplay lifetime.</summary>
        public bool IsFinished => isFinished;

        /// <summary>Whether multiple instances of this effect type may coexist.</summary>
        public bool IsStackable => Data != null && Data.MaxStackSize > 1;

        /// <summary>Current effect level. The consuming system is responsible for enforcing Data.Levels.</summary>
        public int Level = 1;

        private bool initialized;
        private bool disposing;
        private bool disposed;

        /// <summary>Creates a runtime effect and clones its authoring definition.</summary>
        /// <param name="effectData">Definition asset to clone.</param>
        /// <exception cref="ArgumentNullException">Thrown when the definition is null.</exception>
        protected RuntimeEffect(EffectDefinition effectData)
        {
            if (effectData == null)
            {
                throw new ArgumentNullException(nameof(effectData), "A runtime effect requires an EffectDefinition.");
            }

            Data = UnityEngine.Object.Instantiate(effectData);
            Level = Mathf.Clamp(Level, 1, Mathf.Max(1, Data.Levels));
        }

        /// <summary>Associates this effect instance with the object it affects.</summary>
        /// <param name="owner">Target GameObject.</param>
        /// <exception cref="ArgumentNullException">Thrown when owner is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown if this instance has already been initialized for another owner.</exception>
        /// <exception cref="ObjectDisposedException">Thrown after this effect is disposed.</exception>
        public virtual void Initialize(GameObject owner)
        {
            ThrowIfDisposed();

            if (owner == null)
            {
                throw new ArgumentNullException(nameof(owner));
            }

            if (initialized && Owner != owner)
            {
                throw new InvalidOperationException("A RuntimeEffect instance cannot be reinitialized for a different owner. Create a new instance instead.");
            }

            Owner = owner;
            initialized = true;
        }

        /// <summary>Applies the effect. The consumer must call Initialize before invoking this method.</summary>
        /// <exception cref="InvalidOperationException">Thrown if Initialize has not been called.</exception>
        /// <exception cref="ObjectDisposedException">Thrown after this effect is disposed.</exception>
        public abstract void OnApply();

        /// <summary>Advances effect-specific state. Called by the consuming effects controller.</summary>
        /// <param name="delta">Elapsed time in seconds, normally Time.deltaTime.</param>
        public virtual void OnUpdate(float delta)
        {
        }

        /// <summary>Releases effect-specific resources when the effect is removed.</summary>
        public virtual void OnRemove()
        {
        }

        /// <summary>
        /// Calls <see cref="OnRemove"/> and destroys this instance's cloned definition.
        /// Safe to call more than once. Call this instead of only calling OnRemove when removing an effect.
        /// </summary>
        public void Dispose()
        {
            if (disposed || disposing)
            {
                return;
            }

            disposing = true;
            try
            {
                // OnRemove may still access the instance's state, so mark disposed only after cleanup.
                OnRemove();
            }
            finally
            {
                if (Data != null)
                {
                    if (Application.isPlaying)
                    {
                        UnityEngine.Object.Destroy(Data);
                    }
                    else
                    {
                        UnityEngine.Object.DestroyImmediate(Data);
                    }

                    Data = null;
                }

                Owner = null;
                initialized = false;
                disposed = true;
                disposing = false;
            }

            GC.SuppressFinalize(this);
        }

        /// <summary>Throws if public lifecycle operations are attempted after disposal.</summary>
        protected void ThrowIfDisposed()
        {
            if (disposed || disposing)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        /// <summary>Returns whether Initialize has been called for this instance.</summary>
        protected bool IsInitialized => initialized;
    }
}

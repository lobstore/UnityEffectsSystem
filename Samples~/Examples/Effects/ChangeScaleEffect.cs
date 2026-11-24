using LOGIYGames.EffectSystem;
using LOGIYGames.Timers;
using System;
using UnityEngine;

/// <summary>Temporarily changes the owner's local scale and restores it when removed.</summary>
[Serializable]
public class ChangeScaleEffect : RuntimeEffect
{
    /// <summary>Countdown that determines this effect's duration.</summary>
    public CountdownTimer Timer { get; private set; }

    public override EffectLifeType LifecicleType => EffectLifeType.Countdown;

    private readonly float scaleFactor;
    private Vector3 originalScale;

    /// <summary>Creates a temporary scale effect.</summary>
    public ChangeScaleEffect(ScalingEffectDefinition effectData, float duration) : base(effectData)
    {
        if (duration < 0f || float.IsNaN(duration) || float.IsInfinity(duration))
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }

        scaleFactor = effectData.ScaleFactor;
        Timer = new CountdownTimer(duration);
        Timer.OnTimerStop += () => isFinished = true;
    }

    public override void OnApply()
    {
        ThrowIfDisposed();
        if (!IsInitialized || Owner == null)
        {
            throw new InvalidOperationException("Initialize must be called before applying ChangeScaleEffect.");
        }

        originalScale = Owner.transform.localScale;
        Timer.Start();
        Owner.transform.localScale = Vector3.one * scaleFactor;
    }

    public override void OnUpdate(float delta)
    {
        if (Data != null && Timer != null)
        {
            Data.Value = Timer.CurrentTime;
        }
    }

    public override void OnRemove()
    {
        Timer?.Dispose();
        if (Owner != null)
        {
            Owner.transform.localScale = originalScale;
        }
    }
}

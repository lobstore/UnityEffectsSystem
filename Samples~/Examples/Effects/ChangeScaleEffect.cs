using LOGIYGames.EffectSystem;
using LOGIYGames.Timers;
using System;
using UnityEngine;

[Serializable]
public class ChangeScaleEffect : RuntimeEffect
{
    public CountdownTimer Timer { get; private set; }
    public override EffectLifeType LifecicleType => EffectLifeType.Countdown;

    private float scaleFactor;
    public ChangeScaleEffect(ScalingEffectDefinition effectData, float duration) : base(effectData)
    {
        scaleFactor = effectData.ScaleFactor;
        Timer = new CountdownTimer(duration);
        Timer.OnTimerStop += () => isFinished = true;
    }
    public override void OnApply()
    {
        Timer.Start();
        Owner.gameObject.transform.localScale = Vector3.one * scaleFactor;
    }
    public override void OnUpdate(float delta)
    {
        Data.Value = Timer.CurrentTime;
    }
    public override void OnRemove()
    {
        Timer.Stop();
        Owner.gameObject.transform.localScale = Vector3.one;
    }

}
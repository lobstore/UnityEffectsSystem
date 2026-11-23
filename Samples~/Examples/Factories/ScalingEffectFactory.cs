using LOGIYGames.EffectSystem;
using System;
using UnityEngine;

[Serializable]
public class ScalingEffectFactory : EffectFactory
{
    public ScalingEffectDefinition effectData;

    [SerializeField] private float duration;

    public override RuntimeEffect Create()
    {
        return new ChangeScaleEffect(effectData, duration);
    }
}

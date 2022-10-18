using Godot;
using System;

public class GrassStepEffect : AnimatedSprite
{
    public override void _Ready()
    {
        Frame = 0;
        Playing = true;
    }

    public void OnGrassStepEffectAnimationFinished()
    {
        QueueFree();
    }
}

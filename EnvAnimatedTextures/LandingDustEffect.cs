using Godot;
using System;

public class LandingDustEffect : AnimatedSprite
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Frame = 0;
        Playing = true;
    }

    public void OnLandingDustEffectAnimationFinished()
    {
        QueueFree();
    }
}

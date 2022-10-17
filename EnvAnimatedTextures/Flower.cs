using Godot;
using System;

public class Flower : AnimatedSprite
{
    public override void _Ready()
    {
        //Doing this to make all flowers to play synchronously.
        Playing = true;
    }
}

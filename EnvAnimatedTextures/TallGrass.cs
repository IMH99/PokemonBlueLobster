using Godot;
using System;

public class TallGrass : Node2D
{
    AnimationPlayer                 _animPlayer;

    private TextureRect             _grassOverlay;

    //Pre load the texture and effect we are going to use.
    private Texture                 _grassOverlayTexture = ResourceLoader.Load<Texture>("res://Assets/Tiles/stepped_tall_grass.png");;
    public PackedScene              _grassStepEffect = ResourceLoader.Load<PackedScene>("res://EnvAnimatedTextures/GrassStepEffect.tscn");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    }

    public void PlayerExitingGrass()
    {
        //Make sure there is an overlay created.
        if(IsInstanceValid(_grassOverlay))
        {
            //Delete the overlay.
            _grassOverlay.QueueFree();

            //NOTE: we are not deleting the grass step effect because it deletes itself once the
            //animation is completed, check GrassEffect.cs
        }
    }

    public void PlayerEnteredGrass()
    {
        //Create the overlay and set its position.
        _grassOverlay = new TextureRect();
        _grassOverlay.Texture = _grassOverlayTexture;
        _grassOverlay.RectPosition = Position;

        //Add it to the scene.
        GetTree().CurrentScene.AddChild(_grassOverlay);

        //Set the position of the effect and add it to the scene.
        if (IsInstanceValid(_grassStepEffect))
        {
            AnimatedSprite anim_sprite = _grassStepEffect.Instance() as AnimatedSprite;

            anim_sprite.Position = Position;
            GetTree().CurrentScene.AddChild(anim_sprite);
        }
    }

    public void OnArea2DBodyEntered(Area2D other)
    {
        PlayerEnteredGrass();
        _animPlayer.Play("Step");
    }

    public void OnArea2DBodyExited(Area2D other)
    {
        PlayerExitingGrass();
    }
}

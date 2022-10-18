using Godot;
using System;

public class TallGrass : Node2D
{
    AnimationPlayer                 _animPlayer;

    private Texture                 _grassOverlayTexture;
    private TextureRect             _grassOverlay;
    private bool                    _playerInside = false;

    [Export]
    public GrassStepEffect         _grassStepEffect;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        //Pre load the texture and effect we are going to use.
        _grassOverlayTexture = ResourceLoader.Load<Texture>("res://Assets/Tiles/stepped_tall_grass.png");
        //_grassStepEffect = (GrassStepEffect)ResourceLoader.Load<GrassStepEffect>("res://EnvAnimatedTextures/GrassStepEffect.tscn");

        //Bind the delegates of the player to custom functions of this class.
        Player player = (Player)GetTree().CurrentScene.FindNode("Player");

        if(player != null)
        {
            player.Connect("PlayerMoving", this, "PlayerExitingGrass");
            player.Connect("PlayerStopped", this, "PlayerEnteredGrass");
        }
    }

    public void PlayerExitingGrass()
    {
        _playerInside = false;

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
        if (_playerInside)
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
                _grassStepEffect.Position = Position;
                GetTree().CurrentScene.AddChild(_grassStepEffect);
            }
        }
    }

    public void OnArea2DBodyEntered(Area2D other)
    {
        _playerInside = true;
        _animPlayer.Play("Step");
    }
}

using Godot;
using System;

public class Door : Area2D
{
    //Editor Variables.
    [Export(PropertyHint.File, "*.tscn")]
    public string                           NextScenePath = "";
    [Export]
    public bool                             IsInvisible = false;
    [Export]
    public Vector2                          SpawnLocation = new Vector2(0.0f, 0.0f);
    [Export]
    public Vector2                          SpawnDirection = new Vector2(0.0f, 0.0f);

    //Member variables.
    private AnimationPlayer                 _animPlayer;
    private bool                            _playerEntered = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        if(IsInvisible)
        {
            GetNode<Sprite>("Sprite").Texture = null;
        }
        GetNode<Sprite>("Sprite").Visible = false;

        Player player = (Player)FindParent("CurrentScene").GetChild(FindParent("CurrentScene").GetChildCount() - 1).FindNode("Player");

        if(IsInstanceValid(player))
        {
            player.Connect("OnPlayerEnteringDoor", this, "EnterDoor");
            player.Connect("OnPlayerEnteredDoor", this, "CloseDoor");
        }
    }

    public void EnterDoor()
    {
        if(_playerEntered)
        {
            _animPlayer.Play("OpenDoor");
        }
    }

    public void CloseDoor()
    {
        if (_playerEntered)
        {
            _animPlayer.Play("CloseDoor");
        }
    }

    public void DoorClosed()
    {
        if (_playerEntered)
        {
            SceneManager scene_manager = (SceneManager)GetNode(new NodePath("/root/SceneManager"));

            if (IsInstanceValid(scene_manager))
            {
                scene_manager.TransitionToScene(NextScenePath, SpawnLocation, SpawnDirection);
            }
        }
    }

    public void OnDoorBodyEntered(object body)
    {
        _playerEntered = true;
    }

    public void OnDoorBodyExited(object body)
    {
        _playerEntered = false;
    }
}

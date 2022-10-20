using Godot;
using System;

public class Door : Area2D
{
    [Export(PropertyHint.File, "*.tscn")]
    public string NextScenePath = "";

    private AnimationPlayer _animPlayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

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
        _animPlayer.Play("OpenDoor");
    }

    public void CloseDoor()
    {
        _animPlayer.Play("CloseDoor");
    }

    public void DoorClosed()
    {
        SceneManager scene_manager = (SceneManager)GetNode(new NodePath("/root/SceneManager"));

        if (IsInstanceValid(scene_manager))
        {
            scene_manager.TransitionToScene(NextScenePath);
        }
    }
}

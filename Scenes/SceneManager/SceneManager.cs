using Godot;
using System;

public class SceneManager : Node2D
{
    private string              _nextScene;
    private Vector2             _spawnPosition;
    private Vector2             _spawnDirection;
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        
    }
    public void TransitionToScene(string new_scene, Vector2 spawn_position, Vector2 spawn_direction)
    {
        _nextScene = new_scene;
        _spawnPosition = spawn_position;
        _spawnDirection = spawn_direction;
        GetNode<CanvasLayer>("ScreenTransition").GetNode<AnimationPlayer>("AnimationPlayer").Play("FadeToBlack");
    }

    public void FinishFading()
    {
        Node2D current_scene = GetNode<Node2D>("CurrentScene");

        if(IsInstanceValid(current_scene))
        {
            //The first child is expected to be the scene to delete.
            current_scene.GetChild(0).QueueFree();

            //Load the next scene and add it to the hierarchy.
            PackedScene scene = (PackedScene)ResourceLoader.Load<PackedScene>(_nextScene);
            current_scene.AddChild(scene.Instance());

            Player player = (Player)GetNode<Node2D>("CurrentScene").GetChild(GetNode<Node2D>("CurrentScene").GetChildCount() - 1).FindNode("Player");

            if(IsInstanceValid(player))
            {
                player.SetSpawn(_spawnPosition, _spawnDirection);
            }

            GetNode<CanvasLayer>("ScreenTransition").GetNode<AnimationPlayer>("AnimationPlayer").Play("BlackToNormal");
        }
    }
}

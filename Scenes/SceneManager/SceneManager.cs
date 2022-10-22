using Godot;
using System;

public class SceneManager : Node2D
{
    enum TransitionType
    {
        kTransitionType_NewScene,
        kTransitionType_PartyScreen,
        kTransitionType_MenuOnly
    }

    private string              _nextScene;
    private Vector2             _spawnPosition;
    private Vector2             _spawnDirection;
    private TransitionType      _transitionType = TransitionType.kTransitionType_NewScene;

    public void TransitionToPartyScene()
    {
        GetNode<CanvasLayer>("ScreenTransition").GetNode<AnimationPlayer>("AnimationPlayer").Play("FadeToBlack");
        _transitionType = TransitionType.kTransitionType_PartyScreen;
    }
    public void TransitionExitPartyScene()
    {
        GetNode<CanvasLayer>("ScreenTransition").GetNode<AnimationPlayer>("AnimationPlayer").Play("FadeToBlack");
        _transitionType = TransitionType.kTransitionType_MenuOnly;
    }

    public void TransitionToScene(string new_scene, Vector2 spawn_position, Vector2 spawn_direction)
    {
        _nextScene = new_scene;
        _spawnPosition = spawn_position;
        _spawnDirection = spawn_direction;
        _transitionType = TransitionType.kTransitionType_NewScene;
        GetNode<CanvasLayer>("ScreenTransition").GetNode<AnimationPlayer>("AnimationPlayer").Play("FadeToBlack");
    }

    public void FinishFading()
    {
        switch(_transitionType)
        {
            case TransitionType.kTransitionType_NewScene:
                Node2D current_scene = GetNode<Node2D>("CurrentScene");

                if (IsInstanceValid(current_scene))
                {
                    //The first child is expected to be the scene to delete.
                    current_scene.GetChild(0).QueueFree();

                    //Load the next scene and add it to the hierarchy.
                    PackedScene scene = (PackedScene)ResourceLoader.Load<PackedScene>(_nextScene);
                    current_scene.AddChild(scene.Instance());

                    Utils.Instance().GetPlayerNode().SetSpawn(_spawnPosition, _spawnDirection);
                }
                break;

            case TransitionType.kTransitionType_PartyScreen:
                Menu menu = (Menu)GetNode<Menu>("Menu");

                if(IsInstanceValid(menu))
                {
                    menu.LoadPartyScreen();
                }

                break;

            case TransitionType.kTransitionType_MenuOnly:
                Menu m = (Menu)GetNode<Menu>("Menu");

                if (IsInstanceValid(m))
                {
                    m.UnLoadPartyScreen();
                }
                break;
        }

        GetNode<CanvasLayer>("ScreenTransition").GetNode<AnimationPlayer>("AnimationPlayer").Play("BlackToNormal");
    }
}

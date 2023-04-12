using Godot;
using System;

public class Utils : Node
{
    //Member variables.
    private static Utils            _instance;
    private Node                    _sceneManagerNode;
    private Player                  _playerRef;
    private SceneManager            _sceneManagerRef; 
    private Database                _database;

    //Member functions.
    public static Utils Instance()
    {
        return _instance;
    }

    public Player GetPlayerNode()
    {
        return _playerRef;
    }

    public void SetPlayerNode(Player p)
    {
        _playerRef = p;
    }

    public SceneManager GetSceneManager()
    {
        return _sceneManagerRef;
    }

    public override void _Ready()
    {
        //Initializing the instance to this object.
        _instance = this;

        _sceneManagerNode = GetTree().Root.GetChild(GetTree().Root.GetChildCount() - 1);
        _sceneManagerRef = (SceneManager)_sceneManagerNode;
        _playerRef = (Player)_sceneManagerNode.GetNode<Node2D>("CurrentScene").FindNode("Player");
        _database = (Database)_sceneManagerNode.GetNode<Node2D>("CurrentScene").FindNode("Database");
    }
}

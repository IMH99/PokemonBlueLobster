using Godot;
using System;

public class LedgeTileMap : TileMap
{

    [Export]
    public Enums.FacingDirection Direction = Enums.FacingDirection.kFacingDirection_None;

    public override void _Ready()
    {
        
    }

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}

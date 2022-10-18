using Godot;
using System;

public class Player : KinematicBody2D
{
    //Enums.
    enum PlayerState
    {
        PlayerState_None = -1,
        PlayerState_Idle,
        PlayerState_Turning,
        PlayerState_Walking
    }

    enum FacingDirection
    {
        FacingDirection_None = -1,
        FacingDirection_Left,
        FacingDirection_Right,
        FacingDirection_Up,
        FacingDirection_Down
    }

    [Signal]
    public delegate void                        OnPlayerTileMoved();
    [Signal]
    public delegate void                        OnPlayerTileMoving();

    //Constant value for the tile size.
    const int                                   TILE_SIZE = 16;

    //Editor Variables.
    [Export]
    public float                                 WalkSpeed = 4.0f;

    //Member Variables.
    private Vector2                             _initialPosition;
    private Vector2                             _inputDirection;
    private float                               _percentMovedToNextTile = 0.0f; //From 0 to 1.
    private bool                                _isMoving = false;
    private AnimationTree                       _AnimTree;
    private AnimationNodeStateMachinePlayback   _AnimState;
    private PlayerState                         _PlayerState = PlayerState.PlayerState_Idle;
    private FacingDirection                     _FacingDirection = FacingDirection.FacingDirection_Down;
    private RayCast2D                           _Ray;

    

    //Member Functions.
    public void ProcessPlayerInput()
    {
        //Doing this to not move diagonally.
        if(_inputDirection.y == 0)
        {
            _inputDirection.x = Convert.ToInt32(Input.IsActionPressed("Right")) - Convert.ToInt32(Input.IsActionPressed("Left")); ;
        }
        if (_inputDirection.x == 0)
        {
            _inputDirection.y = Convert.ToInt32(Input.IsActionPressed("Down")) - Convert.ToInt32(Input.IsActionPressed("Up")); ;
        }

        //Means that it is moving now.
        if(_inputDirection != Vector2.Zero)
        {
            //Update the Blend position with the input direction.
            _AnimTree.Set("parameters/Idle/blend_position", _inputDirection);
            _AnimTree.Set("parameters/Walk/blend_position", _inputDirection);
            _AnimTree.Set("parameters/Turn/blend_position", _inputDirection);

            //Update the player state based on the action performed.
            if(NeedToTurn())
            {
                _PlayerState = PlayerState.PlayerState_Turning;
                _AnimState.Travel("Turn");
            }
            else
            {
                _PlayerState = PlayerState.PlayerState_Walking;
                _initialPosition = Position;
            }
        }
        else
        {
            _PlayerState = PlayerState.PlayerState_Idle;
            _AnimState.Travel("Idle");
        }
    }

    public void Move(float delta)
    {
        //Making sure the raycast is pointing to the direction the player is facing.
        Vector2 desired_step = _inputDirection * TILE_SIZE * 0.5f;
        _Ray.CastTo = desired_step;
        _Ray.ForceRaycastUpdate();

        if(!_Ray.IsColliding())
        {
            //Update the percentage.
            _percentMovedToNextTile += WalkSpeed * delta;

            //Move and reset the values when the percentage reachs its maximum.
            if (_percentMovedToNextTile >= 1.0f)
            {
                Position = _initialPosition + (TILE_SIZE * _inputDirection);
                _percentMovedToNextTile = 0.0f;
                _PlayerState = PlayerState.PlayerState_Idle;
            }
            //If the percentage is not at its maximum, interpolate the position with the percentage.
            else
            {
                // When getting close to the tile, send signal.
                if (_percentMovedToNextTile <= 0.8f)
                {
                    if (!_isMoving) {
                        EmitSignal(nameof(OnPlayerTileMoving));
                    }
                    _isMoving = true;
                } else {
                    if (_isMoving) {
                        EmitSignal(nameof(OnPlayerTileMoved));
                    }
                    _isMoving = false;
                }

                Position = _initialPosition + (TILE_SIZE * _inputDirection * _percentMovedToNextTile);
            }
        }
        else
        {
            _PlayerState = PlayerState.PlayerState_Idle;
        }
    }

    public bool NeedToTurn()
    {
        FacingDirection new_facing_direction = FacingDirection.FacingDirection_None;

        if(_inputDirection.x < 0)
        {
            new_facing_direction = FacingDirection.FacingDirection_Left;
        }
        else if (_inputDirection.x > 0)
        {
            new_facing_direction = FacingDirection.FacingDirection_Right;
        }
        else if (_inputDirection.y < 0)
        {
            new_facing_direction = FacingDirection.FacingDirection_Up;
        }
        else if (_inputDirection.y > 0)
        {
            new_facing_direction = FacingDirection.FacingDirection_Down;
        }

        if(_FacingDirection != new_facing_direction)
        {
            _FacingDirection = new_facing_direction;
            return true;
        }

        _FacingDirection = new_facing_direction;
        return false;
    }

    public void FinishedTurning()
    {
        //When finished turning the player stays in idle.
        _PlayerState = PlayerState.PlayerState_Idle;
    }

    //Start Function.
    public override void _Ready()
    {
        //Initialize the position of the player.
        _initialPosition = Position;

        //Initialize the animation objects.
        _AnimTree = GetNode<AnimationTree>("AnimationTree");
        _AnimState = (AnimationNodeStateMachinePlayback)_AnimTree.Get("parameters/playback");

        _AnimTree.Active = true;

        //Initialize the RayCast Object.
        _Ray = GetNode<RayCast2D>("RayCast2D");
    }

    //Called Every Frame.
    public override void _PhysicsProcess(float delta)
    {
        if(_PlayerState == PlayerState.PlayerState_Turning)
        {
            return;
        }
        else if(_PlayerState != PlayerState.PlayerState_Walking)
        {
            ProcessPlayerInput();
        }
        //If the direction is not zero means the player has to move and perform animation.
        else if(_inputDirection != Vector2.Zero)
        {
            _PlayerState = PlayerState.PlayerState_Walking;
            _AnimState.Travel("Walk");
            Move(delta);
        }
        else
        {
            _PlayerState = PlayerState.PlayerState_Idle;
            _AnimState.Travel("Idle");
        }
    }
}

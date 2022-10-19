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
    public float                                 JumpSpeed = 4.0f;

    //Member Variables.
    private Vector2                             _initialPosition;
    private Vector2                             _inputDirection;
    private float                               _percentMovedToNextTile = 0.0f; //From 0 to 1.
    private bool                                _isMoving = false;
    private AnimationTree                       _animTree;
    private AnimationNodeStateMachinePlayback   _animState;
    private PlayerState                         _playerState = PlayerState.PlayerState_Idle;
    private FacingDirection                     _facingDirection = FacingDirection.FacingDirection_Down;
    private RayCast2D                           _blockingRay;
    private RayCast2D                           _jumpingRay;
    private bool                                _isJumping = false;

    

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
            _animTree.Set("parameters/Idle/blend_position", _inputDirection);
            _animTree.Set("parameters/Walk/blend_position", _inputDirection);
            _animTree.Set("parameters/Turn/blend_position", _inputDirection);

            //Update the player state based on the action performed.
            if(NeedToTurn())
            {
                _playerState = PlayerState.PlayerState_Turning;
                _animState.Travel("Turn");
            }
            else
            {
                _playerState = PlayerState.PlayerState_Walking;
                _initialPosition = Position;
            }
        }
        else
        {
            _playerState = PlayerState.PlayerState_Idle;
            _animState.Travel("Idle");
        }
    }

    public void Move(float delta)
    {
        //Making sure the raycast is pointing to the direction the player is facing.
        Vector2 desired_step = _inputDirection * TILE_SIZE * 0.5f;

        _blockingRay.CastTo = desired_step;
        _blockingRay.ForceRaycastUpdate();

        _jumpingRay.CastTo = desired_step;
        _jumpingRay.ForceRaycastUpdate();

        Vector2 vec = new Vector2(0, 1);

        //Check if the player is about to jump.
        if ((_blockingRay.IsColliding() && _inputDirection == vec) || _isJumping)
        {
            _percentMovedToNextTile += JumpSpeed * delta;

            if(_percentMovedToNextTile >= 2.0f)
            {
                Position = _initialPosition + (_inputDirection * TILE_SIZE * 2);
                _percentMovedToNextTile = 0.0f;
                _playerState = PlayerState.PlayerState_Idle;
                _isJumping = false;
            }
            else
            {
                _isJumping = true;
                float input = _inputDirection.y * TILE_SIZE * _percentMovedToNextTile;
                Position = new Vector2(Position.x, _initialPosition.y + (-0.96f - 0.53f * input + 0.05f * (input * input)));
            }
        }
        //If the player is not colliding.
        else if (!_blockingRay.IsColliding())
        {
            //Update the percentage.
            _percentMovedToNextTile += WalkSpeed * delta;

            //Move and reset the values when the percentage reachs its maximum.
            if (_percentMovedToNextTile >= 1.0f)
            {
                Position = _initialPosition + (TILE_SIZE * _inputDirection);
                _percentMovedToNextTile = 0.0f;
                _playerState = PlayerState.PlayerState_Idle;
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
            _playerState = PlayerState.PlayerState_Idle;
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

        if(_facingDirection != new_facing_direction)
        {
            _facingDirection = new_facing_direction;
            return true;
        }

        _facingDirection = new_facing_direction;
        return false;
    }

    public void FinishedTurning()
    {
        //When finished turning the player stays in idle.
        _playerState = PlayerState.PlayerState_Idle;
    }

    //Start Function.
    public override void _Ready()
    {
        //Initialize the position of the player.
        _initialPosition = Position;

        //Initialize the animation objects.
        _animTree = GetNode<AnimationTree>("AnimationTree");
        _animState = (AnimationNodeStateMachinePlayback)_animTree.Get("parameters/playback");

        _animTree.Active = true;

        //Initialize the RayCast Objects.
        _blockingRay = GetNode<RayCast2D>("BlockingRayCast2D");
        _jumpingRay = GetNode<RayCast2D>("JumpingRayCast2D");
    }

    //Called Every Frame.
    public override void _PhysicsProcess(float delta)
    {
        if(_playerState == PlayerState.PlayerState_Turning)
        {
            return;
        }
        else if(_playerState != PlayerState.PlayerState_Walking)
        {
            ProcessPlayerInput();
        }
        //If the direction is not zero means the player has to move and perform animation.
        else if(_inputDirection != Vector2.Zero)
        {
            _playerState = PlayerState.PlayerState_Walking;
            _animState.Travel("Walk");
            Move(delta);
        }
        else
        {
            _playerState = PlayerState.PlayerState_Idle;
            _animState.Travel("Idle");
        }
    }
}

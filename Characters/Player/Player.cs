using Godot;
using System;

public class Player : KinematicBody2D
{
    //Enums.
    enum PlayerState
    {
        kPlayerState_None = -1,
        kPlayerState_Idle,
        kPlayerState_Turning,
        kPlayerState_Walking
    }

    enum FacingDirection
    {
        kFacingDirection_None = -1,
        kFacingDirection_Left,
        kFacingDirection_Right,
        kFacingDirection_Up,
        kFacingDirection_Down
    }

    //Events for tall grass animation.
    [Signal]
    public delegate void                        OnPlayerTileMoved();
    [Signal]
    public delegate void                        OnPlayerTileMoving();

    //Events for door animation.
    [Signal]
    public delegate void OnPlayerEnteringDoor();
    [Signal]
    public delegate void OnPlayerEnteredDoor();

    //Constant value for the tile size.
    const int                                   TILE_SIZE = 16;

    //Editor Variables.
    [Export]
    public float                                 WalkSpeed = 4.0f;
    [Export]
    public float                                 JumpSpeed = 4.0f;

    //Member Variables.
    private Vector2                             _initialPosition = new Vector2(0, 0);
    private Vector2                             _inputDirection = new Vector2(0, 1);
    private float                               _percentMovedToNextTile = 0.0f; //From 0 to 1.
    private bool                                _isMoving = false;
    private bool                                _stopInput = false;
    private AnimationTree                       _animTree;
    private AnimationNodeStateMachinePlayback   _animState;
    private PlayerState                         _playerState = PlayerState.kPlayerState_Idle;
    private FacingDirection                     _facingDirection = FacingDirection.kFacingDirection_Down;
    private RayCast2D                           _blockingRay;
    private RayCast2D                           _jumpingRay;
    private RayCast2D                           _doorRay;
    private bool                                _isJumping = false;
    private Sprite                              _shadowSprite;
    private PackedScene                         _landingDustEffect = ResourceLoader.Load<PackedScene>("res://EnvAnimatedTextures/LandingDustEffect.tscn");



    //Member Functions.
    public bool IsMoving()
    {
        return _playerState != PlayerState.kPlayerState_Idle;
    }

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
                _playerState = PlayerState.kPlayerState_Turning;
                _animState.Travel("Turn");
            }
            else
            {
                _playerState = PlayerState.kPlayerState_Walking;
                _initialPosition = Position;
            }
        }
        else
        {
            _playerState = PlayerState.kPlayerState_Idle;
            _animState.Travel("Idle");
        }
    }

    public void SetSpawn(Vector2 spawn_location, Vector2 spawn_direction)
    {
        //Update the Blend position with the input direction.
        _animTree.Set("parameters/Idle/blend_position", spawn_direction);
        _animTree.Set("parameters/Walk/blend_position", spawn_direction);
        _animTree.Set("parameters/Turn/blend_position", spawn_direction);

        Position = spawn_location;
    }

    public void Move(float delta)
    {
        //Making sure the raycast is pointing to the direction the player is facing.
        Vector2 desired_step = _inputDirection * TILE_SIZE * 0.5f;

        _blockingRay.CastTo = desired_step;
        _blockingRay.ForceRaycastUpdate();

        _jumpingRay.CastTo = desired_step;
        _jumpingRay.ForceRaycastUpdate();

        _doorRay.CastTo = desired_step;
        _doorRay.ForceRaycastUpdate();

        Vector2 vec = new Vector2(0, 1);

        //Check if the player is about to enter a door.
        if (_doorRay.IsColliding())
        {
            if (_percentMovedToNextTile == 0.0f)
            {
                EmitSignal(nameof(OnPlayerEnteringDoor));
            }
            if (_percentMovedToNextTile >= 1.0f)
            {
                Position = _initialPosition + (_inputDirection * TILE_SIZE);
                _percentMovedToNextTile = 0.0f;
                _playerState = PlayerState.kPlayerState_Idle;
                _stopInput = true;

                GetNode<AnimationPlayer>("AnimationPlayer").Play("Disappear");
                GetNode<Camera2D>("Camera2D").ClearCurrent();
            }
            else
            {
                Position = _initialPosition + (TILE_SIZE * _inputDirection * _percentMovedToNextTile);
            }

            _percentMovedToNextTile += WalkSpeed * delta;
        }
        //Check if the player is about to jump.
        else if ((_jumpingRay.IsColliding() && _inputDirection == vec) || _isJumping)
        {
            _percentMovedToNextTile += JumpSpeed * delta;

            if(_percentMovedToNextTile >= 2.0f)
            {
                Position = _initialPosition + (_inputDirection * TILE_SIZE * 2);
                _percentMovedToNextTile = 0.0f;
                _playerState = PlayerState.kPlayerState_Idle;
                _isJumping = false;
                _shadowSprite.Visible = false;

                //Create the animation jumping dust animation instance and add it to the scene.
                AnimatedSprite anim_sprite = _landingDustEffect.Instance() as AnimatedSprite;
                anim_sprite.Position = Position;
                GetTree().CurrentScene.AddChild(anim_sprite);
            }
            else
            {
                _shadowSprite.Visible = true;
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
                _playerState = PlayerState.kPlayerState_Idle;
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
            _playerState = PlayerState.kPlayerState_Idle;
        }
    }

    public bool NeedToTurn()
    {
        FacingDirection new_facing_direction = FacingDirection.kFacingDirection_None;

        if(_inputDirection.x < 0)
        {
            new_facing_direction = FacingDirection.kFacingDirection_Left;
        }
        else if (_inputDirection.x > 0)
        {
            new_facing_direction = FacingDirection.kFacingDirection_Right;
        }
        else if (_inputDirection.y < 0)
        {
            new_facing_direction = FacingDirection.kFacingDirection_Up;
        }
        else if (_inputDirection.y > 0)
        {
            new_facing_direction = FacingDirection.kFacingDirection_Down;
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
        _playerState = PlayerState.kPlayerState_Idle;
    }

    public void EnteredDoor()
    {
        EmitSignal(nameof(OnPlayerEnteredDoor));
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
        _doorRay = GetNode<RayCast2D>("DoorRayCast2D");

        //Initialize the shadow sprite for ledge jumping.
        _shadowSprite = GetNode<Sprite>("Shadow");
        _shadowSprite.Visible = false;

        //Make sure the player sprite always starts visible
        GetNode<Sprite>("Sprite").Visible = true;

        //Update the Blend position with the input direction.
        _animTree.Set("parameters/Idle/blend_position", _inputDirection);
        _animTree.Set("parameters/Walk/blend_position", _inputDirection);
        _animTree.Set("parameters/Turn/blend_position", _inputDirection);

        //Setting the player instance on the utils because the player changes when changing the scene.
        //NOTE: this has to be reorganized, maybe only having a ref on the utils class and instancing that
        //reference every time we change scenes.
        Utils.Instance().SetPlayerNode(this);
    }

    //Called Every Frame.
    public override void _PhysicsProcess(float delta)
    {
        if(_playerState == PlayerState.kPlayerState_Turning || _stopInput)
        {
            return;
        }
        else if(_playerState != PlayerState.kPlayerState_Walking)
        {
            ProcessPlayerInput();
        }
        //If the direction is not zero means the player has to move and perform animation.
        else if(_inputDirection != Vector2.Zero)
        {
            _playerState = PlayerState.kPlayerState_Walking;
            _animState.Travel("Walk");
            Move(delta);
        }
        else
        {
            _playerState = PlayerState.kPlayerState_Idle;
            _animState.Travel("Idle");
        }
    }
}

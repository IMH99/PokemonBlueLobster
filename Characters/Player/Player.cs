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
    private Enums.FacingDirection               _facingDirection = Enums.FacingDirection.kFacingDirection_Down;
    private RayCast2D                           _blockingRay;
    private RayCast2D                           _jumpingRay;
    private RayCast2D                           _doorRay;
    private bool                                _isJumping = false;
    private Sprite                              _shadowSprite;
    private PackedScene                         _landingDustEffect = ResourceLoader.Load<PackedScene>("res://EnvAnimatedTextures/LandingDustEffect.tscn");
    private bool                                _isColliding = false;

    private Godot.Collections.Array<Pokemon>    _pokemonParty;


    //Member Functions.
    public bool IsMoving()
    {
        return _playerState != PlayerState.kPlayerState_Idle;
    }

    public bool IsJumping()
    {
        return _isJumping;
    }

    public bool NeedToTurn()
    {
        Enums.FacingDirection new_facing_direction = Enums.FacingDirection.kFacingDirection_None;

        if (_inputDirection.x < 0)
        {
            new_facing_direction = Enums.FacingDirection.kFacingDirection_Left;
        }
        else if (_inputDirection.x > 0)
        {
            new_facing_direction = Enums.FacingDirection.kFacingDirection_Right;
        }
        else if (_inputDirection.y < 0)
        {
            new_facing_direction = Enums.FacingDirection.kFacingDirection_Up;
        }
        else if (_inputDirection.y > 0)
        {
            new_facing_direction = Enums.FacingDirection.kFacingDirection_Down;
        }

        if (_facingDirection != new_facing_direction)
        {
            _facingDirection = new_facing_direction;
            return true;
        }

        _facingDirection = new_facing_direction;
        return false;
    }

    public Godot.Collections.Array<Pokemon> GetPokemonParty()
    {
        return _pokemonParty;
    }

    public void SetSpawn(Vector2 spawn_location, Vector2 spawn_direction)
    {
        //Update the Blend position with the input direction.
        _animTree.Set("parameters/Idle/blend_position", spawn_direction);
        _animTree.Set("parameters/Walk/blend_position", spawn_direction);
        _animTree.Set("parameters/Turn/blend_position", spawn_direction);

        Position = spawn_location;
    }

    private void ProcessPlayerInput()
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

            //Making sure the raycast is pointing to the direction the player is facing.

            Vector2 desired_step = _inputDirection * TILE_SIZE * 0.5f;

            _blockingRay.CastTo = desired_step;
            _blockingRay.ForceRaycastUpdate();

            _jumpingRay.CastTo = desired_step;
            _jumpingRay.ForceRaycastUpdate();

            _doorRay.CastTo = desired_step;
            _doorRay.ForceRaycastUpdate();
        }
        else
        {
            _playerState = PlayerState.kPlayerState_Idle;
            _animState.Travel("Idle");
        }
    }

    public void HandleEntertingDoor(float delta)
    {
        //On the first frame we send the signal to the door object which will activate the door animation.
        if (_percentMovedToNextTile == 0.0f)
        {
            EmitSignal(nameof(OnPlayerEnteringDoor));
        }
        else if (_percentMovedToNextTile >= 1.0f)
        {
            //When we moved one tile we finished the movement so we reset the values.
            Position = _initialPosition + (_inputDirection * TILE_SIZE);
            _percentMovedToNextTile = 0.0f;
            _playerState = PlayerState.kPlayerState_Idle;
            _stopInput = true;

            //Call the animation for disappear and clear the camera.
            GetNode<AnimationPlayer>("AnimationPlayer").Play("Disappear");
            GetNode<Camera2D>("Camera2D").ClearCurrent();
        }
        else
        {
            //Between 0 and 1 percent the player will move.
            Position = _initialPosition + (TILE_SIZE * _inputDirection * _percentMovedToNextTile);
        }

        _percentMovedToNextTile += WalkSpeed * delta;
    }

    public void HandleJumping(float delta)
    {
        if (_percentMovedToNextTile == 0.0f)
        {
            //On the first frame we get the tilemap to access the direction of the ledges.
            TileMap collider = (TileMap)_jumpingRay.GetCollider();
            if (IsInstanceValid(collider))
            {
                LedgeTileMap ledge = (LedgeTileMap)collider;

                //If the player direction is equal to the ledge then we check if we can jump.
                if (ledge.Direction == _facingDirection)
                {
                    //To check if the player can jump we move the raycast to the next tile and check if collides.
                    Vector2 _tmpJumpingRayPosition = _jumpingRay.Position;
                    _jumpingRay.Position = new Vector2(_tmpJumpingRayPosition.x + (_inputDirection.x * TILE_SIZE * 2.0f), _tmpJumpingRayPosition.y + (_inputDirection.y * TILE_SIZE * 2.0f));
                    _jumpingRay.ForceRaycastUpdate();

                    if (_jumpingRay.IsColliding())
                    {
                        //If collides, return the initial value to the raycast and set the player state to idle.
                        _jumpingRay.Position = _tmpJumpingRayPosition;
                        _jumpingRay.ForceRaycastUpdate();

                        _playerState = PlayerState.kPlayerState_Idle;

                        return;
                    }

                    //If we do not collide pick the animation from the direction we are facing and set the initial value of the raycast.
                    switch (_facingDirection)
                    {
                        case Enums.FacingDirection.kFacingDirection_Down:
                            GetNode<AnimationPlayer>("AnimationPlayer").Play("JumpDown");
                            break;
                        case Enums.FacingDirection.kFacingDirection_Left:
                            GetNode<AnimationPlayer>("AnimationPlayer").Play("JumpLeft");
                            break;
                        case Enums.FacingDirection.kFacingDirection_Up:
                            GetNode<AnimationPlayer>("AnimationPlayer").Play("JumpUp");
                            break;
                        case Enums.FacingDirection.kFacingDirection_Right:
                            GetNode<AnimationPlayer>("AnimationPlayer").Play("JumpRight");
                            break;
                    };

                    _isJumping = true;
                    _jumpingRay.Position = _tmpJumpingRayPosition;
                    _jumpingRay.ForceRaycastUpdate();
                }
                else
                {
                    //If we do not face the same direction as the edge then make the player idle to stop moving.
                    _playerState = PlayerState.kPlayerState_Idle;

                    return;
                }
            }
        }

        //On the next frames the player is going to move with the percent.
        _percentMovedToNextTile += JumpSpeed * delta;

        if (_percentMovedToNextTile >= 2.0f)
        {
            //When we moved 2 tiles we finished the movement.
            Position = _initialPosition + (_inputDirection * TILE_SIZE * 2);
            _percentMovedToNextTile = 0.0f;
            _playerState = PlayerState.kPlayerState_Idle;
            _isJumping = false;

            //Create the animation jumping dust animation instance and add it to the scene.
            AnimatedSprite anim_sprite = _landingDustEffect.Instance() as AnimatedSprite;
            anim_sprite.Position = Position;
            GetTree().CurrentScene.AddChild(anim_sprite);
        }
        else
        {
            //Between 0 and 2 percent the player will move.
            Position = _initialPosition + (TILE_SIZE * _inputDirection * _percentMovedToNextTile);
        }
    }

    public void HandleColliding(float delta)
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
                if (!_isMoving)
                {
                    EmitSignal(nameof(OnPlayerTileMoving));
                }
                _isMoving = true;
            }
            else
            {
                if (_isMoving)
                {
                    EmitSignal(nameof(OnPlayerTileMoved));
                }
                _isMoving = false;
            }

            Position = _initialPosition + (TILE_SIZE * _inputDirection * _percentMovedToNextTile);
        }
    }

    private void Move(float delta)
    {
        //Check if the player is about to enter a door.
        if (_doorRay.IsColliding())
        {
            HandleEntertingDoor(delta);
        }
        //Check if the player is about to jump.
        else if ((_jumpingRay.IsColliding()) || _isJumping)
        {
            HandleJumping(delta);
        }
        //If the player is not colliding.
        else if (!_blockingRay.IsColliding())
        {
            HandleColliding(delta);
        }
        else
        {
            _playerState = PlayerState.kPlayerState_Idle;
        }
    }

    private void FinishedTurning()
    {
        //When finished turning the player stays in idle.
        _playerState = PlayerState.kPlayerState_Idle;
    }

    private void EnteredDoor()
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

        //Adding manually the Pokemon Party by Pokemon number (check constructors in Pokemon.cs to see the posibilities). 
        _pokemonParty = new Godot.Collections.Array<Pokemon>();
        _pokemonParty.Add(new Pokemon(1));
        _pokemonParty.Add(new Pokemon(4));
        _pokemonParty.Add(new Pokemon(7));
        _pokemonParty.Add(new Pokemon(12));
        _pokemonParty.Add(new Pokemon(17));
        _pokemonParty.Add(new Pokemon(25));
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

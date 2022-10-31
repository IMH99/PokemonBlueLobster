using Godot;
using System;

public class PokemonPartyScreen : Node2D
{
    //Enum for the different options the player can select in this menu.
    enum Options
    {
        kOptions_FirstSlot,
        kOptions_SecondSlot,
        kOptions_ThirdSlot,
        kOptions_FouthSlot,
        kOptions_FifthSlot,
        kOptions_SixthSlot,
        kOptions_CancelButton
    }

    private Options                                         _selectedOption = Options.kOptions_FirstSlot;
    private Godot.Collections.Dictionary<Options, Sprite>   _optionsDictionary = new Godot.Collections.Dictionary<Options, Sprite>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _optionsDictionary.Add(Options.kOptions_FirstSlot, GetNode<Node2D>("FirstPokemonSlot").GetNode<Sprite>("Background"));
        _optionsDictionary.Add(Options.kOptions_SecondSlot, GetNode<Node2D>("SecondPokemonSlot").GetNode<Sprite>("Background"));
        _optionsDictionary.Add(Options.kOptions_ThirdSlot, GetNode<Node2D>("ThirdPokemonSlot").GetNode<Sprite>("Background"));
        _optionsDictionary.Add(Options.kOptions_FouthSlot, GetNode<Node2D>("FourthPokemonSlot").GetNode<Sprite>("Background"));
        _optionsDictionary.Add(Options.kOptions_FifthSlot, GetNode<Node2D>("FifthPokemonSlot").GetNode<Sprite>("Background"));
        _optionsDictionary.Add(Options.kOptions_SixthSlot, GetNode<Node2D>("SixthPokemonSlot").GetNode<Sprite>("Background"));
        _optionsDictionary.Add(Options.kOptions_CancelButton, GetNode<Sprite>("CancelButtonSprite"));

        SetActiveOption();

        Godot.Collections.Array<Sprite> party_sprites = new Godot.Collections.Array<Sprite>();
        party_sprites.Add(GetNode<Node2D>("FirstPokemonSlot").GetNode<Sprite>("PokemonPartyImage"));
        party_sprites.Add(GetNode<Node2D>("SecondPokemonSlot").GetNode<Sprite>("PokemonPartyImage"));
        party_sprites.Add(GetNode<Node2D>("ThirdPokemonSlot").GetNode<Sprite>("PokemonPartyImage"));
        party_sprites.Add(GetNode<Node2D>("FourthPokemonSlot").GetNode<Sprite>("PokemonPartyImage"));
        party_sprites.Add(GetNode<Node2D>("FifthPokemonSlot").GetNode<Sprite>("PokemonPartyImage"));
        party_sprites.Add(GetNode<Node2D>("SixthPokemonSlot").GetNode<Sprite>("PokemonPartyImage"));

        Godot.Collections.Array<Pokemon> player_party = Utils.Instance().GetPlayerNode().GetPokemonParty();

        for (int i = 0; i < player_party.Count; ++i)
        {
            party_sprites[i].Texture = player_party[i].GetPartySprite().Texture;
        }
    }

    public void UnsetActiveOption()
    {
        _optionsDictionary[_selectedOption].Frame = 0;
    }

    public void SetActiveOption()
    {
        _optionsDictionary[_selectedOption].Frame = 1;
    }

    public override void _Input(InputEvent @event)
    {
        if(@event.IsActionPressed("ui_down"))
        {
            UnsetActiveOption();
            _selectedOption = (Options)(((int)_selectedOption + 1) % 7);
            SetActiveOption();
        }
        else if(@event.IsActionPressed("ui_up"))
        {
            UnsetActiveOption();
            if(_selectedOption == Options.kOptions_FirstSlot)
            {
                _selectedOption = Options.kOptions_CancelButton;
            }
            else
            {
                _selectedOption -= 1;
            }
            SetActiveOption();
        }
        else if(@event.IsActionPressed("ui_left"))
        {
            UnsetActiveOption();
            _selectedOption = Options.kOptions_FirstSlot;
            SetActiveOption();
        }
        else if (@event.IsActionPressed("ui_right") && _selectedOption == Options.kOptions_FirstSlot)
        {
            UnsetActiveOption();
            _selectedOption = Options.kOptions_SecondSlot;
            SetActiveOption();
        }
        else if (@event.IsActionPressed("Select"))
        {
            switch(_selectedOption)
            {
                case Options.kOptions_FirstSlot:
                    break;

                case Options.kOptions_SecondSlot:
                    break;

                case Options.kOptions_ThirdSlot:
                    break;

                case Options.kOptions_FouthSlot:
                    break;

                case Options.kOptions_FifthSlot:
                    break;

                case Options.kOptions_SixthSlot:
                    break;

                case Options.kOptions_CancelButton:
                    Utils.Instance().GetSceneManager().TransitionExitPartyScene();
                    break;
            }
        }
    }
}

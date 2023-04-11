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

    private PokemonOptions                                  _pokemonOptions;
    private Options                                         _selectedOption = Options.kOptions_FirstSlot;
    private Godot.Collections.Dictionary<Options, Sprite>   _optionsDictionary = new Godot.Collections.Dictionary<Options, Sprite>();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        //Initialize the pokemon options menu.
        _pokemonOptions = GetNode<PokemonOptions>("PokemonOptions");
        _pokemonOptions._menuControl.SetProcessInput(false);


        //Easy access to the nodes.
        Godot.Collections.Array<Node2D> pokemonPartyNodes = new Godot.Collections.Array<Node2D>();
        pokemonPartyNodes.Add(GetNode<Node2D>("FirstPokemonSlot"));
        pokemonPartyNodes.Add(GetNode<Node2D>("SecondPokemonSlot"));
        pokemonPartyNodes.Add(GetNode<Node2D>("ThirdPokemonSlot"));
        pokemonPartyNodes.Add(GetNode<Node2D>("FourthPokemonSlot"));
        pokemonPartyNodes.Add(GetNode<Node2D>("FifthPokemonSlot"));
        pokemonPartyNodes.Add(GetNode<Node2D>("SixthPokemonSlot"));

        //Get the backgrounds to add them to the dictionary. 
        for (int i = 0; i < pokemonPartyNodes.Count; ++i)
        {
            _optionsDictionary.Add((Options)i, pokemonPartyNodes[i].GetNode<Sprite>("Background"));
        }

        //Also the cancel button and set the active option.
        _optionsDictionary.Add(Options.kOptions_CancelButton, GetNode<Sprite>("CancelButtonSprite"));
        SetActiveOption();

        //Set the menu information based on the player party
        //NOTE: at the moment the pokemon names are not displayed in the party screen, as we need a method to retrieve the name of the pokemon from JSON or DB.
        Godot.Collections.Array<Pokemon> player_party = Utils.Instance().GetPlayerNode().GetPokemonParty();
        for (int i = 0; i < player_party.Count; ++i)
        {
            Pokemon.PokemonInfo info = player_party[i].GetPokemonInfo();

            pokemonPartyNodes[i].GetNode<Sprite>("PokemonPartyImage").Texture = player_party[i].GetPartySprite().Texture;
            pokemonPartyNodes[i].GetNode<Label>("PokemonName").Text = info.Name;
            pokemonPartyNodes[i].GetNode<Label>("PokemonLevel").Text = info.Level.ToString();
            
            pokemonPartyNodes[i].GetNode<Sprite>("PokemonGender").Texture = (Texture)GD.Load("res://Assets/Tiles/Menus/Pokemon/gender_icons.png");
            pokemonPartyNodes[i].GetNode<Sprite>("PokemonGender").Frame = (int)info.Gender - 1;
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
        //We have to do this because setting the blocking signal to false makes the input 
        //be received by this function.
        if(!_pokemonOptions._skipFrame)
        {
            //While Pokemon Options is not proccessing input
            if (_pokemonOptions._menuControl.IsBlockingSignals())
            {
                if (@event.IsActionPressed("ui_down"))
                {
                    UnsetActiveOption();
                    _selectedOption = (Options)(((int)_selectedOption + 1) % 7);
                    SetActiveOption();
                }
                else if (@event.IsActionPressed("ui_up"))
                {
                    UnsetActiveOption();
                    if (_selectedOption == Options.kOptions_FirstSlot)
                    {
                        _selectedOption = Options.kOptions_CancelButton;
                    }
                    else
                    {
                        _selectedOption -= 1;
                    }
                    SetActiveOption();
                }
                else if (@event.IsActionPressed("ui_left"))
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
                    if (_selectedOption == Options.kOptions_FirstSlot || _selectedOption == Options.kOptions_SecondSlot ||
                        _selectedOption == Options.kOptions_ThirdSlot || _selectedOption == Options.kOptions_FouthSlot ||
                        _selectedOption == Options.kOptions_FifthSlot || _selectedOption == Options.kOptions_SixthSlot)
                    {
                        //Show Pokemon Options Menu.
                        _pokemonOptions.ShowMenu();
                    }
                    else
                    {
                        //Exit Menu (the last option is to select exit button).
                        Utils.Instance().GetSceneManager().TransitionExitPartyScene();
                    }
                }
                else if (@event.IsActionPressed("ExitMenu"))
                {
                    Utils.Instance().GetSceneManager().TransitionExitPartyScene();
                }
            }
        }
        else
        {
            _pokemonOptions._skipFrame = false;
        }
    }    
}

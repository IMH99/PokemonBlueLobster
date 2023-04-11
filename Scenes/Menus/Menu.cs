using Godot;
using System;

public class Menu : CanvasLayer
{
    //Enum to handle the input differently based on these options.
    //NOTE: in the future we should add bag, trainer card, options, etc.
    enum ScreenLoaded
    {
        kScreenLoaded_Nothing,
        kScreenLoaded_Menu,
        kScreenLoaded_Pokedex,
        kScreenLoaded_Party,
        kScreenLoaded_Bag,
        kScreenLoaded_User,
        kScreenLoaded_Options
    }

    enum OptionSelected
    {
        kOptionSelected_Pokedex,
        kOptionSelected_Pokemon,
        kOptionSelected_Bag,
        kOptionSelected_User,
        kOptionSelected_Save,
        kOptionSelected_Option,
        kOptionSelected_Exit
    }

    private TextureRect             _selectArrow;
    private Control                 _menuControl;
    private ScreenLoaded            _screenLoaded = ScreenLoaded.kScreenLoaded_Nothing;
    private OptionSelected          _optionSelected = OptionSelected.kOptionSelected_Pokedex;
    private PackedScene             _pokemonPartyScreen = ResourceLoader.Load<PackedScene>("res://Scenes/Menus/PokemonPartyScreen.tscn");

    public void LoadPartyScreen()
    {
        _menuControl.Visible = false;
        _screenLoaded = ScreenLoaded.kScreenLoaded_Party;
        AddChild(_pokemonPartyScreen.Instance());
    }

    public void UnLoadPartyScreen()
    {
        _menuControl.Visible = true;
        _screenLoaded = ScreenLoaded.kScreenLoaded_Menu;
        RemoveChild(GetNode<Node2D>("PokemonPartyScreen"));
    }

    private void UpdateArrowPosition(int option)
    {
        //This position matches the x and y for the option arrow.
        //The second 6 is the number of options we have in our enum.
        //The 15 is an increment to how much the arrow moves up and down.
        _selectArrow.RectPosition = new Vector2(8, 6 + (option % 8) * 15);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _menuControl = (Control)GetNode<Control>("Control");

        if (IsInstanceValid(_menuControl))
        {
            _menuControl.Visible = false;
            _selectArrow = (TextureRect)_menuControl.GetNode<NinePatchRect>("NinePatchRect").GetNode<TextureRect>("SelectArrow");

            //This position matches the x and y for the option arrow.
            //The second 6 is the number of options we have in our enum.
            //The 15 is an increment to how much the arrow moves up and down.
            UpdateArrowPosition((int)_optionSelected);

        }
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        switch(_screenLoaded)
        {
            case ScreenLoaded.kScreenLoaded_Nothing:
                if(@event.IsActionPressed("Menu"))
                {
                    if (!Utils.Instance().GetPlayerNode().IsMoving())
                    {
                        Utils.Instance().GetPlayerNode().SetPhysicsProcess(false);
                        _menuControl.Visible = true;
                        _screenLoaded = ScreenLoaded.kScreenLoaded_Menu;
                    }
                }
                break;

            case ScreenLoaded.kScreenLoaded_Menu:
                if (@event.IsActionPressed("Menu") || @event.IsActionPressed("ExitMenu"))
                {
                    Utils.Instance().GetPlayerNode().SetPhysicsProcess(true);
                    _menuControl.Visible = false;
                    _screenLoaded = ScreenLoaded.kScreenLoaded_Nothing;
                }
                else if(@event.IsActionPressed("ui_up"))
                {
                    if(_optionSelected != OptionSelected.kOptionSelected_Pokedex)
                    {
                        _optionSelected -= 1;
                        UpdateArrowPosition((int)_optionSelected);
                    }
                }
                else if (@event.IsActionPressed("ui_down"))
                {
                    if (_optionSelected != OptionSelected.kOptionSelected_Exit)
                    {
                        _optionSelected += 1;
                        UpdateArrowPosition((int)_optionSelected);
                    }    
                }
                else if(@event.IsActionPressed("Select"))
                {
                    if(_optionSelected == OptionSelected.kOptionSelected_Pokemon)
                    {
                        Utils.Instance().GetSceneManager().TransitionToPartyScene();
                    }
                }
                break;

            case ScreenLoaded.kScreenLoaded_Pokedex:

                break;

            case ScreenLoaded.kScreenLoaded_Party:
                if (@event.IsActionPressed("ExitMenu"))
                {
                    //We moved this functionality to PokemonParty script.
                    //Utils.Instance().GetSceneManager().TransitionExitPartyScene();
                }
                break;

            case ScreenLoaded.kScreenLoaded_Bag:

                break;

            case ScreenLoaded.kScreenLoaded_User:

                break;

            case ScreenLoaded.kScreenLoaded_Options:

                break;
        }
    }
}

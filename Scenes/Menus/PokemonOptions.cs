using Godot;
using System;

public class PokemonOptions : CanvasLayer
{
    enum OptionSelected
    {
        kOptionSelected_Data,
        kOptionSelected_Switch,
        kOptionSelected_Item,
        kOptionSelected_Exit
    }

    public bool                 _skipFrame = false;

    public Control              _menuControl;
    private TextureRect         _selectArrow;
    private OptionSelected      _optionSelected = OptionSelected.kOptionSelected_Data;

    private void UpdateArrowPosition(int option)
    {
        //This position matches the x and y for the option arrow.
        //The second 4 is the number of options we have in our enum.
        //The 15 is an increment to how much the arrow moves up and down.
        _selectArrow.RectPosition = new Vector2(8, 6 + (option % 4) * 15);
    }

    public void ShowMenu()
    {
        _optionSelected = OptionSelected.kOptionSelected_Data;
        UpdateArrowPosition((int)_optionSelected);

        _menuControl.Visible = true;
        _menuControl.SetBlockSignals(false);
    }

    private void ExitMenu()
    {
        _menuControl.Visible = false;
        _menuControl.SetBlockSignals(true);
        _skipFrame = true;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _menuControl = (Control)GetNode<Control>("Control");

        if (IsInstanceValid(_menuControl))
        {
            _menuControl.Visible = false;
            _menuControl.SetBlockSignals(true);
            _selectArrow = (TextureRect)_menuControl.GetNode<NinePatchRect>("NinePatchRect").GetNode<TextureRect>("SelectArrow");
            UpdateArrowPosition((int)_optionSelected);
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (!_menuControl.IsBlockingSignals())
        {
            if (@event.IsActionPressed("ExitMenu"))
            {
                ExitMenu();   
            }
            else if (@event.IsActionPressed("ui_up"))
            {
                if (_optionSelected != OptionSelected.kOptionSelected_Data)
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
            else if (@event.IsActionPressed("Select"))
            {
                switch (_optionSelected)
                {
                    case OptionSelected.kOptionSelected_Data:
                        //Transition to Data Scene.
                        break;
                    case OptionSelected.kOptionSelected_Switch:
                        //Switch position logic.
                        break;
                    case OptionSelected.kOptionSelected_Item:
                        //Transition to Bag.
                        break;
                    case OptionSelected.kOptionSelected_Exit:
                        ExitMenu();
                        break;
                }
            }
        }
    }   
}

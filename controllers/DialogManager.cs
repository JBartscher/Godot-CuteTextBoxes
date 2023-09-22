using System;
using CuteTextBoxes.scenes.text_box;
using Godot;
using Godot.Collections;

namespace CuteTextBoxes.scenes;

public partial class DialogManager : Node
{
    private PackedScene _textBoxScene;
    private text_box.TextBox _currentTextBox;

    private Array<String> _dialogLines = new Array<string>();
    private int _dialogLineIndex = 0;

    private bool _isActive = false;
    private bool _canAdvanceLine = false;

    public override void _Ready()
    {
        GD.Print("_Ready in DialogManger");
        _textBoxScene = (PackedScene)ResourceLoader.Load("res://scenes/text_box/text_box.tscn");
    }

    public void StartDialog(Vector2 at, Array<String> lines)
    {
        if (_isActive)
        {
            return;
        }

        _dialogLines = lines;
        ShowTextBox(at);
        _isActive = true;
    }

    /**
     * Actually display the TextBox by adding it to the tree root and setting its position and the first line
     */
    private void ShowTextBox(Vector2 at)
    {
        _currentTextBox = _textBoxScene.Instantiate<TextBox>();
        //GetTree().Root.CallDeferred("add_child", _currentTextBox);
        AddChild(_currentTextBox);

        // when all text is displayed make ready to advance to the next line (this will be done by input) 
        _currentTextBox.FinishedText += () => { _canAdvanceLine = true; };
        // position TextBox
        _currentTextBox.GlobalPosition = at;

        // set the first sentence as the text to be displayed
        _currentTextBox.DisplayText(_dialogLines[_dialogLineIndex]);
        _canAdvanceLine = false;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("advance_dialog") && _canAdvanceLine && _isActive)
        {
            GD.Print("advance dialog");
            var at = _currentTextBox.GlobalPosition;
            _currentTextBox.QueueFree();

            _dialogLineIndex++;

            // dialog finished => reset DialogManager
            if (_dialogLineIndex >= _dialogLines.Count)
            {
                _isActive = false;
                _dialogLineIndex = 0;
                _currentTextBox = null;

                return;
            }

            // dialog not finished => show new TextBox with next line
            ShowTextBox(at);
        }
    }
}
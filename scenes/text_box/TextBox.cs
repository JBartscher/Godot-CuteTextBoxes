using System;
using Godot;

namespace CuteTextBoxes.scenes.text_box;

public partial class TextBox : MarginContainer
{
    private Timer _letterTimer;
    private Label _label;

    private const int MaxWidth = 256;
    private string _currentText = "";
    private int _letterIndex = 0;

    private float _letterTime = 0.03f;
    private float _spaceTime = 0.06f;
    private float _punctuationTime = 0.03f;

    [Signal]
    public delegate void FinishedTextEventHandler();

    public override void _Ready()
    {
        GD.Print("_Ready in TextBox");
        _letterTimer = GetNode<Timer>("LetterDisplayTimer");
        _label = GetNode<Label>("LabelMarginContainer/Label");

        // this will continue to draw letters until all letters are printed
        _letterTimer.Timeout += DisplayLetter;
    }

    public void DisplayText(String textToDisplay)
    {
        _currentText = textToDisplay;
        _label.Text = textToDisplay;

        Resized += () =>
        {
            GD.Print("resized!");

            var customSize = CustomMinimumSize;
            customSize.X = Mathf.Min(Size.X, MaxWidth);
            CustomMinimumSize = customSize;
            if (Size.X > MaxWidth)
            {
                _label.AutowrapMode = TextServer.AutowrapMode.Word;
                Resized += () => { GD.Print("waited for X resize"); }; // wait for x to resize
                Resized += () => { GD.Print("waited for Y resize"); }; // wait for y to resize
               customSize = CustomMinimumSize;
                customSize.Y = Size.Y;
                CustomMinimumSize = customSize;
            }
        };

        

        GlobalPosition = new Vector2(Size.X / 2, Size.Y + 24);
        _label.Text = "";

        // start typing
        DisplayLetter();
    }

    private void DisplayLetter()
    {
        _label.Text += _currentText[_letterIndex];
        _letterIndex++;
        if (_letterIndex >= _currentText.Length)
        {
            EmitSignal(SignalName.FinishedText);
            return;
        }

        // not the last letter, wait short duration than continue
        switch (_currentText[_letterIndex])
        {
            case '!':
            case '.':
            case ',':
            case ':':
            case '?':
                _letterTimer.Start(_punctuationTime);
                break;
            case ' ':
                _letterTimer.Start(_spaceTime);
                break;
            default:
                _letterTimer.Start(_letterTime);
                break;
        }
    }
}
using System;
using Godot;
using Godot.Collections;

namespace CuteTextBoxes.scenes;

public partial class TestDialogBox : Node2D
{
    public override void _Ready()
    {
        GD.Print("_Ready in TestDialogBox");
        Array<string> lines = new Godot.Collections.Array<string>()
        {
            "One Moment ...", "I know you from somewhere", "...",
            "Maybe in that pub in that little town by that lovely river next to that mountain with the caves and caverns ...",
            "Hmmm ... never mind."
        };


        var dialogManager = GetNode<DialogManager>("/root/DialogManager");

        dialogManager.StartDialog(new Vector2(250, 200), lines);
    }
}
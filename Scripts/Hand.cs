using Godot;

[GlobalClass]
public partial class Hand : HBoxContainer
{
    public override void _Ready()
    {
        foreach (Node child in GetChildren())
        {
            if (child is CardUI cardUi)
            {
                cardUi.ReparentRequest += _OnCardUiReparentRequested;
            }
        }
    }

    private void _OnCardUiReparentRequested(CardUI child)
    {
        child.Reparent(this);
    }
}

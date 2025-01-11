using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class PetAnimation : Node2D
{
    private Node2D slot;
    private Vector2 _targetPosition;
    private Game game {get {return MainNode.game;}}

    public override void _Ready()
    {
        slot = (Node2D)GetParent();
    }

    public async Task AnimateSwap(Node swapSlot)
    {
        Sprite2D _sprite1 = slot.GetNode<Sprite2D>("Pet");
        Sprite2D _sprite2 = swapSlot.GetNode<Sprite2D>("Pet");

        Vector2 sprite1Start = _sprite2.ToLocal(_sprite1.GlobalPosition);
        Vector2 sprite2Start = _sprite1.ToLocal(_sprite2.GlobalPosition);
        _sprite2.ZIndex = 1;
        _sprite1.ZIndex = 1;

        // Create a new Tween instance
        Tween tween = GetTree().CreateTween();

        // Animate sprite1 moving to sprite2's position
        tween.Parallel().
            TweenProperty(_sprite1, "position", sprite2Start, 0.1f)
            .SetTrans(Tween.TransitionType.Linear)
            .SetEase(Tween.EaseType.InOut);

        // Animate sprite2 moving to sprite1's position
        tween.Parallel().
            TweenProperty(_sprite2, "position", sprite1Start, 0.1f)
            .SetTrans(Tween.TransitionType.Linear)
            .SetEase(Tween.EaseType.InOut);

        await ToSignal(tween, "finished");
        _sprite2.ZIndex = 0;
        _sprite1.ZIndex = 0;
    }

    public async Task AnimateAttack()
    {
        //-1 if on left, 1 if on right
        int positionSign = Math.Sign(slot.Position.X - (int)ProjectSettings.GetSetting("display/window/size/viewport_width")/2);
        Sprite2D _sprite = slot.GetNode<Sprite2D>("Pet");
        _sprite.ZIndex = 1;

        Tween tween = GetTree().CreateTween();

        tween.
            TweenProperty(_sprite, "position", new Vector2(_sprite.Position.X - positionSign * 100f, _sprite.Position.Y), 0.5f)
            .SetTrans(Tween.TransitionType.Linear)
            .SetEase(Tween.EaseType.InOut);

        await ToSignal(tween, "finished");
    }

    public async Task AnimateAttackRecover()
    {
        int positionSign = Math.Sign(slot.Position.X - (int)ProjectSettings.GetSetting("display/window/size/viewport_width")/2);
        Sprite2D _sprite = slot.GetNode<Sprite2D>("Pet");
        _sprite.ZIndex = 1;

        Tween tween = GetTree().CreateTween();

        tween.
            TweenProperty(_sprite, "position", new Vector2(_sprite.Position.X + positionSign * 100f, _sprite.Position.Y), 0.5f)
            .SetTrans(Tween.TransitionType.Linear)
            .SetEase(Tween.EaseType.InOut);

        await ToSignal(tween, "finished");
    }

    public async Task AnimateAttackFaint()
    {
        Tween tween = GetTree().CreateTween();

        // Tween progress callback to move the object in an arc
        tween.TweenMethod(
            new Callable(this, nameof(UpdateArc)),
            0f, // Start value of t
            1f, // End value of t
            .3f  // Duration in seconds
        ).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);

        await ToSignal(tween, "finished");

    }

    // This method calculates the position of the arc for a given t
    private void UpdateArc(float t)
    {
        int positionSign = Math.Sign(slot.Position.X - (int)ProjectSettings.GetSetting("display/window/size/viewport_width")/2);

        Sprite2D _sprite = slot.GetNode<Sprite2D>("Pet");

        Node2D targetNode = _sprite;
        Vector2 start = _sprite.Position;
        Vector2 end = new Vector2(700 * positionSign, 200);
        int arcHeight = 150;

        // Calculate current position
        float x = Mathf.Lerp(start.X, end.X, t); // Linear interpolation for X
        float y = Mathf.Lerp(start.Y, end.Y, t) - arcHeight * Mathf.Sin(Mathf.Pi * t); // Adjust Y for the arc

        targetNode.Position = new Vector2(x, y); // Update position
    }
}
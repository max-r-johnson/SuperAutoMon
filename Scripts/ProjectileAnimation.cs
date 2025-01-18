using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class ProjectileAnimation : Node2D
{
    private Node2D slot;
    int _arcHeight;
    float _duration;
    private Vector2 _targetPosition;
    private Vector2 _startPosition;
    private Vector2 _midPoint;
    private Game game {get {return MainNode.game;}}

    public override void _Ready()
    {
        slot = (Node2D)GetParent();
        slot.Scale = new Vector2(1.1f,1.1f);
    }

    public async Task FireProjectile(Node target)
    {
        slot.ZIndex = 1;

        Tween tween = GetTree().CreateTween();
        _targetPosition = target.GetNode<Sprite2D>("Pet").GlobalPosition - new Vector2(0,20);
        _startPosition = slot.Position - new Vector2(0,20);
        if(_startPosition == _targetPosition)
        {
            _arcHeight = 100;
            _duration = .2f;
        }
        else
        {
            _arcHeight = 175;
            _duration = .5f;
        }
        _midPoint = new Vector2((_startPosition.X + _targetPosition.X)/2, _startPosition.Y - _arcHeight);

        tween.TweenMethod(
            new Callable(this, nameof(UpdateFireArc)),
            0f, // Start value of t
            1f, // End value of t
            _duration  // Duration in seconds
        ).SetTrans(Tween.TransitionType.Cubic);
        await ToSignal(tween, "finished");
        slot.QueueFree();
    }

    public async Task Buff()
    {
        slot.ZIndex = 1;

        Tween tween = GetTree().CreateTween();
        _duration = .2f;

        tween.
            TweenProperty(slot, "position", new Vector2(slot.Position.X, slot.Position.Y - 40), _duration)
            .SetTrans(Tween.TransitionType.Linear);
        await ToSignal(tween, "finished");
        await Task.Delay(500);
        slot.QueueFree();
    }

    private void UpdateFireArc(float t)
    {
        Node2D targetNode = slot;

        // 3 point bezier curve
        Vector2 pos = (float)Math.Pow(1 - t,2) * _startPosition + 2 * (1-t) * t * _midPoint + (float)Math.Pow(t,2) * _targetPosition;
        targetNode.Position = pos;
    }
}

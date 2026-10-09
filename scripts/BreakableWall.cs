using Godot;
using System;

public partial class BreakableWall : StaticBody2D
{
	[Export] public int Health = 1;
	[Export] public Node2D Visuals;
	[Export] public CollisionShape2D Collision;
	[Export] public CpuParticles2D CrumbleParticles;

	private bool _isDestroyed = false;

	public override void _Ready()
	{
		if (Collision == null)
		{
			Collision = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");
		}
		if (Visuals == null)
		{
			Visuals = GetNodeOrNull<Node2D>("Visuals");
		}
		if (CrumbleParticles == null)
		{
			CrumbleParticles = GetNodeOrNull<CpuParticles2D>("CrumbleParticles");
		}
	}

	public void TakeHit(int damage = 1)
	{
		if (_isDestroyed) return;

		Health -= damage;
		if (Health <= 0)
		{
			DestroyWall();
		}
		else
		{
			// Flash effect on hit
			if (Visuals != null)
			{
				var tween = CreateTween();
				tween.TweenProperty(Visuals, "modulate", new Color(2f, 1.5f, 1.5f), 0.05);
				tween.TweenProperty(Visuals, "modulate", Colors.White, 0.1);
			}
		}
	}

	private void DestroyWall()
	{
		if (_isDestroyed) return;
		_isDestroyed = true;

		// Disable collision immediately so player & bullets pass through
		if (Collision != null)
		{
			Collision.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		}

		if (Visuals != null)
		{
			Visuals.Visible = false;
		}

		if (CrumbleParticles != null)
		{
			CrumbleParticles.Emitting = true;
			var timer = GetTree().CreateTimer(CrumbleParticles.Lifetime + 0.1);
			timer.Timeout += () => QueueFree();
		}
		else
		{
			QueueFree();
		}
	}
}

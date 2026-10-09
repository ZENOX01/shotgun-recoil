using Godot;
using System;

public partial class JumpPad : Area2D
{
	[Export] public float LaunchForce = 440f;
	[Export] public Sprite2D PadSprite;
	[Export] public CpuParticles2D SparkParticles;

	private bool _isLaunching = false;

	public override void _Ready()
	{
		if (PadSprite == null) PadSprite = GetNodeOrNull<Sprite2D>("PadSprite");
		if (SparkParticles == null) SparkParticles = GetNodeOrNull<CpuParticles2D>("SparkParticles");

		BodyEntered += OnBodyEntered;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is Player player)
		{
			// Propel player upwards with high spring force
			player.Bounce(new Vector2(player.Velocity.X, -LaunchForce));
			player.RefillAmmo();

			TriggerLaunchAnimation();
		}
	}

	private void TriggerLaunchAnimation()
	{
		if (_isLaunching) return;
		_isLaunching = true;

		if (SparkParticles != null)
		{
			SparkParticles.Emitting = true;
		}

		if (PadSprite != null)
		{
			// Switch to charged/compressed frame (frame 1: region x: 32..64)
			PadSprite.RegionRect = new Rect2(32, 0, 32, 20);

			var tween = CreateTween();
			// First squish down
			tween.TweenProperty(PadSprite, "scale", new Vector2(1.25f, 0.65f), 0.05);
			// Then pop up high
			tween.TweenProperty(PadSprite, "scale", new Vector2(0.85f, 1.35f), 0.12)
				.SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
			// Settle back to normal idle
			tween.TweenProperty(PadSprite, "scale", Vector2.One, 0.12);
			tween.TweenCallback(Callable.From(() =>
			{
				PadSprite.RegionRect = new Rect2(0, 0, 32, 20);
				_isLaunching = false;
			}));
		}
		else
		{
			_isLaunching = false;
		}
	}
}

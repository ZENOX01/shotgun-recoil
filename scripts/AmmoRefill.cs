using Godot;
using System;

public partial class AmmoRefill : Area2D
{
	[Export] public float RespawnTime = 2.5f;
	[Export] public Sprite2D ShellSprite;
	[Export] public Sprite2D HaloSprite;
	[Export] public CpuParticles2D SparkleParticles;
	[Export] public CollisionShape2D Collision;

	private Vector2 _basePos;
	private float _time = 0f;
	private bool _isCollected = false;

	public override void _Ready()
	{
		_basePos = Position;
		if (ShellSprite == null) ShellSprite = GetNodeOrNull<Sprite2D>("ShellSprite");
		if (HaloSprite == null) HaloSprite = GetNodeOrNull<Sprite2D>("HaloSprite");
		if (SparkleParticles == null) SparkleParticles = GetNodeOrNull<CpuParticles2D>("SparkleParticles");
		if (Collision == null) Collision = GetNodeOrNull<CollisionShape2D>("CollisionShape2D");

		BodyEntered += OnBodyEntered;
	}

	public override void _Process(double delta)
	{
		float dt = (float)delta;
		_time += dt;

		if (!_isCollected)
		{
			// Gentle floating bob
			Position = _basePos + new Vector2(0f, Mathf.Sin(_time * 4f) * 4f);

			// Pulsing glow halo
			if (HaloSprite != null)
			{
				float pulse = (Mathf.Sin(_time * 6f) + 1f) * 0.5f;
				HaloSprite.Modulate = new Color(1f, 0.9f, 0.3f, 0.4f + pulse * 0.4f);
				HaloSprite.Scale = Vector2.One * (1.1f + pulse * 0.25f);
			}
		}
	}

	private void OnBodyEntered(Node2D body)
	{
		if (_isCollected) return;

		if (body is Player player)
		{
			// Only trigger if player doesn't have max ammo or wants guaranteed refresh
			player.RefillAmmo();
			Collect();
		}
	}

	private void Collect()
	{
		_isCollected = true;
		SetDeferred(Area2D.PropertyName.Monitoring, false);

		if (Collision != null)
		{
			Collision.SetDeferred(CollisionShape2D.PropertyName.Disabled, true);
		}

		if (SparkleParticles != null)
		{
			SparkleParticles.Emitting = true;
		}

		// Pop and fade out
		if (ShellSprite != null)
		{
			var tween = CreateTween();
			tween.SetParallel(true);
			tween.TweenProperty(ShellSprite, "scale", ShellSprite.Scale * 1.5f, 0.15);
			tween.TweenProperty(ShellSprite, "modulate:a", 0f, 0.15);
			if (HaloSprite != null)
			{
				tween.TweenProperty(HaloSprite, "modulate:a", 0f, 0.15);
			}
		}

		// Respawn timer
		var timer = GetTree().CreateTimer(RespawnTime);
		timer.Timeout += Respawn;
	}

	private void Respawn()
	{
		_isCollected = false;
		SetDeferred(Area2D.PropertyName.Monitoring, true);

		if (Collision != null)
		{
			Collision.SetDeferred(CollisionShape2D.PropertyName.Disabled, false);
		}

		if (ShellSprite != null)
		{
			ShellSprite.Scale = Vector2.One;
			ShellSprite.Modulate = new Color(1f, 1f, 1f, 0f);

			var tween = CreateTween();
			tween.SetParallel(true);
			tween.TweenProperty(ShellSprite, "modulate:a", 1f, 0.3);
			if (HaloSprite != null)
			{
				tween.TweenProperty(HaloSprite, "modulate:a", 0.6f, 0.3);
			}
		}
	}
}

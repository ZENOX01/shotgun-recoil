using Godot;
using System;

public partial class MenuBackground : Control
{
	[Export] public Node2D PlayerCharacter;
	[Export] public Node2D ShotgunPivot;
	[Export] public Sprite2D ShotgunSprite;
	[Export] public AnimatedSprite2D PlayerSprite;
	[Export] public CpuParticles2D MuzzleParticles;
	[Export] public Sprite2D MuzzleFlash;

	private Vector2 _playerBasePos = Vector2.Zero;
	private Vector2 _recoilOffset = Vector2.Zero;
	private float _time = 0f;

	public override void _Ready()
	{
		if (PlayerCharacter != null)
		{
			_playerBasePos = PlayerCharacter.Position;
		}

		if (MuzzleFlash != null)
		{
			MuzzleFlash.Visible = false;
		}

		if (ShotgunPivot != null)
		{
			// Default stance: aiming towards the left where the title/buttons are
			ShotgunPivot.Rotation = Mathf.Pi;
			UpdateShotgunFlip(Mathf.Pi);
		}

		SetProcess(true);
	}

	public override void _Process(double delta)
	{
		float dt = (float)delta;
		_time += dt;

		// Recover from shotgun blast recoil smoothly
		_recoilOffset = _recoilOffset.MoveToward(Vector2.Zero, dt * 80f);

		// Subtle idle breathing bob
		if (PlayerCharacter != null)
		{
			PlayerCharacter.Position = _playerBasePos + _recoilOffset + new Vector2(0f, Mathf.Sin(_time * 2.5f) * 2f);
		}

		// Aim shotgun towards mouse cursor
		if (ShotgunPivot != null)
		{
			Vector2 mousePos = GetViewport().GetMousePosition();
			if (mousePos.LengthSquared() > 10f)
			{
				Vector2 dir = (mousePos - ShotgunPivot.GlobalPosition).Normalized();
				float targetAngle = dir.Angle();
				ShotgunPivot.Rotation = Mathf.LerpAngle(ShotgunPivot.Rotation, targetAngle, dt * 7f);
				UpdateShotgunFlip(ShotgunPivot.Rotation);
			}
		}
	}

	public override void _GuiInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseBtn && mouseBtn.Pressed && mouseBtn.ButtonIndex == MouseButton.Left)
		{
			FireMenuBlast();
		}
	}

	private void FireMenuBlast()
	{
		if (ShotgunPivot == null) return;

		Vector2 shootDir = Vector2.Right.Rotated(ShotgunPivot.Rotation);

		// Apply tactile recoil backwards!
		_recoilOffset = -shootDir * 14f;

		// Muzzle particles
		if (MuzzleParticles != null)
		{
			MuzzleParticles.Rotation = ShotgunPivot.Rotation;
			MuzzleParticles.Emitting = true;
		}

		// Brief muzzle flash
		if (MuzzleFlash != null)
		{
			MuzzleFlash.Visible = true;
			MuzzleFlash.Modulate = Colors.White;
			var tween = CreateTween();
			tween.TweenProperty(MuzzleFlash, "modulate:a", 0f, 0.08);
			tween.TweenCallback(Callable.From(() => MuzzleFlash.Visible = false));
		}
	}

	private void UpdateShotgunFlip(float rotation)
	{
		bool isAimingLeft = Mathf.Abs(rotation) > Mathf.Pi * 0.5f;

		if (ShotgunSprite != null)
		{
			ShotgunSprite.FlipV = isAimingLeft;
		}

		if (PlayerSprite != null)
		{
			// FlipH = false is facing left, true is facing right
			PlayerSprite.FlipH = !isAimingLeft;
		}
	}
}

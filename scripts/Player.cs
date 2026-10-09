using Godot;
using System;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;

public partial class Player : CharacterBody2D
{
	private const float Speed = 10.0f;
	private const float recoil = 150f;

	private const float recoilDelay = 10f;

	[Export] public int maxBullets = 2;
	[Export] public int currentShot = 2;

	[Export] public AnimatedSprite2D playerAni;

	[Signal] public delegate void ShootBulletEventHandler(Vector2 pos ,  Vector2 direction);
	[Signal] public delegate void AmmoChangedEventHandler(int currentAmmo, int maxAmmo);

	public int CurrentAmmo => currentShot;
	public int MaxAmmo => maxBullets;

	public override void _Ready()
	{
		EmitSignal(SignalName.AmmoChanged, currentShot, maxBullets);
	}

	private Vector2 recoilVelocity = Vector2.Zero;
	[Export] public Node2D Shotgun;

	Vector2 offset;

	[Export] public float DeathY = 700f;

	public void RefillAmmo(int amount = -1)
	{
		if (amount < 0)
		{
			currentShot = maxBullets;
		}
		else
		{
			currentShot = Mathf.Min(maxBullets, currentShot + amount);
		}
		EmitSignal(SignalName.AmmoChanged, currentShot, maxBullets);
	}

	public void Bounce(Vector2 bounceVelocity)
	{
		Velocity = bounceVelocity;
		recoilVelocity = Vector2.Zero;
		if (playerAni != null)
		{
			playerAni.Play("jump");
		}
	}

	private bool _wasOnFloor = true;

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

		// Add gravity
		if (!IsOnFloor())
		{
			if (velocity.Y > 0)
			{
				velocity += GetGravity() * 0.6f * (float)delta;
			}
			else
			{
				velocity += GetGravity() * (float)delta;
			}

			if (playerAni != null && playerAni.Animation != "jump")
			{
				playerAni.Play("jump");
			}

			// Dynamic airborne tilt in flight
			if (playerAni != null)
			{
				float targetRot = Mathf.Clamp(velocity.X * 0.0008f, -0.2f, 0.2f);
				playerAni.Rotation = Mathf.Lerp(playerAni.Rotation, targetRot, (float)delta * 10f);
			}
		}
		else
		{
			if (!_wasOnFloor)
			{
				PlayLandingSquash();
			}

			if (currentShot < maxBullets)
			{
				currentShot = maxBullets;
				EmitSignal(SignalName.AmmoChanged, currentShot, maxBullets);
			}

			if (playerAni != null && playerAni.Animation != "idle")
			{
				playerAni.Play("idle");
			}

			if (playerAni != null)
			{
				playerAni.Rotation = Mathf.Lerp(playerAni.Rotation, 0f, (float)delta * 14f);
			}

			velocity = Vector2.Zero;
		}

		_wasOnFloor = IsOnFloor();

		// Face and aim towards mouse cursor
		Vector2 mousePos = GetGlobalMousePosition();
		bool aimLeft = mousePos.X < GlobalPosition.X;
		if (playerAni != null)
		{
			// Base sprite faces left: FlipH = false is left, true is right
			playerAni.FlipH = !aimLeft;
		}

		if (Shotgun != null)
		{
			Shotgun.LookAt(mousePos);
			float angle = Shotgun.Rotation;
			bool gunAimLeft = Mathf.Abs(angle) > Mathf.Pi * 0.5f;
			Shotgun.Scale = new Vector2(4.51f, gunAimLeft ? -4.51f : 4.51f);
		}

		Shoot();

		recoilVelocity = recoilVelocity.MoveToward(Vector2.Zero, (float)(recoil * recoilDelay * delta));

		velocity += recoilVelocity;
		Velocity = velocity;
		MoveAndSlide();

		if (!_isDead && GlobalPosition.Y > DeathY)
		{
			Die();
		}
	}

	private bool _isDead = false;

	public bool IsDead => _isDead;

	public void Die()
	{
		if (_isDead) return;
		_isDead = true;

		SetPhysicsProcess(false);
		SetProcess(false);
		Velocity = Vector2.Zero;

		CallDeferred(MethodName.ReloadSceneSafely);
	}

	private void ReloadSceneSafely()
	{
		var tree = GetTree();
		if (tree == null) return;

		if (tree.CurrentScene != null && !string.IsNullOrEmpty(tree.CurrentScene.SceneFilePath))
		{
			tree.ReloadCurrentScene();
		}
		else
		{
			tree.ChangeSceneToFile("res://scenes/level_1.tscn");
		}
	}

	public void Shoot()
	{
		if (Input.IsActionJustPressed("Shoot") && currentShot > 0)
		{
			Vector2 lookDirection = (GetGlobalMousePosition() - GlobalPosition).Normalized();
			ApplyRecoil(lookDirection * recoil);
			currentShot--;
			EmitSignal(SignalName.AmmoChanged, currentShot, maxBullets);

			PlayRecoilSquash();

			Vector2 shootDirection = Vector2.Right.Rotated(Rotation).Normalized();
			EmitSignal(SignalName.ShootBullet, GlobalPosition, lookDirection);
		}
	}

	private void PlayRecoilSquash()
	{
		if (playerAni == null) return;
		var tween = CreateTween();
		playerAni.Scale = new Vector2(0.62f, 0.42f);
		tween.TweenProperty(playerAni, "scale", new Vector2(0.505f, 0.505f), 0.15f)
			.SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
	}

	private void PlayLandingSquash()
	{
		if (playerAni == null) return;
		var tween = CreateTween();
		playerAni.Scale = new Vector2(0.58f, 0.44f);
		tween.TweenProperty(playerAni, "scale", new Vector2(0.505f, 0.505f), 0.12f)
			.SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
	}

	public void ApplyRecoil(Vector2 force)
	{
		recoilVelocity = force;
	}
}

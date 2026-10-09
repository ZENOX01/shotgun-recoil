using Godot;
using System;

public partial class Bullets : Area2D
{
	[Export] public float Speed = 600f;
	[Export] public Sprite2D BulletSprite;
	[Export] public AnimatedSprite2D ImpactSprite;
	[Export] public CpuParticles2D SparkParticles;
	[Export] public CollisionShape2D CollisionShape;

	private Vector2 _direction = Vector2.Right;
	public Vector2 Direction
	{
		get => _direction;
		set
		{
			_direction = value;
			if (_direction != Vector2.Zero && !_isDestroyed)
			{
				Rotation = _direction.Angle();
			}
		}
	}

	public Rid ShooterRid;

	private bool _isDestroyed = false;
	private float _lifetime = 0f;
	private const float MaxLifetime = 3.0f;

	public override void _Ready()
	{
		if (_direction != Vector2.Zero)
		{
			Rotation = _direction.Angle();
		}

		BodyEntered += OnBodyEntered;
		AreaEntered += OnAreaEntered;

		if (ImpactSprite != null)
		{
			ImpactSprite.Visible = false;
			ImpactSprite.AnimationFinished += OnImpactAnimationFinished;
		}

		if (SparkParticles != null)
		{
			SparkParticles.Emitting = false;
			SparkParticles.OneShot = true;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (_isDestroyed || !IsInsideTree()) return;

		float dt = (float)delta;
		_lifetime += dt;
		if (_lifetime >= MaxLifetime)
		{
			QueueFree();
			return;
		}

		Vector2 moveStep = Direction * Speed * dt;
		Vector2 nextPos = GlobalPosition + moveStep;

		// High-speed Continuous Collision Detection (Raycast query)
		var world = GetWorld2D();
		if (world != null && world.DirectSpaceState != null)
		{
			var spaceState = world.DirectSpaceState;
			var query = PhysicsRayQueryParameters2D.Create(GlobalPosition, nextPos, CollisionMask);
			
			var excludeArray = new Godot.Collections.Array<Rid> { GetRid() };
			if (ShooterRid.IsValid)
			{
				excludeArray.Add(ShooterRid);
			}
			query.Exclude = excludeArray;

			var hit = spaceState.IntersectRay(query);
			if (hit.Count > 0)
			{
				var collider = hit["collider"].AsGodotObject();
				if (collider is not Player && collider is not CharacterBody2D)
				{
					Vector2 hitPos = (Vector2)hit["position"];
					Vector2 normal = (Vector2)hit["normal"];
					if (collider is BreakableWall wall)
					{
						wall.TakeHit();
					}
					Explode(hitPos, normal);
					return;
				}
			}
		}

		GlobalPosition = nextPos;
	}

	private void OnBodyEntered(Node2D body)
	{
		if (_isDestroyed) return;
		if (body is Player || body is CharacterBody2D) return; // Never explode on the shooter itself

		if (body is BreakableWall wall)
		{
			wall.TakeHit();
		}

		Vector2 normal = -Direction;
		Explode(GlobalPosition, normal);
	}

	private void OnAreaEntered(Area2D area)
	{
		if (_isDestroyed) return;
		if (area == this) return;

		if (area.GetParent() is BreakableWall wall)
		{
			wall.TakeHit();
		}

		// If it hits spikes or other solid areas in the level
		if (area is Spikes)
		{
			Explode(GlobalPosition, -Direction);
		}
	}

	public void Explode(Vector2 hitPos, Vector2 normal)
	{
		if (_isDestroyed) return;
		_isDestroyed = true;

		GlobalPosition = hitPos;

		// Disable collision detection immediately
		SetDeferred("monitoring", false);
		SetDeferred("monitorable", false);
		if (CollisionShape != null)
		{
			CollisionShape.SetDeferred("disabled", true);
		}

		// Hide the flying bullet sprite
		if (BulletSprite != null)
		{
			BulletSprite.Visible = false;
		}

		// Emit glowing sparks bursting outward along surface normal
		if (SparkParticles != null)
		{
			SparkParticles.Direction = normal;
			SparkParticles.Rotation = 0;
			SparkParticles.Emitting = true;
		}

		// Play pixel-art impact destruction animation
		if (ImpactSprite != null)
		{
			ImpactSprite.Visible = true;
			ImpactSprite.Rotation = normal.Angle();
			ImpactSprite.Play("impact");
		}
		else
		{
			// Fallback cleanup if impact sprite is missing
			var tween = CreateTween();
			tween.TweenInterval(0.3);
			tween.TweenCallback(Callable.From(QueueFree));
		}
	}

	private void OnImpactAnimationFinished()
	{
		QueueFree();
	}
}

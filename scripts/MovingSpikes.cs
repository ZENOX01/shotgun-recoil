using Godot;
using System;

public partial class MovingSpikes : Node2D
{
	[Export] public Vector2 MoveOffset = new Vector2(64f, 0f);
	[Export] public float Speed = 50f;
	[Export] public float PauseDuration = 0.3f;

	private Vector2 _startPos;
	private Vector2 _targetPos;
	private float _progress = 0f;
	private bool _movingForward = true;
	private float _pauseTimer = 0f;

	public override void _Ready()
	{
		_startPos = Position;
		_targetPos = _startPos + MoveOffset;

		var area = GetNodeOrNull<Area2D>("Area2D");
		if (area != null)
		{
			area.BodyEntered += OnBodyEntered;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		float dt = (float)delta;

		if (_pauseTimer > 0f)
		{
			_pauseTimer -= dt;
			return;
		}

		float totalDist = _startPos.DistanceTo(_targetPos);
		if (totalDist <= 0.1f) return;

		float step = (Speed / totalDist) * dt;

		if (_movingForward)
		{
			_progress += step;
			if (_progress >= 1f)
			{
				_progress = 1f;
				_movingForward = false;
				_pauseTimer = PauseDuration;
			}
		}
		else
		{
			_progress -= step;
			if (_progress <= 0f)
			{
				_progress = 0f;
				_movingForward = true;
				_pauseTimer = PauseDuration;
			}
		}

		// Smooth ease-in-out cosine interpolation
		float t = (1f - Mathf.Cos(_progress * Mathf.Pi)) * 0.5f;
		Position = _startPos.Lerp(_targetPos, t);
	}

	private void OnBodyEntered(Node2D body)
	{
		if (body is Player player)
		{
			player.Die();
		}
	}
}

using Godot;
using System;

public partial class Main : Node2D
{
	public PackedScene BulletScene = GD.Load<PackedScene>("res://scenes/Bullets.tscn");
	[Export] public Node2D Bullets;

	private Player _player;

	public override void _Ready()
	{
		if (Bullets == null)
		{
			Bullets = GetNodeOrNull<Node2D>("Bullets");
		}

		_player = GetNodeOrNull<Player>("Player");
		if (_player != null)
		{
			var methodCallable = new Callable(this, MethodName._on_player_shoot_bullet);
			if (!_player.IsConnected(Player.SignalName.ShootBullet, methodCallable))
			{
				_player.Connect(Player.SignalName.ShootBullet, methodCallable);
			}
		}
	}

	public void _on_player_shoot_bullet(Vector2 pos, Vector2 direction)
	{
		if (BulletScene == null) return;

		var bullet = BulletScene.Instantiate() as Bullets;
		if (bullet == null) return;

		if (_player != null)
		{
			bullet.ShooterRid = _player.GetRid();
		}

		Vector2 fireDir = -direction;
		bullet.Direction = fireDir;
		// Spawn bullet outside player body in firing direction so it never clips the player
		bullet.GlobalPosition = pos + fireDir * 18f;

		if (Bullets != null)
		{
			Bullets.AddChild(bullet);
		}
		else
		{
			AddChild(bullet);
		}
	}
}

using Godot;
using System;

public partial class AmmoHUD : Control
{
	[Export] public TextureRect Shell1;
	[Export] public TextureRect Shell2;
	[Export] public Label AmmoLabel;
	[Export] public Control ShellsContainer;

	private Texture2D _texFull;
	private Texture2D _texEmpty;
	private Player _player;
	private int _lastAmmo = 2;

	public override void _Ready()
	{
		_texFull = GD.Load<Texture2D>("res://Assets/hud_shell_full.png");
		_texEmpty = GD.Load<Texture2D>("res://Assets/hud_shell_empty.png");

		// Pivot at center of each shell for nice scaling animations
		if (Shell1 != null) Shell1.PivotOffset = Shell1.Size / 2f;
		if (Shell2 != null) Shell2.PivotOffset = Shell2.Size / 2f;

		CallDeferred(MethodName.ConnectToPlayer);
	}

	private void ConnectToPlayer()
	{
		if (_player != null && IsInstanceValid(_player)) return;

		// 1. Look in parent level node or owner
		_player = GetParent()?.FindChild("Player", true, false) as Player;
		if (_player == null)
		{
			_player = GetOwner()?.FindChild("Player", true, false) as Player;
		}

		// 2. Look anywhere under SceneTree.Root
		if (_player == null && GetTree()?.Root != null)
		{
			_player = GetTree().Root.FindChild("Player", true, false) as Player;
		}

		if (_player != null)
		{
			_player.AmmoChanged += OnAmmoChanged;
			UpdateDisplay(_player.CurrentAmmo, false);
		}
	}

	public override void _Process(double delta)
	{
		// Keep in sync in case of scene reload or deferred hook
		if (_player != null && IsInstanceValid(_player))
		{
			if (_player.CurrentAmmo != _lastAmmo)
			{
				UpdateDisplay(_player.CurrentAmmo, true);
			}
		}
		else
		{
			ConnectToPlayer();
		}
	}

	private void OnAmmoChanged(int currentAmmo, int maxAmmo)
	{
		UpdateDisplay(currentAmmo, true);
	}

	public void UpdateDisplay(int ammo, bool animate)
	{
		bool reloaded = ammo > _lastAmmo;
		_lastAmmo = ammo;

		UpdateShell(Shell1, ammo >= 1, reloaded, animate);
		UpdateShell(Shell2, ammo >= 2, reloaded, animate);

		// If out of ammo, pulse amber warning
		if (AmmoLabel != null)
		{
			if (ammo == 0)
			{
				AmmoLabel.Modulate = new Color("EF4444"); // Red: empty
				AmmoLabel.Text = "RELOAD!";
			}
			else
			{
				AmmoLabel.Modulate = new Color("FBBF24"); // Radiant gold
				AmmoLabel.Text = $"{ammo}/2";
			}
		}
	}

	private void UpdateShell(TextureRect shell, bool isFull, bool wasReloaded, bool animate)
	{
		if (shell == null) return;

		shell.Texture = isFull ? _texFull : _texEmpty;

		if (animate)
		{
			if (isFull && wasReloaded)
			{
				// Snappy reload pop & flash
				var tween = CreateTween();
				shell.Scale = new Vector2(1.25f, 1.25f);
				shell.Modulate = new Color(1.6f, 1.6f, 1.3f, 1.0f); // Shiny reload flash
				tween.SetParallel(true);
				tween.TweenProperty(shell, "scale", Vector2.One, 0.2)
					.SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
				tween.TweenProperty(shell, "modulate", Colors.White, 0.2);
			}
			else if (!isFull)
			{
				// Recoil spent drop animation
				var tween = CreateTween();
				shell.Scale = new Vector2(0.8f, 0.8f);
				shell.Modulate = new Color(0.7f, 0.7f, 0.8f, 0.5f);
				tween.TweenProperty(shell, "scale", Vector2.One, 0.15)
					.SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
			}
		}
		else
		{
			shell.Scale = Vector2.One;
			shell.Modulate = isFull ? Colors.White : new Color(0.7f, 0.7f, 0.8f, 0.5f);
		}
	}
}

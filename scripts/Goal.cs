using Godot;
using System;

public partial class Goal : Area2D
{
	[Export] public string NextLevelPath = "";
	[Export] public string LevelTitle = "★ LEVEL CLEARED! ★";

	private bool _reached = false;
	private bool _isTransitioning = false;
	private CanvasLayer _winUi;
	private Button _nextLevelButton;
	private Button _playAgainButton;
	private Button _menuButton;
	private Label _titleLabel;
	private Label _subTitleLabel;

	public override void _Ready()
	{
		BodyEntered += _on_body_entered;

		_winUi = GetNodeOrNull<CanvasLayer>("WinUI");
		if (_winUi != null)
		{
			_winUi.Visible = false;
			_nextLevelButton = _winUi.GetNodeOrNull<Button>("Control/CenterBox/VBoxButtons/NextLevelButton");
			_playAgainButton = _winUi.GetNodeOrNull<Button>("Control/CenterBox/VBoxButtons/PlayAgainButton");
			_menuButton = _winUi.GetNodeOrNull<Button>("Control/CenterBox/VBoxButtons/MenuButton");
			_titleLabel = _winUi.GetNodeOrNull<Label>("Control/CenterBox/Title");
			_subTitleLabel = _winUi.GetNodeOrNull<Label>("Control/CenterBox/SubTitle");

			if (_nextLevelButton != null)
			{
				if (!string.IsNullOrEmpty(NextLevelPath))
				{
					_nextLevelButton.Visible = true;
					_nextLevelButton.Pressed += GoToNextLevel;
				}
				else
				{
					_nextLevelButton.Visible = false;
				}
			}

			if (_playAgainButton != null)
			{
				_playAgainButton.Pressed += RestartLevel;
			}

			if (_menuButton != null)
			{
				_menuButton.Pressed += GoToMainMenu;
			}
		}
	}

	public override void _Process(double delta)
	{
		if (_reached && !_isTransitioning && (Input.IsActionJustPressed("ui_accept") || Input.IsActionJustPressed("Shoot")))
		{
			if (!string.IsNullOrEmpty(NextLevelPath))
			{
				GoToNextLevel();
			}
			else
			{
				RestartLevel();
			}
		}
	}

	public void _on_body_entered(Node body)
	{
		if (!_reached && (body is Player || body is CharacterBody2D))
		{
			_reached = true;
			if (_winUi != null)
			{
				_winUi.Visible = true;

				if (_titleLabel != null)
				{
					_titleLabel.Text = string.IsNullOrEmpty(NextLevelPath) ? "★ ALL LEVELS CLEARED! ★" : LevelTitle;
				}

				if (_subTitleLabel != null)
				{
					_subTitleLabel.Text = string.IsNullOrEmpty(NextLevelPath) 
						? "YOU ARE THE ULTIMATE SHOTGUN MASTER!" 
						: "SHOTGUN RECOIL MASTERED!";
				}

				if (_nextLevelButton != null)
				{
					_nextLevelButton.Visible = !string.IsNullOrEmpty(NextLevelPath);
				}
			}
			GD.Print("LEVEL CLEARED! Next: " + NextLevelPath);
		}
	}

	public void GoToNextLevel()
	{
		if (_isTransitioning) return;
		if (string.IsNullOrEmpty(NextLevelPath))
		{
			RestartLevel();
			return;
		}

		_isTransitioning = true;
		GetTree()?.ChangeSceneToFile(NextLevelPath);
	}

	public void RestartLevel()
	{
		if (_isTransitioning) return;
		_isTransitioning = true;

		var tree = GetTree();
		if (tree == null) return;

		if (tree.CurrentScene != null && !string.IsNullOrEmpty(tree.CurrentScene.SceneFilePath))
		{
			tree.CallDeferred(SceneTree.MethodName.ReloadCurrentScene);
		}
		else
		{
			tree.ChangeSceneToFile("res://scenes/level_1.tscn");
		}
	}

	public void GoToMainMenu()
	{
		if (_isTransitioning) return;
		_isTransitioning = true;
		GetTree()?.ChangeSceneToFile("res://scenes/MainMenu.tscn");
	}
}

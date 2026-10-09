using Godot;
using System;

public partial class MainMenu : Control
{
	[Export] public Button PlayButton;
	[Export] public Button LevelSelectButton;
	[Export] public Button HowToPlayButton;
	[Export] public Button QuitButton;

	[Export] public Control LevelSelectModal;
	[Export] public Button CloseLevelSelectButton;
	[Export] public Button Level1Btn;
	[Export] public Button Level2Btn;
	[Export] public Button Level3Btn;
	[Export] public Button Level4Btn;
	[Export] public Button Level5Btn;

	[Export] public Control HowToPlayModal;
	[Export] public Button CloseModalButton;
	[Export] public Label MadeByLabel;
	[Export] public Control TitleContainer;

	private float _time = 0f;
	private Vector2 _titleBasePos = Vector2.Zero;
	private bool _isTransitioning = false;

	public override void _Ready()
	{
		if (PlayButton != null) PlayButton.Pressed += OnPlayPressed;
		if (LevelSelectButton != null) LevelSelectButton.Pressed += OnLevelSelectPressed;
		if (HowToPlayButton != null) HowToPlayButton.Pressed += OnHowToPlayPressed;
		if (QuitButton != null) QuitButton.Pressed += OnQuitPressed;

		if (CloseModalButton != null) CloseModalButton.Pressed += OnCloseModalPressed;
		if (CloseLevelSelectButton != null) CloseLevelSelectButton.Pressed += OnCloseLevelSelectPressed;

		if (Level1Btn != null) Level1Btn.Pressed += () => LoadLevel("res://scenes/level_1.tscn");
		if (Level2Btn != null) Level2Btn.Pressed += () => LoadLevel("res://scenes/level_2.tscn");
		if (Level3Btn != null) Level3Btn.Pressed += () => LoadLevel("res://scenes/level_3.tscn");
		if (Level4Btn != null) Level4Btn.Pressed += () => LoadLevel("res://scenes/level_4.tscn");
		if (Level5Btn != null) Level5Btn.Pressed += () => LoadLevel("res://scenes/level_5.tscn");

		if (TitleContainer != null) _titleBasePos = TitleContainer.Position;

		if (HowToPlayModal != null) HowToPlayModal.Visible = false;
		if (LevelSelectModal != null) LevelSelectModal.Visible = false;

		ConfigureMadeByLabel();

		// Smooth fade in on start
		Modulate = new Color(1, 1, 1, 0);
		var tween = CreateTween();
		tween.TweenProperty(this, "modulate", Colors.White, 0.35);
	}

	private void ConfigureMadeByLabel()
	{
		if (MadeByLabel == null) return;

		var font = GD.Load<Font>("res://Assets/fonts/PressStart2P.ttf");
		if (font != null)
		{
			MadeByLabel.AddThemeFontOverride("font", font);
			MadeByLabel.AddThemeFontSizeOverride("font_size", 12);
		}

		MadeByLabel.AddThemeColorOverride("font_color", new Color("FBBF24")); // Radiant gold
		MadeByLabel.AddThemeColorOverride("font_shadow_color", new Color(0, 0, 0, 0.85f));
		MadeByLabel.AddThemeConstantOverride("shadow_offset_x", 2);
		MadeByLabel.AddThemeConstantOverride("shadow_offset_y", 2);
		MadeByLabel.Text = "★ MADE BY ZENO ★";
	}

	public override void _Process(double delta)
	{
		_time += (float)delta;

		// Gentle floating title bob
		if (TitleContainer != null)
		{
			TitleContainer.Position = _titleBasePos + new Vector2(
				0,
				Mathf.Sin(_time * 2f) * 4f
			);
		}

		// Subtle golden glow pulse on "Made by Zeno"
		if (MadeByLabel != null)
		{
			float pulse = (Mathf.Sin(_time * 3f) + 1f) * 0.5f;
			Color baseCol = new Color("F59E0B");
			Color brightCol = new Color("FEF08A");
			MadeByLabel.Modulate = baseCol.Lerp(brightCol, pulse);
		}
	}

	private void OnPlayPressed()
	{
		LoadLevel("res://scenes/level_1.tscn");
	}

	private void LoadLevel(string scenePath)
	{
		if (_isTransitioning) return;
		_isTransitioning = true;

		DisableAllButtons();

		var tween = CreateTween();
		tween.TweenProperty(this, "modulate", new Color(0, 0, 0, 1), 0.3);
		tween.TweenCallback(Callable.From(() =>
		{
			GetTree().ChangeSceneToFile(scenePath);
		}));
	}

	private void DisableAllButtons()
	{
		if (PlayButton != null) PlayButton.Disabled = true;
		if (LevelSelectButton != null) LevelSelectButton.Disabled = true;
		if (HowToPlayButton != null) HowToPlayButton.Disabled = true;
		if (QuitButton != null) QuitButton.Disabled = true;
		if (Level1Btn != null) Level1Btn.Disabled = true;
		if (Level2Btn != null) Level2Btn.Disabled = true;
		if (Level3Btn != null) Level3Btn.Disabled = true;
		if (Level4Btn != null) Level4Btn.Disabled = true;
		if (Level5Btn != null) Level5Btn.Disabled = true;
	}

	private void OnLevelSelectPressed()
	{
		OpenModal(LevelSelectModal);
	}

	private void OnCloseLevelSelectPressed()
	{
		CloseModal(LevelSelectModal);
	}

	private void OnHowToPlayPressed()
	{
		OpenModal(HowToPlayModal);
	}

	private void OnCloseModalPressed()
	{
		CloseModal(HowToPlayModal);
	}

	private void OpenModal(Control modal)
	{
		if (modal == null) return;
		modal.Visible = true;
		modal.Modulate = new Color(1, 1, 1, 0);
		modal.Scale = new Vector2(0.92f, 0.92f);
		modal.PivotOffset = modal.Size / 2f;

		var tween = CreateTween();
		tween.SetParallel(true);
		tween.TweenProperty(modal, "modulate", Colors.White, 0.18);
		tween.TweenProperty(modal, "scale", Vector2.One, 0.18)
			.SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
	}

	private void CloseModal(Control modal)
	{
		if (modal == null) return;

		var tween = CreateTween();
		tween.SetParallel(true);
		tween.TweenProperty(modal, "modulate", new Color(1, 1, 1, 0), 0.15);
		tween.TweenProperty(modal, "scale", new Vector2(0.92f, 0.92f), 0.15);
		tween.Chain().TweenCallback(Callable.From(() =>
		{
			modal.Visible = false;
		}));
	}

	private void OnQuitPressed()
	{
		GetTree().Quit();
	}
}

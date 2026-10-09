using Godot;
using System;

public partial class MenuButton : Button
{
	[Export] public string PrefixOnHover = "► ";
	[Export] public bool EnableHoverAnimation = true;

	private string _originalText = "";
	private Tween _hoverTween;
	private Vector2 _defaultScale = Vector2.One;

	public override void _Ready()
	{
		_originalText = Text;
		PivotOffset = Size / 2f;
		_defaultScale = Scale;

		ApplyThemeStyles();

		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
		ButtonDown += OnButtonDown;
		ButtonUp += OnButtonUp;
		Resized += OnResized;
	}

	private void OnResized()
	{
		PivotOffset = Size / 2f;
	}

	private void ApplyThemeStyles()
	{
		// Load custom font
		var font = GD.Load<Font>("res://Assets/fonts/PressStart2P.ttf");
		if (font != null)
		{
			AddThemeFontOverride("font", font);
			AddThemeFontSizeOverride("font_size", 16);
		}

		// Text colors
		AddThemeColorOverride("font_color", new Color("F3F4F6"));
		AddThemeColorOverride("font_hover_color", new Color("FFFFFF"));
		AddThemeColorOverride("font_pressed_color", new Color("FEF08A"));
		AddThemeColorOverride("font_focus_color", new Color("FFFFFF"));

		// Normal style
		var styleNormal = new StyleBoxFlat
		{
			BgColor = new Color(0.11f, 0.07f, 0.17f, 0.92f), // #1c122c
			BorderWidthLeft = 3,
			BorderWidthTop = 3,
			BorderWidthRight = 3,
			BorderWidthBottom = 3,
			BorderColor = new Color("D97706"), // Brass gold
			CornerRadiusTopLeft = 4,
			CornerRadiusTopRight = 4,
			CornerRadiusBottomRight = 4,
			CornerRadiusBottomLeft = 4,
			ContentMarginLeft = 24,
			ContentMarginRight = 24,
			ContentMarginTop = 14,
			ContentMarginBottom = 14,
			ShadowColor = new Color(0, 0, 0, 0.6f),
			ShadowSize = 4,
			ShadowOffset = new Vector2(0, 4)
		};

		// Hover style (glowing border & richer background)
		var styleHover = new StyleBoxFlat
		{
			BgColor = new Color(0.20f, 0.12f, 0.32f, 0.98f), // #331f52
			BorderWidthLeft = 3,
			BorderWidthTop = 3,
			BorderWidthRight = 3,
			BorderWidthBottom = 3,
			BorderColor = new Color("FBBF24"), // Bright radiant gold
			CornerRadiusTopLeft = 4,
			CornerRadiusTopRight = 4,
			CornerRadiusBottomRight = 4,
			CornerRadiusBottomLeft = 4,
			ContentMarginLeft = 24,
			ContentMarginRight = 24,
			ContentMarginTop = 14,
			ContentMarginBottom = 14,
			ShadowColor = new Color("F59E0B").Darkened(0.2f),
			ShadowSize = 6,
			ShadowOffset = new Vector2(0, 4)
		};

		// Pressed style (tactile depressed)
		var stylePressed = new StyleBoxFlat
		{
			BgColor = new Color(0.08f, 0.05f, 0.13f, 0.98f), // #140d21
			BorderWidthLeft = 3,
			BorderWidthTop = 3,
			BorderWidthRight = 3,
			BorderWidthBottom = 3,
			BorderColor = new Color("B45309"), // Deep burnt gold
			CornerRadiusTopLeft = 4,
			CornerRadiusTopRight = 4,
			CornerRadiusBottomRight = 4,
			CornerRadiusBottomLeft = 4,
			ContentMarginLeft = 24,
			ContentMarginRight = 24,
			ContentMarginTop = 16,
			ContentMarginBottom = 12,
			ShadowColor = new Color(0, 0, 0, 0.4f),
			ShadowSize = 2,
			ShadowOffset = new Vector2(0, 2)
		};

		// Focus style
		var styleFocus = new StyleBoxEmpty();

		AddThemeStyleboxOverride("normal", styleNormal);
		AddThemeStyleboxOverride("hover", styleHover);
		AddThemeStyleboxOverride("pressed", stylePressed);
		AddThemeStyleboxOverride("focus", styleFocus);
	}

	private void OnMouseEntered()
	{
		if (!string.IsNullOrEmpty(PrefixOnHover))
		{
			Text = PrefixOnHover + _originalText;
		}

		if (EnableHoverAnimation)
		{
			_hoverTween?.Kill();
			_hoverTween = CreateTween();
			_hoverTween.SetParallel(true);
			_hoverTween.TweenProperty(this, "scale", _defaultScale * 1.04f, 0.15)
				.SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
			_hoverTween.TweenProperty(this, "modulate", new Color("FFFBEB"), 0.15);
		}
	}

	private void OnMouseExited()
	{
		Text = _originalText;

		if (EnableHoverAnimation)
		{
			_hoverTween?.Kill();
			_hoverTween = CreateTween();
			_hoverTween.SetParallel(true);
			_hoverTween.TweenProperty(this, "scale", _defaultScale, 0.15)
				.SetTrans(Tween.TransitionType.Quad).SetEase(Tween.EaseType.Out);
			_hoverTween.TweenProperty(this, "modulate", Colors.White, 0.15);
		}
	}

	private void OnButtonDown()
	{
		_hoverTween?.Kill();
		_hoverTween = CreateTween();
		_hoverTween.TweenProperty(this, "scale", _defaultScale * 0.97f, 0.05);
	}

	private void OnButtonUp()
	{
		_hoverTween?.Kill();
		_hoverTween = CreateTween();
		_hoverTween.TweenProperty(this, "scale", _defaultScale * 1.04f, 0.1)
			.SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
	}
}

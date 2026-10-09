using Godot;
using System;

public partial class Spikes : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		BodyEntered += _on_body_entered;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_body_entered(Node body)
	{
		if (body is Player player)
		{
			player.Die();
		}
		else if (body is CharacterBody2D character)
		{
			if (character.HasMethod("Die"))
			{
				character.Call("Die");
			}
			else
			{
				var tree = GetTree();
				if (tree?.CurrentScene != null && !string.IsNullOrEmpty(tree.CurrentScene.SceneFilePath))
				{
					tree.CallDeferred(SceneTree.MethodName.ReloadCurrentScene);
				}
			}
		}
	}
}

using Godot;
using System;

public partial class EndArea : Area3D
{
	[Export] private Control _gameOverUI;
	private bool _triggered = false;

	public override void _Ready()
	{
		_gameOverUI.Visible = false;
	}

	private void OnBodyEntered(Node3D body)
	{
		if (_triggered) return;
		if (body is Character)
		{
			_triggered = true;
			_gameOverUI.Visible = true;
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}

	private void OnRestartPressed()
	{
		GetTree().ReloadCurrentScene();
	}
}

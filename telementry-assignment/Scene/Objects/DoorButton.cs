using Godot;
using System;

public partial class DoorButton : StaticBody3D
{
	[Export] public AnimationPlayer _door;
	[Export] public CharacterBody3D player;
	[Export] public MeshInstance3D _mesh;
	[Export] public MeshInstance3D button;
	private bool isPressed;
	private Vector2 playerPos;
	private Vector2 buttonPos;
	private float interactDis = 2.5f;
	public override void _Ready()
	{
	}

	

	public override void _Process(double delta)
	{
		reactButton();
	}

	private void reactButton()
	{
		if(isInArea() && !isPressed)
		{
			_mesh.Visible = true;
			if(Input.IsActionJustPressed("interact_button"))
			{
				_mesh.Visible = false;
				isPressed = true;
				button.GlobalPosition = new Vector3(button.GlobalPosition.X,button.GlobalPosition.Y - 0.05f, button.GlobalPosition.Z);
				_door.Play("open");
				TelemetryManager.Instance?.LogEvent(TelemetryManager.DoorButton, GlobalPosition);
			}

		}
		else
			_mesh.Visible = false;
	}

	private bool isInArea()
	{
		playerPos = new Vector2(player.GlobalPosition.X,player.GlobalPosition.Z);
		buttonPos = new Vector2(GlobalPosition.X,GlobalPosition.Z);

		float dis = Math.Abs((playerPos - buttonPos).Length());

		if(dis <= interactDis)
			return true;
		return false; 
	}
}

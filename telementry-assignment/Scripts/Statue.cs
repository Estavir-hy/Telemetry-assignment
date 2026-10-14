using Godot;
using System;


public partial class Statue : AnimatableBody3D
{
	[Export] public Node3D Player;
	[Export] public float InteractRange = 2.5f;
	[Export] public float MoveSpeed = 3.0f;
	[Export] public float RotateSpeed = 90f; //90 degree/sec
	[Export] public MeshInstance3D F;
	[Export] public Label ins;

	private bool isGrabbed;

	private bool isRotate;
	private Vector2 _offset;

	private Vector2 playerPos;
	private Vector2 statuePos;

    public override void _PhysicsProcess(double delta)
    {
		playerPos = new Vector2(Player.GlobalPosition.X,Player.GlobalPosition.Z);
		statuePos = new Vector2(GlobalPosition.X,GlobalPosition.Z);

		if((statuePos - playerPos).Length() <= InteractRange)
		{
			if(isGrabbed)
			{
				F.Visible = false;
				ins.Visible = true;
			}
			else
			{
				F.Visible = true;
				ins.Visible = false;
			}
				
			if(Player !=null && Input.IsActionJustPressed("interact_button"))
			{
				if(isGrabbed)
				{
					isGrabbed = false;
					TelemetryManager.Instance?.LogEvent(TelemetryManager.ReleaseStatue, GlobalPosition);
				}

				else
				{
					isGrabbed = true;
					_offset = statuePos - playerPos;
					TelemetryManager.Instance?.LogEvent(TelemetryManager.GrabStatue, GlobalPosition);
				}
					
			}
		}
		else
        	F.Visible = false;

		if(!isGrabbed)
			return;

		GlobalPosition = new Vector3(playerPos.X + _offset.X, GlobalPosition.Y, playerPos.Y + _offset.Y);

		float rotate = 0f;
		if(Input.IsActionPressed("statue_rotate_right"))
		{
			rotate -= 1f;
			isRotate = true;
		}
		if(Input.IsActionPressed("statue_rotate_left"))
		{
			rotate += 1f;
			isRotate = true;
		}
		if(rotate != 0f)
			RotateObjectLocal(Vector3.Up,Mathf.DegToRad(rotate*RotateSpeed*(float)delta));
		else
			isRotate = false;
    }

	public bool IsGrabbed
	{
		get{return isGrabbed;}
	}

	public bool IsRotate
	{
		get{return isRotate;}
	}
}

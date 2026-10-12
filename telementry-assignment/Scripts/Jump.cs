using Godot;
using System;

public partial class Jump : Area3D
{
	private Character player;
	public bool jumpActive;
	public bool animPlay;
	[Export] public MeshInstance3D _mesh;
	[Export] public AnimationPlayer _anim;
	[Export] public Label enableJump;
	private float insTime = 3f;
	private bool showIns = false;

    public override void _Process(double delta)
    {
        if(showIns)
		{
			insTime -= (float)delta;
			if(insTime <= 0)
			{
				enableJump.Visible = false;
				showIns = false;
			}
		}
    }

	private void OnBodyEntered(Node3D body)
	{
		if(body is Character _player)
		{
			player = _player;
			animPlay = false;
			_anim.Stop();
			jumpActive = true;
			_mesh.Visible = false;

		}
	}

	private void OnBodyExited(Node3D body)
	{
		if (body == player)
			player = null;
		enableJump.Visible = true;
		showIns = true;
	}
}

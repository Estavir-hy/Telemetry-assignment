using Godot;
using System;

public partial class World : Node3D
{
	[Export] public AnimationPlayer _anim;
	[Export] public Label cameraIns;
	private float insTime = 5f;
	public override void _Ready()
	{
		_anim.Play("rotate");
	}

    public override void _Process(double delta)
    {
        insTime -= (float)delta;
		if(insTime <= 0f)
		{
			cameraIns.Visible = false;
		}
    }


	
}

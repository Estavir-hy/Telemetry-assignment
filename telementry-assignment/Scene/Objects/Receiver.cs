using Godot;
using System;

public partial class Receiver : StaticBody3D
{
	private bool _powered;
	private MeshInstance3D _visual;

	[Export] public Mech mech;

	public void SetPowered(bool on)
	{
		if(_powered == on) return;
		_powered = on;
		mech?.SetOpen(on);
	}
}

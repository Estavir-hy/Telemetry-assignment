using Godot;
using System;

public partial class SingleCam : Node3D
{

	public Camera3D _cam;
	[Export] public float offset_v = 15f;
	[Export] public float offset_h = 15f;
	[Export] public float lookingAngle =-45f;
	[Export] public float followDuration =0.3f;

	private bool _isRotating;

	[Export] public Statue _statue;
	public override void _Ready()
	{
		_cam = GetNode<Camera3D>("Camera3D");
		_cam.Rotation = new Vector3(Mathf.DegToRad(lookingAngle),0,0);
		_cam.Position = new Vector3(0,offset_h,offset_v);
	}

	private void CameraMove(float angle)
	{
		_isRotating = true;

		var camTween =CreateTween();

		camTween.TweenProperty( 
			this,
			"rotation:y",
			Rotation.Y + Mathf.DegToRad(angle),
			followDuration
		).SetEase(Tween.EaseType.InOut);

		camTween.Finished += () => _isRotating = false;
	}

	private void HandleInput()
	{
		if(Input.IsActionJustPressed("Camera_RotateR") && !_isRotating)
			CameraMove(45f);
		else if(Input.IsActionJustPressed("Camera_RotateL") && !_isRotating)
			CameraMove(-45f);
	}

	public override void _Process(double delta)
	{
		if(_statue != null && _statue.IsGrabbed)
			return;
		HandleInput();
	}

}

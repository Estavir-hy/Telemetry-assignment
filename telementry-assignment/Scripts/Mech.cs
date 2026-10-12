using Godot;
using System;

public partial class Mech : StaticBody3D
{
	[Export] public string AnimPlayerNode = "slide";
    [Export] public string AnimName = "slide";
	private AnimationPlayer _anim;
	private bool openRetrack;
	private bool _moving;
	private bool _started;
	private const float Speed = 1.0f;
	public override void _Ready()
	{
		_anim = GetNode<AnimationPlayer>(AnimPlayerNode);
        _anim.CurrentAnimation = AnimName;
        _anim.Stop();
        _anim.Seek(0f, true);
	}

	public void SetOpen(bool on)
	{
		if(on == openRetrack) return;
		openRetrack = on;

		_moving = true; 
	}
	public override void _Process(double delta)
	{
		if(!_moving) return;

		float dir = openRetrack ? 1f : -1f;
		float t = (float)(_anim.CurrentAnimationPosition + dir * Speed *delta);
		t = (float)Mathf.Clamp(t, 0.0, _anim.CurrentAnimationLength);
    	_anim.Seek(t, true);

		if (t <= 0.0 || t >= _anim.CurrentAnimationLength)
        _moving = false; 
	}
}

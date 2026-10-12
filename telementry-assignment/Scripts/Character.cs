using Godot;
using System;

public partial class Character : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	private Vector3 direction;
	[Export] public Camera3D camNode; 
	[Export] public Statue _statue;
	[Export] public Jump jump;

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor() && jump.jumpActive)
		{
			velocity.Y = JumpVelocity;
		}


		if(_statue.IsRotate)
			 return;
		

		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_backward");
		Vector3 rawDir = camNode.GlobalBasis * new Vector3(inputDir.X, 0, inputDir.Y);
		rawDir.Y = 0;
		direction = rawDir.Normalized();


		characterRotation(velocity);


		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	private void characterRotation(Vector3 velocity)
	{
		Vector2 characterDir;

		if(new Vector2(velocity.X,velocity.Z).Length() > 0)
		{
			if (direction != Vector3.Zero)
			{
				characterDir = new Vector2(direction.Z, direction.X);
				
			}
			else
			{
				characterDir = new Vector2(velocity.Z, velocity.X);
			}

			float inputAngle = characterDir.Angle();

			float currentAngle = Rotation.Y;
			
			float lerpAngle = Mathf.LerpAngle(currentAngle, inputAngle, 0.1f);

			Rotation = new Vector3(Rotation.X, lerpAngle, Rotation.Z);

		}
	}
}

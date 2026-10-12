using Godot;

public partial class BeamRibbon : Node3D
{
	[Export] public Marker3D Muzzle; 
	[Export] public CollisionObject3D Emitter; 
	[Export] public Vector3 EmitDirection = new Vector3(0, 0, 1);
	[Export] public float MaxLength = 100f;
	[Export] public uint CollisionMask = 1;

	private Node3D _currentReceiver;

	public override void _PhysicsProcess(double delta)
	{
		if (Muzzle == null || Emitter == null) return;

		Vector3 from = Muzzle.GlobalPosition;
		Vector3 dir = Emitter.GlobalTransform.Basis * EmitDirection.Normalized();
		Vector3 to = from + dir * MaxLength;

		var query = PhysicsRayQueryParameters3D.Create(from + dir * 0.01f, to);
		query.CollisionMask = CollisionMask;
		query.Exclude = new Godot.Collections.Array<Rid> { Emitter.GetRid() };

		var hit = GetWorld3D().DirectSpaceState.IntersectRay(query);
	

		GlobalPosition = from;
		Vector3 target = hit.Count > 0 ? hit["position"].AsVector3() : to;
		Vector3 up = Mathf.Abs(dir.Y) > 0.999f ? Vector3.Right : Vector3.Up;
		LookAt(target, up);

		Node3D hitReceiver = null;

		if(hit.Count > 0)
		{
			var collide = hit["collider"].AsGodotObject() as Node3D;
			if(collide != null && collide.IsInGroup("receiver"))
				hitReceiver = collide;
		}

		if(hitReceiver != _currentReceiver)
		{
			if(_currentReceiver != null)
				_currentReceiver.Call("SetPowered",false);
			_currentReceiver = hitReceiver;
			if(hitReceiver != null)
				hitReceiver.Call("SetPowered", true);
		}
	}
}

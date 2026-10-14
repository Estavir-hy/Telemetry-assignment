using Godot;
using System;
using System.Text.Json;

public partial class TelemetryManager : Node
{
	[Export] public float SampleInterval = 2.0f;
	[Export] public float DeadZone = 0.5f;

	private CharacterBody3D player;
	

	private TelemetrySession _session;
	private bool sessionActive;
	private string sessionID;
	private ulong startMsec;            //when game start

	private float samplingTimer;
	private bool wroteAnySample;
	private Vector3 lastSamplePos;

	//--------------------events------------------
	public const string GrabStatue = "grab_statue";
	public const string ReleaseStatue = "release_statue";
	public const string ReceiverPowered = "receiver_powered";
	public const string ReceiverUnpowered = "receiver_unpowered";
	public const string JumpBuff = "jump_buff";
	public const string DoorButton = "door_button";
 	public const string LevelComplete = "level_complete";

	public static TelemetryManager Instance {get; private set;}

    public override void _Ready()
    {
        Instance = this;
    }


    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
            EndSession();
    }

    public override void _Process(double delta)
    {
		if(!sessionActive) return;
		samplingTimer += (float)delta;

		if(samplingTimer >=SampleInterval)
		{
			samplingTimer -= SampleInterval;
			RecordSample();
		}
    }


	//start a game
	public void BeginSession()
	{
		if(sessionActive) EndSession();

		startMsec = Time.GetTicksMsec();
		samplingTimer = 0f;
		wroteAnySample = false;
		sessionActive = true;

		_session = new TelemetrySession
		{
			SessionId = SessionIdNow(),
			SessionStartMs = (long) startMsec,
			SampleInterval = SampleInterval,
			DeadZoneThreshold = DeadZone
		};

		player = FindPlayer();
		if(player != null)
			RecordSample();
		GD.Print($"[Telemetry] session started: {_session.SessionId}");

	}


	//write json into the file folder
	public void EndSession()
	{
		if(!sessionActive) return;
		sessionActive = false;
		WriteFile();
	}


	//samples record
	public void LogEvent(string type, Vector3? pos = null, string detail = "")
	{
		if(!sessionActive) return;

		Vector3 p = pos ?? (player?.GlobalPosition ?? Vector3.Zero);
        _session.Events.Add(new TelemetryEvent
		{
			RelativeTimeMs = NowMs(),
			Type = type,
			X = p.X, Y = p.Y, Z = p.Z,
			Detail = detail,
		});

		GD.Print($"[Telemetry] event {type} @ {NowMs()}ms");
	}


	//inner part

	private void RecordSample()
	{
		if(player == null) return;

		Vector3 pos = player.GlobalPosition;
		

		//DeadZone
		if(wroteAnySample)
		{
			float dis = new Vector2(pos.X,pos.Z).DistanceTo(new Vector2(lastSamplePos.X, lastSamplePos.Z));

			if(dis<DeadZone) return;
		}

		wroteAnySample = true;
		lastSamplePos = pos;

		_session.Samples.Add(new TelemetrySample
        {
            RelativeTimeMs = NowMs(),
            X = pos.X,
            Y = pos.Y,
            Z = pos.Z,
            RotY = player.Rotation.Y
        });
	}

	private void WriteFile()
	{
		if(_session == null) return;

		string json = JsonSerializer.Serialize(_session, new JsonSerializerOptions {WriteIndented = true});

		DirAccess.MakeDirRecursiveAbsolute("user://telemetry");
		string fullPath = ProjectSettings.GlobalizePath($"user://telemetry/session_{_session.SessionId}.json");

		using var file = FileAccess.Open(fullPath, FileAccess.ModeFlags.Write);
		file.StoreString(json);

		GD.Print($"[Telemetry] saved: {fullPath}  " +
                 $"({_session.Samples.Count} samples, {_session.Events.Count} events)");
	}

	private CharacterBody3D FindPlayer()
        => GetTree().Root.FindChild("Character", true, false) as CharacterBody3D;

    private long NowMs()
        => (long)(Time.GetTicksMsec() - startMsec);

    private string SessionIdNow()
        => Time.GetDatetimeStringFromSystem().Replace(":", "-");

	public static string Sec(long ms) => (ms / 1000.0).ToString("0.00") + "s";

	public static string Clock(long ms) => TimeSpan.FromMilliseconds(ms).ToString(@"m\:ss");

}

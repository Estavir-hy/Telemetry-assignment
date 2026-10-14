using Godot;
using System;
using System.Collections.Generic;

public partial class TelemetrySession
{
	 public string SessionId { get; set; }
    public long SessionStartMs { get; set; }
    public float SampleInterval { get; set; }
    public float DeadZoneThreshold { get; set; }
    public List<TelemetrySample> Samples { get; set; } = new();
    public List<TelemetryEvent> Events { get; set; } = new();
}

public class TelemetrySample
{
	public long RelativeTimeMs {get; set;}
	public float X {get; set;}
	public float Y {get; set;}
    public float Z {get; set;}
    public float RotY {get; set;}
}

public class TelemetryEvent
{
    public long RelativeTimeMs { get; set; }
    public string Type { get; set; }
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public string Detail { get; set; }
}

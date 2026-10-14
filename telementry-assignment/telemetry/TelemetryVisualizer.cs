using Godot;
using System;
using System.Text.Json;


[Tool]
public partial class TelemetryVisualizer : EditorScript
{
	 private static readonly Color[] SessionColors =
    {
        new Color(1f, 0.45f, 0.35f),    // red
        new Color(0.4f, 0.7f, 1f),      // blue
        new Color(0.45f, 0.95f, 0.5f),  // green
        new Color(1f, 0.85f, 0.3f),     // yellow
        new Color(0.85f, 0.5f, 1f),     // purple
    };


    public override void _Run()
    {
		//get the 3d root scene
        Node3D sceneRoot = GetEditorInterface().GetEditedSceneRoot() as Node3D;
		if(sceneRoot == null)
		{
			return;
		}

		Node existing = sceneRoot.GetNodeOrNull("TelemetryVisualization");
		if (existing != null)
		{
			existing.QueueFree();
			GD.Print("[Visualizer] cleared");
			return;
		}

		//makesure no over laps
		sceneRoot.GetNodeOrNull("TelemetryVisualization")?.QueueFree();

		Node3D container = new Node3D{Name = "TelemetryVisualization"};
		sceneRoot.AddChild(container);
		container.Owner = sceneRoot; //run when want to save scene file

		//scan telemetry folder json
		string dir = "user://telemetry";
		string[] files = DirAccess.GetFilesAt(dir);
		Array.Sort(files);                     //sort with time

		int colorIndex = 0;
		foreach (string file in files)
		{
			if(!file.StartsWith("session_") || !file.EndsWith(".json"))
				continue;
			
			string text = FileAccess.GetFileAsString($"{dir}/{file}");
			TelemetrySession session = JsonSerializer.Deserialize<TelemetrySession>(text);
			if(session == null) continue;

			Node3D sessionRoot = new Node3D { Name = file.Replace(".json", "") };
			container.AddChild(sessionRoot);
			sessionRoot.Owner = sceneRoot;

			DrawSession(sessionRoot, session, SessionColors[colorIndex % SessionColors.Length]);
			GD.Print($"[Visualizer] {file}: " +
                     $"{session.Samples.Count} samples, {session.Events.Count} events, " +
                     $"completion={TelemetryManager.Clock(session.Events.Count > 0 ? LastCompleteMs(session) : 0)}");
			colorIndex++;
		}
    }


	private void DrawSession(Node3D parent, TelemetrySession session, Color color)
	{
		var mat = new StandardMaterial3D {AlbedoColor = color};

		//small sphere <- samples
		for(int i = 0; i < session.Samples.Count; i++)
		{
			TelemetrySample s = session.Samples[i];
			parent.AddChild(new MeshInstance3D
			{
				Name = $"Sample_{i}",
				Position = new Vector3(s.X, s.Y, s.Z),
				Mesh = new SphereMesh {Radius = 0.15f, Height = 0.3f},
				MaterialOverride = mat
			});
		}

		//big sphere <- events
		for(int i = 0; i <session.Events.Count; i++)
		{
			TelemetryEvent e = session.Events[i];
            Vector3 pos = new Vector3(e.X, e.Y, e.Z);


			parent.AddChild(new MeshInstance3D
            {
                Name = $"event_{e.Type}_{i}",
                Position = pos,
                Mesh = new SphereMesh { Radius = 0.45f, Height = 0.9f },
                MaterialOverride = mat
            });

			parent.AddChild(new Label3D
            {
                Text = $"{e.Type}  {TelemetryManager.Sec(e.RelativeTimeMs)}",
                Position = pos + Vector3.Up * 0.9f,
                Billboard = BaseMaterial3D.BillboardModeEnum.Enabled,
                FontSize = 48,
                PixelSize = 0.02f
            });
		}
	}


	private static long LastCompleteMs(TelemetrySession s)
    {
        foreach (var e in s.Events)
            if (e.Type == TelemetryManager.LevelComplete)
                return e.RelativeTimeMs;
        return 0;
    }

}

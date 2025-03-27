using Godot;
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

public partial class StarNode : Node2D
{
	[Export]
	ParticleProcessMaterial particleAffect;
  
	[Export]
	Gradient indicatorGradient;

	[Export]
   public Godot.Collections.Array<StarNode> adjacentStars;

	[Export]
	float centerFocusRange = 300f;

	[Export]
	float lineSpeed = 0.5f;

	private Sprite2D foundSprite;
	private float timeToRegister = 2f;
	private float timeElapsed = 0f;

	private int indicatorCompleteCount = 0;


	private Godot.Collections.Array<Line2D> indicatorList;

	private Godot.Collections.Array<Line2D> lineList;
	private Godot.Collections.Array<Vector2> lineTargets;
	private Godot.Collections.Array<float> lineProgress;
	private Godot.Collections.Array<GpuParticles2D> particleList;

	private StarsParent parent;
	private Label numberDisplay;

	private bool hasDisplayed = false;
	public bool isFound = false;
	
	AudioStreamPlayer audioStreamPlayer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foundSprite = GetNode("FoundSprite") as Sprite2D;
		numberDisplay = GetChild(2) as Label;
		parent = GetParent() as StarsParent;
		lineList = new Godot.Collections.Array<Line2D>();
		lineTargets = new Godot.Collections.Array<Vector2>();
		lineProgress = new Godot.Collections.Array<float>();
		particleList = new Godot.Collections.Array<GpuParticles2D>();

		indicatorList = new Godot.Collections.Array<Line2D>();
		timeElapsed = 0f;
		
		audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!isFound)
		{
			if (GlobalPosition.DistanceTo(GetViewport().GetCamera2D().GetScreenCenterPosition()) < centerFocusRange)
			{
				timeElapsed += (float)delta;

				if (timeElapsed > timeToRegister)
				{
					isFound = true;
					StarFound();
				}

			}
		}
		else
		{     //shows the number for the star
			if (!hasDisplayed)
			{
				Tween transition = GetTree().CreateTween();
				transition.Parallel().TweenProperty(GetNode("Sprite"), "modulate", Colors.Transparent, 1f);
				transition.Parallel().TweenProperty(foundSprite, "modulate", new Color(foundSprite.Modulate.R, foundSprite.Modulate.G, foundSprite.Modulate.B, 1f), 1f);
				transition.Parallel().TweenProperty(numberDisplay, "modulate", new Color(numberDisplay.Modulate.R, numberDisplay.Modulate.G, numberDisplay.Modulate.B, 1f), 1f);
				hasDisplayed = true;
				
				audioStreamPlayer.Play();
			}
		}


		//Interp the connected lines 
		if (lineList.Count > 0)
		{
			for (int i = 0; i < lineList.Count; i++)
			{
				if (lineProgress[i] < 1f)
				{
					lineProgress[i] += (float)delta * lineSpeed;

					lineProgress[i] = Mathf.Clamp(lineProgress[i], 0f, 1f);

					Vector2 linePos = lineList[i].Points[0].Lerp(lineTargets[i], lineProgress[i]);

					lineList[i].SetPointPosition(1, linePos);
					particleList[i].GlobalPosition = linePos;
					GD.Print("Particles emitting from line ", i);
				}
				else
				{
					particleList[i].Emitting = false;
				}
			}
		}
	}

	public void ConnectStars(Node2D star)
	{
   
		Line2D connection = new Line2D();

		connection.AddPoint(GlobalPosition);
		connection.AddPoint(GlobalPosition);
 
		connection.Width = 10;
		connection.DefaultColor = new Color(1, 1, 1, 0.2f);

		this.AddSibling(connection);

		particleList.Add(SetUpParticles(star,connection));

		lineList.Add(connection);
		lineProgress.Add(0f);
		lineTargets.Add(star.GlobalPosition);
	}

	public void FreeIndicator(Node2D star)
	{
		GD.Print("passed:indicator");
		float bestProd = 0;
		int index = 0;


		Vector2 starDir = (GlobalPosition - star.GlobalPosition).Normalized();
	 
		for (int i = 0; i < indicatorList.Count; i++)
		{
			Vector2 indDir = (indicatorList[i].GetPointPosition(0) - indicatorList[i].GetPointPosition(1)).Normalized();
			float dotProd = indDir.Dot(starDir);
			GD.Print("Dot:" + dotProd);
			if (dotProd > bestProd)
			{
				index = i;
				bestProd = dotProd;
			}
		}
		GD.Print("Index:" + index);
		if (indicatorList[index] != null)
		{
			//Add a fade out here instead of just deleting 
			fadeInOrOut(indicatorList[index], false, true);
			indicatorList.RemoveAt(index);
		}
	}

	private void StarFound()
	{
		foreach (StarNode star in adjacentStars)
		{
			if (star.isFound)
			{
				GD.Print("passed:found");
				ConnectStars(star);
				star.FreeIndicator(this);
			}
			else
			{
				StarIndicators(star);
			}
		}
	}

	private void StarIndicators(Node2D star)
	{
		Line2D indicator = new Line2D();
		Vector2 starDir = GlobalPosition.DirectionTo(star.GlobalPosition);

		indicator.AddPoint(GlobalPosition + (starDir * 150f));
		indicator.AddPoint(GlobalPosition + (starDir * 190f));

		indicator.Modulate = new Color(indicator.Modulate.R, indicator.Modulate.G, indicator.Modulate.B, 0);
		indicator.Gradient = indicatorGradient;


		this.AddSibling(indicator);

		fadeInOrOut(indicator,true);

		indicatorList.Add(indicator);
	}

   private GpuParticles2D SetUpParticles(Node2D target,Line2D connection)
	{
		GpuParticles2D trail = new GpuParticles2D();
		particleAffect.Direction = new Vector3(GlobalPosition.DirectionTo(target.GlobalPosition).X, GlobalPosition.DirectionTo(target.GlobalPosition).Y, 0);
		trail.GlobalPosition = connection.GetPointPosition(1);
		trail.ProcessMaterial = particleAffect;
		trail.Emitting = true;


		particleList.Add(trail);


		return trail;
	}

	private void fadeInOrOut(Node2D node,bool isfadeIn, bool isFreed = false)
	{
		Tween fade = GetTree().CreateTween();

		if (isfadeIn)
		{
			fade.TweenProperty(node, "modulate", new Color(node.Modulate.R,node.Modulate.G,node.Modulate.B,1f), 1f);
		}
		else
		{
			fade.TweenProperty(node, "modulate", Colors.Transparent, 1f);
			if (isFreed) fade.TweenCallback(Callable.From(node.QueueFree));
		}

	}
}

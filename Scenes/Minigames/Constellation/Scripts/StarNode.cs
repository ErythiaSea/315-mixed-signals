using Godot;
using System;
using System.Globalization;

public partial class StarNode : Node2D
{
    [Export]
    Gradient indicatorGradient;

    [Export]
    Godot.Collections.Array<StarNode> adjacentStars;

    [Export]
    float centerFocusRange = 300f;

    [Export]
    float lineSpeed = 2.0f;

    private float timeToRegister = 2f;
    private float timeElapsed = 0f;
    private float alphaChange = 0f;

    private Godot.Collections.Array<Line2D> indicatorList;

    private Godot.Collections.Array<Line2D> lineList;
    private Godot.Collections.Array<Vector2> lineTargets;
    private Godot.Collections.Array<float> lineProgress;

    private StarsParent parent;
    private Label numberDisplay;

    public bool isFound = false;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        numberDisplay = GetChild(1) as Label;
        parent = GetParent() as StarsParent;
        lineList = new Godot.Collections.Array<Line2D>();
        lineTargets = new Godot.Collections.Array<Vector2>();
        lineProgress = new Godot.Collections.Array<float>();

        indicatorList = new Godot.Collections.Array<Line2D>();
        timeElapsed = 0f;
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
        {
            if (alphaChange <= 1)
            {
                alphaChange += (float)delta * parent.displaySpeed;
                numberDisplay.Modulate = new Color(numberDisplay.Modulate.R, numberDisplay.Modulate.G, numberDisplay.Modulate.B, alphaChange);
            }
        }


        if (lineList.Count > 0)
        {
            for(int i = 0; i < lineList.Count; i++)
            {
                if (lineProgress[i] < 1f)
                {
                    lineProgress[i] += (float)delta * lineSpeed;

                    Mathf.Clamp(lineProgress[i], 0f, 1f);

                    Vector2 linePos = lineList[i].Points[0].Lerp(lineTargets[i], lineProgress[i]);

                    lineList[i].SetPointPosition(1, linePos);
                }
            }
        }

        if(indicatorList.Count > 0)
        {
            for(int i = 0; i < indicatorList.Count; i++)
            {
                if (indicatorList[i].Modulate.A < 1f)
                {
                    float newAlpha = indicatorList[i].Modulate.A + (float)delta * parent.displaySpeed;

                    indicatorList[i].Modulate = new Color(indicatorList[i].Modulate.R, indicatorList[i].Modulate.G, indicatorList[i].Modulate.B, newAlpha);
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
        connection.DefaultColor = Colors.White;
        this.AddSibling(connection);

       
        lineList.Add(connection);
        lineProgress.Add(0f);
        lineTargets.Add(star.GlobalPosition);
    }

    public void FreeIndicator(Node2D star)
    {
        float bestProd = 0;
        int index = 0;


        Vector2 starDir = GlobalPosition - star.GlobalPosition;
     

        GD.Print(indicatorList.Count);
        for (int i = 0; i < indicatorList.Count; i++)
        {
            Vector2 indDir = indicatorList[i].GetPointPosition(0) - indicatorList[i].GetPointPosition(1);
            float dotProd = indDir.Dot(starDir);

            if (dotProd > bestProd)
            {
                index = i;
                bestProd = dotProd;
            }
        }

        if (indicatorList[index] != null)
        {
            indicatorList[index].Free();
        }
    }

    private void StarFound()
    {
        foreach (StarNode star in adjacentStars)
        {
            if (star.isFound)
            {
                ConnectStars(star);
                //star.FreeIndicator(this);
            }
            else
            {
                StarIndicators(star);
            }
        }
    }

    private void StarIndicators(Node2D star)
    {
        GD.Print("DRAWing");
        Line2D indicator = new Line2D();
        Vector2 starDir = GlobalPosition.DirectionTo(star.GlobalPosition);

        indicator.AddPoint(GlobalPosition + (starDir * 50f));
        indicator.AddPoint(GlobalPosition + (starDir * 100f));

        indicator.Modulate = new Color(indicator.Modulate.R, indicator.Modulate.G, indicator.Modulate.B, 0);

        indicator.Gradient = indicatorGradient;

        this.AddSibling(indicator);

        indicatorList.Add(indicator);
    }

   
}

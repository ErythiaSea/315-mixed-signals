using Godot;
using System;


public partial class GodChoice : CanvasLayer
{

	[Export]
	public Godot.Collections.Array<Texture2D> poloroidSprites;
	private Node2D Parent;
	private ColorRect blurRect;
	private Sprite2D godsSprite;

	private bool clicked = false;
	private bool isDisplaying = false;

	private Sprite2D poloroid;

	private Godot.Collections.Array<TextureButton> godButtons;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		godButtons = GetButtons();

        Parent = GetParent() as Node2D;
        Parent.GetNode<InteractBox>("GodBox").Interacted += BlurScene;

		godsSprite = Parent.GetNode("GodsSprite") as Sprite2D; 
		blurRect = GetNode("Blur") as ColorRect;

		poloroid = GetNode("Poloroid") as Sprite2D;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (clicked)
		{
            if (Input.IsActionPressed("close"))
			{
				UnBlurScene();
			}

			if (!isDisplaying)
			{
				DisplayGods();
				isDisplaying = true;
			}
        }
	}

	public void BlurScene()
	{
		GD.Print("gods time");
		Globals.PushGamestate(GAMESTATE.CUTSCENE);
		this.Visible = true;
		clicked = true;
		Tween fade = GetTree().CreateTween();
		fade.TweenProperty(blurRect, "modulate", new Color(blurRect.Modulate.R, blurRect.Modulate.G, blurRect.Modulate.B, 1f), 1f);

	}

	public void UnBlurScene()
	{
		Globals.PopGamestate(GAMESTATE.CUTSCENE);
        clicked = false;
        Tween fade = GetTree().CreateTween();
        fade.TweenProperty(blurRect, "modulate", new Color(blurRect.Modulate.R, blurRect.Modulate.G, blurRect.Modulate.B, 0f), 1f);

    }

	public void ConfirmationButton(TextureButton pressedButton)
	{
		foreach(TextureButton button in godButtons)
		{
			if(button != pressedButton)
			{
				HBoxContainer boxDisplay = button.GetChild(0) as HBoxContainer;
				if(boxDisplay.Modulate.A != 0)
				{
                    fade(boxDisplay,0f,false);
                }
				
			}
			else
			{
                HBoxContainer boxDisplay = button.GetChild(0) as HBoxContainer;
                if (boxDisplay.Modulate.A != 1)
				{
					fade(boxDisplay, 1f,true);
				}

            }
		}
	}

	public void CorvusConfirm()
	{
		if(Globals.Instance.corvusLove > 2)
		{
			poloroid.Texture = poloroidSprites[0];
		}
		else
		{
			poloroid.Texture = poloroidSprites[3];
		}

        DisplayEndScreen();
    }

	public void PyxisConfirm()
	{
        if (Globals.Instance.pyxisLove > 2)
        {
			poloroid.Texture = poloroidSprites[2];
        }
        else
        {
			poloroid.Texture = poloroidSprites[5];
        }

        DisplayEndScreen();
    }

	public void CassioConfirm()
	{
        if (Globals.Instance.cassioLove > 2)
        {
			poloroid.Texture = poloroidSprites[1];
        }
        else
        {
			poloroid.Texture = poloroidSprites[4];
        }

		DisplayEndScreen();
    }

	private void DisplayEndScreen()
	{
		blurRect.Material = null;
		blurRect.ZIndex = 1;
		poloroid.ZIndex = 1;

		Tween display = CreateTween();
        display.Parallel().TweenProperty(poloroid, "modulate", new Color(poloroid.Modulate.R, poloroid.Modulate.G, poloroid.Modulate.B, 1f), 2f);
        display.Parallel().TweenProperty(poloroid,"position", new Vector2(poloroid.Position.X, 350f), 3f);

    }
	private void DisplayGods()
	{

		Tween display = GetTree().CreateTween();

		display.Parallel().TweenProperty(godsSprite, "modulate", new Color(godsSprite.Modulate.R, godsSprite.Modulate.G, godsSprite.Modulate.B, 0f), 1f);

		GD.Print("GD num: " + godButtons.Count);
		foreach(TextureButton button in godButtons)
		{
            display.Parallel().TweenProperty(button, "modulate", new Color(button.Modulate.R, button.Modulate.G, button.Modulate.B, 1f), 2f);
			display.Parallel().TweenProperty(button, "position", new Vector2(button.Position.X, 350f), 3f);

			button.Pressed += (() => ConfirmationButton(button));
        }
		
		
    }

	private Godot.Collections.Array<TextureButton> GetButtons()
	{
		Godot.Collections.Array<TextureButton> array = new Godot.Collections.Array<TextureButton>();

		foreach(Node child in GetChildren())
		{
			if(child is TextureButton)
			{
				TextureButton chd = child as TextureButton; 
                array.Add(chd);
				
            }
		}

		return array;
	}

	private void fade(Control obj, float fadeVal, bool visible)
	{
		Tween fade = CreateTween();

		if (visible)
		{
			obj.Visible = visible;
			fade.TweenProperty(obj, "modulate", new Color(obj.Modulate.R, obj.Modulate.G, obj.Modulate.B, 1f), 1f);
        }
		else
		{
			fade.TweenProperty(obj, "modulate", new Color(obj.Modulate.R, obj.Modulate.G, obj.Modulate.B, 0f), 1f);
			fade.TweenProperty(obj, "visible", visible, 0.1f);
        }
	}
}

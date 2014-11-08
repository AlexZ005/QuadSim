
using UnityEngine;
using Rapid.Tools;

/// <summary>
/// This is a simple example of how you could draw bar graphs to measure performance or show statistics.
/// </summary>
public class GraphGUI : MonoBehaviour
{
	public Material RenderMaterial;
	public Color MainColor;
	public Color SubColor;
	public Color OutlineColor;
	public Color OverflowColor = Color.red;
	public Color RealValueIndicatorColor = Color.white;
	GraphBar _deltaBar;
	GraphBar _smoothDeltaBar;
	GraphBar _bar;
	
	System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
	
	
	void Awake()
	{
		var cam = Camera.main;
		
		var subcol = Color.white*0.7f;
		subcol.a = 1f;
		
		_deltaBar = new GraphBar(cam, RenderMaterial){ Position = new Vector2(Screen.width/3, 30f), MainColor = this.MainColor, SubColor = this.SubColor, OutlineColor = this.OutlineColor, OverflowColor=this.OverflowColor, RealValueColor=this.RealValueIndicatorColor };
		_smoothDeltaBar = new GraphBar(cam, RenderMaterial){ Position = new Vector2(20f, 70f), MainColor = this.MainColor, SubColor = this.SubColor, OutlineColor = this.OutlineColor, OverflowColor=this.OverflowColor, RealValueColor=this.RealValueIndicatorColor };
		_bar = new GraphBar(cam, RenderMaterial){ Position = new Vector2(20f, 110f), MainColor = this.MainColor, SubColor = this.SubColor, OutlineColor = this.OutlineColor, OverflowColor=this.OverflowColor, RealValueColor=this.RealValueIndicatorColor };
	}
	
	void Update()
	{
		_deltaBar.Update(Time.deltaTime*10f);
		_smoothDeltaBar.Update(Time.smoothDeltaTime*10f);
		
		stopwatch.Reset();
		stopwatch.Start();
		PerformHeavyCalculations(Random.Range(50000, 70000));
		stopwatch.Stop();
		
		_bar.Update((float)stopwatch.ElapsedMilliseconds / 10f);
	}
	
	void PerformHeavyCalculations(int amount)
	{
		float v=0f;
		for(int i=0; i<amount; ++i)
		{
			v += Mathf.Sin(i);
		}
	}
	
	void OnPostRender()
	{
		_deltaBar.Draw();
		//_smoothDeltaBar.Draw();
		
		//_bar.Draw();
	}
	
	public bool ScreenSpace
	{
		get { return _deltaBar.ScreenSpace; }
		set {
			_deltaBar.ScreenSpace = _smoothDeltaBar.ScreenSpace = _bar.ScreenSpace = value;
		}
	}
	
	void OnGUI()
	{
		ScreenSpace = GUILayout.Toggle(ScreenSpace, "Screen space");
		
		if(!ScreenSpace) return;
		
		var rect = new Rect(_deltaBar.EndPosition.x, 0f, 200f, 20f);
		var h = _deltaBar.Thickness*0.5f;
		
		rect.y = Screen.height - _deltaBar.Position.y - h;
		
		rect.y = Screen.height - _smoothDeltaBar.Position.y - h;
		
		rect.y = Screen.height - _bar.Position.y - h;
		rect.width = Screen.width/2;
		//GUI.Label(rect, "<- System.Diagnostics.Stopwatch measured time");
	}
};
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// [ExecuteInEditMode]
public class BlurHighlight : BaseCompletePP
{
	[Range(1, 50)]
	public int blurRadius = 20;
	[Range(0.0f, 100.0f)]
	public float radius = 10;
	[Range(0.0f, 100.0f)]
	public float softenEdge = 30;
	[Range(0.0f, 1.0f)]
	public float shade = 0.5f;
	public Transform trackedObject;
	private Vector4 center;
	
	private RenderTexture blurOutput = null;
	private int kernelBlurPass = -1;
	// private int kernelHorzPassID = -1;
	// private int kernelVertPassID = -1;
	
	protected override void Init()
	{
		center = new Vector4();
		kernelName = "Highlight";
		kernelBlurPass = shader.FindKernel("BlurPass");
		// kernelHorzPassID = shader.FindKernel("HorzPass");
		// kernelVertPassID = shader.FindKernel("VertPass");
		base.Init();
	}
	
	protected override void CreateTextures()
	{
		base.CreateTextures();
		CreateTexture(ref blurOutput);
		shader.SetTexture(kernelBlurPass,"source",renderedSource);
		shader.SetTexture(kernelBlurPass,"blurOutput",blurOutput);
		shader.SetTexture(kernelHandle,"blurOutput",blurOutput);
	}
	
	private void OnValidate()
	{
		if(!init) Init();
		SetProperties();
	}
	
	protected void SetProperties()
	{
		float rad = (radius / 100.0f) * texSize.y;
		shader.SetFloat("radius", rad);
		shader.SetFloat("edgeWidth", rad * softenEdge / 100.0f);
		shader.SetFloat("shade", shade);
		shader.SetInt("blurRadius",blurRadius);
	}
	
	protected override void DispatchWithSource(ref RenderTexture source, ref RenderTexture destination)
	{
		if (!init) return;
		Graphics.Blit(source, renderedSource);
		shader.Dispatch(kernelBlurPass,groupSize.x,groupSize.y,1);
		shader.Dispatch(kernelHandle, groupSize.x, groupSize.y, 1);
		Graphics.Blit(output, destination);
	}
	
	protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (shader == null)
		{
			Graphics.Blit(source, destination);
		}
		else
		{
			if (trackedObject && thisCamera)
			{
				Vector3 pos = thisCamera.WorldToScreenPoint(trackedObject.position);
				center.x = pos.x;
				center.y = pos.y;
				shader.SetVector("center", center);
			}
			CheckResolution(out bool resChange);
			if (resChange) SetProperties();
			DispatchWithSource(ref source, ref destination);
		}
	}
}
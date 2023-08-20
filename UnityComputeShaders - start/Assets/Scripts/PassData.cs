using UnityEngine;
using System.Collections;

public class PassData : MonoBehaviour
{
	public ComputeShader shader;
	public int texResolution = 1024;
	
	private Renderer rend;
	private RenderTexture outputTexture;
	
	private int clearHandle;
	private int circlesHandle;
	
	public Color clearColor = new Color();
	public Color circleColor = new Color();
	
	private void Start()
	{
		outputTexture = new RenderTexture(texResolution, texResolution, 0);
		outputTexture.enableRandomWrite = true;
		outputTexture.Create();
		rend = GetComponent<Renderer>();
		rend.enabled = true;
		InitShader();
	}
	
	private void InitShader()
	{
		shader.SetInt(nameof(texResolution),texResolution);
		rend.material.SetTexture("_MainTex",outputTexture);
		
		clearHandle = shader.FindKernel("Clear");
		circlesHandle = shader.FindKernel("Circles");
		shader.SetTexture(clearHandle,"Result",outputTexture);
		shader.SetTexture(circlesHandle,"Result",outputTexture);
	}
	
	private void DispatchKernel(int count)
	{
		shader.Dispatch(clearHandle, texResolution/8, texResolution/8, 1);
		shader.Dispatch(circlesHandle, count, 1, 1);
	}
	
	private void Update()
	{
		shader.SetFloat("time",Time.time*0.001f);
		shader.SetVector(nameof(clearColor),clearColor);
		shader.SetVector(nameof(circleColor),circleColor);
		DispatchKernel(4);
	}
}
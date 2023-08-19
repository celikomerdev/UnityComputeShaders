using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssignTexture : MonoBehaviour
{
	[SerializeField]private Renderer rend = null;
	[SerializeField]private ComputeShader shader = null;
	[SerializeField]private int texResolution = 256;
	[SerializeField]private string kernelName = "CSMain";
	
	private RenderTexture outputTexture = null;
	private int kernelHandle = 0;
	
	private void Start()
	{
		outputTexture = new RenderTexture(texResolution,texResolution,0);
		outputTexture.enableRandomWrite = true;
		outputTexture.Create();
		rend.material.SetTexture("_MainTex",outputTexture);
		InitShader();
		// DispatchShader(texResolution/16,texResolution/16);
	}
	
	private void InitShader()
	{
		kernelHandle = shader.FindKernel(name:kernelName);
		shader.SetTexture(kernelHandle,"Result",outputTexture);
		shader.SetInt(nameof(texResolution),texResolution);
	}
	
	private void DispatchShader(int x,int y)
	{
		shader.Dispatch(kernelHandle,x,y,1);
	}
	
	
	private void Update()
	{
		if(Input.GetKeyUp(KeyCode.Space)) DispatchShader(texResolution/8,texResolution/8);
	}
}
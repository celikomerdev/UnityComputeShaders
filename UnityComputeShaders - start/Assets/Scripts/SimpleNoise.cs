using UnityEngine;

public class SimpleNoise : MonoBehaviour
{
	public ComputeShader shader;
	public int texResolution = 256;
	private Renderer rend;
	private RenderTexture outputTexture;
	private int kernelHandle;
	
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
		kernelHandle = shader.FindKernel("CSMain");
		shader.SetInt("texResolution", texResolution);
		shader.SetTexture(kernelHandle, "Result", outputTexture);
		rend.material.SetTexture("_MainTex", outputTexture);
	}
	
	private void DispatchShader(int x, int y)
	{
		shader.SetFloat("time", Time.time);
		shader.Dispatch(kernelHandle, x, y, 1);
	}
	
	private void Update()
	{
		DispatchShader(texResolution / 8, texResolution / 8);
	}
}
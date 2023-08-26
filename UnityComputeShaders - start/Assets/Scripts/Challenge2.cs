using UnityEngine;

public class Challenge2 : MonoBehaviour
{
	public ComputeShader shader;
	public int texResolution = 1024;
	
	private Renderer rend;
	private RenderTexture outputTexture;
	
	int kernelHandle;
	public Color fillColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);
	public Color clearColor = new Color( 0, 0, 0.3f, 1.0f );
	
	void Start()
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

		shader.SetVector("fillColor", fillColor);
		shader.SetVector("clearColor", clearColor);

		shader.SetInt("texResolution", texResolution);
		shader.SetInt("sides",6);
		
		shader.SetTexture(kernelHandle, "Result", outputTexture);
	
		rend.material.SetTexture("_MainTex", outputTexture);
	}
	
	private void DispatchShader(int x, int y)
	{
		shader.Dispatch(kernelHandle, x, y, 1);
	}
	
	private void Update()
	{
		shader.SetFloat("time",Time.time);
		DispatchShader(texResolution / 8, texResolution / 8);
	}
}
using UnityEngine;
using System.Collections;
using PBDFluid;

public class PassData : MonoBehaviour
{
	public ComputeShader shader;
	public int texResolution = 1024;
	public int count = 4;
	
	private Renderer rend;
	private RenderTexture outputTexture;
	
	private int clearHandle;
	private int circlesHandle;
	
	public Color clearColor = new Color();
	public Color circleColor = new Color();
	
	private ComputeBuffer buffer;
	private Circle[] circleData;
	struct Circle
	{
		public float radius;
		public Vector2 origin;
		public Vector2 velocity;
	}
	
	private void Start()
	{
		outputTexture = new RenderTexture(texResolution, texResolution, 0);
		outputTexture.enableRandomWrite = true;
		outputTexture.Create();
		rend = GetComponent<Renderer>();
		rend.enabled = true;
		InitData();
		InitShader();
	}
	
	private void InitData()
	{
		circlesHandle = shader.FindKernel("Circles");
		uint threadGroupSizeX = 1;
		shader.GetKernelThreadGroupSizes(circlesHandle,out threadGroupSizeX,out _,out _);
		int total = (int)threadGroupSizeX*count;
		
		circleData = new Circle[total];
		for (int i = 0; i < circleData.Length; i++)
		{
			Circle circle = circleData[i];
			circle.origin.x = Random.Range(0,texResolution);
			circle.origin.y = Random.Range(0,texResolution);
			circle.velocity.x = Random.Range(-100f,100f);
			circle.velocity.y = Random.Range(-100f,100f);
			circle.radius = Random.Range(10f,30f);
			circleData[i] = circle;
		}
		
	}
	
	private void InitShader()
	{
		shader.SetInt(nameof(texResolution),texResolution);
		rend.material.SetTexture("_MainTex",outputTexture);
		
		clearHandle = shader.FindKernel("Clear");
		circlesHandle = shader.FindKernel("Circles");
		shader.SetTexture(clearHandle,"Result",outputTexture);
		shader.SetTexture(circlesHandle,"Result",outputTexture);
		
		
		int stride = sizeof(float)*5;
		buffer = new ComputeBuffer(circleData.Length,stride);
		buffer.SetData(circleData);
		shader.SetBuffer(circlesHandle,"circlesBuffer",buffer);
	}
	
	private void DispatchKernel(int count)
	{
		shader.Dispatch(clearHandle, texResolution/8, texResolution/8, 1);
		shader.Dispatch(circlesHandle, count, 1, 1);
	}
	
	private void Update()
	{
		shader.SetFloat("time",Time.time);
		shader.SetVector(nameof(clearColor),clearColor);
		shader.SetVector(nameof(circleColor),circleColor);
		DispatchKernel(count);
	}
}
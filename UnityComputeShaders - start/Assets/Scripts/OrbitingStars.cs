using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingStars : MonoBehaviour
{
	[SerializeField]private uint starCount = 100;
	[SerializeField]private ComputeShader shader = null;
	[SerializeField]private GameObject prefab = null;
	
	private int kernOrbitingStars = 0;
	private uint threadGroupSizeX = 1;
	private int groupSizeX = 1;
	
	private Transform[] stars = new Transform[0];
	
	private void Start()
	{
		kernOrbitingStars = shader.FindKernel("OrbitingStars");
		shader.GetKernelThreadGroupSizes(kernOrbitingStars, out threadGroupSizeX, out _, out _);
		groupSizeX = (int)((starCount + threadGroupSizeX - 1) / threadGroupSizeX);
		
		stars = new Transform[starCount];
		for (int i = 0; i < starCount; i++)
		{
			stars[i] = Instantiate(prefab, transform).transform;
		}
		
		InitBuffer();
	}
	
	
	private ComputeBuffer positionBuffer = null;
	private Vector3[] outputPositions = new Vector3[0];
	private void InitBuffer()
	{
		positionBuffer = new ComputeBuffer((int)starCount,sizeof(float)*3);
		shader.SetBuffer(kernOrbitingStars,nameof(positionBuffer),positionBuffer);
		outputPositions = new Vector3[starCount];
	}
	
	
	private void Update()
	{
		shader.SetFloat("time",Time.time);
		shader.Dispatch(kernOrbitingStars,groupSizeX,1,1);
		
		positionBuffer.GetData(outputPositions);
		for (int i = 0; i < outputPositions.Length; i++)
		{
			stars[i].position = outputPositions[i];
		}
	}
	
	private void OnDestroy()
	{
		positionBuffer.Dispose();
	}
}
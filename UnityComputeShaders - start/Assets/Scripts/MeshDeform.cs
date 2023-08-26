using UnityEngine;

public class MeshDeform : MonoBehaviour
{
	private void Start()
	{
		InitVertexArrays();
		InitGPUBuffers();
	}
	
	[Range(0.5f,2.0f)]
	[SerializeField]private float radius = 1f;
	private void Update()
	{
		shader.SetFloat("radius", radius);
		float delta = (Mathf.Sin(Time.time) + 1)/ 2;
		shader.SetFloat("delta", delta);
		shader.Dispatch(kernelMeshDeform,threadGroupsX:mesh.vertexCount,1,1);
		GetVerticesFromGPU();
	}
	
	
	private Mesh mesh = null;
	private Vertex[] vertexOriginal = new Vertex[0];
	private Vertex[] vertexModified = new Vertex[0];
	private void InitVertexArrays()
	{
		mesh = GetComponent<MeshFilter>().mesh;
		vertexOriginal = new Vertex[mesh.vertexCount];
		vertexModified = new Vertex[mesh.vertexCount];
		for (int i = 0; i < mesh.vertexCount; i++)
		{
			Vertex vert = new Vertex(mesh.vertices[i],mesh.normals[i]);
			vertexOriginal[i] = vert;
			vertexModified[i] = vert;
		}
	}
	
	[SerializeField]private ComputeShader shader = null;
	private int kernelMeshDeform = -1;
	private ComputeBuffer bufferOriginal = null;
	private ComputeBuffer bufferModified = null;
	private void InitGPUBuffers()
	{
		kernelMeshDeform = shader.FindKernel("MeshDeform");
		bufferOriginal = new ComputeBuffer(mesh.vertexCount,6*sizeof(float));
		bufferModified = new ComputeBuffer(mesh.vertexCount,6*sizeof(float));
		
		bufferOriginal.SetData(vertexOriginal);
		bufferModified.SetData(vertexModified);
		
		shader.SetBuffer(kernelMeshDeform,"bufferOriginal",bufferOriginal);
		shader.SetBuffer(kernelMeshDeform,"bufferModified",bufferModified);
	}
	
	private void GetVerticesFromGPU()
	{
		bufferModified.GetData(vertexModified);
		Vector3[] returnedPositions = new Vector3[mesh.vertexCount];
		Vector3[] returnedNormals = new Vector3[mesh.vertexCount];
		for (int i = 0; i < mesh.vertexCount; i++)
		{
			returnedPositions[i] = vertexModified[i].position;
			returnedNormals[i] = vertexModified[i].normal;
		}
		mesh.SetVertices(returnedPositions);
		mesh.SetNormals(returnedNormals);
	}
	
	
	private struct Vertex
	{
		public Vector3 position;
		public Vector3 normal;
		public Vertex(Vector3 position,Vector3 normal)
		{
			this.position = position;
			this.normal = normal;
		}
	}
}
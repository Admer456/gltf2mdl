// SPDX-FileCopyrightText: 2024 Admer Šuko
// SPDX-License-Identifier: MIT

using System.Numerics;

namespace Gltf2Mdl.Data
{
	internal class GltfMesh
	{
		public List<Vector3> Positions { get; set; } = new();
		public List<Vector2> Uvs { get; set; } = new();
		public List<Vector3> Normals { get; set; } = new();
		public List<Vector4> BoneWeights { get; set; } = new();
		public List<Vector4> BoneIndices { get; set; } = new();
		public List<uint> Indices { get; set; } = new();

		public string MaterialName { get; set; } = string.Empty;

		public int NumTriangles => Indices.Count / 3;
		public int NumVertices => Positions.Count;

		public void AddVertex( Vector3 position, Vector2 uv, Vector3 normal )
			=> AddVertex( position, uv, normal, Vector4.Zero, Vector4.Zero );

		public void AddVertex( Vector3 position, Vector2 uv, Vector3 normal, Vector4 boneWeight, Vector4 boneIndex )
		{
			Positions.Add( position );
			Uvs.Add( uv );
			Normals.Add( normal );
			BoneWeights.Add( boneWeight );
			BoneIndices.Add( boneIndex );
		}

		public void AddTriangle( int a, int b, int c )
		{
			Indices.AddRange( [(uint)a, (uint)b, (uint)c] );
		}

		public void AddTriangle( uint a, uint b, uint c )
		{
			Indices.AddRange( [a, b, c] );
		}
	}
}

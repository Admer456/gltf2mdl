// SPDX-FileCopyrightText: 2024 Admer Šuko
// SPDX-License-Identifier: MIT

using Gltf2Mdl.Data;
using System.Numerics;

namespace Gltf2Mdl.Utilities
{
	internal static class SmdHelpers
	{
		public static SmdModel InitStaticProp( this SmdModel smd )
		{
			SmdJoint originBone = new()
			{
				Id = 0,
				ParentId = -1,
				Name = "origin"
			};

			SmdJointFrame originFrame = new()
			{
				Frame = 0,
				BoneId = 0,
				Position = Vector3.Zero,
				Rotation = Vector3.Zero
			};

			smd.Version = 1;
			smd.Joints = [originBone];
			smd.Animations = [originFrame];

			return smd;
		}

		public static SmdVertex ExtractVertex( this GltfMesh mesh, int vertexId )
			=> new()
			{
				// Todo: I think a good initial strategy here would
				// be to choose the bone with the heaviest weight
				BoneId = (int)mesh.BoneIndices[vertexId].X,
				Position = mesh.Positions[vertexId],
				Normal = mesh.Normals[vertexId],
				Uv = mesh.Uvs[vertexId]
			};

		public static List<SmdTriangle> ExtractTriangles( this GltfMesh mesh )
		{
			List<SmdTriangle> result = new();

			for ( int triangleId = 0; triangleId < mesh.NumTriangles; triangleId++ )
			{
				int vertexIndexId = triangleId * 3;
				int a = (int)mesh.Indices[vertexIndexId];
				int b = (int)mesh.Indices[vertexIndexId + 1];
				int c = (int)mesh.Indices[vertexIndexId + 2];

				result.Add( new()
				{
					Texture = mesh.MaterialName,
					Vertices = [
						mesh.ExtractVertex( a ),
						mesh.ExtractVertex( b ),
						mesh.ExtractVertex( c )
					]
				} );
			}

			return result;
		}
	}
}

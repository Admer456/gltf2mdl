// SPDX-FileCopyrightText: 2024 Admer Šuko
// SPDX-License-Identifier: MIT

using Gltf2Mdl.Data;
using SharpGLTF.Schema2;
using System.Numerics;

namespace Gltf2Mdl.Utilities
{
	internal static class GltfHelpers
	{
		// If I have 1,2,3 in glTF's Y-up, then
		// I will get 1,3,-2 in GoldSRC's Z-up
		public static Vector3 TransformYUp( Vector3 yup )
		{
			return new( yup.X, yup.Z, -yup.Y );
		}

		public static Vector4 TransformYUp( Vector4 yup )
		{
			return new( yup.X, yup.Z, -yup.Y, yup.W );
		}

		public static List<Vector3> LoadPositions( Accessor value )
			=> value.AsVector3Array().ToList();

		public static List<Vector3> LoadNormals( Accessor value )
			=> value.AsVector3Array().ToList();

		public static List<Vector2> LoadUvs( Accessor value )
			=> value.AsVector2Array().ToList();

		public static List<Vector4> LoadJoints( Accessor value )
			=> value.AsVector4Array().ToList();

		public static List<Vector4> LoadWeights( Accessor value )
			=> value.AsVector4Array().ToList();

		public static List<uint> LoadIndices( Accessor value )
			=> value.AsIndicesArray().ToList();

		public static GltfMesh ExtractMesh( MeshPrimitive primitive, bool yIntoZ = false )
		{
			GltfMesh result = new();

			foreach ( var accessor in primitive.VertexAccessors )
			{
				switch ( accessor.Key )
				{
					case "POSITION": result.Positions = LoadPositions( accessor.Value ); break;
					case "NORMAL": result.Normals = LoadNormals( accessor.Value ); break;
					case "TEXCOORD_0": result.Uvs = LoadUvs( accessor.Value ); break;
					case "JOINTS_0": result.BoneIndices = LoadJoints( accessor.Value ); break;
					case "WEIGHTS_0": result.BoneWeights = LoadWeights( accessor.Value ); break;
				}

				if ( yIntoZ )
				{
					result.Positions = result.Positions.Select( TransformYUp ).ToList();
					result.Normals = result.Normals.Select( TransformYUp ).ToList();
				}

				// In case there's no index or weight data, we gotta fill it in with zeroes
				if ( result.BoneIndices.Count != result.Positions.Count )
				{
					result.BoneIndices = result.Positions.Select( v => Vector4.Zero ).ToList();
					result.BoneWeights = result.Positions.Select( v => Vector4.Zero ).ToList();
				}

				result.Indices = primitive.IndexAccessor.AsIndicesArray().ToList();
				result.MaterialName = primitive.Material.Name;
			}

			return result;
		}
	}
}

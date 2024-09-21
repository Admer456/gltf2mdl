// SPDX-FileCopyrightText: 2024 Admer Šuko
// SPDX-License-Identifier: MIT

using System.Numerics;

namespace Gltf2Mdl.Data
{
	internal struct SmdVertex
	{
		public int BoneId { get; set; }
		public Vector3 Position { get; set; }
		public Vector3 Normal { get; set; }
		public Vector2 Uv { get; set; }
	}

	internal class SmdTriangle
	{
		public string Texture { get; set; } = string.Empty;
		public SmdVertex[] Vertices { get; set; } = new SmdVertex[3];
	}
}

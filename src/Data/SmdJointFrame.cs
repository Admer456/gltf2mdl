// SPDX-FileCopyrightText: 2024 Admer Šuko
// SPDX-License-Identifier: MIT

using System.Numerics;

namespace Gltf2Mdl.Data
{
	internal class SmdJointFrame
	{
		public int Frame { get; set; }
		public int BoneId { get; set; }
		public Vector3 Position { get; set; }
		public Vector3 Rotation { get; set; }
	}
}

// SPDX-FileCopyrightText: 2024 Admer Šuko
// SPDX-License-Identifier: MIT

namespace Gltf2Mdl.Data
{
	internal class SmdModel
	{
		public int Version { get; set; } = 1;
		public List<SmdJoint> Joints { get; set; } = new();
		public List<SmdJointFrame> Animations { get; set; } = new();
		public List<SmdTriangle> Triangles { get; set; } = new();
	}
}

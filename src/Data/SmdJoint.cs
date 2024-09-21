// SPDX-FileCopyrightText: 2024 Admer Šuko
// SPDX-License-Identifier: MIT

namespace Gltf2Mdl.Data
{
	internal class SmdJoint
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public int ParentId { get; set; }
	}
}

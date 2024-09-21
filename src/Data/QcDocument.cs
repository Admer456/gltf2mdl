// SPDX-FileCopyrightText: 2024 Admer Šuko
// SPDX-License-Identifier: MIT

using System.Numerics;

// Stuff here was derived from The303:
// https://the303.org/tutorials/gold_qc.htm
// and by looking at example QCs. It is not meant to be a
// 100% complete QC-in-C# specification, but rather just
// covers most of the QC spec so GLTF can be supported.

namespace Gltf2Mdl.Data
{
	public class QcBody
	{
		public string GroupName { get; set; } = "studio";
		public string SmdName { get; set; } = string.Empty;
		public bool Reverse { get; set; } = false;
		public float Scale { get; set; } = 1.0f;
	}

	public class QcEvent
	{
		public int Id { get; set; }
		public int Frame { get; set; }
		public string? Parameter { get; set; }
	}

	public class QcSequence
	{
		public string Name { get; set; } = string.Empty;
		public string SmdName { get; set; } = string.Empty;
		public float Framerate { get; set; } = 30.0f;
		public bool Looped { get; set; } = false;

		public List<QcEvent> Events { get; set; } = new();
	}

	public enum QcRenderMode
	{
		Normal,
		Additive,
		Masked // alphatest
	}

	public class QcTextureMode
	{
		public string TextureName { get; set; } = string.Empty;
		public QcRenderMode RenderMode { get; set; } = QcRenderMode.Normal;
	}

	public class QcDocument
	{
		public string ModelName { get; set; } = string.Empty;
		public List<QcBody> Bodies { get; set; } = new();
		public List<QcSequence> Sequences { get; set; } = new();

		// TODO: bounding box, collision box, hitboxes, flags

		public Vector3 Origin { get; set; } = Vector3.Zero;
		public Vector3 EyePosition { get; set; } = Vector3.Zero;
		public float Scale { get; set; } = 1.0f;

		public List<QcTextureMode> TextureRenderModes { get; set; } = new();
	}
}

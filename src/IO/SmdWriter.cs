// SPDX-FileCopyrightText: 2024 Admer Šuko
// SPDX-License-Identifier: MIT

using Gltf2Mdl.Data;
using System.Text;

namespace Gltf2Mdl.IO
{
	internal static class SmdWriter
	{
		public static void Write( SmdModel model, string path )
		{
			// Animation data needs to be organised into time frames
			Dictionary<int, List<SmdJointFrame>> jointFrameDict = new( model.Animations.Count );
			foreach ( var anim in model.Animations )
			{
				// TODO: possibly sort everything later
				if ( !jointFrameDict.ContainsKey( anim.Frame ) )
				{
					jointFrameDict.Add( anim.Frame, new( model.Joints.Count ) );
				}

				jointFrameDict[anim.Frame].Add( anim );
			}

			StringBuilder sb = new();
			sb.AppendLine( $"version {model.Version}" );
			sb.AppendLine( "nodes" );
			foreach ( var joint in model.Joints )
			{
				sb.AppendLine( $"  {joint.Id} \"{joint.Name}\" {joint.ParentId}" );
			}
			sb.AppendLine( "end" );
			sb.AppendLine( "skeleton" );
			foreach ( var frameTime in jointFrameDict )
			{
				sb.AppendLine( $"time {frameTime.Key}" );
				foreach ( var jointFrame in frameTime.Value )
				{
					sb.Append( $"  {jointFrame.BoneId}" );
					sb.Append( $" {jointFrame.Position.X} {jointFrame.Position.Y} {jointFrame.Position.Z}" );
					sb.AppendLine( $" {jointFrame.Rotation.X} {jointFrame.Rotation.Y} {jointFrame.Rotation.Z}" );
				}
			}
			sb.AppendLine( "end" );
			sb.AppendLine( "triangles" );
			foreach ( var triangle in model.Triangles )
			{
				sb.AppendLine( triangle.Texture );
				foreach ( var vertex in triangle.Vertices )
				{
					sb.AppendFormat( "  {0} {1} {2} {3} {4} {5} {6} {7} {8}\n",
						vertex.BoneId,
						vertex.Position.X, vertex.Position.Y, vertex.Position.Z,
						vertex.Normal.X, vertex.Normal.Y, vertex.Normal.Z,
						vertex.Uv.X, vertex.Uv.Y );
				}
			}
			sb.AppendLine( "end" );

			File.WriteAllText( path, sb.ToString() );
		}
	}
}

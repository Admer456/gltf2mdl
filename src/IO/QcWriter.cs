﻿// SPDX-FileCopyrightText: 2024 Admer Šuko
// SPDX-License-Identifier: MIT

using Gltf2Mdl.Data;
using System.Text;

namespace Gltf2Mdl.IO
{
	internal static class QcWriter
	{
		public static void Write( QcDocument value, string path )
		{
			StringBuilder sb = new();

			sb.AppendLine( "// This was autogenerated by gltf2mdl. If you edit this," );
			sb.AppendLine( "// it will be overwritten next time you run gltf2mdl." );

			sb.AppendLine();

			sb.AppendLine( $"$modelname \"{value.ModelName}\"" );
			sb.AppendLine( "$cd \".\"" );
			sb.AppendLine( "$cdtexture \".\"" );
			sb.AppendLine( $"$scale {value.Scale}" );
			sb.AppendLine( "$flags 0" );

			sb.AppendLine();

			sb.AppendLine( $"$origin {value.Origin.X} {value.Origin.Y} {value.Origin.Z}" );
			sb.AppendLine( $"$eyeposition {value.EyePosition.X} {value.EyePosition.Y} {value.EyePosition.Z}" );

			sb.AppendLine();

			// TODO: bodygroups eventually
			foreach ( var body in value.Bodies )
			{
				sb.AppendLine( $"$body \"{body.GroupName}\" \"{body.SmdName}\" {(body.Reverse ? "reverse" : "")} scale {body.Scale}" );
			}

			sb.AppendLine();

			foreach ( var textureRenderMode in value.TextureRenderModes )
			{
				sb.AppendLine( $"$texrendermode \"{textureRenderMode.TextureName}\" {textureRenderMode.RenderMode.ToString().ToLower()}" );
			}

			sb.AppendLine();

			foreach ( var sequence in value.Sequences )
			{
				sb.AppendLine( $"$sequence \"{sequence.Name}\" {{" );
				sb.AppendLine( $"	\"{sequence.SmdName}\"" );
				//sb.AppendLine( $"	\"{sequence.Activity}\"" );
				foreach ( var @event in sequence.Events )
				{
					sb.AppendLine( $"	{{ event {@event.Id} {@event.Frame} {@event.Parameter ?? ""} }}" );
				}
				sb.AppendLine( $"	fps {sequence.Framerate}" );
				if ( sequence.Looped )
				{
					sb.AppendLine( $"	loop" );
				}
				sb.AppendLine( "}" );
			}

			File.WriteAllText( path, sb.ToString() );
		}
	}
}

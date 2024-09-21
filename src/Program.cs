// SPDX-FileCopyrightText: 2024 Admer Šuko
// SPDX-License-Identifier: MIT

using Gltf2Mdl.Data;
using Gltf2Mdl.IO;
using Gltf2Mdl.Utilities;
using SharpGLTF.Schema2;
using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics;

var app = new CommandApp();

app.Configure( config =>
{
	config.SetInterceptor( new DebugCatchInterceptor() );

	config.AddCommand<ConvertCommand>( "convert" )
		.WithDescription( "Converts a glTF 2.0 file into a Half-Life MDL." )
		.WithExample( "A:/Models/train.glb" )
		.WithExample( "A:/Models/barrel001.gltf", "-o barrel" );
} );

app.SetDefaultCommand<ConvertCommand>();
app.Run( args );

internal sealed class DebugCatchInterceptor : ICommandInterceptor
{
	public void Intercept( CommandContext context, CommandSettings settings )
	{
		foreach ( var arg in context.Remaining.Raw )
		{
			if ( arg == "debugfreeze" )
			{
				Console.WriteLine( "[gltf2mdl] You've entered debug freeze mode." );
				Console.WriteLine( "[gltf2mdl] Attach a debugger to continue, or press Ctrl+C to disengage." );

				while ( !Debugger.IsAttached )
				{
					Thread.Sleep( 100 );
				}

				Console.WriteLine( "[gltf2mdl] Let's go!" );
			}
		}
	}
}

internal sealed class ConvertCommand : Command<ConvertCommand.Settings>
{
	public sealed class Settings : CommandSettings
	{
		[Description( "A path to a GLTF or GLB file." )]
		[CommandArgument( 0, "<path>" )]
		public string ModelPath { get; init; } = string.Empty;

		[Description( "The output name of the file." )]
		[CommandOption( "-o|--output-name" )]
		[DefaultValue( null )]
		public string? OutputName { get; init; }
	}

	private static void CreateDirectoryAdhoc( string path )
	{
		string? directoryPath = Path.GetDirectoryName( path );
		if ( directoryPath is null )
		{
			return;
		}

		if ( Directory.Exists( path ) )
		{
			return;
		}

		Directory.CreateDirectory( path );
	}

	private static void StartModelCompiler( string workingDirectory, string qcPath )
	{
		var compilerProcess = Process.Start( new ProcessStartInfo()
		{
			Arguments = $"\"{qcPath}\"",
			CreateNoWindow = true,
			FileName = "studiomdl.exe",
			RedirectStandardError = true,
			RedirectStandardOutput = true,
			WorkingDirectory = workingDirectory
		} );

		if ( compilerProcess is null )
		{
			return;
		}

		compilerProcess.BeginOutputReadLine();
		compilerProcess.BeginErrorReadLine();

		compilerProcess.OutputDataReceived += OnModelCompilerMessage;
		compilerProcess.ErrorDataReceived += OnModelCompilerError;

		compilerProcess.WaitForExit();
	}

	private static bool IsInError = false;

	private static void OnModelCompilerError( object sender, DataReceivedEventArgs e )
	{
		if ( e.Data is null )
		{
			return;
		}

		Console.ForegroundColor = Color.Black;
		Console.BackgroundColor = Color.Red;
		if ( !IsInError )
		{
			Console.Write( "[studiomdl][ERROR] " );
		}
		Console.WriteLine( e.Data ?? "unknown" );
		Console.ResetColor();

		IsInError = true;
	}

	private static void OnModelCompilerMessage( object sender, DataReceivedEventArgs e )
	{
		Console.WriteLine( $"[studiomdl] {e.Data}" );

		IsInError = false;
	}

	public override int Execute( CommandContext context, Settings settings )
	{
		// gltf2mdl has a few places for its output:
		// Input - the [filename].gltf file
		// Intermediate - the .smd and .qc files: [filename]_int/
		// Output - the [filename/outputname].mdl file
		string baseName = Path.GetFileNameWithoutExtension( settings.ModelPath );
		string basePath = Path.ChangeExtension( settings.ModelPath, null );
		string? baseDir = Path.GetDirectoryName( Path.GetFullPath( settings.ModelPath ) );
		if ( baseDir is null )
		{
			return 1;
		}

		baseDir = baseDir.Replace( '\\', '/' );

		string intermediateDir = $"{basePath}_int/";
		string outputPath = $"{baseDir}/{settings.OutputName ?? baseName}.mdl";

		CreateDirectoryAdhoc( intermediateDir );

		ModelRoot gltf = ModelRoot.Load( settings.ModelPath, new()
		{
			Validation = SharpGLTF.Validation.ValidationMode.Strict
		} );

		List<GltfMesh> gltfMeshes = new();
		foreach ( var mesh in gltf.LogicalMeshes )
		{
			foreach ( var primitive in mesh.Primitives )
			{
				gltfMeshes.Add( GltfHelpers.ExtractMesh( primitive, yIntoZ: true ) );
			}
		}

		// TODO: make a new SmdModel every 2048 vertices
		SmdModel smd = new SmdModel().InitStaticProp();
		foreach ( var mesh in gltfMeshes )
		{
			smd.Triangles.AddRange( mesh.ExtractTriangles() );
		}

		QcDocument qc = new()
		{
			ModelName = outputPath,
			Bodies = [new() { SmdName = "mesh1" }],
			Sequences = [new() { SmdName = "mesh1", Name = "default", Framerate = 1 }]
		};

		SmdWriter.Write( smd, $"{intermediateDir}/mesh1.smd" );
		Console.WriteLine( "[gltf2mdl] Wrote mesh1.smd" );

		QcWriter.Write( qc, $"{intermediateDir}/script.qc" );
		Console.WriteLine( "[gltf2mdl] Wrote script.qc" );

		Console.WriteLine( "[gltf2mdl] Calling StudioMDL to compile the model..." );
		StartModelCompiler( intermediateDir, "script.qc" );
		Console.WriteLine( "[gltf2mdl] Done, check for errors, cuz' I am not intelligent enough at the moment." );

		return 0;
	}

	public override ValidationResult Validate( CommandContext context, Settings settings )
	{
		return base.Validate( context, settings );
	}
}

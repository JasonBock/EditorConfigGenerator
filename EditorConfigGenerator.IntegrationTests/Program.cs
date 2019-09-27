using EditorConfigGenerator.Core.Styles;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EditorConfigGenerator.IntegrationTests
{
	class Program
	{
		private const string CodeDirectory = "Code";

		static async Task Main()
		{
			var currentDirectory = @"M:\JasonBock\ECGCode";
			var codeDirectory = Path.Combine(currentDirectory, Program.CodeDirectory);

			Console.Out.WriteLine($"Code directory: {codeDirectory}");

			if (!Directory.Exists(codeDirectory))
			{
				Console.Out.WriteLine($"Creating {codeDirectory}...");
				Directory.CreateDirectory(codeDirectory);
			}

			var projects = Program.GetProjects();

			foreach (var project in projects)
			{
				var projectDirectory = Path.Combine(codeDirectory, project.Name);

				if (!Directory.Exists(projectDirectory))
				{
					Console.Out.WriteLine($"Cloning {project.Name} from {project.Repo} into {projectDirectory}...");
					Directory.CreateDirectory(projectDirectory);
					Repository.Clone(project.Repo.AbsoluteUri, projectDirectory);
					Console.Out.WriteLine($"Cloning {project.Name} from {project.Repo} complete.");
					Console.Out.WriteLine();
				}
				else
				{
					// Fetch update it
					using var repo = new Repository(projectDirectory);
					var logMessage = string.Empty;

					foreach (var remote in repo.Network.Remotes)
					{
						var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
						Console.Out.WriteLine($"Fetching {project.Name}...");
						Commands.Fetch(repo, remote.Name, refSpecs, null, logMessage);
						Console.Out.WriteLine($"Fetching {project.Name} complete.");
						Console.Out.WriteLine();
					}
				}

				await Program.ProcessDirectory(projectDirectory);
			}

			Console.Out.WriteLine();
		}

		private static async Task ProcessDirectory(string projectDirectory)
		{
			Console.Out.WriteLine($"Analyzing {projectDirectory}...");
			Console.Out.WriteLine(await StyleGenerator.Generate(projectDirectory, Console.Out));
			Console.Out.WriteLine($"Analyzing {projectDirectory} complete.");
			Console.Out.WriteLine();
		}

		// TODO, make this a set and do equality pattern to ProjectInformation
		private static ImmutableList<ProjectInformation> GetProjects() =>
			new List<ProjectInformation>
			{
				new ProjectInformation("AngleSharp", new Uri("https://github.com/AngleSharp/AngleSharp.git")),
				new ProjectInformation("Autofac", new Uri("https://github.com/autofac/Autofac.git")),
				new ProjectInformation("AutoMapper", new Uri("https://github.com/AutoMapper/AutoMapper.git")),
				new ProjectInformation("CSLA", new Uri("https://github.com/MarimerLLC/csla.git")),
				new ProjectInformation("ImageSharp", new Uri("https://github.com/SixLabors/ImageSharp.git")),
				new ProjectInformation("Moq", new Uri("https://github.com/moq/moq.git")),
				new ProjectInformation("Newtonsoft.Json", new Uri("https://github.com/JamesNK/Newtonsoft.Json.git")),
				new ProjectInformation("NodaTime", new Uri("https://github.com/nodatime/nodatime.git")),
				new ProjectInformation("Rocks", new Uri("https://github.com/JasonBock/Rocks.git")),
				new ProjectInformation("Roslyn", new Uri("https://github.com/dotnet/roslyn.git")),
				new ProjectInformation("Rx", new Uri("https://github.com/Reactive-Extensions/Rx.NET.git")),
				new ProjectInformation("Serilog", new Uri("https://github.com/serilog/serilog.git")),
				new ProjectInformation("ZeroLog", new Uri("https://github.com/Abc-Arbitrage/ZeroLog.git")),
			}.ToImmutableList();
	}
}
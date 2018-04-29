using EditorConfigGenerator.Core.Styles;
using LibGit2Sharp;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EditorConfigGenerator.IntegrationTests
{
	class Program
	{
		private const string CodeDirectory = "Code";

		static async Task Main(string[] args)
		{
			var currentDirectory = @"M:\JasonBock\ECGCode";
			//var currentDirectory = args?.Length == 1 ? args[0] : Directory.GetCurrentDirectory();
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
					using (var repo = new Repository(projectDirectory))
					{
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
				}

				Console.Out.WriteLine($"Analyzing {projectDirectory}...");
				Console.Out.WriteLine(await StyleGenerator.GenerateFromDirectoryAsync(projectDirectory, Console.Out));
				Console.Out.WriteLine($"Analyzing {projectDirectory} complete.");
				Console.Out.WriteLine();
			}

			Console.Out.WriteLine();

			//async Task AnalyzeSolutionsAsync(string rootDirectory)
			//{
			//	foreach (var directory in Directory.GetDirectories(rootDirectory))
			//	{
			//		foreach (var file in Directory.GetFiles(directory))
			//		{
			//			if(Path.GetExtension(file).ToLower() == ".sln")
			//			{
			//				Console.Out.WriteLine($"Analyzing {file}...");
			//				Console.Out.WriteLine(await StyleGenerator.GenerateFromSolutionAsync(file, Console.Out));
			//				Console.Out.WriteLine($"Analyzing {file} complete.");
			//				Console.Out.WriteLine();
			//			}
			//		}

			//		await AnalyzeSolutionsAsync(directory);
			//	}
			//}

			//await AnalyzeSolutionsAsync(codeDirectory);
		}

		// TODO, make this a set and do equality pattern to ProjectInformation
		private static ImmutableList<ProjectInformation> GetProjects() =>
			ImmutableList.Create<ProjectInformation>(
					new ProjectInformation("AngleSharp", new Uri("https://github.com/AngleSharp/AngleSharp.git")),
					new ProjectInformation("Autofac", new Uri("https://github.com/autofac/Autofac.git")),
					new ProjectInformation("AutoMapper", new Uri("https://github.com/AutoMapper/AutoMapper.git")),
					new ProjectInformation("CSLA", new Uri("https://github.com/MarimerLLC/csla.git")),
					new ProjectInformation("Moq", new Uri("https://github.com/moq/moq.git")),
					new ProjectInformation("Newtonsoft.Json", new Uri("https://github.com/JamesNK/Newtonsoft.Json.git")),
					new ProjectInformation("NodaTime", new Uri("https://github.com/nodatime/nodatime.git")),
					new ProjectInformation("Rocks", new Uri("https://github.com/JasonBock/Rocks.git")),
					new ProjectInformation("Serilog", new Uri("https://github.com/serilog/serilog.git")));
	}
}
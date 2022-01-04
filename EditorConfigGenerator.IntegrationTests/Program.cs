using EditorConfigGenerator.Core.Styles;
using EditorConfigGenerator.IntegrationTests;
using LibGit2Sharp;
using System.Collections.Immutable;

const string CodeDirectory = "Code";

var currentDirectory = new DirectoryInfo(@"M:\JasonBock\ECGCode");
var codeDirectory = Path.Combine(currentDirectory.FullName, CodeDirectory);

await Console.Out.WriteLineAsync($"Code directory: {codeDirectory}").ConfigureAwait(false);

if (!Directory.Exists(codeDirectory))
{
	await Console.Out.WriteLineAsync($"Creating {codeDirectory}...").ConfigureAwait(false);
	Directory.CreateDirectory(codeDirectory);
}

var projects = GetProjects();

foreach (var project in projects)
{
	var projectDirectory = new DirectoryInfo(Path.Combine(codeDirectory, project.Name));

	if (!projectDirectory.Exists)
	{
		await Console.Out.WriteLineAsync($"Cloning {project.Name} from {project.Repo} into {projectDirectory}...").ConfigureAwait(false);
		projectDirectory.Create();
		Repository.Clone(project.Repo.AbsoluteUri, projectDirectory.FullName);
		await Console.Out.WriteLineAsync($"Cloning {project.Name} from {project.Repo} complete.").ConfigureAwait(false);
		await Console.Out.WriteLineAsync().ConfigureAwait(false);
	}
	else
	{
		using var repo = new Repository(projectDirectory.FullName);
		var logMessage = string.Empty;

		foreach (var remote in repo.Network.Remotes)
		{
			var refSpecs = remote.FetchRefSpecs.Select(x => x.Specification);
			await Console.Out.WriteLineAsync($"Fetching {project.Name}...").ConfigureAwait(false);
			Commands.Fetch(repo, remote.Name, refSpecs, null, logMessage);
			await Console.Out.WriteLineAsync($"Fetching {project.Name} complete.").ConfigureAwait(false);
			await Console.Out.WriteLineAsync().ConfigureAwait(false);
		}
	}

	await ProcessDirectoryAsync(projectDirectory).ConfigureAwait(false);
}

await Console.Out.WriteLineAsync().ConfigureAwait(false);

static async Task ProcessDirectoryAsync(DirectoryInfo projectDirectory)
{
	await Console.Out.WriteLineAsync($"Analyzing {projectDirectory}...").ConfigureAwait(false);
	await Console.Out.WriteLineAsync(await StyleGenerator.Generate(projectDirectory, Console.Out, true).ConfigureAwait(false)).ConfigureAwait(false);
	await Console.Out.WriteLineAsync($"Analyzing {projectDirectory} complete.").ConfigureAwait(false);
	await Console.Out.WriteLineAsync().ConfigureAwait(false);
}

// TODO, make this a set and do equality pattern to ProjectInformation
static ImmutableList<ProjectInformation> GetProjects() =>
	new List<ProjectInformation>
	{
		//new ProjectInformation("AngleSharp", new Uri("https://github.com/AngleSharp/AngleSharp.git")),
		//new ProjectInformation("Autofac", new Uri("https://github.com/autofac/Autofac.git")),
		//new ProjectInformation("AutoMapper", new Uri("https://github.com/AutoMapper/AutoMapper.git")),
		//new ProjectInformation("CSLA", new Uri("https://github.com/MarimerLLC/csla.git")),
		//new ProjectInformation("CsvHelper", new Uri("https://github.com/JoshClose/CsvHelper.git")),
		new ProjectInformation("ImageSharp", new Uri("https://github.com/SixLabors/ImageSharp.git")),
		//new ProjectInformation("Moq", new Uri("https://github.com/moq/moq.git")),
		//new ProjectInformation("Newtonsoft.Json", new Uri("https://github.com/JamesNK/Newtonsoft.Json.git")),
		//new ProjectInformation("NodaTime", new Uri("https://github.com/nodatime/nodatime.git")),
		new ProjectInformation("Rocks", new Uri("https://github.com/JasonBock/Rocks.git")),
		//new ProjectInformation("Roslyn", new Uri("https://github.com/dotnet/roslyn.git")),
		//new ProjectInformation("Rx", new Uri("https://github.com/Reactive-Extensions/Rx.NET.git")),
		//new ProjectInformation("Serilog", new Uri("https://github.com/serilog/serilog.git")),
		//new ProjectInformation("ZeroLog", new Uri("https://github.com/Abc-Arbitrage/ZeroLog.git")),
	}.ToImmutableList();
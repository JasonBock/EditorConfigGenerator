using System;

namespace EditorConfigGenerator.IntegrationTests
{
	internal sealed class ProjectInformation
	{
		public ProjectInformation(string name, Uri repo)
		{
			this.Name = name ?? throw new ArgumentNullException(nameof(name));
			this.Repo = repo ?? throw new ArgumentNullException(nameof(repo));
		}

		public string Name { get; }
		public Uri Repo { get; }
	}
}

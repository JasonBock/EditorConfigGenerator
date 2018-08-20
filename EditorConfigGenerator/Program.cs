﻿using EditorConfigGenerator.Core.Styles;
using System;
using System.IO;
using System.Threading.Tasks;

namespace EditorConfigGenerator
{
	class Program
	{
		static async Task Main(string[] args)
		{
			if (args?.Length != 1)
			{
				Console.Out.WriteLine("Usage: {fileName}, where the extension is .sln, .csproj, or .cs");
			}
			else
			{
				var arg = args[0];

				if(Directory.Exists(arg))
				{
					Console.Out.WriteLine(await StyleGenerator.Generate(arg, Console.Out));
				}
				else
				{
					var extension = Path.GetExtension(arg);

					if (extension == ".cs")
					{
						Console.Out.WriteLine(StyleGenerator.GenerateFromDocument(arg, Console.Out));
					}
					else
					{
						Console.Out.WriteLine("Usage: {fileName}, where the extension is .sln, .csproj, or .cs");
					}
				}
			}
		}
	}
}

#if NET472
/*
 * This implementation of Interop.GetRandomBytes was copied from https://github.com/mono/mono/blob/256a6c45ec75874c3f634cadb92091b4c30dfb3c/mcs/class/corlib/corefx/Interop.GetRandomBytes.Mono.cs.
 * Its associated license (https://github.com/mono/mono/blob/master/LICENSE) has been added to this repository's third party notices file.
 */
using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

namespace EditorConfigGenerator.Core.Statistics
{
	internal partial class Interop
	{
		static class MonoGetRandomBytesFallback
		{
			static object _rngAccess = new object();
			static RNGCryptoServiceProvider _rng = new RNGCryptoServiceProvider();

			internal static void GetRandomBytes(byte[] buffer)
			{
				lock (_rngAccess)
				{
					if (_rng == null)
						_rng = new RNGCryptoServiceProvider();
					_rng.GetBytes(buffer);
				}
			}

			internal static unsafe void GetRandomBytes(byte* buffer, int length)
			{
				lock (_rngAccess)
				{
					if (_rng == null)
						_rng = new RNGCryptoServiceProvider();
					var safe = new byte[length];
					_rng.GetBytes(safe);
					for (var i = 0; i < length; i++)
					{
						buffer[i] = safe[i];
					}
				}
			}
		}

		internal static unsafe void GetRandomBytes(byte* buffer, int length) => MonoGetRandomBytesFallback.GetRandomBytes(buffer, length);
	}
}
#endif
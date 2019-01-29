using System;
using System.IO;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure
{
    public static class StreamExtensions
    {
        [NotNull][Pure]
        public static byte[] ReadAllBytes([NotNull] this Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            var bytes  = new byte[stream.Length];
            var read = stream.Read(bytes, 0, (int) stream.Length);
            if (read != stream.Length)
                throw new InvalidOperationException(); // Formal
            return bytes;
        }
    }
}

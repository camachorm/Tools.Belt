using System;
using System.Diagnostics.CodeAnalysis;

namespace Tools.Belt.Common.Mocks
{
    [ExcludeFromCodeCoverage]
    public class DisposableScope : IDisposable
    {
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool dispose)
        {
        }
    }
}
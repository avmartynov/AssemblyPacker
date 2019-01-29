using System;
using System.Collections.Generic;

namespace Twidlle.Infrastructure
{
    /// <inheritdoc />
    /// <summary>  Управление временем жизни набора Disposable-объектов как единого Disposable-объекта. </summary>
    public sealed class MultiDisposable : IDisposable
    {
        /// <summary> Иициализирует класс массивом одноразовых объектов. 
        /// Все эти объекты будут освобождены при освобождении данного класса. </summary>
        public MultiDisposable(params IDisposable[] services)
        {
            _services = services;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed)
                return;

            foreach (var disposable in _services)
                disposable.Dispose();

            _disposed = true;
        }

        #region Private members

        private bool _disposed;
        private readonly IEnumerable<IDisposable> _services;

        #endregion Private members
    }
}

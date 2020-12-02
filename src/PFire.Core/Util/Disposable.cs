using System;

namespace PFire.Core.Util
{
    public abstract class Disposable : IDisposable
    {
        protected bool Disposed;
        protected bool IsDisposing;
        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    try
                    {
                        IsDisposing = true;
                        DisposeManagedResources();
                    }
                    finally
                    {
                        IsDisposing = false;
                    }
                }

                Disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void DisposeManagedResources() { }
    }
}

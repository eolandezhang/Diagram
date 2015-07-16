﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP
{
    public class DisposableBase : IDisposable
    {
        private bool disposed;

        protected bool Disposed
        {
            get
            {
                lock (this)
                {
                    return disposed;
                }
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            lock (this)
            {
                if (disposed == false)
                {
                    Cleanup();
                    disposed = true;

                    GC.SuppressFinalize(this);
                }
            }
        }

        #endregion

        protected virtual void Cleanup()
        {
            // override to provide cleanup
        }

        ~DisposableBase()
        {
            Cleanup();
        }
    }
}

using System;
using System.ComponentModel;
using Twidlle.Infrastructure.CodeAnnotation;

namespace Twidlle.Infrastructure
{
    public abstract class Presenter<TView>
        where TView  : class 
    {
        protected Presenter([NotNull] TView view)
        {
            View  = view  ?? throw new ArgumentNullException(nameof(view));
        }

        [NotNull]
        protected TView View { get; }
    }

    public abstract class Presenter<TView, TViewModel> : Presenter<TView>
        where TView  : class 
        where TViewModel : class, new()
    {
        protected Presenter([NotNull] TView view, [CanBeNull] TViewModel viewModel = null)
            : base(view)
        {
            ViewModel = viewModel ?? new TViewModel();
        }

        [NotNull]
        public TViewModel ViewModel { get; protected set; }
    }


    public abstract class PresenterComponent<TView> : Presenter<TView>, IComponent 
        where TView : class
    {
        protected PresenterComponent([NotNull] TView view)
            : base(view)
        {
        }

        #region IComponent 

        public ISite Site { get; set; }
        public event EventHandler Disposed;

        public void Dispose()
        {
            Dispose(true);        
            GC.SuppressFinalize(this);
            Disposed?.Invoke(this, EventArgs.Empty);        
        }

        protected virtual void Dispose(bool dispose)
        {
        }

        #endregion
    }


    public abstract class PresenterComponent<TView, TViewModel> : Presenter<TView, TViewModel>, IComponent 
        where TView : class 
        where TViewModel : class, new()
    {
        protected PresenterComponent([NotNull] TView view, TViewModel viewModel = null)
            : base(view, viewModel)
        {
        }

        #region IComponent 

        public ISite Site { get; set; }
        public event EventHandler Disposed;

        public void Dispose()
        {
            Dispose(true);        
            GC.SuppressFinalize(this);
            Disposed?.Invoke(this, EventArgs.Empty);        
        }

        protected virtual void Dispose(bool dispose)
        {
        }

        #endregion
    }
}

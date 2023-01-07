namespace oprot.plot.wpf.ViewModels
{
    public interface IViewModelProvider 
    {
        public TViewModel GetFor<TViewModel>(object m) where TViewModel : class;
    }

}

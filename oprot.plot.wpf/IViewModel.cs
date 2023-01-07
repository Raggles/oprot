using Windows.Gaming.Input;

namespace oprot.plot.wpf
{
    public interface IViewModel<TModel> where TModel : class
    {
        public TModel Model { get; set; }

        //public abstract object GetModel();

        public bool IsViewModelOf(TModel o) => Model == o;
    }
}

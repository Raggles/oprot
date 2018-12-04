using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace oprot.plot.wpf
{
    public static class InputBindingsManager
    {

        public static readonly DependencyProperty UpdateOnEnterProperty = DependencyProperty.RegisterAttached(
                "UpdateOnEnter", typeof(DependencyProperty), typeof(InputBindingsManager), new PropertyMetadata(null, OnUpdateOnEnterPropertyChanged));

        static InputBindingsManager()
        {

        }

        public static void SetUpdateOnEnter(DependencyObject dp, DependencyProperty value)
        {
            dp.SetValue(UpdateOnEnterProperty, value);
        }

        public static DependencyProperty GetUpdateOnEnter(DependencyObject dp)
        {
            return (DependencyProperty)dp.GetValue(UpdateOnEnterProperty);
        }

        private static void OnUpdateOnEnterPropertyChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = dp as UIElement;

            if (element == null)
            {
                return;
            }

            if (e.OldValue != null)
            {
                element.PreviewKeyDown -= HandlePreviewKeyDown;
            }

            if (e.NewValue != null)
            {
                element.PreviewKeyDown += new KeyEventHandler(HandlePreviewKeyDown);
            }
        }

        static void HandlePreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DoUpdateSource(e.Source);
            }
        }

        static void DoUpdateSource(object source)
        {
            DependencyProperty property = GetUpdateOnEnter(source as DependencyObject);

            if (property == null)
            {
                return;
            }

            UIElement elt = source as UIElement;

            if (elt == null)
            {
                return;
            }

            BindingExpression binding = BindingOperations.GetBindingExpression(elt, property);

            if (binding != null)
            {
                binding.UpdateSource();
            }
        }
    }
}

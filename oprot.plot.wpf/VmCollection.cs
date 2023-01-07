﻿using CommunityToolkit.Mvvm.ComponentModel;
using oprot.plot.core;
using oprot.plot.wpf.ViewModels;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace oprot.plot.wpf
{
    /// <summary>
    /// Observable collection of ViewModels that pushes changes to a related collection of models
    /// </summary>
    /// <typeparam name="TViewModel">Type of ViewModels in collection</typeparam>
    /// <typeparam name="TModel">Type of models in underlying collection</typeparam>
    public class VmCollection<TViewModel, TModel> : ObservableCollection<TViewModel>
        where TViewModel : class, IViewModel<TModel>
        where TModel : class

    {
        private readonly object _context;
        private readonly ObservableCollection<TModel> _models;
        private bool _synchDisabled;
        private readonly IViewModelProvider _viewModelProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="models">List of models to synch with</param>
        /// <param name="viewModelProvider"></param>
        /// <param name="context"></param>
        /// <param name="autoFetch">
        /// Determines whether the collection of ViewModels should be
        /// fetched from the model collection on construction
        /// </param>
        public VmCollection(ObservableCollection<TModel> models, IViewModelProvider viewModelProvider, object context = null, bool autoFetch = true)
        {
            _models = models;
            _context = context;

            _viewModelProvider = viewModelProvider;

            // Register change handling for synchronization
            // from ViewModels to Models
            CollectionChanged += ViewModelCollectionChanged;

            // If model collection is observable register change
            // handling for synchronization from Models to ViewModels
            if (models is ObservableCollection<TModel>)
            {
                var observableModels = models as ObservableCollection<TModel>;
                observableModels.CollectionChanged += ModelCollectionChanged;
            }


            // Fecth ViewModels
            if (autoFetch) FetchFromModels();
        }

        /// <summary>
        /// CollectionChanged event of the ViewModelCollection
        /// </summary>
        public override sealed event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { base.CollectionChanged += value; }
            remove { base.CollectionChanged -= value; }
        }

        /// <summary>
        /// Load VM collection from model collection
        /// </summary>
        public void FetchFromModels()
        {
            // Deactivate change pushing
            _synchDisabled = true;

            // Clear collection
            Clear();

            // Create and add new VM for each model
            foreach (var model in _models)
                AddForModel(model);

            // Reactivate change pushing
            _synchDisabled = false;
        }

        private void ViewModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Return if synchronization is internally disabled
            if (_synchDisabled) return;

            // Disable synchronization
            _synchDisabled = true;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var m in e.NewItems.OfType<IViewModel<TModel>>().Select(v => v.Model).OfType<TModel>())
                        _models.Add(m);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var m in e.OldItems.OfType<IViewModel<TModel>>().Select(v => v.Model).OfType<TModel>())
                        _models.Remove(m);
                    break;
                case NotifyCollectionChangedAction.Move:
                    _models.Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _models.Clear();
                    foreach (var m in e.NewItems.OfType<IViewModel<TModel>>().Select(v => v.Model).OfType<TModel>())
                        _models.Add(m);
                    break;
            }

            //Enable synchronization
            _synchDisabled = false;
        }

        private void ModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_synchDisabled) return;
            _synchDisabled = true;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var m in e.NewItems.OfType<TModel>())
                        this.AddIfNotNull(CreateViewModel(m));
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (var m in e.OldItems.OfType<TModel>())
                        this.RemoveIfContains(GetViewModelOfModel(m));
                    break;
                case NotifyCollectionChangedAction.Move:
                    Move(e.OldStartingIndex, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Clear();
                    FetchFromModels();
                    break;
            }

            _synchDisabled = false;
        }

        private void RemoveIfContains(TViewModel viewModel)
        {
            if (Contains(viewModel))
            {
                Remove(viewModel);
            }
        }

        private void AddIfNotNull(TViewModel viewModel)
        {
            if (viewModel != null)
            {
                Add(viewModel);
            }
        }

        private TViewModel CreateViewModel(TModel model)
        {
            var vm = _viewModelProvider.GetFor<TViewModel>(model);
            if (vm == null)
                throw new Exception("wasn't expecting to land here!");
            return vm;
        }

        private TViewModel GetViewModelOfModel(TModel model)
        {
            return Items.OfType<IViewModel<TModel>>().FirstOrDefault(v => v.IsViewModelOf(model)) as TViewModel;
        }

        /// <summary>
        /// Adds a new ViewModel for the specified Model instance
        /// </summary>
        /// <param name="model">Model to create ViewModel for</param>
        public void AddForModel(TModel model)
        {
            Add(CreateViewModel(model));
        }

        /// <summary>
        /// Adds a new ViewModel with a new model instance of the specified type,
        /// which is the ModelType or derived from the Model type
        /// </summary>
        /// <typeparam name="TSpecificModel">Type of Model to add ViewModel for</typeparam>
        public void AddNew<TSpecificModel>() where TSpecificModel : TModel, new()
        {
            var m = new TSpecificModel();
            Add(CreateViewModel(m));
        }
    }
}

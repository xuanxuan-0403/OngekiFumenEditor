﻿using Caliburn.Micro;
using OngekiFumenEditor.Utils.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace OngekiFumenEditor.Utils
{
    public static class ViewCreateHelper
    {
        public static UIElement CreateViewByViewModelType<T>() where T : new() => CreateViewByViewModelType(() => new T());

        public static UIElement CreateViewByViewModelType<T>(Func<T> modelGenerator = null)
        {
            var viewModel = modelGenerator is null ? IoC.Get<T>() : modelGenerator();
            return CreateView(viewModel);
        }

        public static UIElement CreateView(object viewModel)
        {
            var view = (viewModel.GetType().GetCustomAttribute<MapToViewAttribute>() is MapToViewAttribute mtv ? Activator.CreateInstance(mtv.ViewType) : ViewLocator.LocateForModel(viewModel, null, null)) as UIElement;
            ViewModelBinder.Bind(viewModel, view, null);
            return view;
        }
    }
}

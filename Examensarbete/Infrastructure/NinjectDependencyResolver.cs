﻿using Domain.Abstract;
using Domain.Concrete;
using Ninject;
using Service.Implementation;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Examensarbete.Infrastructure
{
    public class NinjectDependencyResolver : IDependencyResolver
    {
        private IKernel kernel;
        public NinjectDependencyResolver(IKernel kernelParam)
        {
            kernel = kernelParam;
            AddBindings();
        }
        public object GetService(Type serviceType)
        {
            return kernel.TryGet(serviceType);
        }
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return kernel.GetAll(serviceType);
        }
        private void AddBindings()
        {            // put bindings here
            kernel.Bind<ICategoryRepository>().To<CategoryRepository>();
            kernel.Bind<IBookingRepository>().To<BookingRepository>();
            kernel.Bind<IBookingService>().To<BookingService>();
            kernel.Bind<ICategoryService>().To<CategoryService>();
        }
    }
}
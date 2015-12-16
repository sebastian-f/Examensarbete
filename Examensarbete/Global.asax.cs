using AutoMapper;
using Domain.Entities;
using Service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Examensarbete
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //TODO: Move this code!
            Mapper.CreateMap<Category, Models.CategoryModel>();
            Mapper.CreateMap<DatePrice, Models.DatePriceModel>();
            Mapper.CreateMap<Domain.Entities.Image, Models.Image>();

            //TODO:Move code to config file in Service layer
            Mapper.CreateMap<Category, Service.DTO.CategoryModel>();
            Mapper.CreateMap<DatePrice, Service.DTO.DatePriceModel>();
            Mapper.CreateMap<Image, Service.DTO.ImageModel>();
            Mapper.CreateMap<Service.DTO.ImageModel,Image>();
            Mapper.CreateMap<CategoryModel, Category>();
            Mapper.CreateMap<RoomModel, Room>();
            Mapper.CreateMap<Room, RoomModel>();
            Mapper.CreateMap<Booking, BookingModel>();
            Mapper.CreateMap<BookingModel, Booking>();

        }
    }
}

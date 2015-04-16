using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

using CMS;
using CMS.Core;
using CMS.DataEngine;
using CMS.Mvc;

[assembly: RegisterModule(typeof(CMS_MvcModule))]

namespace CMS.Mvc
{
    /// <summary>
    /// Represents the CMSApp_MVC web application module.
    /// </summary>
    public class CMS_MvcModule : Module
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public CMS_MvcModule()
            : base(new CMS_MvcModuleMetadata())
        {
        }


        /// <summary>
        /// Initializes the module.
        /// </summary>
        protected override void OnInit()
        {
            base.OnInit();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}
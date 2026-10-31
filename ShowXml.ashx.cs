using System;
using System.Web;
using DotNetNuke.Abstractions;
using DotNetNuke.Abstractions.Portals;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using BCC.Modules.BCCForms.Components;
using DotNetNuke.Security.Permissions;
using Microsoft.Extensions.DependencyInjection;

namespace BCC.Modules.BCCForms
{
    /// <summary>
    /// Summary description for ShowXml
    /// </summary>
    public class ShowXml : IHttpHandler
    {
        // resolved from the current request scope on each use (request-scoped service; see ModuleServices)
        private INavigationManager navigationManager => ModuleServices.Get<INavigationManager>();
        // resolved from the current request scope on each use (request-scoped service; see ModuleServices)
        private IPortalAliasService portalAliasService => ModuleServices.Get<IPortalAliasService>();


        public void ProcessRequest(HttpContext context)
        {
            try
            {
                PortalController.Instance.GetCurrentSettings();

                if ((context.Request.QueryString["tabid"] == null || context.Request.QueryString["mid"] == null) ||
                    !(context.Request.IsAuthenticated))
                {
                    return;
                }
                // get TabId
                var tabId = Convert.ToInt32(-1);
                if (context.Request.QueryString["tabid"] != null)
                {
                    tabId = int.Parse(context.Request.QueryString["tabid"]);
                }

                // get ModuleId
                var moduleId = Convert.ToInt32(-1);
                if (context.Request.QueryString["mid"] != null)
                {
                    moduleId = int.Parse(context.Request.QueryString["mid"]);
                }

                var userInfo = UserController.Instance.GetCurrentUserInfo();
                var mc = ModuleController.Instance;
               
                var moduleInfo = ModuleController.Instance.GetModule(moduleId, tabId, false);
                var settings = moduleInfo.ModuleSettings;

                if (ModulePermissionController.CanManageModule(moduleInfo))
                {
                    var udt = new UserDefinedTableController(moduleId, tabId, userInfo, this.navigationManager, this.portalAliasService);
                    var ds = udt.GetDataSet(true);

                    ds.Tables.Add(udt.Context(moduleInfo, userInfo,
                                                 context.Request[Definition.QueryStringParameter].AsString(),
                                                 settings[SettingName.SortField].AsString(),
                                                 settings[SettingName.SortOrder].AsString(),
                                                 settings[SettingName.Paging].AsString()));
                    context.Response.ContentType = "Text/Xml";
                    ds.WriteXml(context.Response.OutputStream);
                }
            }
            catch (Exception)
            {
                context.Response.Write("Not defined");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
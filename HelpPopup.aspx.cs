using System;
using System.Diagnostics;
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Framework;
using DotNetNuke.Services.Localization;

namespace BCC.Modules.BCCForms
{
    public partial class HelpPopup : PageBase
    {
        /// <summary>
        ///   Parameterless constructor, kept like DNN's own pages do: chains to the injected constructor with
        ///   nulls, and PageBase resolves any null service from DNN's container itself.
        /// </summary>
        public HelpPopup()
            : this(null, null, null, null)
        {
        }

        /// <summary>
        ///   Used when DNN 10 builds the page through dependency injection (HttpRuntime.WebObjectActivator).
        /// </summary>
        public HelpPopup(IPortalController portalController, IApplicationStatusInfo appStatus, IHostSettings hostSettings, IUserController userController)
            : base(portalController, appStatus, hostSettings, userController)
        {
        }

        #region  Vom Web Form Designer generierter Code

        [DebuggerStepThrough]
        void InitializeComponent()
        {
        }

        void Page_Init(object sender, EventArgs e)
        {
            InitializeComponent();
        }

        #endregion

        void Page_Load(object sender, EventArgs e)
        {
            var content = Request.QueryString["resourcekey"];
            if (content.ToLowerInvariant() == "help_hiddencolumns" || content.ToLowerInvariant() == "help_tokens_6")
            {
                Title = Localization.GetString(content + "_Title", LocalResourceFile);
                lblContent.Text = Localization.GetString(content + "_Body", LocalResourceFile);
            }
        }
    }
}
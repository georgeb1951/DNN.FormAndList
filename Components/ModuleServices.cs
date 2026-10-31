using System;
using System.Web;
using DotNetNuke.Abstractions.Application;
using DotNetNuke.Common;
using DotNetNuke.Common.Extensions;
using DotNetNuke.Entities.Portals;
using Microsoft.Extensions.DependencyInjection;

namespace BCC.Modules.BCCForms.Components
{
    /// <summary>
    ///   Resolves DNN services for code that DNN does not construct through dependency injection
    ///   (user controls hosted inside other controls, field-type objects, handlers).
    /// </summary>
    /// <remarks>
    ///   Uses the current request's DI scope via DNN's public <c>HttpContext.GetScope()</c> API.
    ///   Most of the services BCC Forms needs (INavigationManager, IPortalAliasService,
    ///   IApplicationStatusInfo, IEventLogger) are registered as *scoped*, so callers must
    ///   resolve them when they are used - never store them in an object that outlives the
    ///   request (e.g. the statically cached DataType instances).
    ///   Code that DNN builds through DI (BusinessController) takes its services through its
    ///   constructor instead.
    /// </remarks>
    internal static class ModuleServices
    {
        /// <summary>The current request's service provider, or null outside a web request.</summary>
        public static IServiceProvider Current
        {
            get
            {
                var context = HttpContext.Current;
                return context == null ? null : context.GetScope()?.ServiceProvider;
            }
        }

        static IHostSettings _rememberedHostSettings;

        /// <summary>
        ///   DNN's host settings. IHostSettings is a singleton, so unlike the request-scoped services it
        ///   is safe to remember: BusinessController (built by DI, also from the scheduler) hands it over,
        ///   which keeps this available outside a web request (e.g. during search indexing).
        /// </summary>
        public static IHostSettings HostSettings
        {
            get
            {
                var fromRequest = Current?.GetService<IHostSettings>();
                if (fromRequest != null)
                {
                    _rememberedHostSettings = fromRequest;
                    return fromRequest;
                }
                return _rememberedHostSettings ?? Get<IHostSettings>();
            }
        }

        internal static void RememberHostSettings(IHostSettings hostSettings)
        {
            if (hostSettings != null)
            {
                _rememberedHostSettings = hostSettings;
            }
        }

        /// <summary>
        ///   The current portal's settings - from the request, or the host portal's settings outside a
        ///   request. Same behaviour as the deprecated <c>Globals.GetPortalSettings()</c>.
        /// </summary>
        public static PortalSettings PortalSettings
        {
            get { return Globals.GetPortalSettings(HostSettings); }
        }

        /// <summary>Resolves a required service from the current request scope.</summary>
        /// <exception cref="InvalidOperationException">Called outside a web request.</exception>
        public static T Get<T>()
        {
            var provider = Current;
            if (provider == null)
            {
                throw new InvalidOperationException(string.Format(
                    "BCC Forms: {0} can only be resolved during a web request (no HttpContext / DI scope is available).",
                    typeof(T).Name));
            }
            return provider.GetRequiredService<T>();
        }

        /// <summary>
        ///   Creates an object from a type name (the replacement for the deprecated
        ///   <c>Reflection.CreateObject(typeName, cacheKey)</c>).
        /// </summary>
        /// <param name="typeName">Type name, resolved and cached by DNN's <c>Reflection.CreateType</c>.</param>
        /// <param name="useDependencyInjection">
        ///   True for objects that live for one request (edit controls, business controllers): they are
        ///   built with the request's DI container, so constructor injection works. False for objects
        ///   kept in a static cache (field types): they must never capture request-scoped services.
        /// </param>
        /// <exception cref="TypeLoadException">The type could not be found (DNN logs the details).</exception>
        public static object CreateByTypeName(string typeName, bool useDependencyInjection)
        {
            var type = DotNetNuke.Framework.Reflection.CreateType(typeName, typeName, true);
            if (type == null)
            {
                throw new TypeLoadException(string.Format("BCC Forms: type '{0}' could not be loaded.", typeName));
            }

            var provider = useDependencyInjection ? Current : null;
            return provider != null
                ? ActivatorUtilities.CreateInstance(provider, type)
                : Activator.CreateInstance(type);
        }
    }
}

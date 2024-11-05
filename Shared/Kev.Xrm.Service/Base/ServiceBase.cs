using Microsoft.Xrm.Sdk;
using System;
using System.Diagnostics;

namespace Kev.Xrm.Service.Base
{
    public abstract class ServiceBase
    {
        /// <summary>
        /// Service de trace
        /// </summary>
        private readonly ITracingService _tracingService;

        protected ServiceBase(IOrganizationService adminService, IOrganizationService userService, ITracingService tracingService = null)
        {
            AdminService = adminService;
            UserService = userService;

            this._tracingService = tracingService;
        }

        /// <summary>
        /// Service d'organisation avec le compte SYSTEM
        /// </summary>
        /// <remarks>
        /// A ne pas utiliser pour des événements gérant les activités comme le
        /// message SendEmailRequest
        /// </remarks>
        protected IOrganizationService AdminService { get; }

        /// <summary>
        /// Service d'organisation pour le compte utilisateur courant
        /// </summary>
        protected IOrganizationService UserService { get; }

        /// <summary>
        /// Ajoute un message dans la trace
        /// </summary>
        /// <param name="message">Message</param>
        protected void Trace(string message)
        {
            _tracingService?.Trace(message);

#if DEBUG
            Console.WriteLine(message);
#endif
        }

        protected void TraceMethodEnd(string message)
        {
            Trace(message);

            StackTrace stackTrace = new StackTrace();
            Trace($"End - {stackTrace.GetFrame(1).GetMethod().Name}");
        }

        protected void TraceMethodEnd()
        {
            StackTrace stackTrace = new StackTrace();
            Trace($"End - {stackTrace.GetFrame(1).GetMethod().Name}");
        }

        protected void TraceMethodStart()
        {
            StackTrace stackTrace = new StackTrace();
            Trace($"Start - {stackTrace.GetFrame(1).GetMethod().Name}");
        }
    }
}
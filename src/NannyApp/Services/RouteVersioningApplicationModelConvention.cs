using Microsoft.AspNet.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NannyApp.Services
{
    public class RouteVersioningApplicationModelConvention : IApplicationModelConvention
    {
        private const string VersionToken = "[version]";
        private readonly int _maxVersionNumber;

        public RouteVersioningApplicationModelConvention(int maxVersionNumber)
        {
            _maxVersionNumber = maxVersionNumber;
        }

        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                var routeTemplate = string.Empty;

                var controllerRoute = controller.AttributeRoutes.SingleOrDefault();
                if (controllerRoute != null)
                {
                    routeTemplate = string.Concat(routeTemplate, controllerRoute.Template);
                }

                controller.AttributeRoutes.Clear();
                VersionControllerActions(controller, routeTemplate);
            }
        }

        private void VersionControllerActions(ControllerModel controller, string routeTemplate)
        {
            var newActions = CreateVersionedActions(controller, routeTemplate);

            controller.Actions.Clear();
            foreach (var newAction in newActions)
            {
                controller.Actions.Add(newAction);
            }
        }

        private IEnumerable<ActionModel> CreateVersionedActions(ControllerModel controller, string routeTemplate)
        {
            var newActions = new List<ActionModel>();
            foreach (var action in controller.Actions)
            {
                // Empty route needs to be versioned as well.
                if (action.AttributeRouteModel == null)
                {
                    action.AttributeRouteModel = new AttributeRouteModel();
                }

                var minVersion = 1;
                var maxVersion = _maxVersionNumber;

                var actionRouteAttribute = action.AttributeRouteModel.Attribute as VersionedRouteAttribute;
                if (actionRouteAttribute != null)
                {
                    minVersion = actionRouteAttribute.MinVersion;

                    if (actionRouteAttribute.MaxVersion.HasValue)
                    {
                        maxVersion = actionRouteAttribute.MaxVersion.Value;
                    }
                }

                for (var version = minVersion; version <= maxVersion; version++)
                {
                    var newAction = CreateVersionedAction(action, routeTemplate, version);
                    newActions.Add(newAction);
                }
            }

            return newActions;
        }

        private static ActionModel CreateVersionedAction(ActionModel action, string routeTemplate, int version)
        {
            var newAction = new ActionModel(action);
            var versionedRoute = routeTemplate.Replace(VersionToken, version.ToString());

            newAction.AttributeRouteModel.Template =
                $"{versionedRoute}/{newAction.AttributeRouteModel.Template}";

            return newAction;
        }
    }
}

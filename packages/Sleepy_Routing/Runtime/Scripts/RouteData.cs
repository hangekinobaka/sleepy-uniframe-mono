using System.Collections.Generic;
using UnityEngine;

namespace Sleepy.Routing
{
    #region For Route Setting

    [System.Serializable]
    public class Route
    {
        public string name = "";
        public string path = "";

        public Route() { }
        public Route(Route route)
        {
            name = route.name;
            path = route.path;
        }
    }

    [System.Serializable]
    public class RouteList
    {
        public List<Route> routes = new List<Route>();
    }

    public enum RouteListCheckError
    {
        EmptyEntry,
        RepeatName
    }

    public class RouteGroup
    {
        public string Prefix { get; set; }
        public List<Route> Routes { get; set; }

        public RouteGroup(string prefix)
        {
            Prefix = prefix;
            Routes = new List<Route>();
        }
    }

    #endregion

    #region For Routing (active and deactive)

    [System.Serializable]
    public class RoutePage
    {
        public string routeName;
        public RouteDetail detail;
        public Page page;
        public List<GameObject> objs;
    }

    [System.Serializable]
    public class RouteDetail
    {
        public string routeName;
        public string sceneName;
        public string hierarchyPath;
        public string groupName;
    }

    public class RouteDetailedGroup
    {
        public string Prefix { get; set; }
        public Dictionary<string, RouteDetail> Routes { get; set; }

        public RouteDetailedGroup(string prefix)
        {
            Prefix = prefix;
            Routes = new Dictionary<string, RouteDetail>();
        }
    }

    #endregion
}
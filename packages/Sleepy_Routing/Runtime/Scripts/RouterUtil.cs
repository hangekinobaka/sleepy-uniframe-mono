using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Sleepy.Routing
{
    public static class RouterUtil
    {
        #region Load & Save Route Config

        public static bool TryLoadRoutes(out RouteList result)
        {
            return CommonUtil.TryLoadJSON<RouteList>(RouterPath.ROUTE_RESOURCE_PATH, out result, false);
        }

        public static void SaveRoutes(RouteList routes)
        {
            CommonUtil.SaveJSON(RouterPath.ROUTE_JSON_PATH, routes);
        }

        public static Dictionary<string, RouteGroup> GroupRoutesByPrefix(IEnumerable<Route> routes)
        {
            var groups = new Dictionary<string, RouteGroup>();

            foreach (var route in routes)
            {
                // 使用Split方法分割路径，并取前两部分作为分组的键
                var parts = route.path.Split('/');
                if (parts.Length > 1) // 确保路径至少有两部分(到 Canvas 为止)
                {
                    // 将前两部分用'/'重新连接，得到分组的键
                    var prefix = string.Join("/", parts.Take(2));

                    if (!groups.ContainsKey(prefix))
                    {
                        groups[prefix] = new RouteGroup(prefix);
                    }

                    groups[prefix].Routes.Add(route);
                }
                else
                {
                    // 如果路径没有'/'或只有一个'/'，则将其视为其它组(一般不会有这种情况)
                    if (!groups.ContainsKey("."))
                    {
                        groups[route.path] = new RouteGroup(".");
                    }

                    groups[route.path].Routes.Add(route);
                }
            }

            return groups;
        }

        #endregion

        #region Route Handling

        /// <summary>
        /// Route List 不可以有空条目或者重复Name! <br/>
        /// Empty entry and repeated name are not allowed in the Route List!
        /// </summary>
        /// <param name="routeList"></param>
        /// <param name="errorMap">A map tells you which route has what error</param>
        /// <returns></returns>
        public static bool ValidateRouteList(RouteList routeList, ref Dictionary<Route, RouteListCheckError> errorMap)
        {
            errorMap.Clear();
            bool passCheck = true;

            // 检查空条目
            // Check empty entry
            foreach (var route in routeList.routes)
            {
                if (string.IsNullOrWhiteSpace(route.name) || string.IsNullOrWhiteSpace(route.path))
                {
                    errorMap[route] = RouteListCheckError.EmptyEntry;
                    passCheck = false;
                }
            }

            // 检查重复的name
            // Check repeated name
            var nameGroup = routeList.routes.GroupBy(r => r.name);
            foreach (var group in nameGroup)
            {
                if (group.Count() > 1)
                {
                    foreach (var route in group)
                    {
                        errorMap[route] = RouteListCheckError.RepeatName;
                    }
                    passCheck = false;
                }
            }

            return passCheck;
        }
        /// <summary>
        /// Route List 不可以有空条目或者重复Name! <br/>
        /// Empty entry and repeated name are not allowed in the Route List!
        /// </summary>
        /// <param name="routeList"></param>
        /// <returns></returns>
        public static bool ValidateRouteList(RouteList routeList)
        {
            bool passCheck = true;

            // 检查空条目
            // Check empty entry
            foreach (var route in routeList.routes)
            {
                if (string.IsNullOrWhiteSpace(route.name) || string.IsNullOrWhiteSpace(route.path))
                {
                    passCheck = false;
                }
            }

            // 检查重复的name
            // Check repeated name
            var nameGroup = routeList.routes.GroupBy(r => r.name);
            foreach (var group in nameGroup)
            {
                if (group.Count() > 1)
                {
                    passCheck = false;
                }
            }

            return passCheck;
        }

        /// <summary>
        /// 尝试从提供的路径中解析出场景名称和层级路径。<br/>
        /// Tries to parse the scene name and hierarchy path from the provided path.
        /// </summary>
        /// <param name="route">包含Name和Path。Contains Name and Path.</param>
        /// <param name="detail">如果解析成功，返回包含场景名称和层级路径的RouteDetail对象。If parsing is successful, returns a RouteDetail object containing scene name and hierarchy path.</param>
        /// <returns>Returns true if the path is valid and successfully parsed; otherwise, false.</returns>
        public static bool TryGetRouteDetail(Route route, out RouteDetail detail)
        {
            detail = new RouteDetail();
            detail.routeName = route.name;

            var parts = route.path.Split('/');
            if (parts.Length < 2)
            {
                Dev.Error($"Route path is not correct: {route.path}");
                return false;
            }

            detail.sceneName = parts[0];
            detail.hierarchyPath = string.Join("/", parts.Skip(1));
            return true;
        }

        #endregion

        #region Get & Set Path

        /// <summary>
        /// 尝试根据给定路径和场景名内部设置路径对象的激活状态。<br/>
        /// Tries to internally set the active state of path objects based on the given path and scene name.
        /// </summary>
        /// <param name="path">对象的路径。/ The object's path.</param>
        /// <param name="sceneName">场景名。/ The scene name.</param>
        /// <param name="gos">找到的游戏对象列表。/ The list of found game objects.</param>
        /// <param name="isActive">要设置的激活状态，可以为空。/ The active state to set, can be null.</param>
        /// <returns>如果成功找到并设置了对象，则为 true；否则为 false。/ True if objects were successfully found and set; otherwise, false.</returns>
        private static bool TrySetPathActiveInternal(string path, string sceneName, out List<GameObject> gos, bool? isActive = null)
        {
            gos = new List<GameObject>();

            path = path.Trim();
            if (string.IsNullOrWhiteSpace(path))
            {
                Dev.Error("Path error: path is null or whitespace.");
                return false;
            }

            string[] parts = path.Split('/');
            GameObject current = null;

            foreach (string part in parts)
            {
                if (current == null)
                {
                    // 从最顶层的对象开始查找 / Start searching from the top-level object
                    Scene targetScene = SceneManager.GetSceneByName(sceneName);
                    GameObject[] rootObjects = targetScene.GetRootGameObjects();
                    current = rootObjects.FirstOrDefault(obj => obj.name == part);

                    if (current == null)
                    {
                        Dev.Error($"Path error: cannot find '{part}'");
                        return false; // 顶层对象都找不到，直接返回错误 / If the top-level object is not found, return an error immediately
                    }
                }
                else
                {
                    // 在当前对象的子对象中查找 / Search in the child objects of the current object
                    Transform child = current.transform.Find(part);

                    if (child != null)
                    {
                        current = child.gameObject;
                    }
                    else
                    {
                        Dev.Error($"Path error: cannot find '{part}'");
                        return false; // 子对象找不到，返回错误 / If a child object is not found, return an error
                    }
                }

                // 添加到物件列表
                gos.Add(current);

                // 激活/隐藏找到的对象 / Activate or deactivate the found object
                if (isActive != null) current.SetActive(isActive.Value);
            }

            return true;
        }

        /// <summary>
        /// 尝试根据给定路径和场景名设置路径对象的激活状态，并返回最后一个目标对象。<br/>
        /// Tries to set the active state of path objects based on the given path and scene name, and returns the last target object.
        /// </summary>
        /// <param name="path">对象的路径。/ The object's path.</param>
        /// <param name="sceneName">场景名。/ The scene name.</param>
        /// <param name="isActive">要设置的激活状态。/ The active state to set.</param>
        /// <param name="lastTarget">最后一个找到的目标对象。/ The last found target object.</param>
        /// <returns>如果成功设置了对象的激活状态，则为 true；否则为 false。/ True if the active state of the objects was successfully set; otherwise, false.</returns>
        public static bool TrySetPathActive(string path, string sceneName, bool isActive, out GameObject lastTarget)
        {
            lastTarget = null;

            if (TrySetPathActiveInternal(path, sceneName, out List<GameObject> gos, isActive))
            {
                //Dev.Log($"All objects for '{path}' have been set to {isActive}.");
                lastTarget = gos[gos.Count - 1];
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试根据给定路径和场景名设置路径对象的激活状态。<br/>
        /// Tries to set the active state of path objects based on the given path and scene name.
        /// </summary>
        /// <param name="path">对象的路径。/ The object's path.</param>
        /// <param name="sceneName">场景名。/ The scene name.</param>
        /// <param name="isActive">要设置的激活状态。/ The active state to set.</param>
        /// <returns>如果成功设置了对象的激活状态，则为 true；否则为 false。/ True if the active state of the objects was successfully set; otherwise, false.</returns>
        public static bool TrySetPathActive(string path, string sceneName, bool isActive)
        {
            return TrySetPathActive(path, sceneName, isActive, out GameObject lastTarget);
        }

        /// <summary>
        /// 尝试获取给定路径和场景名中的所有路径对象。<br/>
        /// Tries to get all path objects in the given path and scene name.
        /// </summary>
        /// <param name="path">对象的路径。/ The object's path.</param>
        /// <param name="sceneName">场景名。/ The scene name.</param>
        /// <param name="gos">找到的游戏对象列表。/ The list of found game objects.</param>
        /// <returns>如果成功找到对象，则为 true；否则为 false。/ True if objects were successfully found; otherwise, false.</returns>
        public static bool TryGetPathObjects(string path, string sceneName, out List<GameObject> gos)
        {
            gos = new List<GameObject>();

            if (TrySetPathActiveInternal(path, sceneName, out gos))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 尝试获取给定路径和场景名中的最后一个路径对象。<br/>
        /// Tries to get the last path object in the given path and scene name.
        /// </summary>
        /// <param name="path">对象的路径。/ The object's path.</param>
        /// <param name="sceneName">场景名。/ The scene name.</param>
        /// <param name="lastTarget">找到的最后一个游戏对象。/ The last found game object.</param>
        /// <returns>如果成功找到最后一个对象，则为 true；否则为 false。/ True if the last object was successfully found; otherwise, false.</returns>
        public static bool TryGetPathLastObject(string path, string sceneName, out GameObject lastTarget)
        {
            lastTarget = null;

            if (TrySetPathActiveInternal(path, sceneName, out List<GameObject> gos))
            {
                lastTarget = gos[gos.Count - 1];
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检查给定路径和场景名中的最后一个路径对象是否处于激活状态。<br/>
        /// Checks if the last path object in the given path and scene name is active.
        /// </summary>
        /// <param name="path">对象的路径。/ The object's path.</param>
        /// <param name="sceneName">场景名。/ The scene name.</param>
        /// <returns>如果最后一个对象处于激活状态，则为 true；否则为 false。/ True if the last object is active; otherwise, false.</returns>
        public static bool CheckPathLastObjectActive(string path, string sceneName)
        {
            if (TrySetPathActiveInternal(path, sceneName, out List<GameObject> gos))
            {
                return gos[gos.Count - 1].activeSelf;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sleepy.Loading;
using UnityEngine;

namespace Sleepy.Routing
{
    public static class Router
    {
        /** Local **/
        static Dictionary<string, RouteDetail> _routes = new Dictionary<string, RouteDetail>();
        static Dictionary<string, RouteDetailedGroup> _routesGroups = new Dictionary<string, RouteDetailedGroup>();
        static Dictionary<string, RoutePage> _routePages = new Dictionary<string, RoutePage>();

        // 页面栈，用于管理页面的导航和回退 / Page stack for managing page navigation and back navigation
        static Stack<RoutePage> _pageStack = new Stack<RoutePage>();
        // 当前激活中的页面（以最后一个打开的页面为准） / The current active page(the last page opened)
        static RoutePage _curActiveRoutePage;

        #region Route prepare

        /// <summary>
        /// 在场景加载前初始化路由。<br/>
        /// Initializes routes before the scene loads.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Initialize()
        {
            LoadRoutes();
        }

        /// <summary>
        /// 载入并解析路由配置。<br/>
        /// Loads and parses route configurations.
        /// </summary>
        internal static void LoadRoutes()
        {
            // 首先尝试读取路由配置文件 / First, try to read the route configuration file
            if (RouterUtil.TryLoadRoutes(out RouteList routeList))
            {
                // 如果路由配置文件存在错误，则不继续执行 / If there are errors in the route configuration file, do not proceed
                if (!RouterUtil.ValidateRouteList(routeList))
                {
                    Dev.Warning($"There are errors in your route file - {RouterPath.ROUTE_JSON_PATH}!");
                    return;
                }

                // 将所有路由信息缓存至本地 / Cache all route information locally
                _routes.Clear();
                _routesGroups.Clear();

                // 获取组信息 / Get route group info
                var groups = RouterUtil.GroupRoutesByPrefix(routeList.routes);
                foreach (var groupPair in groups)
                {
                    _routesGroups.Add(groupPair.Key, new RouteDetailedGroup(groupPair.Key));
                    foreach (var route in groupPair.Value.Routes)
                    {
                        // Get details
                        if (!RouterUtil.TryGetRouteDetail(route, out RouteDetail detail)) continue;
                        detail.groupName = groupPair.Key;

                        // Add to maps
                        _routes.Add(detail.routeName, detail);
                        _routesGroups[groupPair.Key].Routes.Add(detail.routeName, detail);
                    }
                }
            }
            else
            {
                Dev.Error($"Load route fail. Please make sure you have a valid route file in {RouterPath.ROUTE_JSON_PATH}");
            }
        }

        #endregion

        #region PUBLIC FUNC

        /// <summary>
        /// 加载指定路由的页面<br/>
        /// Lloads a page for the specified route
        /// </summary>
        /// <param name="routeName">路由名称，用于识别加载哪个页面。<br/>The name of the route, used to identify which page to load.</param>
        /// <param name="dataTransferObject">可选参数，用于页面间数据传递。<br/>Optional parameter for data transfer between pages.</param>
        /// <param name="progress">用于报告页面加载进度的 IProgress 实例。<br/>An IProgress instance for reporting the progress of the page loading.</param>
        public async static UniTask LoadPage(string routeName, object dataTransferObject = null, IProgress<float> progress = null)
        {
            // 检查路由映射表中是否包含指定路由名称 / Check if the route map contains the specified route name
            if (!_routes.ContainsKey(routeName))
            {
                Dev.Error($"{routeName} is not existed in our route map.");
                return;
            }

            // 获取页面的详细路由信息 / Get the detailed route information for the page
            RouteDetail detail = _routes[routeName];

            // 如果目标场景尚未加载，则开始加载 / If the target scene is not yet loaded, start loading
            if (!SceneDirector.IsSceneActive(detail.sceneName))
            {
                await SceneDirector.LoadSceneAsync(detail.sceneName, UnityEngine.SceneManagement.LoadSceneMode.Additive, progress);
                progress?.Report(1);
                await UniTask.Yield();
            }

            // 尝试获取这个 Gameobject 和其组内所有 Gameobject 的信息
            // Try to get this Gameobject and all of its group member's Gameobject info.
            var group = _routesGroups[detail.groupName];
            foreach (var gr in group.Routes)
            {
                if (!_routePages.ContainsKey(gr.Key) || _routePages[gr.Key].page == null)
                {
                    if (!RouterUtil.TryGetPathObjects(gr.Value.hierarchyPath, gr.Value.sceneName, out List<GameObject> objs))
                    {
                        Dev.Error($"{gr.Value.routeName} cannot be found in the scene");
                        return;
                    }

                    // 获取页面组件 / Get the page component
                    if (!objs[objs.Count - 1].TryGetComponent<Page>(out Page page))
                    {
                        Dev.Error($"You forgot to attach the page comp to the {routeName}!");
                        return;
                    }

                    // 缓存页面信息 / Cache the page 
                    RoutePage rp = new RoutePage()
                    {
                        routeName = gr.Value.routeName,
                        objs = objs,
                        page = page,
                        detail = detail
                    };
                    if (_routePages.ContainsKey(gr.Key)) _routePages[gr.Key] = rp;
                    else _routePages.Add(gr.Value.routeName, rp);

                    await page.OnInitBase(gr.Value.routeName);
                }
            }

            RoutePage routePage = _routePages[routeName];
            List<GameObject> gos = routePage.objs;

            if (routePage.page.IsActive) return;

            // 缓存当前活跃页面记录 / Cache the current active page 
            _curActiveRoutePage = routePage;

            // 激活相应生命周期回调 / Call lifecycle hooks
            await routePage.page.OnEnterBase(dataTransferObject);
            await routePage.page.OnResumeBase();

            // 激活对应页面的GameObject / Activate the GameObject for the corresponding page
            foreach (var go in gos)
            {
                go.SetActive(true);
            }
        }

        /// <summary>
        /// 切换到指定路由的页面。注意！这个方法只能用作切换当前活跃页面，最佳用途是全屏页面。本方法自动记录页面历史。<br/>
        /// Switches to a page for the specified route. Note! that this can only switch the current active page. This function will hold the page history automatically.
        /// </summary>
        /// <param name="routeName">目标页面的路由名称。<br/>The route name of the target page.</param>
        /// <param name="dataTransferObject">可选参数，用于页面间数据传递。<br/>Optional parameter for data transfer between pages.</param>
        /// <param name="progress">用于报告页面切换进度的 IProgress 实例。<br/>An IProgress instance for reporting the progress of switching pages.</param>
        public async static UniTask SwitchPage(string routeName, object dataTransferObject = null, IProgress<float> progress = null)
        {
            if (_curActiveRoutePage != null)
            {
                // 暂停当前页面 / Pause the current page
                await ExitPage(_curActiveRoutePage.routeName, true);
            }
            else
            {
                Dev.Warning("There is no current active page!");
            }
            // 开启下一个页面// Load the new page
            await LoadPage(routeName, dataTransferObject, progress);
        }

        /// <summary>
        /// 清除历史记录堆栈。<br/>
        /// Clears the history stack.
        /// </summary>
        public static void ClearHistory()
        {
            _pageStack.Clear();
        }

        /// <summary>
        /// 返回历史堆里的上一个页面。<br/>
        /// Goes back to the previous page that exist in the history stack.
        /// </summary>
        /// <param name="closeCurrent">同时关闭现在激活的页面 / Close the cur active page meanwhile.</param>
        /// <param name="progress">加载进度的回调。/ Callback for load progress.</param>
        public static async UniTask GoBack(bool closeCurrent = false, IProgress<float> progress = null)
        {
            //Dev.Log($"{_pageStack.Count} pages in the history");
            // Try to close current page 
            if (closeCurrent)
            {
                if (_curActiveRoutePage != null)
                {
                    await ExitPage(_curActiveRoutePage.routeName);
                }
                else
                {
                    Dev.Warning("No cur page...");
                }
            }

            if (_pageStack.Count >= 1)
            {
                // 显示上一个页面 / Display the previous page
                RoutePage preRoutePage = _pageStack.Pop();
                await LoadPage(preRoutePage.routeName, null, progress);
            }
            else
            {
                Dev.Warning("No prev page...");
            }
        }

        /// <summary>
        /// 异步退出指定的页面，可选地关闭其所在场景。<br/>
        /// Asynchronously exits the specified page, with an option to close its scene.
        /// </summary>
        /// <param name="pageName">要退出的页面名称。<br/>The name of the page to exit.</param>
        /// <param name="pushToHistory">是否将页面推入历史记录堆栈。<br/>Whether to push the page onto the history stack.</param>
        /// <param name="closeScene">是否在退出页面后关闭其所在的场景。<br/>Whether to close the scene in which the page resides after exiting the page.</param>
        public static async UniTask ExitPage(string pageName, bool pushToHistory = false, bool closeScene = false)
        {
            // 检查路由映射表中是否包含指定路由名称 / Check if the route map contains the specified route name
            if (!_routes.ContainsKey(pageName) || !_routePages.ContainsKey(pageName))
            {
                Dev.Error($"{pageName} is not existed in our route maps.");
                return;
            }
            RoutePage routePage = _routePages[pageName];

            // 如果需要推入历史的话便推入历史 / Push to the history stack if needed
            if (pushToHistory)
            {
                _pageStack.Push(routePage);
            }

            // 等待页面暂停基础任务完成 / Wait for the page pause base task to complete
            await _routePages[pageName].page.OnExitBase();

            // 尝试关闭页面的GameObject / Attempt to deactivate the GameObject for the page
            routePage.page.gameObject.SetActive(false);

            // 如果选择同时关闭场景则检查当前组是否都被关闭了，如果是的话就关闭场景
            // If opting to close the scene, check if all pages in the current group are closed; if so, then close the scene
            if (closeScene)
            {
                foreach (var route in _routesGroups[routePage.detail.groupName].Routes)
                {
                    if (IsPageActive(route.Value.routeName)) return; // 如果任何页面仍然激活，则不关闭场景 / If any page is still active, do not close the scene
                }
                // 关闭场景 / Close the scene
                if (SceneDirector.IsSceneActive(routePage.detail.sceneName))
                {
                    await SceneDirector.UnloadSceneAsync(routePage.detail.sceneName);
                }
            }
        }

        /// <summary>
        /// 检查指定页面是否处于激活状态。<br/>
        /// Checks if the specified page is active.
        /// </summary>
        /// <param name="pageName">页面名称，用于标识要检查的页面。<br/>The name of the page to check for activity.</param>
        /// <returns>如果页面已激活则返回 true，否则返回 false。<br/>Returns true if the page is active, otherwise returns false.</returns>
        public static bool IsPageActive(string pageName)
        {
            // 如果map里找不到这个page说明还没有初始化，直接返回false / If the map does not contain this page, it means it's not initialized, so return false
            if (!_routePages.ContainsKey(pageName))
            {
                return false;
            }

            // 检查这个page的active property / Check the active property of this page
            return _routePages[pageName].page.IsActive;
        }

        #endregion

    }
}
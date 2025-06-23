#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Sleepy.Routing
{

    public class RouteConfigWindow : EditorWindow
    {
        public static void ShowWindow()
        {
            var window = GetWindow<RouteConfigWindow>("Route Config");
            window.LoadRoutes(); // 打开窗口时加载路由信息; Load the route info when the window is opened
        }

        /** Const **/
        readonly Dictionary<RouteListCheckError, string> ROUTE_ERROR_MSG_MAP = new Dictionary<RouteListCheckError, string>() {
            { RouteListCheckError.EmptyEntry, "Name and path cannot be empty."},
            {RouteListCheckError.RepeatName, "Name must be unique." }
        };

        /** Textures **/
        Texture2D _deleteIcon;

        /** UI **/
        Vector2 _scrollPosition;

        /** Local **/
        RouteList _routeList;
        Route _editingRoute;
        Dictionary<Route, RouteListCheckError> _routeErrors = new Dictionary<Route, RouteListCheckError>(); // Store error message
        Dictionary<string, bool> _groupExpandedStates = new Dictionary<string, bool>();

        private void Awake()
        {
            _deleteIcon = EditorCommonUtil.LoadDeleteIcon();
            Dev.Log(_deleteIcon);
        }

        void OnGUI()
        {
            // 设置界面的边距 / Set margins for the UI
            GUILayout.Space(10);

            // 开始绘制保存按钮区域 / Start drawing the save button area
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            // 设置按钮的颜色为绿色 / Set the button color to green
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Save"))
            {
                SaveRoutes(); // 调用保存路由的方法 / Invoke the method to save routes
            }
            // 按钮颜色重置为默认值 / Reset button color to default
            GUI.backgroundColor = Color.white;
            GUILayout.FlexibleSpace(); // 在按钮后添加弹性空间以实现左对齐 / Add flexible space after button to align left
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            // 检查路由列表是否有内容 / Check if the route list has any items
            if (_routeList.routes.Count > 0)
            {
                List<Route> routesToRemove = new List<Route>(); // 存储需要移除的路由 / Store routes to be removed

                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition); // 开始滚动视图 / Begin scroll view
                                                                              // 对路由进行分组并显示 / Group routes and display them
                var groupedRoutes = RouterUtil.GroupRoutesByPrefix(_routeList.routes).Values;
                foreach (var group in groupedRoutes)
                {
                    // 如果分组的展开状态未在字典中定义，则默认展开 / Default to expanded if group's expanded state is not defined in dictionary
                    if (!_groupExpandedStates.ContainsKey(group.Prefix))
                    {
                        _groupExpandedStates[group.Prefix] = true;
                    }

                    // 使用字典中的值决定Foldout是否展开 / Use value from dictionary to decide if Foldout is expanded
                    _groupExpandedStates[group.Prefix] = EditorGUILayout.Foldout(_groupExpandedStates[group.Prefix], $"{group.Prefix}");

                    if (_groupExpandedStates[group.Prefix])
                    {
                        // 显示每个组内的路由信息 / Display route information within each group
                        foreach (var route in group.Routes)
                        {
                            GUILayout.BeginHorizontal();
                            GUILayout.Space(20); // 增加左边距，使内容离窗口边缘有一定距离 / Add left margin to distance content from window edge

                            // 自动扩展标签以填满可用空间 / Auto-expand label to fill available space
                            GUILayout.Label($"{route.name}: {route.path}", GUILayout.ExpandWidth(true));

                            // 为编辑和删除按钮添加空间，使界面更清晰 / Add space for edit and delete buttons for clearer interface
                            GUILayout.Space(10);

                            if (GUILayout.Button("Edit", GUILayout.Width(50), GUILayout.Height(25)))
                            {
                                ShowEditRouteDialog(route); // 显示编辑路由对话框 / Show edit route dialog
                            }
                            if (GUILayout.Button(new GUIContent(_deleteIcon, "Delete"), GUILayout.Width(25), GUILayout.Height(25)))
                            {
                                routesToRemove.Add(route); // 添加到待移除路由列表 / Add to list of routes to be removed
                            }
                            GUILayout.Space(10);
                            GUILayout.EndHorizontal();

                            // 显示路由错误信息 / Display route error messages
                            if (_routeErrors.TryGetValue(route, out RouteListCheckError error))
                            {
                                EditorGUILayout.HelpBox(ROUTE_ERROR_MSG_MAP[error], MessageType.Error);
                            }
                        }
                    }
                }
                GUILayout.EndScrollView();

                // 处理待移除的路由 / Handle routes to be removed
                foreach (var route in routesToRemove)
                {
                    _routeList.routes.Remove(route); // 从路由列表中移除 / Remove from route list
                    RouterUtil.ValidateRouteList(_routeList, ref _routeErrors); // 验证更新后的路由列表 / Validate updated route list
                }
            }

            // 开始绘制添加新路由按钮区域 / Start drawing the add new route button area
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            if (GUILayout.Button("Add a New Route"))
            {
                ShowEditRouteDialog(new Route(), true); // 显示编辑新路由对话框 / Show edit dialog for new route
            }
            GUILayout.FlexibleSpace(); // 在按钮后添加弹性空间以实现左对齐 / Add flexible space after button to align left
            GUILayout.Space(10);
            GUILayout.EndHorizontal();

            // 设置界面底部边距 / Set bottom margin for the UI
            GUILayout.Space(20);
        }


        void LoadRoutes()
        {
            if (!RouterUtil.TryLoadRoutes(out _routeList))
            {
                Dev.Log("<color=#FF7226>There is no valid route list file at the moment.</color>");
                _routeList = new RouteList();
            }
            else
            {
                Dev.Log("Found the route list file, render it now...");
                RouterUtil.ValidateRouteList(_routeList, ref _routeErrors);
            }
        }

        void SaveRoutes()
        {
            if (RouterUtil.ValidateRouteList(_routeList, ref _routeErrors))
            {
                RouterUtil.SaveRoutes(_routeList);
            }
        }

        private void ShowEditRouteDialog(Route route, bool isAdding = false)
        {
            _editingRoute = route; // 设置正在编辑的路由 / Set the route being edited

            EditRouteDialog.ShowWindow(route, OnRouteEdited, isAdding);
        }

        private void OnRouteEdited(Route editedRoute, bool isAdding = false)
        {
            _editingRoute.name = editedRoute.name; // 更新路由名称 / Update route name
            _editingRoute.path = editedRoute.path; // 更新路由路径 / Update route path

            if (isAdding)
            {
                _routeList.routes.Add(_editingRoute);
            }

            RouterUtil.ValidateRouteList(_routeList, ref _routeErrors); // 验证更新后的路由列表 / Validate the updated route list
            Repaint(); // 重绘编辑器窗口以显示更新后的路由列表 / Repaint the editor window to show the updated route list
        }

        private class EditRouteDialog : EditorWindow
        {
            private Route _currentRoute;
            private UnityAction<Route, bool> _onRouteEdited; // 路由编辑完成时的回调函数 / Callback function for when the route is edited
            private bool _isAdding;

            /// <summary>
            /// Use this function to start an edit window
            /// </summary>
            /// <param name="route"></param>
            /// <param name="onEdited"></param>
            /// <param name="isAdding"></param>
            public static void ShowWindow(Route route, UnityAction<Route, bool> onEdited, bool isAdding = false)
            {
                EditRouteDialog window = (EditRouteDialog)GetWindow(typeof(EditRouteDialog), true, "Edit Route");
                window.Init(route, onEdited, isAdding);
                window.ShowUtility();
            }

            public void Init(Route route, UnityAction<Route, bool> onEdited, bool isAdding = false)
            {
                _currentRoute = route;
                _onRouteEdited = onEdited;
                _isAdding = isAdding;
            }

            void OnGUI()
            {
                EditorGUILayout.Space();

                _currentRoute.name = EditorGUILayout.TextField("Name:", _currentRoute.name);
                _currentRoute.path = EditorGUILayout.TextField("Path:", _currentRoute.path);

                EditorGUILayout.Space();

                // 确定按钮 / OK button
                if (GUILayout.Button("OK"))
                {
                    if (!string.IsNullOrEmpty(_currentRoute.name) && !string.IsNullOrEmpty(_currentRoute.path))
                    {
                        _onRouteEdited?.Invoke(_currentRoute, _isAdding); // 调用回调函数并关闭窗口 / Invoke callback function and close window
                        this.Close();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error", "Name and path cannot be empty.", "OK"); // 错误提示 / Error message
                    }
                }
                // 取消按钮 / Cancel button
                if (GUILayout.Button("Cancel"))
                {
                    this.Close(); // 关闭窗口 / Close window
                }
            }
        }

    }
}

#endif
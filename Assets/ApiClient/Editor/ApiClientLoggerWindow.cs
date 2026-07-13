using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;

namespace ApiClientLib.Editor
{
    public class ApiClientLoggerWindow : EditorWindow
    {
        private const string EditorSymbol = "UNITY_EDITOR";

        #region Menu / Window lifecycle

        [MenuItem("Window/ApiClient/Logger")]
        private static void ShowWindow()
        {
            var window = GetWindow<ApiClientLoggerWindow>("ApiClient Logger");
            window.minSize = new Vector2(680, 360);
            window.Show();
        }

        private static readonly HashSet<ApiClientLoggerWindow> OpenWindows = new HashSet<ApiClientLoggerWindow>();

        private void OnEnable()
        {
            OpenWindows.Add(this);
            InitMethodFilters();
        }

        private void OnDisable()
        {
            OpenWindows.Remove(this);
        }

        #endregion

        #region Log entry point / storage

        private static readonly List<LogEntry> Events = new List<LogEntry>();

        [Conditional(EditorSymbol)]
        public static void Log(ApiEventData data)
        {
            Events.Add(new LogEntry(data));

            foreach (var window in OpenWindows)
            {
                window.Repaint();
            }
        }

        private class LogEntry
        {
            public readonly ApiEventData Data;
            public bool Expanded;

            private bool _bodiesProcessed;
            private string _prettyRequestBody;
            private string _prettyResponseBody;

            public LogEntry(ApiEventData data)
            {
                Data = data;
            }

            public string PrettyRequestBody
            {
                get
                {
                    EnsureBodies();
                    return _prettyRequestBody;
                }
            }

            public string PrettyResponseBody
            {
                get
                {
                    EnsureBodies();
                    return _prettyResponseBody;
                }
            }

            private void EnsureBodies()
            {
                if (_bodiesProcessed) return;
                _bodiesProcessed = true;
                _prettyRequestBody = PrettyPrintJson(Data.RequestBody);
                _prettyResponseBody = PrettyPrintJson(Data.ResponseBody);
            }
        }

        #endregion

        #region Filter state

        private enum SuccessFilterMode
        {
            All,
            Success,
            Fail
        }

        private static readonly RequestMethod[] AllMethods = (RequestMethod[])Enum.GetValues(typeof(RequestMethod));
        private static readonly string[] SuccessFilterLabels = { "All", "Success", "Fail" };

        private readonly Dictionary<RequestMethod, bool> _methodEnabled = new Dictionary<RequestMethod, bool>();
        private SuccessFilterMode _successFilter = SuccessFilterMode.All;
        private int? _selectedStatusCode;
        private int? _selectedInstanceID;
        private string _searchText = string.Empty;

        private void InitMethodFilters()
        {
            foreach (var method in AllMethods)
            {
                _methodEnabled.TryAdd(method, true);
            }
        }

        private bool PassesFilters(ApiEventData d)
        {
            if (_methodEnabled.TryGetValue(d.Method, out var methodOn) && !methodOn) return false;
            if (_successFilter == SuccessFilterMode.Success && !d.Success) return false;
            if (_successFilter == SuccessFilterMode.Fail && d.Success) return false;
            if (_selectedStatusCode.HasValue && d.StatusCode != _selectedStatusCode.Value) return false;
            if (_selectedInstanceID.HasValue && d.InstanceID != _selectedInstanceID.Value) return false;
            if (!string.IsNullOrEmpty(_searchText) && !MatchesSearch(d, _searchText)) return false;
            return true;
        }

        private static bool MatchesSearch(ApiEventData d, string term)
        {
            return ContainsIgnoreCase(d.URL, term)
                   || ContainsIgnoreCase(d.RequestBody, term)
                   || ContainsIgnoreCase(d.ResponseBody, term)
                   || ContainsIgnoreCase(d.ErrorMessage, term);
        }

        private static bool ContainsIgnoreCase(string source, string term)
        {
            return !string.IsNullOrEmpty(source) && source.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        #endregion

        #region Colors

        private static readonly Color FailRowColor = new Color(0.45f, 0.15f, 0.15f);
        private static readonly Color CacheRowColor = new Color(0.15f, 0.3f, 0.5f);
        private static readonly Color ExceptionBadgeColor = new Color(0.35f, 0.05f, 0.05f);
        private static readonly Color ErrorTextColor = new Color(1f, 0.4f, 0.4f);

        private static readonly Color StatusOkColor = new Color(0.35f, 0.85f, 0.35f);
        private static readonly Color StatusNoneColor = new Color(1f, 0.85f, 0.15f);
        private static readonly Color StatusErrorColor = new Color(0.95f, 0.3f, 0.3f);
        private static readonly Color StatusUnsetColor = new Color(0.1f, 0.99f, 0.99f);

        private static Color? GetRowColor(ApiEventData d)
        {
            if (!d.Success) return FailRowColor;
            if (d.FromCache) return CacheRowColor;
            return null;
        }

        private static Color GetMethodColor(RequestMethod method)
        {
            return method switch
            {
                RequestMethod.GET => new Color(0.20f, 0.85f, 0.20f),
                RequestMethod.POST => new Color(0.15f, 0.50f, 1f),
                RequestMethod.PUT => new Color(0.95f, 0.52f, 0.0f),
                RequestMethod.PATCH => new Color(0.65f, 0.28f, 0.90f),
                RequestMethod.DELETE => new Color(0.90f, 0.15f, 0.15f),
                RequestMethod.GET_SPRITE => new Color(0.10f, 0.85f, 0.85f),
                RequestMethod.GET_AUDIOCLIP => new Color(0.95f, 0.85f, 0.0f),
                _ => Color.gray
            };
        }

        private static Color GetStatusCodeColor(int statusCode)
        {
            if (statusCode == -1) return StatusUnsetColor;
            if (statusCode == 0) return StatusNoneColor;
            if (statusCode is >= 200 and < 300) return StatusOkColor;
            return StatusErrorColor;
        }

        private static Texture2D _borderedBrightTexture;
        private const int BadgeBorderThickness = 1;

        private static Texture2D BorderedBrightTexture
        {
            get
            {
                if (_borderedBrightTexture == null)
                {
                    const int size = 8;
                    _borderedBrightTexture = new Texture2D(size, size);
                    for (var y = 0; y < size; y++)
                    {
                        for (var x = 0; x < size; x++)
                        {
                            var isBorder = x < BadgeBorderThickness || y < BadgeBorderThickness ||
                                           x >= size - BadgeBorderThickness || y >= size - BadgeBorderThickness;
                            _borderedBrightTexture.SetPixel(x, y, isBorder ? Color.black : 0.9f * Color.white);
                        }
                    }

                    _borderedBrightTexture.Apply();
                    _borderedBrightTexture.filterMode = FilterMode.Point;
                    _borderedBrightTexture.hideFlags = HideFlags.HideAndDontSave;
                }

                return _borderedBrightTexture;
            }
        }

        private static Texture2D _solidWhiteTexture;

        private static Texture2D SolidWhiteTexture
        {
            get
            {
                if (_solidWhiteTexture == null)
                {
                    _solidWhiteTexture = new Texture2D(1, 1);
                    _solidWhiteTexture.SetPixel(0, 0, Color.white);
                    _solidWhiteTexture.Apply();
                    _solidWhiteTexture.hideFlags = HideFlags.HideAndDontSave;
                }

                return _solidWhiteTexture;
            }
        }

        #endregion

        #region Formatting helpers

        private static string FormatDuration(float durationSeconds)
        {
            var rounded = Math.Round(durationSeconds, 2, MidpointRounding.AwayFromZero);
            var formatted = rounded.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
            return formatted + "s";
        }

        private static string FormatTimestamp(DateTime timestamp)
        {
            return timestamp.ToString("HH:mm:ss");
        }

        private static string PrettyPrintJson(string raw)
        {
            if (string.IsNullOrEmpty(raw)) return raw;

            try
            {
                var token = JToken.Parse(raw);
                return token.ToString(Formatting.Indented);
            }
            catch (Newtonsoft.Json.JsonException)
            {
                return raw;
            }
        }

        #endregion

        #region Styles (lazily created inside OnGUI)

        private GUIStyle _noUrlStyle;
        private GUIStyle _urlStyle;
        private GUIStyle _miniInfoStyle;
        private GUIStyle _methodTagStyle;
        private GUIStyle _statusCodeStyle;
        private GUIStyle _exceptionBadgeStyle;
        private GUIStyle _coloredRowStyle;
        private GUIStyle _sectionTitleStyle;
        private GUIStyle _bodyTextStyle;
        private GUIStyle _headerKeyStyle;
        private GUIStyle _headerValueStyle;
        private GUIStyle _errorMessageStyle;

        private void EnsureStyles()
        {
            if (_urlStyle != null) return;

            _urlStyle = new GUIStyle(EditorStyles.label);

            _noUrlStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Italic
            };
            _noUrlStyle.normal.textColor = Color.gray;

            _miniInfoStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter
            };

            _methodTagStyle = new GUIStyle(GUI.skin.box)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                border = new RectOffset(BadgeBorderThickness, BadgeBorderThickness, BadgeBorderThickness,
                    BadgeBorderThickness)
            };
            _methodTagStyle.normal.textColor = Color.white;
            _methodTagStyle.normal.background = BorderedBrightTexture;

            _statusCodeStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold
            };

            _exceptionBadgeStyle = new GUIStyle(GUI.skin.box)
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };
            _exceptionBadgeStyle.normal.textColor = Color.white;
            _exceptionBadgeStyle.normal.background = SolidWhiteTexture;

            _coloredRowStyle = new GUIStyle(EditorStyles.helpBox);
            _coloredRowStyle.normal.background = SolidWhiteTexture;

            _sectionTitleStyle = new GUIStyle(EditorStyles.boldLabel);

            _bodyTextStyle = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true,
                richText = false
            };

            _headerKeyStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold
            };
            _headerKeyStyle.normal.textColor = new Color(0.45f, 0.70f, 1f);

            _headerValueStyle = new GUIStyle(EditorStyles.label);
            _headerValueStyle.normal.textColor = new Color(0.85f, 0.85f, 0.85f);

            _errorMessageStyle = new GUIStyle(EditorStyles.wordWrappedLabel)
            {
                fontStyle = FontStyle.Bold
            };
            _errorMessageStyle.normal.textColor = ErrorTextColor;
        }

        #endregion

        #region OnGUI

        private Vector2 _scrollPos;
        private bool _autoScroll = true;
        private bool _lastAutoScroll = true;
        private int _lastSeenEventCount = -1;

        private void OnGUI()
        {
            EnsureStyles();
            DrawToolbar();

            var filtered = Events.Where(e => PassesFilters(e.Data)).ToList();
            EditorGUILayout.LabelField($"{filtered.Count} / {Events.Count} events", EditorStyles.miniLabel);

            var countChanged = Events.Count != _lastSeenEventCount;
            var justToggledOn = _autoScroll && !_lastAutoScroll;
            _lastSeenEventCount = Events.Count;
            _lastAutoScroll = _autoScroll;

            if (_autoScroll && (countChanged || justToggledOn))
            {
                _scrollPos.y = float.MaxValue;
            }

            _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);
            foreach (var entry in filtered)
            {
                DrawRow(entry);
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            _searchText =
                EditorGUILayout.TextField(_searchText, EditorStyles.toolbarSearchField, GUILayout.MinWidth(150));

            if (GUILayout.Button("X", EditorStyles.toolbarButton, GUILayout.Width(18)))
            {
                _searchText = string.Empty;
                GUI.FocusControl(null);
            }

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Clear", EditorStyles.toolbarButton, GUILayout.Width(50)))
            {
                Events.Clear();
            }

            _autoScroll = GUILayout.Toggle(_autoScroll, "Autoscroll", EditorStyles.toolbarButton, GUILayout.Width(80));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            _successFilter = (SuccessFilterMode)GUILayout.Toolbar(
                (int)_successFilter, SuccessFilterLabels, EditorStyles.toolbarButton, GUILayout.Width(180));

            GUILayout.Space(10);
            DrawStatusCodeFilter();

            GUILayout.Space(10);
            DrawInstanceIdFilter();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            GUILayout.Label("Methods:", EditorStyles.miniLabel, GUILayout.Width(55));
            DrawMethodFilterRow();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }

        private List<ApiEventData> EventsIgnoringStatusFilter()
        {
            var saved = _selectedStatusCode;
            _selectedStatusCode = null;
            try
            {
                return Events.Select(e => e.Data).Where(PassesFilters).ToList();
            }
            finally
            {
                _selectedStatusCode = saved;
            }
        }

        private List<ApiEventData> EventsIgnoringInstanceFilter()
        {
            var saved = _selectedInstanceID;
            _selectedInstanceID = null;
            try
            {
                return Events.Select(e => e.Data).Where(PassesFilters).ToList();
            }
            finally
            {
                _selectedInstanceID = saved;
            }
        }

        private void DrawStatusCodeFilter()
        {
            var codes = EventsIgnoringStatusFilter().Select(d => d.StatusCode).Distinct().OrderBy(c => c).ToList();

            if (_selectedStatusCode.HasValue && !codes.Contains(_selectedStatusCode.Value))
            {
                _selectedStatusCode = null;
            }

            var labels = new string[codes.Count + 1];
            labels[0] = "None";
            for (var i = 0; i < codes.Count; i++)
            {
                labels[i + 1] = codes[i] == -1 ? "C" : codes[i].ToString();
            }

            var currentIndex = _selectedStatusCode.HasValue ? codes.IndexOf(_selectedStatusCode.Value) + 1 : 0;

            GUILayout.Label("Status:", EditorStyles.miniLabel, GUILayout.Width(40));
            var newIndex = EditorGUILayout.Popup(currentIndex, labels, EditorStyles.toolbarPopup, GUILayout.Width(80));

            _selectedStatusCode = newIndex == 0 ? (int?)null : codes[newIndex - 1];
        }

        private void DrawInstanceIdFilter()
        {
            var ids = EventsIgnoringInstanceFilter().Select(d => d.InstanceID).Distinct().OrderBy(i => i).ToList();

            if (_selectedInstanceID.HasValue && !ids.Contains(_selectedInstanceID.Value))
            {
                _selectedInstanceID = null;
            }

            var labels = new string[ids.Count + 1];
            labels[0] = "All";
            for (var i = 0; i < ids.Count; i++)
            {
                labels[i + 1] = "#" + ids[i];
            }

            var currentIndex = _selectedInstanceID.HasValue ? ids.IndexOf(_selectedInstanceID.Value) + 1 : 0;

            GUILayout.Label("Instance:", EditorStyles.miniLabel, GUILayout.Width(55));
            var newIndex = EditorGUILayout.Popup(currentIndex, labels, EditorStyles.toolbarPopup, GUILayout.Width(70));

            _selectedInstanceID = newIndex == 0 ? (int?)null : ids[newIndex - 1];
        }

        private void DrawMethodFilterRow()
        {
            foreach (var method in AllMethods)
            {
                var isOn = _methodEnabled[method];

                var prevBg = GUI.backgroundColor;
                GUI.backgroundColor = isOn ? GetMethodColor(method) : Color.gray;

                var newVal = GUILayout.Toggle(
                    isOn, method.ToString(), EditorStyles.toolbarButton, GUILayout.Width(GetMethodTagMinWidth(method)));

                GUI.backgroundColor = prevBg;
                _methodEnabled[method] = newVal;
            }
        }

        private static float GetMethodTagMinWidth(RequestMethod method)
        {
            return method.ToString().Length * 8f + 16f;
        }

        #endregion

        #region Row drawing

        private void DrawRow(LogEntry entry)
        {
            var d = entry.Data;
            var rowColor = GetRowColor(d);

            if (rowColor.HasValue)
            {
                var prevBg = GUI.backgroundColor;
                GUI.backgroundColor = rowColor.Value;
                EditorGUILayout.BeginVertical(_coloredRowStyle);
                GUI.backgroundColor = prevBg;
            }
            else
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            }

            var miniRowRect = EditorGUILayout.BeginHorizontal();

            DrawMethodTag(d.Method);

            var hasUrl = !string.IsNullOrEmpty(d.URL);
            GUILayout.Label(hasUrl ? d.URL : "<no url>", hasUrl ? _urlStyle : _noUrlStyle, GUILayout.ExpandWidth(true));

            GUILayout.FlexibleSpace();

            DrawStatusCodeLabel(d.StatusCode);
            GUILayout.Label(FormatDuration(d.Duration), _miniInfoStyle, GUILayout.Width(55));
            GUILayout.Label(FormatTimestamp(d.Timestamp), _miniInfoStyle, GUILayout.Width(65));
            GUILayout.Label("#" + d.InstanceID, _miniInfoStyle, GUILayout.Width(35));

            if (d.Exception != null)
            {
                DrawExceptionBadge(d.Exception);
            }

            EditorGUILayout.EndHorizontal();

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0 &&
                miniRowRect.Contains(Event.current.mousePosition))
            {
                entry.Expanded = !entry.Expanded;
                Event.current.Use();
                Repaint();
            }

            if (entry.Expanded)
            {
                DrawExpanded(entry);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawMethodTag(RequestMethod method)
        {
            var prevBg = GUI.backgroundColor;
            GUI.backgroundColor = GetMethodColor(method);
            GUILayout.Label(method.ToString(), _methodTagStyle, GUILayout.Width(GetMethodTagMinWidth(method)));
            GUI.backgroundColor = prevBg;
        }

        private void DrawStatusCodeLabel(int statusCode)
        {
            var prevColor = GUI.contentColor;
            GUI.contentColor = GetStatusCodeColor(statusCode);
            GUILayout.Label(statusCode == -1 ? "C" : statusCode.ToString(), _statusCodeStyle, GUILayout.Width(42));
            GUI.contentColor = prevColor;
        }

        private void DrawExceptionBadge(Exception exception)
        {
            var prevBg = GUI.backgroundColor;
            GUI.backgroundColor = ExceptionBadgeColor;
            var typeName = exception.GetType().Name;
            GUILayout.Label(typeName, _exceptionBadgeStyle, GUILayout.Width(typeName.Length * 7f + 16f));
            GUI.backgroundColor = prevBg;
        }

        #endregion

        #region Expanded details drawing

        private void DrawExpanded(LogEntry entry)
        {
            var d = entry.Data;

            EditorGUILayout.Space(4);
            EditorGUI.indentLevel++;

            DrawTextSection("Request Body", entry.PrettyRequestBody);
            DrawTextSection("Response Body", entry.PrettyResponseBody);

            DrawHeaderSection("Request Headers", d.RequestHeaders);
            DrawHeaderSection("Query Params", d.QueryParams);
            DrawHeaderSection("Response Headers", d.ResponseHeaders);

            if (!string.IsNullOrEmpty(d.ErrorMessage))
            {
                EditorGUILayout.Space(6);
                EditorGUILayout.LabelField("Error", _sectionTitleStyle);
                EditorGUILayout.LabelField(d.ErrorMessage, _errorMessageStyle);
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(4);
        }

        private void DrawTextSection(string sectionTitle, string content)
        {
            if (string.IsNullOrEmpty(content)) return;

            EditorGUILayout.LabelField(sectionTitle, _sectionTitleStyle);

            var lineCount = content.Split('\n').Length;
            var height = EditorGUIUtility.singleLineHeight * Mathf.Clamp(lineCount, 1, 20) + 6f;
            EditorGUILayout.SelectableLabel(content, _bodyTextStyle, GUILayout.MinHeight(height));
        }

        private void DrawHeaderSection(string sectionTitle, IReadOnlyDictionary<string, string> dict)
        {
            if (dict == null || dict.Count == 0) return;

            EditorGUILayout.LabelField(sectionTitle, _sectionTitleStyle);

            foreach (var kvp in dict)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label(kvp.Key, _headerKeyStyle, GUILayout.Width(180));
                GUILayout.Label(kvp.Value ?? string.Empty, _headerValueStyle);
                EditorGUILayout.EndHorizontal();
            }
        }

        #endregion
    }
}
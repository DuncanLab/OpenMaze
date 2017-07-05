using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Gaia
{
    /// <summary>
    /// Handy helper for all things Gaia
    /// </summary>
    public class GaiaManagerEditor : EditorWindow
    {
        private GUIStyle m_boxStyle;
        private GUIStyle m_wrapStyle;
        private Vector2 m_scrollPosition = Vector2.zero;
        private GaiaSettings m_settings;
        private GaiaDefaults m_defaults;
        private GaiaResource m_resources;
        private Gaia.GaiaConstants.ManagerEditorMode m_managerMode = Gaia.GaiaConstants.ManagerEditorMode.Standard;

        //Extension manager
        bool m_needsScan = true;
        GaiaExtensionManager m_extensionMgr = new GaiaExtensionManager();
        private bool m_drawGXInstalled = true;

        #pragma warning disable 414
        //private GaiaSessionManager m_sessionManager = null;
        #pragma warning restore 414

        private bool m_foldoutSession = false;
        private bool m_foldoutTerrain = false;
        private bool m_foldoutSpawners = false;
        private bool m_foldoutCharacters = false;

        #region Gaia Menu Commands
        /// <summary>
        /// Show Gaia Manager editor window
        /// </summary>
        [MenuItem("Window/Gaia/Show Gaia Manager... %g", false, 1)]
        public static void ShowGaiaManager()
        {
            var manager = EditorWindow.GetWindow<Gaia.GaiaManagerEditor>(false, "Gaia Manager");
            manager.Show();
            SetGaiaDefinesStatic();
        }

        /// <summary>
        /// Show Gaia resource editor window
        /// </summary>
        [MenuItem("Window/Gaia/About...", false, 1)]
        private static void ShowAboutWindow()
        {
            var about = EditorWindow.GetWindow<Gaia.AboutEditor>(false, "About Gaia");
            about.minSize = new Vector2(300, 170);
            about.maxSize = new Vector2(300, 170);
            about.Show();
        }

        #endregion

        /// <summary>
        /// Creates a new Gaia settings asset
        /// </summary>
        /// <returns>New gaia settings asset</returns>
        public static GaiaSettings CreateSettingsAsset()
        {
            GaiaSettings settings = ScriptableObject.CreateInstance<Gaia.GaiaSettings>();
            AssetDatabase.CreateAsset(settings, "Assets/Gaia/Data/GaiaSettings.asset");
            AssetDatabase.SaveAssets();
            return settings;
        }

        /// <summary>
        /// Create and returns a defaults asset
        /// </summary>
        /// <returns>New defaults asset</returns>
        public static GaiaDefaults CreateDefaultsAsset()
        {
            GaiaDefaults defaults = ScriptableObject.CreateInstance<Gaia.GaiaDefaults>();
            AssetDatabase.CreateAsset(defaults, string.Format("Assets/Gaia/Data/GD-{0:yyyyMMdd-HHmmss}.asset", DateTime.Now));
            AssetDatabase.SaveAssets();
            return defaults;
        }

        /// <summary>
        /// Create and returns a resources asset
        /// </summary>
        /// <returns>New resources asset</returns>
        public static GaiaResource CreateResourcesAsset()
        {
            GaiaResource resources = ScriptableObject.CreateInstance<Gaia.GaiaResource>();
            AssetDatabase.CreateAsset(resources, string.Format("Assets/Gaia/Data/GR-{0:yyyyMMdd-HHmmss}.asset", DateTime.Now));
            AssetDatabase.SaveAssets();
            return resources;
        }

        /// <summary>
        /// Set up the gaia defines
        /// </summary>
        public void SetGaiaDefines()
        {
            string currBuildSettings = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            //Check for and inject GAIA_PRESENT
            if (!currBuildSettings.Contains("GAIA_PRESENT"))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currBuildSettings + ";GAIA_PRESENT");
                m_needsScan = true;
            }
        }

        /// <summary>
        /// Set up the Gaia Present defines
        /// </summary>
        public static void SetGaiaDefinesStatic()
        {
            string currBuildSettings = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

            //Check for and inject GAIA_PRESENT
            if (!currBuildSettings.Contains("GAIA_PRESENT"))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, currBuildSettings + ";GAIA_PRESENT");
            }
        }

        /// <summary>
        /// See if we can preload the manager with existing settings
        /// </summary>
        void OnEnable()
        {
            //Signal we need a scan
            m_needsScan = true;
            
            //Set the Gaia directories up
            Utils.CreateGaiaAssetDirectories();

            //Get or create existing settings object
            if (m_settings == null)
            {
                m_settings = (GaiaSettings)Utils.GetAssetScriptableObject("GaiaSettings");
                if (m_settings == null)
                {
                    m_settings = CreateSettingsAsset();
                }
            }

            //Grab first default we can find
            if (m_defaults == null)
            {
                if (m_settings.m_currentDefaults != null)
                {
                    m_defaults = m_settings.m_currentDefaults;
                }
                else
                {
                    m_defaults = (GaiaDefaults)Utils.GetAssetScriptableObject("GaiaDefaults");
                    m_settings.m_currentDefaults = m_defaults;
                    EditorUtility.SetDirty(m_settings);
                }
            }

            //Grab first resource we can find
            if (m_resources == null)
            {
                if (m_settings.m_currentResources != null)
                {
                    m_resources = m_settings.m_currentResources;
                }
                else
                {
                    m_resources = (GaiaResource)Utils.GetAssetScriptableObject("GaiaResources");
                    m_settings.m_currentResources = m_resources;
                    EditorUtility.SetDirty(m_settings);
                }
            }
        }

        void OnDisable()
        {
        }

        void OnGUI()
        {
            //Set up the box style
            if (m_boxStyle == null)
            {
                m_boxStyle = new GUIStyle(GUI.skin.box);
                m_boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
                m_boxStyle.fontStyle = FontStyle.Bold;
                m_boxStyle.alignment = TextAnchor.UpperLeft;
            }

            //Setup the wrap style
            if (m_wrapStyle == null)
            {
                m_wrapStyle = new GUIStyle(GUI.skin.label);
                m_wrapStyle.fontStyle = FontStyle.Normal;
                m_wrapStyle.wordWrap = true;
            }

            //Check for state of compiler
            if (EditorApplication.isCompiling)
            {
                m_needsScan = true;
            }

            //Text intro
            GUILayout.BeginVertical(string.Format("Gaia Manager ({0}.{1})", Gaia.GaiaConstants.GaiaMajorVersion, Gaia.GaiaConstants.GaiaMinorVersion), m_boxStyle);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("The Gaia manager guides you through common scene creation workflows.", m_wrapStyle);
            GUILayout.EndVertical();

            //Scroll
            m_scrollPosition = GUILayout.BeginScrollView(m_scrollPosition, false, false);

            //Select or create new defaults and resources
            if (m_defaults == null || m_resources == null)
            {

                EditorGUILayout.BeginHorizontal();
                m_defaults = (GaiaDefaults)EditorGUILayout.ObjectField(GetLabel("Defaults"), m_defaults, typeof(GaiaDefaults), false);
                if (GUILayout.Button(GetLabel("New"), GUILayout.Width(45)))
                {
                    m_defaults = CreateDefaultsAsset();
                }
                if (m_settings.m_currentDefaults == null || (m_defaults.GetInstanceID() != m_settings.m_currentDefaults.GetInstanceID()))
                {
                    m_settings.m_currentDefaults = m_defaults;
                    EditorUtility.SetDirty(m_settings);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                m_resources = (GaiaResource)EditorGUILayout.ObjectField(GetLabel("Resources"), m_resources, typeof(GaiaResource), false);
                if (GUILayout.Button(GetLabel("New"), GUILayout.Width(45)))
                {
                    m_resources = CreateResourcesAsset();
                    if (m_defaults != null)
                    {
                        m_resources.m_seaLevel = m_defaults.m_seaLevel;
                        m_resources.m_beachHeight = m_defaults.m_beachHeight;
                    }
                }
                if (m_settings.m_currentResources == null || (m_resources.GetInstanceID() != m_settings.m_currentResources.GetInstanceID()))
                {
                    m_settings.m_currentResources = m_resources;
                    EditorUtility.SetDirty(m_settings);
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                m_defaults = (GaiaDefaults)EditorGUILayout.ObjectField(GetLabel("Defaults"), m_defaults, typeof(GaiaDefaults), false);
                if (GUILayout.Button(GetLabel("New"), GUILayout.Width(45)))
                {
                    m_defaults = CreateDefaultsAsset();
                }
                if (m_settings.m_currentDefaults == null || (m_defaults.GetInstanceID() != m_settings.m_currentDefaults.GetInstanceID()))
                {
                    m_settings.m_currentDefaults = m_defaults;
                    EditorUtility.SetDirty(m_settings);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                m_resources = (GaiaResource)EditorGUILayout.ObjectField(GetLabel("Resources"), m_resources, typeof(GaiaResource), false);
                if (GUILayout.Button(GetLabel("New"), GUILayout.Width(45)))
                {
                    m_resources = CreateResourcesAsset();
                }
                if (m_settings.m_currentResources == null || (m_resources.GetInstanceID() != m_settings.m_currentResources.GetInstanceID()))
                {
                    m_settings.m_currentResources = m_resources;
                    EditorUtility.SetDirty(m_settings);
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();

                if (m_managerMode == Gaia.GaiaConstants.ManagerEditorMode.Standard)
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button(GetLabel("STANDARD")))
                {
                    m_managerMode = Gaia.GaiaConstants.ManagerEditorMode.Standard;
                }
                GUI.enabled = true;

                if (m_managerMode == Gaia.GaiaConstants.ManagerEditorMode.Advanced)
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button(GetLabel("ADVANCED")))
                {
                    m_managerMode = Gaia.GaiaConstants.ManagerEditorMode.Advanced;
                }
                GUI.enabled = true;

                if (m_managerMode == Gaia.GaiaConstants.ManagerEditorMode.Utilities)
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button(GetLabel("UTILITIES")))
                {
                    m_managerMode = Gaia.GaiaConstants.ManagerEditorMode.Utilities;
                }
                GUI.enabled = true;

                if (m_managerMode == Gaia.GaiaConstants.ManagerEditorMode.Extensions)
                {
                    GUI.enabled = false;
                }
                if (GUILayout.Button(GetLabel("GX")))
                {
                    m_managerMode = Gaia.GaiaConstants.ManagerEditorMode.Extensions;
                }
                GUI.enabled = true;
                EditorGUILayout.EndHorizontal();

                //m_managerMode = (Gaia.Constants.ManagerEditorMode)EditorGUILayout.EnumPopup(GetLabel("Operation Mode"), m_managerMode);
                if (m_managerMode == Gaia.GaiaConstants.ManagerEditorMode.Standard)
                {
                    DrawStandardEditor();
                }
                else if (m_managerMode == Gaia.GaiaConstants.ManagerEditorMode.Advanced)
                {
                    DrawAdvancedEditor();
                }
                else if (m_managerMode == Gaia.GaiaConstants.ManagerEditorMode.Utilities)
                {
                    DrawUtilsEditor();
                }
                else
                {
                    DrawExtensionsEditor();
                }
            }

            //End scroll
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// Draw the brief editor
        /// </summary>
        void DrawStandardEditor()
        {
            EditorGUI.indentLevel++;

            if (DisplayButton(GetLabel("1. Create Terrain & Show Stamper")))
            {
                ShowSessionManager();
                CreateTerrain();
                ShowStamper();
            }
            if (DisplayButton(GetLabel("2. Create Spawners")))
            {
                //Only do this if we have 1 terrain
                if (!DisplayErrorIfInvalidTerrainCount(1))
                {
                    //Create the spawners
                    CreateTextureSpawner();
                    CreateCoverageGameObjectSpawner();
                    CreateClusteredTreeSpawner();
                    CreateCoverageTreeSpawner();
                    CreateDetailSpawner();

                    //Select the spawner group
                    Selection.activeGameObject = FindOrCreateGroupSpawner();
                }
            }
            if (DisplayButton(GetLabel("3. Create Player, Wind, Water and Screenshotter")))
            {
                //Only do this if we have 1 terrain
                if (DisplayErrorIfInvalidTerrainCount(1))
                {
                    return;
                }

                CreatePlayer();
                CreateWindZone();
                CreateWater();
                Selection.activeGameObject = CreateScreenShotter();
            }

            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Draw the detailed editor
        /// </summary>
        void DrawAdvancedEditor()
        {
            EditorGUI.indentLevel++;
            if (m_foldoutSession = EditorGUILayout.Foldout(m_foldoutSession, GetLabel("1. Create Session Manager...")))
            {
                EditorGUI.indentLevel++;
                if (DisplayButton(GetLabel("Show Session Manager")))
                {
                    ShowSessionManager();
                }
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }

            if (m_foldoutTerrain = EditorGUILayout.Foldout(m_foldoutTerrain, GetLabel("2. Create your Terrain...")))
            {
                EditorGUI.indentLevel++;
                if (DisplayButton(GetLabel("Create Terrain")))
                {
                    CreateTerrain();
                }
                if (DisplayButton(GetLabel("Show Stamper")))
                {
                    ShowStamper();
                }
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }
            if (m_foldoutSpawners = EditorGUILayout.Foldout(m_foldoutSpawners, GetLabel("3. Create and configure your Spawners...")))
            {
                EditorGUI.indentLevel++;
                //if (DisplayButton(GetLabel("Create Stamp Spawner")))
                //{
                //    Selection.activeObject = CreateStampSpawner();
                //}
                if (DisplayButton(GetLabel("Create Coverage Texture Spawner")))
                {
                    Selection.activeObject = CreateTextureSpawner();
                }
                if (DisplayButton(GetLabel("Create Clustered Detail Spawner")))
                {
                    Selection.activeObject = CreateClusteredDetailSpawner();
                }
                if (DisplayButton(GetLabel("Create Coverage Detail Spawner")))
                {
                    Selection.activeObject = CreateDetailSpawner();
                }
                if (DisplayButton(GetLabel("Create Clustered Tree Spawner")))
                {
                    Selection.activeObject = CreateClusteredTreeSpawner();
                }
                if (DisplayButton(GetLabel("Create Coverage Tree Spawner")))
                {
                    Selection.activeObject = CreateCoverageTreeSpawner();
                }
                if (DisplayButton(GetLabel("Create Clustered GameObject Spawner")))
                {
                    Selection.activeObject = CreateClusteredGameObjectSpawner();
                }
                if (DisplayButton(GetLabel("Create Coverage GameObject Spawner")))
                {
                    Selection.activeObject = CreateCoverageGameObjectSpawner();
                }
                if (DisplayButton(GetLabel("Create Group Spawner")))
                {
                    Selection.activeObject = FindOrCreateGroupSpawner();
                }
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }
            if (m_foldoutCharacters = EditorGUILayout.Foldout(m_foldoutCharacters, GetLabel("4. Add common Game Objects...")))
            {
                EditorGUI.indentLevel++;
                if (DisplayButton(GetLabel("Add Character")))
                {
                    Selection.activeGameObject = CreatePlayer();
                }
                if (DisplayButton(GetLabel("Add Wind Zone")))
                {
                    Selection.activeGameObject = CreateWindZone();
                }
                if (DisplayButton(GetLabel("Add Water")))
                {
                    Selection.activeGameObject = CreateWater();
                }
                if (DisplayButton(GetLabel("Add Screen Shotter")))
                {
                    Selection.activeGameObject = CreateScreenShotter();
                }
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }

            /*
            if (m_foldoutPlugins = EditorGUILayout.Foldout(m_foldoutPlugins, GetLabel("4. Add and configure your Plugins...")))
            {
                EditorGUI.indentLevel++;
                if (m_foldoutTanuki = EditorGUILayout.Foldout(m_foldoutTanuki, GetLabel("Tanuki Digital")))
                {
                    EditorGUI.indentLevel++;
                    if (DisplayButton(GetLabel("Configure Suimono")))
                    {
                        Debug.LogWarning("Not implemented yet");
                    }
                    if (DisplayButton(GetLabel("Configure Tenkoku")))
                    {
                        Debug.LogWarning("Not implemented yet");
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }
            if (m_foldoutOptimisation = EditorGUILayout.Foldout(m_foldoutOptimisation, GetLabel("5. Optimise your Scene...")))
            {
                EditorGUI.indentLevel++;
                if (GUILayout.Button("Optimise Scene"))
                {
                    Debug.LogWarning("Not implemented yet");
                }
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }
             */
            EditorGUILayout.LabelField("Celebrate!", m_wrapStyle);
            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Draw the utils editor
        /// </summary>
        void DrawUtilsEditor()
        {
            EditorGUI.indentLevel++;

            if (DisplayButton(GetLabel("Show Scanner")))
            {
                Selection.activeGameObject = CreateScanner();
            }
            if (DisplayButton(GetLabel("Show Visualiser")))
            {
                Selection.activeGameObject = ShowVisualiser();
            }
            if (DisplayButton(GetLabel("Show Terrain Height Adjuster")))
            {
                ShowTerrainHeightAdjuster();
            }
            if (DisplayButton(GetLabel("Show Texture Splatmap Mask Exporter")))
            {
                ShowTexureMaskExporter();
            }
            if (DisplayButton(GetLabel("Show Grass Splatmap Mask Exporter")))
            {
                ShowGrassMaskExporter();
            }
            if (DisplayButton(GetLabel("Show Waterflow Mask Exporter")))
            {
                ShowFlowMapMaskExporter();
            }
            if (DisplayButton(GetLabel("Show Terrain Normal Mask Exporter")))
            {
                ShowNormalMaskExporter();
            }
            if (DisplayButton(GetLabel("Show Terrain OBJ Exporter")))
            {
                ShowTerrainObjExporter();
            }
            if (DisplayButton(GetLabel("Export Shore Mask as PNG")))
            {
                ExportShoremaskAsPNG();
            }
            if (DisplayButton(GetLabel("Export Terrain Heightmap as PNG")))
            {
                ExportWorldAsHeightmapPNG();
            }
            if (DisplayButton(GetLabel("Show Extension Exporter")))
            {
                ShowExtensionExporterEditor();
            }

            /*
            if (m_foldoutNibbler = EditorGUILayout.Foldout(m_foldoutNibbler, GetLabel("3. Nibblers...")))
            {
                EditorGUI.indentLevel++;
                if (DisplayButton(GetLabel("Show Beach Nibbler")))
                {
                    Debug.LogWarning("Coming next beta");
                }
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;
            }*/

            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Draw the extension editor
        /// </summary>
        void DrawExtensionsEditor()
        {
            EditorGUI.indentLevel++;

            //Every time we go here - check the define settings
            SetGaiaDefines();

            //And scenn if something has changed
            if (m_needsScan)
            {
                m_extensionMgr.ScanForExtensions();
                if (m_extensionMgr.GetInstalledExtensionCount() != 0)
                {
                    m_needsScan = false;
                }
            }

            //Draw menu buttons
            EditorGUILayout.BeginHorizontal();

            if (m_drawGXInstalled)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button(GetLabel("INSTALLED")))
            {
                m_drawGXInstalled = true;
            }
            GUI.enabled = true;

            if (!m_drawGXInstalled)
            {
                GUI.enabled = false;
            }
            if (GUILayout.Button(GetLabel("COMPATIBLE")))
            {
                m_drawGXInstalled = false;
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            int methodIdx = 0;
            string cmdName;
            string currFoldoutName = "";
            string prevFoldoutName = ""; 
            MethodInfo command;
            string[] cmdBreakOut = new string[0];
            List<GaiaCompatiblePackage> packages;
            List<GaiaCompatiblePublisher> publishers = m_extensionMgr.GetPublishers();

            if (m_drawGXInstalled)
            {
                foreach (GaiaCompatiblePublisher publisher in publishers)
                {
                    if (publisher.InstalledPackages() > 0)
                    {
                        if (publisher.m_installedFoldedOut = EditorGUILayout.Foldout(publisher.m_installedFoldedOut, GetLabel(publisher.m_publisherName)))
                        {
                            EditorGUI.indentLevel++;

                            packages = publisher.GetPackages();
                            foreach (GaiaCompatiblePackage package in packages)
                            {
                                if (package.m_isInstalled)
                                {
                                    if (package.m_installedFoldedOut = EditorGUILayout.Foldout(package.m_installedFoldedOut, GetLabel(package.m_packageName)))
                                    {
                                        EditorGUI.indentLevel++;

                                        //Now loop thru and process
                                        while (methodIdx < package.m_methods.Count)
                                        {
                                            command = package.m_methods[methodIdx];
                                            cmdBreakOut = command.Name.Split('_');

                                            //Ignore if we are not a valid thing
                                            if ((cmdBreakOut.GetLength(0) != 2 && cmdBreakOut.GetLength(0) != 3) || cmdBreakOut[0] != "GX")
                                            {
                                                methodIdx++;
                                                continue;
                                            }

                                            //Get foldout and command name
                                            if (cmdBreakOut.GetLength(0) == 2)
                                            {
                                                currFoldoutName = "";
                                            }
                                            else
                                            {
                                                currFoldoutName = Regex.Replace(cmdBreakOut[1], "(\\B[A-Z])", " $1");
                                            }
                                            cmdName = Regex.Replace(cmdBreakOut[cmdBreakOut.GetLength(0) - 1], "(\\B[A-Z])", " $1");

                                            if (currFoldoutName == "")
                                            {
                                                methodIdx++;
                                                if (DisplayButton(GetLabel(cmdName)))
                                                {
                                                    command.Invoke(null, null);
                                                }
                                            }
                                            else
                                            {
                                                prevFoldoutName = currFoldoutName;

                                                //Make sure we have it in our dictionary
                                                if (!package.m_methodGroupFoldouts.ContainsKey(currFoldoutName))
                                                {
                                                    package.m_methodGroupFoldouts.Add(currFoldoutName, false);
                                                }

                                                if (package.m_methodGroupFoldouts[currFoldoutName] = EditorGUILayout.Foldout(package.m_methodGroupFoldouts[currFoldoutName], GetLabel(currFoldoutName)))
                                                {
                                                    EditorGUI.indentLevel++;

                                                    while (methodIdx < package.m_methods.Count && currFoldoutName == prevFoldoutName)
                                                    {
                                                        command = package.m_methods[methodIdx];
                                                        cmdBreakOut = command.Name.Split('_');

                                                        //Drop out if we are not a valid thing
                                                        if ((cmdBreakOut.GetLength(0) != 2 && cmdBreakOut.GetLength(0) != 3) || cmdBreakOut[0] != "GX")
                                                        {
                                                            methodIdx++;
                                                            continue;
                                                        }

                                                        //Get foldout and command name
                                                        if (cmdBreakOut.GetLength(0) == 2)
                                                        {
                                                            currFoldoutName = "";
                                                        }
                                                        else
                                                        {
                                                            currFoldoutName = Regex.Replace(cmdBreakOut[1], "(\\B[A-Z])", " $1");
                                                        }
                                                        cmdName = Regex.Replace(cmdBreakOut[cmdBreakOut.GetLength(0) - 1], "(\\B[A-Z])", " $1");

                                                        if (currFoldoutName != prevFoldoutName)
                                                        {
                                                            continue;
                                                        }

                                                        if (DisplayButton(GetLabel(cmdName)))
                                                        {
                                                            command.Invoke(null, null);
                                                        }

                                                        methodIdx++;
                                                    }

                                                    EditorGUI.indentLevel--;
                                                }
                                                else
                                                {
                                                    while (methodIdx < package.m_methods.Count && currFoldoutName == prevFoldoutName)
                                                    {
                                                        command = package.m_methods[methodIdx];
                                                        cmdBreakOut = command.Name.Split('_');

                                                        //Drop out if we are not a valid thing
                                                        if ((cmdBreakOut.GetLength(0) != 2 && cmdBreakOut.GetLength(0) != 3) || cmdBreakOut[0] != "GX")
                                                        {
                                                            methodIdx++;
                                                            continue;
                                                        }

                                                        //Get foldout and command name
                                                        if (cmdBreakOut.GetLength(0) == 2)
                                                        {
                                                            currFoldoutName = "";
                                                        }
                                                        else
                                                        {
                                                            currFoldoutName = Regex.Replace(cmdBreakOut[1], "(\\B[A-Z])", " $1");
                                                        }
                                                        cmdName = Regex.Replace(cmdBreakOut[cmdBreakOut.GetLength(0) - 1], "(\\B[A-Z])", " $1");

                                                        if (currFoldoutName != prevFoldoutName)
                                                        {
                                                            continue;
                                                        }

                                                        methodIdx++;
                                                    }
                                                }
                                            }
                                        }

                                        /*
                                        foreach (MethodInfo command in package.m_methods)
                                        {
                                            cmdBreakOut = command.Name.Split('_');

                                            if ((cmdBreakOut.GetLength(0) == 2 || cmdBreakOut.GetLength(0) == 3) && cmdBreakOut[0] == "GX")
                                            {
                                                if (cmdBreakOut.GetLength(0) == 2)
                                                {
                                                    currFoldoutName = "";
                                                }
                                                else
                                                {
                                                    currFoldoutName = cmdBreakOut[1];
                                                    Debug.Log(currFoldoutName);
                                                }

                                                cmdName = Regex.Replace(cmdBreakOut[cmdBreakOut.GetLength(0) - 1], "(\\B[A-Z])", " $1");
                                                if (DisplayButton(GetLabel(cmdName)))
                                                {
                                                    command.Invoke(null, null);
                                                }
                                            }
                                        }
                                         */

                                        EditorGUI.indentLevel--;
                                    }
                                }
                            }

                            EditorGUI.indentLevel--;
                        }
                    }
                }
            }
            else
            {
                foreach (GaiaCompatiblePublisher publisher in publishers)
                {
                    if (publisher.CompatiblePackages() > 0)
                    {
                        if (publisher.m_compatibleFoldedOut = EditorGUILayout.Foldout(publisher.m_compatibleFoldedOut, GetLabel(publisher.m_publisherName)))
                        {
                            EditorGUI.indentLevel++;

                            packages = publisher.GetPackages();
                            foreach (GaiaCompatiblePackage package in packages)
                            {
                                if (package.m_isCompatible)
                                {
                                    if (package.m_compatibleFoldedOut = EditorGUILayout.Foldout(package.m_compatibleFoldedOut, GetLabel(package.m_packageName)))
                                    {
                                        EditorGUI.indentLevel++;
                                        EditorGUILayout.BeginVertical(GUILayout.Width(280));
                                        EditorGUILayout.BeginHorizontal();
                                        GUILayout.Space(50);
                                        Texture2D image = Utils.GetAssetTexture2D(package.m_packageImageName);
                                        if (image != null)
                                        {
                                            GUILayout.Label(image, GUILayout.Width(280), GUILayout.Height(140f));
                                        }
                                        EditorGUILayout.EndHorizontal();
                                        GUIStyle ws = GUI.skin.label;
                                        ws.wordWrap = true;
                                        EditorGUILayout.LabelField(package.m_packageDescription, ws);
                                        EditorGUILayout.BeginHorizontal();
                                        GUILayout.Space(50);
                                        if (GUILayout.Button("Display Page", GUILayout.Width(100f)))
                                        {
                                            Application.OpenURL(package.m_packageURL);
                                        }
                                        EditorGUILayout.EndHorizontal();
                                        EditorGUILayout.EndVertical();
                                        EditorGUI.indentLevel--;
                                    }
                                }
                            }

                            EditorGUI.indentLevel--;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Create the terrain
        /// </summary>

        void CreateTerrain()
        {
            //Only do this if we have < 1 terrain
            int actualTerrainCount = Gaia.TerrainHelper.GetActiveTerrainCount();
            if (actualTerrainCount != 0)
            {
                EditorUtility.DisplayDialog("OOPS!", string.Format("You currently have {0} active terrains in your scene, but to use this feature you need {1}. Please add or remove terrains.", actualTerrainCount, 0), "OK");
            }
            else
            {
                m_defaults.CreateTerrain(m_resources);
            }
        }


        /// <summary>
        /// Create / show the session manager
        /// </summary>
        GameObject ShowSessionManager(bool pickupExistingTerrain = false)
        {
            GameObject mgrObj = GaiaSessionManager.GetSessionManager(pickupExistingTerrain).gameObject;
            Selection.activeGameObject = mgrObj;
            return mgrObj;
        }

        /// <summary>
        /// Select or create a stamper
        /// </summary>
        void ShowStamper()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return;
            }

            //Make sure we have a session manager
            //m_sessionManager = m_resources.CreateOrFindSessionManager().GetComponent<GaiaSessionManager>();

            //Make sure we have gaia object
            GameObject gaiaObj = m_resources.CreateOrFindGaia();

            //Create or find the stamper
            GameObject stamperObj = GameObject.Find("Stamper");
            if (stamperObj == null)
            {
                stamperObj = new GameObject("Stamper");
                stamperObj.transform.parent = gaiaObj.transform;
                Stamper stamper = stamperObj.AddComponent<Stamper>();
                stamper.m_resources = m_resources;
                stamper.FitToTerrain();
                stamperObj.transform.position = new Vector3(stamper.m_x, stamper.m_y, stamper.m_z);
            }
            Selection.activeGameObject = stamperObj;
        }

        /// <summary>
        /// Select or create a scanner
        /// </summary>
        GameObject CreateScanner()
        {
            GameObject gaiaObj = m_resources.CreateOrFindGaia();
            GameObject scannerObj = GameObject.Find("Scanner");
            if (scannerObj == null)
            {
                scannerObj = new GameObject("Scanner");
                scannerObj.transform.parent = gaiaObj.transform;
                scannerObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter(false);
                Scanner scanner = scannerObj.AddComponent<Scanner>();

                //Load the material to draw it
                string matPath = GetAssetPath("GaiaScannerMaterial");
                if (!string.IsNullOrEmpty(matPath))
                {
                    scanner.m_previewMaterial = AssetDatabase.LoadAssetAtPath<Material>(matPath);
                }
            }
            return scannerObj;
        }

        /// <summary>
        /// Create or select the existing visualiser
        /// </summary>
        /// <returns>New or exsiting visualiser - or null if no terrain</returns>
        GameObject ShowVisualiser()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            GameObject gaiaObj = m_resources.CreateOrFindGaia();
            GameObject visualiserObj = GameObject.Find("Visualiser");
            if (visualiserObj == null)
            {
                visualiserObj = new GameObject("Visualiser");
                visualiserObj.AddComponent<ResourceVisualiser>();
                visualiserObj.transform.parent = gaiaObj.transform;

                //Center it on the terrain
                visualiserObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter();
            }
            ResourceVisualiser visualiser = visualiserObj.GetComponent<ResourceVisualiser>();
            visualiser.m_resources = m_resources;
            return visualiserObj;
        }



        /// <summary>
        /// Show a normal exporter
        /// </summary>
        void ShowNormalMaskExporter()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return;
            }

            var export = EditorWindow.GetWindow<GaiaNormalExporterEditor>(false, "Normalmap Exporter");
            export.Show();
        }

        /// <summary>
        /// Show the terrain height adjuster
        /// </summary>
        void ShowTerrainHeightAdjuster()
        {
            var export = EditorWindow.GetWindow<GaiaTerrainHeightAdjuster>(false, "Height Adjuster");
            export.Show();
        }

        /// <summary>
        /// Show a texture mask exporter
        /// </summary>
        void ShowTexureMaskExporter()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return;
            }

            var export = EditorWindow.GetWindow<GaiaMaskExporterEditor>(false, "Splatmap Exporter");
            export.Show();
        }

        /// <summary>
        /// Show a grass mask exporter
        /// </summary>
        void ShowGrassMaskExporter()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return;
            }

            var export = EditorWindow.GetWindow<GaiaGrassMaskExporterEditor>(false, "Grassmask Exporter");
            export.Show();
        }

        void ShowFlowMapMaskExporter()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return;
            }

            var export = EditorWindow.GetWindow<GaiaWaterflowMapEditor>(false, "Flowmap Exporter");
            export.Show();
        }

        /// <summary>
        /// Show a terrain obj exporter
        /// </summary>
        void ShowTerrainObjExporter()
        {
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return;
            }

            var export = EditorWindow.GetWindow<ExportTerrain>(false, "Export Terrain");
            export.Show();
        }

        /// <summary>
        /// Export the world as a PNG heightmap
        /// </summary>
        void ExportWorldAsHeightmapPNG()
        {
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return;
            }

            GaiaWorldManager mgr = new GaiaWorldManager(Terrain.activeTerrains);
            if (mgr.TileCount > 0)
            {
                string path = "Assets/GaiaMasks/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                path = Path.Combine(path, Gaia.Utils.FixFileName(string.Format("Terrain-Heightmap-{0:yyyyMMdd-HHmmss}", DateTime.Now)));
                mgr.ExportWorldAsPng(path);
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("Export complete", " Your heightmap has been saved to : " + path, "OK");
            }
        }


        /// <summary>
        /// Export the shore mask as a png file
        /// </summary>
        void ExportShoremaskAsPNG()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return;
            }

            var export = EditorWindow.GetWindow<ShorelineMaskerEditor>(false, "Export Shore");
            export.m_seaLevel = m_resources.m_seaLevel;
            export.Show();
        }

        void ShowExtensionExporterEditor()
        {
            var export = EditorWindow.GetWindow<GaiaExtensionExporterEditor>(false, "Export GX");
            export.Show();
        }

        /*
        /// <summary>
        /// Create a stamp spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateStampSpawner()
        {
            float range = (m_defaults.m_terrainSize / 2) * m_defaults.m_tilesX;
            return m_resources.CreateStampSpawner(range);
        }
         */

        /// <summary>
        /// Display an error if there is not exactly one terrain
        /// </summary>
        /// <param name="requiredTerrainCount">The amount required</param>
        /// <param name="feature">The feature name</param>
        /// <returns>True if an error, false otherwise</returns>
        private bool DisplayErrorIfInvalidTerrainCount(int requiredTerrainCount, string feature = "")
        {
            int actualTerrainCount = Gaia.TerrainHelper.GetActiveTerrainCount();
            if (actualTerrainCount != requiredTerrainCount)
            {
                if (string.IsNullOrEmpty(feature))
                {
                    if (actualTerrainCount < requiredTerrainCount)
                    {
                        EditorUtility.DisplayDialog("OOPS!", string.Format("You currently have {0} active terrains in your scene, but to use this feature you need {1}. Please add terrain.", actualTerrainCount, requiredTerrainCount), "OK");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("OOPS!", string.Format("You currently have {0} active terrains in your scene, but to use this feature you need {1}. Please remove terrain.", actualTerrainCount, requiredTerrainCount), "OK");
                    }
                }
                else
                {
                    if (actualTerrainCount < requiredTerrainCount)
                    {
                        EditorUtility.DisplayDialog("OOPS!", string.Format("You currently have {0} active terrains in your scene, but to use {2} you need {1}. Please add terrain.", actualTerrainCount, requiredTerrainCount, feature), "OK");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("OOPS!", string.Format("You currently have {0} active terrains in your scene, but to use {2} you need {1}. Please remove terrain.", actualTerrainCount, requiredTerrainCount, feature), "OK");
                    }
                }
                
                return true;
            }
            return false;
        }


        /// <summary>
        /// Get the range from the terrain
        /// </summary>
        /// <returns></returns>
        private float GetRangeFromTerrain()
        {
            float range = (m_defaults.m_terrainSize / 2) * m_defaults.m_tilesX;
            Terrain t = Gaia.TerrainHelper.GetActiveTerrain();
            if (t != null)
            {
                range = (Mathf.Max(t.terrainData.size.x, t.terrainData.size.z) / 2f) * m_defaults.m_tilesX;
            }
            return range;
        }

        /// <summary>
        /// Find or create the group spawner in the scene
        /// </summary>
        /// <returns>Returns the group spawner</returns>
        GameObject FindOrCreateGroupSpawner()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            return m_resources.CreateOrFindGroupSpawner().gameObject;
        }


        /// <summary>
        /// Create a texture spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateTextureSpawner()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            return m_resources.CreateCoverageTextureSpawner(GetRangeFromTerrain());
        }

        /// <summary>
        /// Create a detail spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateDetailSpawner()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            return m_resources.CreateCoverageDetailSpawner(GetRangeFromTerrain());
        }

        /// <summary>
        /// Create a clustered detail spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateClusteredDetailSpawner()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            return m_resources.CreateClusteredDetailSpawner(GetRangeFromTerrain());
        }


        /// <summary>
        /// Create a tree spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateClusteredTreeSpawner()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }
            
            return m_resources.CreateClusteredTreeSpawner(GetRangeFromTerrain());
        }

        /// <summary>
        /// Create a tree spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateCoverageTreeSpawner()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            return m_resources.CreateCoverageTreeSpawner(GetRangeFromTerrain());
        }

        /// <summary>
        /// Create a game object spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateCoverageGameObjectSpawner()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            return m_resources.CreateCoverageGameObjectSpawner(GetRangeFromTerrain());
        }



        /// <summary>
        /// Create a game object spawner
        /// </summary>
        /// <returns>Spawner</returns>
        GameObject CreateClusteredGameObjectSpawner()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            return m_resources.CreateClusteredGameObjectSpawner(GetRangeFromTerrain());
        }


        /// <summary>
        /// Create a player
        /// </summary>
        GameObject CreatePlayer()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            GameObject playerObj = GameObject.Find("Player");
            if (playerObj == null)
            {
                string playerPrefabName = m_settings.m_playerPrefabName;
                if (string.IsNullOrEmpty(playerPrefabName))
                {
                    playerPrefabName = "FPSController";
                }

                GameObject fps = Utils.GetAssetPrefab(playerPrefabName);
                if (fps != null)
                {
                    //Place at centre of world at game height
                    Vector3 location = Gaia.TerrainHelper.GetActiveTerrainCenter(true);
                    location.y += 1f;
                    playerObj = Instantiate(fps, location, Quaternion.identity) as GameObject;
                    playerObj.name = "Player";

                    //Make some convenience mods
                    //var firstPersonController = fps.GetComponent("FirstPersonController");
                    //if (firstPersonController != null)
                    //{
                    //    Type firstPersonControllerType = Utils.GetType("UnityStandardAssets.Characters.FirstPerson.FirstPersonController");
                    //    if (firstPersonControllerType != null)
                    //    {

                    //    }
                    //}

                    //Find and raise the camera
                    Transform cameraObj = playerObj.transform.Find("FirstPersonCharacter");
                    if (cameraObj != null)
                    {
                        cameraObj.localPosition = new Vector3(cameraObj.localPosition.x, 1.6f, cameraObj.localPosition.z);
                    }

                    //Put into gaia environment
                    GameObject gaiaObj = GameObject.Find("Gaia Environment");
                    if (gaiaObj == null)
                    {
                        gaiaObj = new GameObject("Gaia Environment");
                    }
                    playerObj.transform.parent = gaiaObj.transform;

                    //Ok - we have added a new camera into the scene - lets disable the existing one
                    GameObject mainCameraObj = GameObject.Find("Main Camera");
                    if (mainCameraObj != null)
                    {
                        mainCameraObj.SetActive(false);
                    }
                }
                else
                {
                    EditorUtility.DisplayDialog("OOPS!", "Unable to locate the FPSCharacter prefab!! Please Import Unity Standard Character Assets and try again.", "OK");
                }
            }
            else
            {
                //Lets just drop them to ground level
                Vector3 location = Gaia.TerrainHelper.GetActiveTerrainCenter(true);
                location.y += 1f;
                playerObj.transform.position = location;
            }
            return playerObj;
        }

        /// <summary>
        /// Create a scene exporter object
        /// </summary>
        /*
        GameObject ShowSceneExporter()
        {
            GameObject exporterObj = GameObject.Find("Exporter");
            if (exporterObj == null)
            {
                exporterObj = new GameObject("Exporter");
                exporterObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter(false);
                GaiaExporter exporter = exporterObj.AddComponent<GaiaExporter>();
                GameObject gaiaObj = GameObject.Find("Gaia");
                if (gaiaObj != null)
                {
                    exporterObj.transform.parent = gaiaObj.transform;
                    exporter.m_rootObject = gaiaObj;
                }
                exporter.m_defaults = m_defaults;
                exporter.m_resources = m_resources;
                exporter.IngestGaiaSetup();
            }
            return exporterObj;
        }
         */

        /// <summary>
        /// Create a wind zone
        /// </summary>
        GameObject CreateWindZone()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            GameObject windZoneObj = GameObject.Find("Wind Zone");
            if (windZoneObj == null)
            {
                windZoneObj = new GameObject("Wind Zone");
                windZoneObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter(false);
                WindZone windZone = windZoneObj.AddComponent<WindZone>();
                windZone.windMain = 0.2f;
                windZone.windTurbulence = 0.2f;
                windZone.windPulseMagnitude = 0.2f;
                windZone.windPulseFrequency = 0.01f;
                GameObject gaiaObj = GameObject.Find("Gaia Environment");
                if (gaiaObj == null)
                {
                    gaiaObj = new GameObject("Gaia Environment");
                }
                windZoneObj.transform.parent = gaiaObj.transform;
            }
            return windZoneObj;
        }


        /// <summary>
        /// Create water
        /// </summary>
        GameObject CreateWater()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }

            GameObject waterObj = GameObject.Find("Water");
            if (waterObj == null)
            {
                string waterPrefabName = m_settings.m_waterPrefabName;
                if (string.IsNullOrEmpty(waterPrefabName))
                {
                    waterPrefabName = "Water4Advanced";
                }

                GameObject waterPrefab = Utils.GetAssetPrefab(waterPrefabName);
                if (waterPrefab != null)
                {
                    GaiaSceneInfo sceneInfo = GaiaSceneInfo.GetSceneInfo();
                    Terrain terrain = Gaia.TerrainHelper.GetActiveTerrain();
                    Vector3 location = sceneInfo.m_centrePointOnTerrain;
                    location.y = sceneInfo.m_seaLevel;
                    waterObj = Instantiate(waterPrefab, location, Quaternion.identity) as GameObject;
                    if (terrain != null)
                    {
                        waterObj.transform.localScale = new Vector3(
                            (Mathf.Max(sceneInfo.m_sceneBounds.size.x, sceneInfo.m_sceneBounds.size.z) * Mathf.Max(m_defaults.m_tilesX, m_defaults.m_tilesZ)) / 100 + 25,
                            0f,
                            (Mathf.Max(sceneInfo.m_sceneBounds.size.x, sceneInfo.m_sceneBounds.size.z) * Mathf.Max(m_defaults.m_tilesX, m_defaults.m_tilesZ)) / 100 + 25);
                    }
                    else
                    {
                        waterObj.transform.localScale = new Vector3(
                            (m_defaults.m_terrainSize * Mathf.Max(m_defaults.m_tilesX, m_defaults.m_tilesZ)) / 100 + 25,
                            0f,
                            (m_defaults.m_terrainSize * Mathf.Max(m_defaults.m_tilesX, m_defaults.m_tilesZ)) / 100 + 25);
                    }
                    waterObj.name = "Water";
                }
                else
                {
                    EditorUtility.DisplayDialog("OOPS!", "Unable to locate the Water4Advanced prefab!! Please Import Unity Standard Environment Assets and try again.", "OK");
                }
                GameObject gaiaObj = GameObject.Find("Gaia Environment");
                if (gaiaObj == null)
                {
                    gaiaObj = new GameObject("Gaia Environment");
                }
                if (waterObj != null)
                {
                    waterObj.transform.parent = gaiaObj.transform;
                }
            }
            return waterObj;
        }

        /// <summary>
        /// Create and return a screen shotter object
        /// </summary>
        /// <returns></returns>
        GameObject CreateScreenShotter()
        {
            //Only do this if we have 1 terrain
            if (DisplayErrorIfInvalidTerrainCount(1))
            {
                return null;
            }
            GameObject shotterObj = GameObject.Find("Screen Shotter");
            if (shotterObj == null)
            {
                shotterObj = new GameObject("Screen Shotter");
                Gaia.ScreenShotter shotter = shotterObj.AddComponent<Gaia.ScreenShotter>();
                shotter.m_watermark = Gaia.Utils.GetAsset("Made With Gaia Watermark.png", typeof(Texture2D)) as Texture2D;

                GameObject gaiaObj = GameObject.Find("Gaia Environment");
                if (gaiaObj == null)
                {
                    gaiaObj = new GameObject("Gaia Environment");
                }
                shotterObj.transform.parent = gaiaObj.transform;
                shotterObj.transform.position = Gaia.TerrainHelper.GetActiveTerrainCenter(false);
            }
            else
            {
                Debug.Log("You already have a Screen Shotter in your scene!");
            }
            return shotterObj;
        }

        /// <summary>
        /// Display a button that takes editor indentation into account
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        bool DisplayButton(GUIContent content)
        {
            TextAnchor oldalignment = GUI.skin.button.alignment;
            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
            Rect btnR = EditorGUILayout.BeginHorizontal();
            btnR.xMin += (EditorGUI.indentLevel * 18f);
            btnR.height += 20f;
            btnR.width -= 4f;
            bool result = GUI.Button(btnR, content);
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(22);
            GUI.skin.button.alignment = oldalignment;
            return result;
        }

        /// <summary>
        /// Get a content label - look the tooltip up if possible
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        GUIContent GetLabel(string name)
        {
            string tooltip = "";
            if (m_tooltips.TryGetValue(name, out tooltip))
            {
                return new GUIContent(name, tooltip);
            }
            else
            {
                return new GUIContent(name);
            }
        }

        /// <summary>
        /// The tooltips
        /// </summary>
        static Dictionary<string, string> m_tooltips = new Dictionary<string, string>
        {
            { "Execution Mode", "The way this spawner runs. Design time : At design time only. Runtime Interval : At run time on a timed interval. Runtime Triggered Interval : At run time on a timed interval, and only when the tagged game object is closer than the trigger range from the center of the spawner." },
        };

   
        /// <summary>
        /// Get the asset path of the first thing that matches the name
        /// </summary>
        /// <param name="name">Name to search for</param>
        /// <returns></returns>
        string GetAssetPath(string name)
        {
            string[] assets = AssetDatabase.FindAssets(name, null);
            if (assets.Length > 0)
            {
                return AssetDatabase.GUIDToAssetPath(assets[0]);
            }
            return null;
        }

        #region GAIA eXtensions GX

        public static List<Type> GetTypesInNamespace(string nameSpace)
        {
            List<Type> gaiaTypes = new List<Type>();

            int assyIdx, typeIdx;
            System.Type[] types;
            System.Reflection.Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            for (assyIdx = 0; assyIdx < assemblies.Length; assyIdx++)
            {
                if (assemblies[assyIdx].FullName.StartsWith("Assembly"))
                {
                    types = assemblies[assyIdx].GetTypes();
                    for (typeIdx = 0; typeIdx < types.Length; typeIdx++ )
                    {
                        if (!string.IsNullOrEmpty(types[typeIdx].Namespace))
                        {
                            if (types[typeIdx].Namespace.StartsWith(nameSpace))
                            {
                                gaiaTypes.Add(types[typeIdx]);
                            }
                        }
                    }
                }
            }
            return gaiaTypes;
        }

        /// <summary>
        /// Return true if image FX have been included
        /// </summary>
        /// <returns></returns>
        public static bool GotImageFX()
        {
            List<Type> types = GetTypesInNamespace("UnityStandardAssets.ImageEffects");
            if (types.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

    }
}
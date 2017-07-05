using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Gaia
{
    /// <summary>
    /// This object stores Gaia settings. It remembers what you have been working on, and resets these when you start up the Gaia Manager window.
    /// </summary>
    public class GaiaSettings : ScriptableObject
    {
        [Tooltip("Current defaults object.")]
        public GaiaDefaults m_currentDefaults;
        [Tooltip("Current resources object.")]
        public GaiaResource m_currentResources;
        [Tooltip("Publisher name for exported extensions.")]
        public string m_publisherName = "";
        [Tooltip("Default prefab name for the player object.")]
        public string m_playerPrefabName = "FPSController";
        [Tooltip("Default prefab name for the water object.")]
        public string m_waterPrefabName = "Water4Advanced";
        [Tooltip("Show or hide tooltips in all custom editors.")]
        public bool m_showTooltips = true;
    }
}
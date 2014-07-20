using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

// presents a ModStatistics compatible interface, to block ModStatistics

namespace ModStatistics
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    internal class ModStatistics : MonoBehaviour
    {
        // has to be a variable, can't be a property
        private static int _version = 0;

        public void Start()
        {
            Debug.Log("[StillBetterThanSpyware] Started");

            int maxValue = 0;
            foreach (var mm in getAllModStatistics())
            {
                var f = mm.GetField("_version", BindingFlags.NonPublic | BindingFlags.Static); // private and static according to forfeit
                if (f != null && f.FieldType == typeof(int))
                {
                    int v = (int)f.GetValue(null);
                    if (v >= maxValue) maxValue = v;
                }
            }

            Debug.Log(String.Format("[StillBetterThanSpyware] Found maxValue: {0}", maxValue));
            if (maxValue == int.MaxValue)
            {
                // we can't set MaxValue + 1, assume ModStatistics has already started
                PopupDialog.SpawnPopupDialog("StillBetterThanSpyware", "WARNING WARNING WARNING\nModStatistics already started. If you don't want to provide pseudoanonymous statistics to ModStatistics check in at the StillBetterThanSpyware thread for assistance.", "OK", false, HighLogic.Skin);
            }
            else
            {
                _version = maxValue + 1; // set our _version to maxValue + 1 so we prevent other ModStatistics instances from starting
            }
        }

        // returns all Types in all loaded Assemblies that use the Type Name ModStatistics, tl;dr gives us a list of all the other ModStatistics DLLs in KSP memory
        private static IEnumerable<Type> getAllModStatistics()
        {
            List<Type> types = new List<Type>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) 
            {
                try
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        if (type.Name != typeof(ModStatistics).Name) continue;
                        types.Add(type);
                    }
                }
                catch
                {
                    Debug.Log("[StillBetterThanSpyware] Exception in getAllModStatistics");
                }
            }
            return types;
        }
    }
}
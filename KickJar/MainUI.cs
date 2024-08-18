using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace KickJar;

[HarmonyPatch(typeof(VersionShower), nameof(VersionShower.Start))] 
public static class VersionShower_Start
{
    
    public static void Postfix(VersionShower __instance)
    {
        var name = $"{Main.ModName} v{Main.PluginVersion}";
        var com = new ColorGradient.Component()
        {
            Gradient = new(new Color32(18, 194, 233, 255), new Color32(196, 113, 237, 255),
                new Color32(246, 79, 89, 255)),
        };
        if (com != null)
        {
            com.Text = name;
            name = com.Generate(false);
        }
        // __instance.text.text =
        //     $"<color={Main.ModColor}>{Main.ModName}</color><color=#00FFFF> v{Main.PluginVersion}";
        // __instance.text.text += Main.ModMode == 1 ? $"C{Main.PluginCanary}" : "";
        // __instance.text.text += "</color>";
        __instance.text.text = name;

    }
}
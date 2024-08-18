using HarmonyLib;
using InnerNet;
using UnityEngine;

namespace KickJar;

//��Դ��https://github.com/tukasa0001/TownOfHost/pull/1265
[HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Start))]
public static class OptionsMenuBehaviourStartPatch
{
    private static ClientOptionItem SwitchVanilla;

#if DEBUG
    private static ClientOptionItem VersionCheat;
    private static ClientOptionItem GodMode;
    private static ClientOptionItem AutoRehost;
#endif

    public static void Postfix(OptionsMenuBehaviour __instance)
    {
        if (__instance.DisableMouseMovement == null) return;
        Main.SwitchVanilla.Value = false;
        
        if (SwitchVanilla == null || SwitchVanilla.ToggleButton == null)
        {
            SwitchVanilla = ClientOptionItem.Create("切换为原版", Main.SwitchVanilla, __instance, SwitchVanillaButtonToggle);
            static void SwitchVanillaButtonToggle()
            {
                if (GetPlayer.isPlayer)
                {
                    AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);
                }
                Harmony.UnpatchAll();
                Main.Instance.Unload();
            }
        }
    }
}

[HarmonyPatch(typeof(OptionsMenuBehaviour), nameof(OptionsMenuBehaviour.Close))]
public static class OptionsMenuBehaviourClosePatch
{
    public static void Postfix()
    {
        ClientOptionItem.CustomBackground?.gameObject.SetActive(false);
    }
}
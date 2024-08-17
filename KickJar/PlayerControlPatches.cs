using HarmonyLib;
using TMPro;

namespace KickJar;

internal class ExilePatch
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    class BaseExileControllerPatch
    {
        public static void Prefix(ExileController __instance)
        {
            if (!AmongUsClient.Instance.AmHost) return;
            try
            {
                if (__instance.initData.networkedPlayer != null)
                {
                    var player = __instance.initData.networkedPlayer;
                    if (player != null)
                    {
                        if (player.Role.IsImpostor)
                            GetPlayer.numImpostors--;
                        else GetPlayer.numCrewmates--;
                    }
                }
            }
            catch { }
        }
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
class FixedUpdatePatch
{
    public static void Postfix(PlayerControl __instance)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        
        if (GetPlayer.IsLobby)
        {
            if (__instance == PlayerControl.LocalPlayer)
            {
                if (!PlayerControl.LocalPlayer.GetRealName().Contains("【罐子游戏】") && !PlayerControl.LocalPlayer
                        .GetRealName().Contains($"KickJar v{Main.PluginVersion}") && !PlayerControl.LocalPlayer.GetRealName().Contains("【系统消息】"))
                {
                    Main.HostRealName = PlayerControl.LocalPlayer.GetRealName();
                }

                if (Main.HostRealName != "" && PlayerControl.LocalPlayer.GetRealName() != "<#0000ff>【罐子游戏】\n<#FFFFFF>" + Main.HostRealName + $"\n<{Main.ModColor}>{Main.ModName} v{Main.PluginVersion}")
                {
                    __instance.RpcSetName("<#0000ff>【罐子游戏】\n<#FFFFFF>" + Main.HostRealName + $"\n<{Main.ModColor}>{Main.ModName} v{Main.PluginVersion}");
                }
            }
        }
                
        __instance.cosmetics.nameText.text = __instance.GetRealName() + "\n";
        __instance.cosmetics.nameText.alignment = TextAlignmentOptions.CenterGeoAligned;
    }
}
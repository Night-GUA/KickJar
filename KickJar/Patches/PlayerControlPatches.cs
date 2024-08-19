using HarmonyLib;
using TMPro;
using UnityEngine;

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
[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ReportDeadBody))]
class ReportDeadBodyPatch
{
    public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] NetworkedPlayerInfo target)
    {
        if (target == null)
        {
            return true;
        }
        Main.Logger.LogWarning(
            $"玩家【{__instance.GetClientId()}:{__instance.GetRealName()}】违反游戏规则：报告【{target?.PlayerName ?? "null"}】，已驳回");
        return false;
    }
}
[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
class PlayerControlCompleteTaskPatch
{
    public static bool Prefix(PlayerControl __instance)
    {
        return false;
    }
}

[HarmonyPatch(typeof(GameManager), nameof(GameManager.CheckTaskCompletion))]
class CheckTaskCompletionPatch
{
    public static bool Prefix(ref bool __result)
    {
        if (!AmongUsClient.Instance.AmHost) return true;
        __result = false;
        return false;
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
                if (!PlayerControl.LocalPlayer.GetRealName().Contains(Main.HostName) &&
                    !PlayerControl.LocalPlayer.GetRealName().Contains(Main.SystemMessageName))
                {
                    Main.HostRealName = PlayerControl.LocalPlayer.GetRealName();
                }
                //var syc = new ColorGradient.Component()
                // {
                //     Gradient = new(new Color32(220, 255, 54, 255), new Color32(63, 255, 54, 255),
                //         new Color32(54, 255, 185, 255)),
                // };
                //
                // if (syc != null)
                // {
                //     syc.Text = Main.HostRealName;
                //     Main.HostRealName = syc.Generate(false);
                // }

                if (Main.HostRealName != "" && PlayerControl.LocalPlayer.GetRealName() != Main.HostName)
                {
                    __instance.RpcSetName(Main.HostName);
                }
            }
        }
                
        // __instance.cosmetics.nameText.text = __instance.GetRealName() + "\n";
        __instance.cosmetics.nameText.alignment = TextAlignmentOptions.CenterGeoAligned;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
class MurderPlayerInHidenSeekPatch
{
    public static void Prefix(PlayerControl __instance,
        [HarmonyArgument(0)] PlayerControl target /*, [HarmonyArgument(1)] MurderResultFlags resultFlags*/)
    {
        if (GetPlayer.IsLobby)
        {
            SendInGamePatch.SendInGame($"恭喜您，您的房间被 {__instance.GetRealName()} 炸辣！\n我们已经尝试拦截，如果没有效果请重开房间");
            if(AmongUsClient.Instance.AmHost) AmongUsClient.Instance.KickPlayer(__instance.GetClientId(),true);
            Logger.Fatal($"房间被炸ERROR，{__instance.GetRealName()}/{__instance.GetClient().ProductUserId}/{__instance.GetClient().FriendCode}","MurderCheck");
        }
    }
}
using AmongUs.Data;
using HarmonyLib;
using InnerNet;

namespace KickJar;

[HarmonyPatch(typeof(IntroCutscene))] //    [HarmonyPatch(nameof(IntroCutscene.CoBegin)), HarmonyPrefix]
class StartPatch
{
    [HarmonyPatch(nameof(IntroCutscene.CoBegin)), HarmonyPrefix]
    public static void Prefix()
    {
        if (!AmongUsClient.Instance.AmHost) return;
        GetPlayer.numImpostors = 0;
        GetPlayer.numCrewmates = 0;
        Main.Logger.LogInfo("[StartPatch]== 游戏开始 ==");
        PlayerControl.LocalPlayer.RpcSetName(Main.HostRealName);
        foreach (var pc1 in Main.AllPlayerControls)
        {
            if (pc1.Data.Role.IsImpostor)
            {
                GetPlayer.numImpostors++;
                pc1.RpcSetName("<#FF3636>" + pc1.GetRealName());
            }
            else
            {
                GetPlayer.numCrewmates++;
            }
        }
    }
}
[HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.BeginGame))]
public class BeginGamePatch
{
    public static bool Prefix(GameStartManager __instance)
    {
        if (!AmongUsClient.Instance.AmHost) return true;
        //Utils.SendMessage(Main.GameRules);
        return true;
    }
}
[HarmonyPatch(typeof(IntroCutscene))]
class IntroCutscenePatch
{
    [HarmonyPatch(nameof(IntroCutscene.OnDestroy)), HarmonyPostfix]
    public static void OnDestroy_Postfix(IntroCutscene __instance)
    {
        if (!AmongUsClient.Instance.AmHost)
        {
            Utils.reportBody(PlayerControl.LocalPlayer.Data);
        }
    }
}
[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CreatePlayer))]
class CreatePlayerPatch
{
    public static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData client)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        _ = new LateTask(() => { Utils.SendMessage(Main.GameRules,client.Character.PlayerId); }, 3f, "SendWelcomeMessage");
        
        //Utils.SendMessage($"罐子游戏规则：\n狼刀人冷却10秒,不得离开大厅只可在大厅内部刀人狼人能破坏反应堆、灭灯、和关闭食堂大门,狼要阻止船员拍桌直到所有好人被杀死,一开始先等好人出去才可以关门\n注意：狼会自动变成红名");
    }
}
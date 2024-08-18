using AmongUs.GameOptions;
using Hazel;
using HarmonyLib;

namespace KickJar;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
internal class RPCHandlerPatch
{
    public static bool ReceiveRpc(PlayerControl pc, byte callId, MessageReader reader)
    {
        if (pc == null || reader == null) return true;
        MessageReader sr = MessageReader.Get(reader);
        var rpc = (RpcCalls)callId;
        switch (rpc)
        {
            case RpcCalls.ReportDeadBody:
                var p1 = GetPlayer.GetPlayerById(sr.ReadByte());
                if (p1 == null) 
                    return true;
                Main.Logger.LogWarning(
                    $"玩家【{pc.GetClientId()}:{pc.GetRealName()}】违反游戏规则：报告【{p1?.GetRealName() ?? "null"}】，已驳回");
                return false;
            break;
        }

        return true;
    }
    public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] byte callId,
        [HarmonyArgument(1)] MessageReader reader)
    {
        Main.Logger.LogMessage("From " +__instance.GetRealName() + "'s RPC:" + callId);
        if (!AmongUsClient.Instance.AmHost) return true;
        return ReceiveRpc(__instance,callId,reader);
    }

}
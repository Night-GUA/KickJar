using HarmonyLib;
using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KickJar;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.UpdateSystem), typeof(SystemTypes), typeof(PlayerControl), typeof(MessageReader))]
public class ShipStatusPatches
{
    public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] SystemTypes systemType,
        [HarmonyArgument(1)] PlayerControl player, [HarmonyArgument(2)] MessageReader reader)
    {
        var amount = MessageReader.Get(reader).ReadByte();
        return RpcUpdateSystemCheck(player,systemType,amount);
    }

    public static bool RpcUpdateSystemCheck(PlayerControl player, SystemTypes systemType, byte amount)
    {
        if (!AmongUsClient.Instance.AmHost) return true;
        var Mapid = GetPlayer.GetActiveMapId();
        Main.Logger.LogInfo("Check sabotage RPC" + ", PlayerName: " + player.GetRealName() + ", SabotageType: " + systemType.ToString() + ", amount: " + amount.ToString());
        Main.Logger.LogInfo("触发飞船事件！"+player.GetRealName()+$"是{player.GetPlayerRoleTeam().ToString()}阵营！");
        if (player == null) return true;
        if (systemType is SystemTypes.Comms or SystemTypes.LifeSupp)
        {
            if (GetPlayer.numImpostors == 1) return true;
            Main.Logger.LogFatal($"{player.GetRealName()} 违反游戏规则");
            return false;
        }
        return true;
    }
}

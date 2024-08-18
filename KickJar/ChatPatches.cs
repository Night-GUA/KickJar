using HarmonyLib;
using Assets.CoreScripts;
using Hazel;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using InnerNet;

namespace KickJar;

[HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
internal class ChatUpdatePatch
{
    public static float chatStop = 3.5f;
    public static bool send = false;

    public static void Postfix(ChatController __instance)
    {
        chatStop = __instance.timeSinceLastMessage;
        if (send)
        {
            Main.Logger.LogInfo("设置tslm");
            __instance.timeSinceLastMessage = 0f;
            send = !send;
        }
    }
}

[HarmonyPatch(typeof(ChatBubble))]
public static class ChatBubblePatch
{

    [HarmonyPatch(nameof(ChatBubble.SetText)), HarmonyPrefix]
    public static void SetText_Prefix(ChatBubble __instance, ref string chatText)
    {
        // if (__instance.playerInfo == PlayerControl.LocalPlayer.Data)
        // {
        //     ChatUpdatePatch.send = true;
        // }
        if (!AmongUsClient.Instance.AmHost) return;
        switch (chatText)
        {
            case "/r":
            case "/rule":
            case "/rules":
                Utils.SendMessage(Main.GameRules);
                break;
            case "/h":
            case "/help":
                Utils.SendMessage(Main.GameHelp);
                break;
            case "/quit":
            case "/qt":
            case "/sair":
                Utils.SendMessage($"很抱歉让您怼该房间厌恶，我们已将{__instance.playerInfo.PlayerName}踢出！（我们真的在努力了）");
                AmongUsClient.Instance.KickPlayer(__instance.playerInfo.ClientId,true);
                break;
        }
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
internal class ChatCommands
{
    public static bool Prefix(ChatController __instance)
    {
        //if()
        PlayerControl.LocalPlayer.RpcSetName(Main.HostRealName);
        return true;
    }
}
[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSendChat))]
class RpcSendChatPatch
{
    public static bool Prefix(PlayerControl __instance, string chatText, ref bool __result)
    {
        if (__instance == PlayerControl.LocalPlayer)
        {
            ChatUpdatePatch.send = true;
        }
        if (string.IsNullOrWhiteSpace(chatText))
        {
            __result = false;
            return false;
        }
        int return_count = PlayerControl.LocalPlayer.name.Count(x => x == '\n');
        chatText = new StringBuilder(chatText).Insert(0, "\n", return_count).ToString();
        if (AmongUsClient.Instance.AmClient && DestroyableSingleton<HudManager>.Instance)
            DestroyableSingleton<HudManager>.Instance.Chat.AddChat(__instance, chatText);
        if (chatText.Contains("who", StringComparison.OrdinalIgnoreCase))
            DestroyableSingleton<UnityTelemetry>.Instance.SendWho();
        MessageWriter messageWriter = AmongUsClient.Instance.StartRpc(__instance.NetId, (byte)RpcCalls.SendChat, SendOption.None);
        messageWriter.Write(chatText);
        messageWriter.EndMessage();
        __result = true;
        return false;
    }
}
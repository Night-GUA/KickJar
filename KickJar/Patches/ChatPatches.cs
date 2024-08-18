using HarmonyLib;
using Assets.CoreScripts;
using Hazel;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using InnerNet;
using UnityEngine;

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
        if (!AmongUsClient.Instance.AmHost || Main.MessagesToSend.Count == 0 || Main.MessagesToSend[0].Item2 == byte.MaxValue ||3.5 > __instance.timeSinceLastMessage) return;
        if (Main.MessagesToSend.Count != 1)
        {
            __instance.timeSinceLastMessage = 0f;
        }
        var player = PlayerControl.LocalPlayer;
        //Logger.Info($"player is null? {player == null}", "ChatUpdatePatch");
        if (player == null) return;

        (string msg, byte sendTo, string title) = Main.MessagesToSend[0];
        Main.Logger.LogInfo($"MessagesToSend - sendTo: {sendTo} - title: {title}");
        
        if (sendTo != byte.MaxValue && GetPlayer.IsLobby)
        {
            if (GetPlayer.GetPlayerInfoById(sendTo) != null)
            {
                var targetinfo = GetPlayer.GetPlayerInfoById(sendTo);

                if (targetinfo.DefaultOutfit.ColorId == -1)
                {
                    var delaymessage = Main.MessagesToSend[0];
                    Main.MessagesToSend.RemoveAt(0);
                    Main.MessagesToSend.Add(delaymessage);
                    return;
                }
                // green beans color id is -1
            }
            // It is impossible to get null player here unless it quits
        }
        Main.MessagesToSend.RemoveAt(0);

        int clientId = sendTo == byte.MaxValue ? -1 : GetPlayer.GetPlayerById(sendTo).GetClientId();
        var name = player.Data.PlayerName;

        //__instance.freeChatField.textArea.characterLimit = 999;

        if (clientId == -1)
        {
            player.SetName(title);
            DestroyableSingleton<HudManager>.Instance.Chat.AddChat(player, msg, false);
            player.SetName(name);
        }
        
        
        var writer = CustomRpcSender.Create("MessagesToSend", SendOption.None);
        writer.StartMessage(clientId);
        writer.StartRpc(player.NetId, (byte)RpcCalls.SetName)
            .Write(player.Data.NetId)
            .Write(title)
            .EndRpc();
        writer.StartRpc(player.NetId, (byte)RpcCalls.SendChat)
            .Write(msg)
            .EndRpc();
        writer.StartRpc(player.NetId, (byte)RpcCalls.SetName)
            .Write(player.Data.NetId)
            .Write(player.Data.PlayerName)
            .EndRpc();
        writer.EndMessage();
        writer.SendMessage();

        __instance.timeSinceLastMessage = 0f;
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
                Utils.SendMessage(Main.GameRules,__instance.playerInfo.PlayerId);
                break;
            case "/h":
            case "/help":
                Utils.SendMessage(Main.GameHelp,__instance.playerInfo.PlayerId);
                break;
            case "/quit":
            case "/qt":
            case "/sair":
                Utils.SendMessage($"很抱歉让您对该房间厌恶，我们已将{__instance.playerInfo.PlayerName}踢出！（我们真的在努力了）",__instance.playerInfo.PlayerId);
                AmongUsClient.Instance.KickPlayer(__instance.playerInfo.ClientId,true);
                break;
        }
    }
}

[HarmonyPatch(typeof(ChatController), nameof(ChatController.SendChat))]
internal class ChatCommands
{
    public static bool isSending;
    public static bool Prefix(ChatController __instance)
    {
        //if()
        isSending = true;
        PlayerControl.LocalPlayer.RpcSetName(Main.HostRealName);
        return true;
    }

    public static void Postfix()
    {
        isSending = false;
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
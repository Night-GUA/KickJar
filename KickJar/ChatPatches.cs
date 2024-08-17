using HarmonyLib;
using Assets.CoreScripts;
using Hazel;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

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
        switch (chatText)
        {
            case "/r":
            case "/rule":
            case "/rules":
                Utils.SendMessage(Main.GameRules);
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
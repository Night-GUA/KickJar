using UnityEngine;
using InnerNet;
using System.Linq;
// using Il2CppSystem.Collections.Generic;
using System.IO;
using Hazel;
using System.Reflection;
using AmongUs.GameOptions;
using Sentry.Internal.Extensions;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace KickJar;

public static class Utils
{
    public static void reportBody(NetworkedPlayerInfo pc)
    {
        var HostData = AmongUsClient.Instance.GetHost();
        if (HostData != null && !HostData.Character.Data.Disconnected)
        {
            MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RpcCalls.ReportDeadBody, SendOption.None, HostData.Id);
            writer.Write(pc.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }
    }
    
    public static void SendMessage(string s)
    {
        if (ChatUpdatePatch.chatStop >= 3.5)
        {
            //ChatUpdatePatch.send = true;
            ChatUpdatePatch.send = true;
            _ = new LateTask(() =>
            {
                PlayerControl.LocalPlayer.RpcSetName("<#0000ff>【系统消息】");
                PlayerControl.LocalPlayer.RpcSendChat(s);
                
                ChatUpdatePatch.send = true;
            }, 1.0f, "SendMessage");
        }
        else
        {
            //ChatUpdatePatch.send = true;
            _ = new LateTask(() =>
            {
                if (ChatUpdatePatch.chatStop >= 3.1)
                {
                    PlayerControl.LocalPlayer.RpcSetName("<#0000ff>【系统消息】");
                    PlayerControl.LocalPlayer.RpcSendChat(s);
                    ChatUpdatePatch.send = true;
                }
                else
                {
                    SendMessage(s);
                }
                
            }, 3.1f, "SendMessage");
        }
    }
    public static void SendMessage(string text, byte sendTo = byte.MaxValue, string title = "", bool logforChatManager = false, bool noReplay = false, bool ShouldSplit = false)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        try
        {
            if (ShouldSplit && text.Length > 1200)
            {
                text.SplitMessage().Do(x => SendMessage(x, sendTo, title));
                return;
            }
            //else if (text.Length > 1200 && (!GetPlayerById(sendTo).IsModClient()))
            //{
            //    text = text.RemoveHtmlTagsIfNeccessary();
            //}
        }
        catch (Exception exx)
        {
            Main.Logger.LogWarning($"Error after try split the msg {text} at: {exx}");
        }

        // set noReplay to false when you want to send previous sys msg or do not want to add a sys msg in the history
        if (!noReplay && GetPlayer.IsInGame) ChatManager.AddSystemChatHistory(sendTo, text);

        if (title == "") title = "<color=#aaaaff>" + "<#0000ff>【系统消息】" + "</color>";

        Main.MessagesToSend.Add((text.RemoveHtmlTagsTemplate(), sendTo, title));
    }
    public static string[] SplitMessage(this string LongMsg)
    {
        List<string> result = [];
        var lines = LongMsg.Split('\n');
        var shortenedtext = string.Empty;

        foreach (var line in lines)
        {

            if (shortenedtext.Length + line.Length < 1200)
            {
                shortenedtext += line + "\n";
                continue;
            }

            if (shortenedtext.Length >= 1200) result.AddRange(shortenedtext.Chunk(1200).Select(x => new string(x)));
            else result.Add(shortenedtext);
            shortenedtext = line + "\n";

        }

        if (shortenedtext.Length > 0) result.Add(shortenedtext);

        return [.. result];
    }
    public static string RemoveHtmlTagsTemplate(this string str) => Regex.Replace(str, "", "");
    public static string getColoredPingText(int ping){

        if (ping <= 100){ // Green for ping < 100

            return $"<color=#00ff00ff>{ping}";//</color>";

        } else if (ping < 400){ // Yellow for 100 < ping < 400

            return $"<color=#ffff00ff>{ping}";//</color>";

        } else{ // Red for ping > 400

            return $"<color=#ff0000ff>{ping}";//</color>";
        }
    }
    public static string ColorString(Color32 color, string str) => $"<color=#{color.r:x2}{color.g:x2}{color.b:x2}{color.a:x2}>{str}</color>";

    public static string getColoredFPSText(float fps)
    {
        string a = "";
        if (fps >= 100){ // Green for fps > 100

            return a + $"<color=#00ff00ff>{fps}";//</color>";

        } else if (fps < 100 & fps > 50){ // Yellow for 100 > fps > 50

            return a + $"<color=#ffff00ff>{fps}";//</color>";

        } else{ // Red for fps < 50

            return a + $"<color=#ff0000ff>{fps}";//</color>";
        }
    }
    public static Color32 GetRoleColor(RoleTypes rt)
    {
        Color32 c = new Color32();
        switch (rt)
        {
            /*=== 船员 === */
            case RoleTypes.Crewmate:
                c = new Color32(30,144,255,byte.MaxValue); // 船员 => 道奇蓝
                break;
            
            case RoleTypes.Noisemaker:
                c = new Color32(0,191,255,byte.MaxValue); // 大嗓门 => 深天蓝
                break;
            
            case RoleTypes.Scientist:
                c = new Color32(0,255,255,byte.MaxValue); // 科学家 => 青色
                break;
            
            case RoleTypes.Engineer:
                c = new Color32(127,255,170,byte.MaxValue); // 工程师 => 绿玉
                break;
            
            case RoleTypes.Tracker:
                c = new Color32(0,128,128,byte.MaxValue); // 追踪 => 水鸭色
                break;
            
            /*=== 内鬼 === */
            case RoleTypes.Impostor:
                c = new Color32(255,0,0,byte.MaxValue); // 内鬼 => 纯红
                break;
            
            case RoleTypes.Shapeshifter:
                c = new Color32(255,69,0,byte.MaxValue); // 变形 => 橙红
                break;
            
            case RoleTypes.Phantom:
                c = new Color32(250,128,114,byte.MaxValue); // 隐身 => 鲜肉
                break;
            
            /*=== 灵魂 === */
            case RoleTypes.CrewmateGhost:
                c = new Color32(220,220,220,byte.MaxValue); // 船员灵魂 => 亮灰色
                break;
            
            case RoleTypes.GuardianAngel:
                c = new Color32(240,128,128,byte.MaxValue); // 天使 => 淡珊瑚
                break;
            
            case RoleTypes.ImpostorGhost:
                c = new Color32(255,228,225,byte.MaxValue); // 内鬼灵魂 => 薄雾玫瑰
                break;
            
        }

        return c;
    }
}
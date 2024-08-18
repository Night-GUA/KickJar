using Hazel;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace KickJar
{
    public class ChatManager
    {
        public static bool cancel = false;
        private static List<Dictionary<byte, string>> chatHistory = [];
        private static Dictionary<byte, string> LastSystemChatMsg = [];
        private const int maxHistorySize = 20;
        public static List<string> ChatSentBySystem = [];
        public static void ResetHistory()
        {
            chatHistory = [];
            LastSystemChatMsg = [];
        }
        public static void ClearLastSysMsg()
        {
            LastSystemChatMsg.Clear();
        }
        public static void AddSystemChatHistory(byte playerId, string msg)
        {
            LastSystemChatMsg[playerId] = msg;
        }
        public static bool CheckCommond(ref string msg, string command, bool exact = true)
        {
            var comList = command.Split('|');
            foreach (string comm in comList)
            {
                if (exact)
                {
                    if (msg == "/" + comm) return true;
                }
                else
                {
                    if (msg.StartsWith("/" + comm))
                    {
                        msg = msg.Replace("/" + comm, string.Empty);
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool CheckName(ref string msg, string command, bool exact = true)
        {
            var comList = command.Split('|');
            foreach (var com in comList)
            {
                if (exact)
                {
                    if (msg.Contains(com))
                    {
                        return true;
                    }
                }
                else
                {
                    int index = msg.IndexOf(com);
                    if (index != -1)
                    {
                        msg = msg.Remove(index, com.Length);
                        return true;
                    }
                }
            }
            return false;
        }

        private static string GetTextHash(string text)
        {
            using SHA256 sha256 = SHA256.Create();

            // get sha-256 hash
            byte[] sha256Bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(text));
            string sha256Hash = BitConverter.ToString(sha256Bytes).Replace("-", "").ToLower();

            // pick front 5 and last 4
            return string.Concat(sha256Hash.AsSpan(0, 5), sha256Hash.AsSpan(sha256Hash.Length - 4));
        }

        public static void AddToHostMessage(string text)
        {
            if (text != "")
            {
                ChatSentBySystem.Add(GetTextHash(text));
            }
        }
        
    }
}
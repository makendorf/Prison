using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;

namespace Network
{
    [Serializable]
    public struct Message
    {
        public string Text;
        public string Name;

        public override string ToString()
        {
            return "Name: " + Name + " Text: " + Text;
        }

        public Message(string name, string text)
        {
            Name = name;
            Text = text;
        }
    }
}

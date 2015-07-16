using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QPP.Wpf.Command
{
    public class EventCommandAttribute : Attribute
    {
        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 事件名稱
        /// </summary>
        public string EventName { get; set; }
        /// <summary>
        /// 目標
        /// </summary>
        public string Target { get; set; }

        public EventCommandAttribute()
        {
        }

        public EventCommandAttribute(string name)
        {
            Name = name;
        }

        public EventCommandAttribute(string name, string eventName)
        {
            Name = name;
            EventName = eventName;
        }

        public EventCommandAttribute(EventCommands eventCommand)
        {
            var e = GetCommand(eventCommand);
            Name = e.Name;
            EventName = e.EventName;
        }

        static EventCommandAttribute GetCommand(EventCommands cmd)
        {
            switch (cmd)
            {
                case EventCommands.Closed: return ClosedEvent;
                case EventCommands.Closing: return ClosingEvent;
                case EventCommands.Deserializing: return DeserializingEvent;
                case EventCommands.Loaded: return LoadedEvent;
                case EventCommands.Serializing: return SerializingEvent;
            }
            return null;
        }
        static EventCommandAttribute()
        {
            LoadedEvent = new EventCommandAttribute("LoadedEvent", "Loaded");
            ClosingEvent = new EventCommandAttribute("ClosingEvent", "Closing");
            ClosedEvent = new EventCommandAttribute("ClosedEvent", "Closed");
            SerializingEvent = new EventCommandAttribute("SerializingEvent", "Serializing");
            DeserializingEvent = new EventCommandAttribute("DeserializingEvent", "Deserializing");
        }

        public static EventCommandAttribute LoadedEvent;
        public static EventCommandAttribute ClosingEvent;
        public static EventCommandAttribute ClosedEvent;
        public static EventCommandAttribute SerializingEvent;
        public static EventCommandAttribute DeserializingEvent;
    }
}

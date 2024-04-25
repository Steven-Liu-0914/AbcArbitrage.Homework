using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbcArbitrage.Homework.Routing
{
    public class Message
    {
        public ClientId ClientId { get; set; }

        public IMessage? message { get; set; }

        public MessagePriority Priority { get; set; }

        public MessageTypeId MessageTypeId { get; set; }
    }
}

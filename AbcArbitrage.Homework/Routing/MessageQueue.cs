// Copyright (C) Abc Arbitrage Asset Management - All Rights Reserved
// Unauthorized copying of this file, via any medium is strictly prohibited
// Proprietary and confidential
// Written by Olivier Coanet <o.coanet@abc-arbitrage.com>, 2020-10-06

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace AbcArbitrage.Homework.Routing
{
    public class MessageQueue
    {
        public List<Message> Data_Message;
        private readonly object lockObject = new object();

        public MessageQueue()
        {
            Data_Message = new List<Message>();
        }
        
        public void EnqueueForClient(ClientId clientId, IMessage message, MessagePriority priority = MessagePriority.Normal)
        {
            lock (lockObject)
            {
                // TODO
                Message _message = new Message();
                _message.ClientId = clientId;
                _message.MessageTypeId = MessageTypeId.FromMessage(message);
                _message.Priority = priority;
                _message.message = message;

                Data_Message.Add(_message);
            }
        }

        public bool TryDequeueForClient(ClientId clientId, out IMessage? message)
        {
            lock (lockObject)
            {
                // TODO
                var Client_Msg = Data_Message.Where(x => x.ClientId.Equals(clientId));

                if (Client_Msg.Any())
                {
                    var result = Client_Msg.OrderByDescending(p => p.Priority).First();
                    message = result.message;
                    Data_Message.Remove(result);

                    return true;
                }

                message = default;

                return false;
            }
        }
    }
}

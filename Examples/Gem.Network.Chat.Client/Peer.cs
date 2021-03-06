﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gem.Network.Messages;
using Gem.Network.Events;
using Gem.Network.Client;
using Gem.Network.Async;
using System.Collections.Concurrent;
using System.Threading;
using Gem.Network.Protocol;
using Gem.Network.Chat.Protocol;

namespace Gem.Network.Chat.Client
{

    public class Peer
    {
        public string Name { get; set; }
        public ConcurrentQueue<string> IncomingMessages;
        private readonly ParallelTaskStarter messageAppender;
        private readonly INetworkEvent onEvent;
        private readonly INetworkEvent onCommandExecute;
        private readonly INetworkEvent protocolExample;

        public Peer(string name)
        {
            CanAppend = true;
            IncomingMessages = new ConcurrentQueue<string>();
            this.Name = name;
            
            protocolExample = GemClient.Profile("GemChat")
                  .CreateNetworkProtocolEvent<Package>()
                  .HandleIncoming((sender, package) =>
                   {
                       QueueMessage(String.Format("Server sent {0} {1}", sender.Statistics.SentBytes, package.Name));
                   })
                  .GenerateSendEvent();

            onEvent = GemClient.Profile("GemChat")
                  .CreateNetworkEvent
                  .AndHandleWith(this, x => new Action<string>(x.QueueMessage));

            onCommandExecute = GemClient.Profile("GemChat")
                  .CreateNetworkEvent
                  .AndHandleWith(this, x => new Action<string>(x.ExecuteCommand));
            
            onEvent.Send(name + " has joined");
            messageAppender = new ParallelTaskStarter(TimeSpan.Zero);
            messageAppender.Start(DequeueIncomingMessages);
        }

        public void Send(string message)
        {
            onEvent.Send(message);
        }
        
        public void SendCommand(string cmd)
        {
            onCommandExecute.Send(cmd);
        }       

        public void ExecuteCommand(string cmd)
        {
            Chat.Executecommand(cmd);
        }

        public void Say(string message)
        {
            string formattedMessage = String.Format(" >> {0} : {1}", Name, message);
            Console.WriteLine("{0} : {1}", Name, message);

            onEvent.Send(formattedMessage);
            //protocolExample.Send(new Package { Name = "invoked protocol " + Name });
        }

        public void ChangeName(string newName)
        {
            string formattedMessage = String.Format(" >> {0} changed his/her name to {1}", Name, newName);

            Console.WriteLine("you have changed your name to {0}", newName);

            this.Name = newName;

            onEvent.Send(formattedMessage);
            GemClient.NotifyServer("newname " + newName);
        }

        public void QueueMessage(string message)
        {
            IncomingMessages.Enqueue(message);
        }

        public bool CanAppend { get; set; }

        private void DequeueIncomingMessages()
        {
            if(Console.CursorLeft==0 && CanAppend)
            {
                Thread.Sleep(10);
                foreach (var msg in IncomingMessages)
                {
                    string message = string.Empty;
                    while (IncomingMessages.TryDequeue(out message))
                    Console.WriteLine(message);
                }
            }
        }
        
        public void SayGoodBye()
        {
            messageAppender.Stop();
            onEvent.Send(" >> " + Name + " has left ");
        }

    }
}

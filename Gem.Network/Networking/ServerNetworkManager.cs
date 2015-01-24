﻿namespace Gem.Network
{
    using System;
    using Lidgren.Network;
    using Gem.Network.Messages;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The server class
    /// </summary>
    public class ServerNetworkManager : INetworkManager
    {

        #region Constructor 

        public ServerNetworkManager(int maxConnections)
        {
            deliveryMethod = NetDeliveryMethod.ReliableUnordered;
            maxConnections = 4;
            disconnectMessage = "bye";
            SendToAll = false;
            sequenceChannel = 1;
            registeredConnections = new Dictionary<string, NetConnection>();
        }

        #endregion

        #region Constants and Fields

        private readonly int maxConnections;

        private bool AllowConnections
        {
            get
            {
                return registeredConnections.Count < maxConnections;
            }
        }

        private Action<NetOutgoingMessage> MessageHandler;

        /// <summary>
        /// The message delivery sequence channel
        /// </summary>
        private readonly int sequenceChannel;
        
        /// <summary>
        /// How the message is delivered
        /// </summary>
        private readonly NetDeliveryMethod deliveryMethod;

        /// <summary>
        /// The is disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// The net server.
        /// </summary>
        private NetServer netServer;

        /// <summary>
        /// The connected users.
        /// </summary>
        private Dictionary<string,NetConnection> registeredConnections;

        /// <summary>
        /// True if the outgoing message is sent to all
        /// </summary>
        public bool SendToAll
        {
            set
            {
                if (value)
                {
                    MessageHandler = SendMessageToAll;
                }
                else
                {
                    MessageHandler = SendMessageToRegistered;
                }
            }
        }

        /// <summary>
        /// This is shown when the server shuts down
        /// </summary>
        private readonly string disconnectMessage;
        
        #endregion


        #region Public Methods and Operators

        /// <summary>
        /// Initialize a new connection
        /// </summary>
        public void Start(string serverName, int port)
        {
            var config = new NetPeerConfiguration(serverName)
                {
                    Port = Convert.ToInt32(port)
                };
            config.EnableMessageType(NetIncomingMessageType.WarningMessage);
            config.EnableMessageType(NetIncomingMessageType.VerboseDebugMessage);
            config.EnableMessageType(NetIncomingMessageType.ErrorMessage);
            config.EnableMessageType(NetIncomingMessageType.Error);
            config.EnableMessageType(NetIncomingMessageType.DebugMessage);
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);

            this.netServer = new NetServer(config);
            this.netServer.Start();
        }

        /// <summary>
        /// Registers a new connection to the server.
        /// Returns false if the connection's identifier is inuse
        /// </summary>
        /// <param name="identifier">The peer's identifier</param>
        /// <param name="connection">The peer's connection details</param>
        /// <returns>Registration success</returns>
        public bool RegisterConnection(string identifier, NetConnection connection)
        {
            if (AllowConnections)
            {
                if (registeredConnections.ContainsKey(identifier))
                {
                    registeredConnections.Add(identifier, connection);
                    return true;
                }
            }
            return false;
        }       

        /// <summary>
        /// Removes a connection
        /// </summary>
        /// <param name="identifier">The connection's identifier</param>
        /// <returns>The removal success</returns>
        public bool DeRegisterConnection(string identifier)
        {
            return registeredConnections.Remove(identifier);
        }

        /// <summary>
        /// The create message.
        /// </summary>
        /// <returns>
        /// </returns>
        public NetOutgoingMessage CreateMessage()
        {
            return this.netServer.CreateMessage();
        }

        /// <summary>
        /// The disconnect.
        /// </summary>
        public void Disconnect()
        {
            this.netServer.Shutdown(disconnectMessage);
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        /// The read message.
        /// </summary>
        /// <returns>
        /// </returns>
        public NetIncomingMessage ReadMessage()
        {
            return this.netServer.ReadMessage();
        }

        /// <summary>
        /// The recycle.
        /// </summary>
        /// <param name="im">
        /// The im.
        /// </param>
        public void Recycle(NetIncomingMessage im)
        {
            this.netServer.Recycle(im);
        }

        /// <summary>
        /// The send message.
        /// </summary>
        /// <param name="gameMessage">
        /// The game message.
        /// </param>
        public void SendMessage(IGameMessage gameMessage)
        {
            NetOutgoingMessage om = this.netServer.CreateMessage();
            om.Write((byte)gameMessage.MessageType);
            gameMessage.Encode(om);

            MessageHandler(om);          
        }

        private void SendMessageToAll(NetOutgoingMessage om)
        {
            this.netServer.SendToAll(om, deliveryMethod);
        }

        private void SendMessageToRegistered(NetOutgoingMessage om)
        {
            this.netServer.SendMessage(om, registeredConnections.Values.ToList(), deliveryMethod, sequenceChannel);
        }

        #endregion


        #region Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="disposing">
        /// The disposing.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.Disconnect();
                }
                this.isDisposed = true;
            }
        }

        #endregion
    }
}
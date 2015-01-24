﻿﻿using System;
using Lidgren.Network;

namespace Gem.Network
{
    public class ServerHandler : IDisposable
    {

        #region Declarations

        private readonly INetworkManager networkManager;

        public string Name { get; private set; }

        public bool IsRunning { get; private set; }

        private readonly int maxConnections;

        #endregion


        #region Constructor

        public ServerHandler(INetworkManager networkManager, string name, int port,int maxConnections)
        {
            this.maxConnections = maxConnections;
            this.Name = name;
            this.networkManager = networkManager;

            try
            {
                networkManager.Start(name, port);
                IsRunning = true;
            }
            catch (Exception ex)
            {
                //TODO: log this
                IsRunning = false;
            }

            InitializeEvents();

        }

        #endregion


        #region Close Connection

        public void Disconnect()
        {
            networkManager.Disconnect();
            IsRunning = false;

        }

        public void Dispose()
        {
            Disconnect();
        }

        #endregion


        #region Initialize Events

        public void InitializeEvents()
        {
            // bannerManager.RequestBanner += (sender, e) => networkManager.SendMessage(new RequestBannerMessage(e.Name, e.Color, e.IsUsingAvatar, e.Pos));
        }

        #endregion


        #region Event Calls

        //public void OnPlayerDisconnect(string name,int color,int pos)
        //{
        //    EventHandler<BannerArgs> playerDisconnected = PlayerDisconnected;

        //    if (playerDisconnected != null)
        //        playerDisconnected(PlayerDisconnected, new BannerArgs(name, color, false,pos));
        //}


        #endregion


        #region Event Handling

        //private void HandleAddPointingAnimationMessage(NetIncomingMessage im)
        //{
        //    var message = new AddPointingAnimationMessage(im);

        //    if (message.Color != TileGrid.ColorComparer(TileGrid.activeColor))
        //        animationManager.AddPointingAnimation(message.Location * (Camera.Scale / message.Scale), TileGrid.ColorComparer(message.Color));

        //    if (this.IsHost)
        //        animationManager.OnAddPointingAnimation(message.Location, message.Color, message.Scale);
        //}


        #endregion


        #region Messages

        private void ProcessNetworkMessages()
        {
            NetIncomingMessage im;

            while ((im = this.networkManager.ReadMessage()) != null)
            {
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.ConnectionApproval:
                        if (im.ReadByte() == (byte)PacketTypes.LOGIN)
                        {
                            //Append to a listener
                            Console.WriteLine("Incoming LOGIN");
                            im.SenderConnection.Approve();
                            networkManager.RegisterConnection("replace this",im.SenderConnection);

                            //Send a message to notify the others
                            NetOutgoingMessage outmsg = networkManager.CreateMessage();    
                            //networkManager.SendMessage(outmsg, im.SenderConnection, NetDeliveryMethod.ReliableOrdered, 0);
                            Console.WriteLine("Approved new connection");
                        }
                        break;
                    case NetIncomingMessageType.StatusChanged:
                        switch ((NetConnectionStatus)im.ReadByte())
                        {
                            case NetConnectionStatus.Connected:
                                //Append to listener
                                Console.WriteLine("Connected to {0}");
                                break;
                            case NetConnectionStatus.Disconnected:
                                     Console.WriteLine(im.SenderConnection.ToString() + " status changed. " + (NetConnectionStatus)im.SenderConnection.Status);
                            if (im.SenderConnection.Status == NetConnectionStatus.Disconnected
                                || im.SenderConnection.Status == NetConnectionStatus.Disconnecting)
                            {
                                //Deregister peer
                                //this.networkManager.DeRegisterConnection(im.Name);
                            }
                                break;

                            case NetConnectionStatus.RespondedConnect:
                                //NetOutgoingMessage hailMessage = this.networkManager.CreateMessage();
                                //im.SenderConnection.Approve(hailMessage);
                                break;
                        }

                        break;
                    case NetIncomingMessageType.Data:
                        //var gameMessageType = (GameMessageTypes)im.ReadByte();
                        //switch (gameMessageType)
                        //{
                        //    case GameMessageTypes.RequestBannerState:
                        //        this.HandleRequestBannerMessage(im);
                        //        break;
                        //}
                        break;
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        //Append to listener
                        Console.WriteLine(im.ReadString());
                        break;
                }
                this.networkManager.Recycle(im);
            }
        }

        #endregion

    }
}
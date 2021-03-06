﻿using Gem.Network.Builders;
using Gem.Network.Client;
using Gem.Network.Messages;
using Lidgren.Network;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using TechTalk.SpecFlow;

//@client
//Scenario: ClientSuccessfulConnect
//    Given  A server is running
//    And I connect to the server
//    When I send a greeding message
//    Then I should get a response message and connection approval

namespace Gem.Network.Tests.Flow
{
    [Binding]
    public class ClientCasesSteps
    {
        private Process server;
        private IClient client;
        private NetIncomingMessage msg;
        private List<RuntimePropertyInfo> propertyList = new List<RuntimePropertyInfo>
            {
                new RuntimePropertyInfo{
                        PropertyName = "Name",
                        PropertyType = typeof(string)
                }
            };
        private Type myNewType;
        private IPocoBuilder pocoBuilder = new ReflectionEmitBuilder();
        [Given(@"A server is running")]
        public void GivenAServerIsRunning()
        {
            server = new Process();
            try
            {
                server.StartInfo.FileName = @"C:\Users\George\Documents\GitHub\Gem\Gem.Network.Example\bin\Debug\Gem.Network.Example.exe";
                server.StartInfo.Arguments = "DecodeIncomingDynamicMessageTest";
                server.Start();
            }
            catch (Exception ex)
            {
                Assert.Fail("Failed to launch the server. Reason:" + ex.Message);
            }
        }

        [Given(@"I connect to the server")]
        public void GivenIConnectToTheServer()
        {
            myNewType = pocoBuilder.Build("POCO", propertyList);
            client = new Peer();
            //client.Connect(new ConnectionConfig
            //{
            //    IPorHost = "127.0.0.1",
            //    Port = 14241,
            //    ServerName = "local"
            //});
        }

        [When(@"I send a greeding message")]
        public void WhenISendAGreedingMessage()
        {
            //wait for the server to respond
            System.Threading.Thread.Sleep(100);
            msg = client.ReadMessage();

            var om = client.CreateMessage();
            client.SendMessage(om);


        }

        [Then(@"I should get a response message and connection approval")]
        public void ThenIShouldGetAResponseMessageAndConnectionApproval()
        {
            //wait for the server to respond
            System.Threading.Thread.Sleep(100);
            msg = client.ReadMessage();

            dynamic readableMessageWithType = MessageSerializer.Decode(msg, myNewType);
            Assert.AreEqual(readableMessageWithType.Name, "DynamicType");
            server.CloseMainWindow();
            server.Close();

        }
    }
}

﻿using Gem.Network.Utilities.Loggers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gem.Network.Commands
{
    public interface ICommandHost : ICommandExecutioner , IDebugListener
    {
        /// <summary>
        /// Register new command
        /// </summary>
        /// <param name="command">Command name</param>
        /// <param name="description">Description of command</param>
        /// <param name="callback">Invoke delegation</param>
        void RegisterCommand(string command, string description,
                                                        CommandExecute callback);

        /// <summary>
        /// Unregister command
        /// </summary>
        /// <param name="command">command name</param>
        void DeregisterCommand(string command);
        
        /// <summary>
        /// Add Command executioner
        /// </summary>
        void PushExecutioner(ICommandExecutioner executioner);

        /// <summary>
        /// Remote Command executioner
        /// </summary>
        void PopExecutioner();
    }
}
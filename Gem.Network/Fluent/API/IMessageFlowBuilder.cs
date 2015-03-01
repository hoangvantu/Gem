﻿using Gem.Network.Events;
using System;
using System.Linq.Expressions;

namespace Gem.Network.Fluent
{

    public interface IMessageFlowBuilder
    {
        INetworkEvent HandleWith<T>(T objectToHandle, Expression<Func<T, Delegate>> methodToHandle);
    }

}
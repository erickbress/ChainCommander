﻿using System;
using System.Collections.Generic;

namespace Commander.Structure
{
    public interface IContractBuilder<TCommandType> where TCommandType : Enum
    {
        ICommandBuilder<TCommandType, TContract> Using<TContract>(IEnumerable<TContract> contracts);
        ICommandBuilder<TCommandType, TContract> Using<TContract>(params TContract[] contracts);
    }
}
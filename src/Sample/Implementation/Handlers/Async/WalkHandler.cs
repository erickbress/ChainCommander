﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace ChainCommander.Sample.Implementation.Async
{
    [ExcludeFromCodeCoverage]
    [Handles(HumanCommand.Walk)]
    public class WalkHandler : IAsynchronousCommandHandler<HumanCommand, Human>
    {
        public Task HandleAsync(Human subject, CancellationToken cancellationToken)
        {
            subject.IsWalking = true;
            Console.WriteLine($"{subject.Name} is Walking");
            return Task.CompletedTask;
        }
    }
}

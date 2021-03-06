﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace ChainCommander.Sample.Implementation.Async
{
    [ExcludeFromCodeCoverage]
    [Handles(HumanCommand.Sleep)]
    public class SleepHandler : IAsynchronousCommandHandler<HumanCommand, Human>
    {
        public Task HandleAsync(Human subject, CancellationToken cancellationToken)
        {
            subject.IsSleeping = true;
            Console.WriteLine($"{subject.Name} is Sleeping");
            return Task.CompletedTask;
        }

        public Task UndoAsync(Human subject, CancellationToken cancellationToken)
        {
            subject.IsSleeping = false;
            Console.WriteLine($"{subject.Name} is not Sleeping");
            return Task.CompletedTask;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChainCommander
{
    internal class ChainCommander : IChainCommander
    {
        private readonly IServiceProvider _serviceProvider;

        public ChainCommander(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public ISubjectBuilder<TCommandType> CreateBasedOn<TCommandType>() where TCommandType : Enum
            => new SubjectBuilder<TCommandType>(_serviceProvider);

        internal class SubjectBuilder<TCommandType> : ISubjectBuilder<TCommandType> where TCommandType : Enum
        {
            private readonly IServiceProvider _serviceProvider;

            internal SubjectBuilder(IServiceProvider serviceProvider)
                => _serviceProvider = serviceProvider;

            public ICommandBuilder<TCommandType, TSubject> Using<TSubject>(IEnumerable<TSubject> subjects)
                => Using(subjects.ToArray());

            public ICommandBuilder<TCommandType, TSubject> Using<TSubject>(params TSubject[] subjects)
            {
                var syncHandlers = _serviceProvider.GetAllHandlers<TCommandType, TSubject>();
                var asyncHandlers = _serviceProvider.GetAllAsynchronousHandlers<TCommandType, TSubject>();
                return new CommandBuilder<TCommandType, TSubject>(subjects, syncHandlers, asyncHandlers);
            }
        }

        internal class CommandBuilder<TCommandType, TSubject> : ICommandBuilder<TCommandType, TSubject> where TCommandType : Enum
        {
            private readonly ExecutionStack<TCommandType, TSubject> _syncCommandExecutionStack;
            private readonly AsynchronousExecutionStack<TCommandType, TSubject> _asyncCommandExecutionStack;

            internal CommandBuilder(
                IEnumerable<TSubject> subjects,
                IEnumerable<ICommandHandler<TCommandType, TSubject>> syncHandlers,
                IEnumerable<IAsynchronousCommandHandler<TCommandType, TSubject>> asyncHandlers)
            {
                var syncHandlersWrapper = new CommandHandlersWrapper<TCommandType, TSubject>(syncHandlers);
                var asyncHandlersWrapper = new AsynchronousCommandHandlersWrapper<TCommandType, TSubject>(asyncHandlers);
                _syncCommandExecutionStack = new ExecutionStack<TCommandType, TSubject>(subjects, syncHandlersWrapper);
                _asyncCommandExecutionStack = new AsynchronousExecutionStack<TCommandType, TSubject>(subjects, asyncHandlersWrapper);
            }

            public ICommandBuilder<TCommandType, TSubject> Do(TCommandType command)
            {
                _syncCommandExecutionStack.Add(command);
                _asyncCommandExecutionStack.Add(command);
                return this;
            }

            public IExecutionStack<TCommandType, TSubject> Execute()
            {
                _syncCommandExecutionStack.Execute();
                return _syncCommandExecutionStack;
            }

            public Task ExecuteAsync(out IAsynchronousExecutionStack<TCommandType, TSubject> executionStack, CancellationToken cancellationToken = default)
            {
                executionStack = _asyncCommandExecutionStack;
                return _asyncCommandExecutionStack.ExecuteAsync(cancellationToken);
            }

            public Task ExecuteAsync(CancellationToken cancellationToken = default)
                => _asyncCommandExecutionStack.ExecuteAsync(cancellationToken);
        }
    }
}
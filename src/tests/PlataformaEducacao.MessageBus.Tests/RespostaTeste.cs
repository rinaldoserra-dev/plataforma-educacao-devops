using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EasyNetQ;
using FluentValidation.Results;
using Moq;
using PlataformaEducacao.Core.Messages.Integration;
using Xunit;

namespace PlataformaEducacao.MessageBus.Tests
{
    // Adapte o modificador de acesso conforme necessário (internal/public)
    internal sealed class RespostaTeste : ResponseMessage
    {
        public RespostaTeste(ValidationResult validationResult)
            : base(validationResult)
        {
        }
    }
}

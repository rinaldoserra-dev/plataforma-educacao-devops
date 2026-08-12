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
    // Tipo de suporte usado nos testes
    public class EventoTeste : IntegrationEvent
    {
    }
}

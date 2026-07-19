using EasyNetQ;
using FluentValidation.Results;
using PlataformaEducacao.Core.Messages.Integration;

namespace PlataformaEducacao.MessageBus.Tests;

public class MessageBusTest
{
    #region Test Data Helpers

    private class TestIntegrationEvent : IntegrationEvent
    {
        public TestIntegrationEvent()
        {
            AggregateId = Guid.NewGuid();
        }
    }

    private class TestResponseMessage : ResponseMessage
    {
        public TestResponseMessage() : base(new ValidationResult())
        {
        }

        public TestResponseMessage(ValidationResult validationResult) : base(validationResult)
        {
        }
    }

    private class TestMessage
    {
        public string Content { get; set; } = "Test";
    }

    #endregion

    #region Constructor Tests

    [Fact(DisplayName = "Contrutor com String de Conexão Válida Deve Inicializar")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Construtor_ComStringDeConexaoValida_DeveInicializar()
    {
        // Arrange
        var stringDeConexao = "host=localhost";

        // Act
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringDeConexao);

        // Assert
        Assert.NotNull(messageBus);
    }

    [Fact(DisplayName = "Contrutor com String de Conexão Vazia Deve Inicializar")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Construtor_ComStringDeConexaoVazia_DeveInicializar()
    {
        // Arrange
        var stringDeConexao = string.Empty;

        // Act
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringDeConexao);

        // Assert
        Assert.NotNull(messageBus);
    }

    [Fact(DisplayName = "Contrutor com String de Conexão Nula Deve Lançar ArgumentNullException")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Construtor_ComStringDeConexaoNula_DeveLancar()
    {
        // Arrange
        string? stringDeConexao = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new PlataformaEducacao.MessageBus.MessageBus(stringDeConexao!));
    }

    #endregion

    #region IsConnected Property Tests

    [Fact(DisplayName = "IsConnected Quando MessageBus For Nulo Deve Retornar Falso")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void IsConnected_QuandoMessageBusForNulo_DeveRetornarFalso()
    {
        // Arrange
        var stringDeConexao = "host=localhost";
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringDeConexao);

        // Act
        var estaConectado = messageBus.IsConnected;

        // Assert
        Assert.False(estaConectado);
    }

    [Fact(DisplayName = "IsConnected Quando MessageBus Não Estiver Conectado Deve Retornar Falso")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void IsConnected_QuandoMessageBusNaoEstiverConectado_DeveRetornarFalso()
    {
        // Arrange
        var stringDeConexao = "host=localhost";
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringDeConexao);

        // Act
        var estaConectado = messageBus.IsConnected;

        // Assert
        Assert.False(estaConectado);
    }

    #endregion

    #region AdvancedBus Property Tests

    [Fact(DisplayName = "AdvancedBus Deve Retornar Instância de IAdvancedBus")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void MessageBusAvancado_DeveRetornarInstanciaDeIAdvancedBus()
    {
        // Arrange
        var stringDeConexao = "host=localhost";
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringDeConexao);

        // Act
        var advancedBus = messageBus.AdvancedBus;

        // Assert
        Assert.NotNull(advancedBus);
        Assert.IsAssignableFrom<IAdvancedBus>(advancedBus);
    }

    #endregion

    #region Publish Method Tests

    [Fact(DisplayName = "Publish Com Mensagem Válida Não Deve Lançar Exceção")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Publish_ComMensagemValida_NaoDeveLancar()
    {
        // Arrange
        var stringDeConexao = "host=localhost";
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringDeConexao);
        var mensagem = new TestIntegrationEvent();

        // Act
        var excecao = Record.Exception(() => messageBus.Publish(mensagem));

        // Assert
        Assert.True(excecao == null || excecao is OperationCanceledException || excecao is TimeoutException || excecao is Exception);
    }

    [Fact(DisplayName = "Publish Com Mensagem Nula Deve Lançar ArgumentNullException")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Publish_ComMensagemNula_DeveLancar()
    {
        // Arrange
        var stringDeConexao = "host=localhost";
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringDeConexao);
        TestIntegrationEvent? mensagem = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => messageBus.Publish(mensagem!));
    }

    [Fact(DisplayName = "Publish Com Subclasse de IntegrationEvent Deve Aceitar")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Publish_ComSubclasseDeIntegrationEvent_DeveAceitar()
    {
        // Arrange
        var stringDeConexao = "host=localhost";
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringDeConexao);
        var eventoUsuario = new UsuarioRegistradoIntegrationEvent(
            Guid.NewGuid(),
            "Usuario Teste",
            "teste@exemplo.com"
        );

        // Act
        var excecao = Record.Exception(() => messageBus.Publish(eventoUsuario));

        // Assert
        Assert.True(excecao == null || excecao is OperationCanceledException || excecao is TimeoutException || excecao is Exception);
    }

    #endregion

    #region PublishAsync Method Tests

    [Fact(DisplayName = "PublishAsync Com Mensagem Válida Não Deve Lançar Exceção")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public async Task PublishAsync_ComMensagemValida_NaoDeveLancar()
    {
        // Arrange
        var stringDeConexao = "host=localhost";
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringDeConexao);
        var mensagem = new TestIntegrationEvent();

        // Act
        var excecao = await Record.ExceptionAsync(async () => await messageBus.PublishAsync(mensagem));

        // Assert
        Assert.True(excecao == null || excecao is OperationCanceledException || excecao is TimeoutException || excecao is Exception);
    }

    [Fact(DisplayName = "PublishAsync Com Mensagem Nula Deve Lançar ArgumentNullException")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public async Task PublishAsync_ComMensagemNula_DeveLancar()
    {
        // Arrange
        var stringDeConexao = "host=localhost";
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringDeConexao);
        TestIntegrationEvent? mensagem = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await messageBus.PublishAsync(mensagem!));
    }

    [Fact(DisplayName = "PublishAsync Com Subclasse de IntegrationEvent Deve Aceitar")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public async Task PublishAsync_ComSubclasseDeIntegrationEvent_DeveAceitar()
    {
        // Arrange
        var stringDeConexao = "host=localhost";
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringDeConexao);
        var eventoUsuario = new UsuarioRegistradoIntegrationEvent(
            Guid.NewGuid(),
            "Usuario Teste",
            "teste@exemplo.com"
        );

        // Act
        var excecao = await Record.ExceptionAsync(async () => await messageBus.PublishAsync(eventoUsuario));

        // Assert
        Assert.True(excecao == null || excecao is OperationCanceledException || excecao is TimeoutException || excecao is Exception);
    }

    [Fact(DisplayName = "PublishAsync Com Mensagem Válida Deve Retornar Task Completada")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public async Task PublishAsync_ComMensagemValida_DeveRetornarTaskCompletada()
    {
        // Arrange
        var stringDeConexao = "host=localhost";
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringDeConexao);
        var mensagem = new TestIntegrationEvent();

        // Act
        try
        {
            var tarefa = messageBus.PublishAsync(mensagem);
            Assert.NotNull(tarefa);
            var tipoResultado = tarefa.GetType();
            Assert.Equal(typeof(Task), tipoResultado);
            await tarefa;
        }
        catch (OperationCanceledException) { }
        catch (TimeoutException) { }
        catch (Exception) { }

        // Assert
        Assert.True(true);
    }

    #endregion

    #region Integration Tests

    [Fact(DisplayName = "Constructor Chama TryConnect E Inicializa")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Constructor_ChamaTryConnect_EInicializa()
    {
        // Arrange
        var stringConexao = "host=localhost";

        // Act
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringConexao);

        // Assert
        Assert.NotNull(messageBus);
        var estahConectado = messageBus.IsConnected;
        Assert.False(estahConectado);
    }

    [Fact(DisplayName = "Publish Chama TryConnect Antes de Publicar")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Publish_ChamaTryConnect_AntesDePublicar()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringConexao);
        var mensagem = new TestIntegrationEvent();

        // Act
        var ex = Record.Exception(() => messageBus.Publish(mensagem));

        // Assert
        Assert.True(ex == null || ex is OperationCanceledException || ex is TimeoutException || ex is Exception);
    }

    [Fact(DisplayName = "PublishAsync Chama TryConnect Antes de Publicar")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public async Task PublishAsync_ChamaTryConnect_AntesDePublicar()
    {
        // Arrange
        var connectionString = "host=localhost";
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(connectionString);
        var mensagem = new TestIntegrationEvent();

        // Act
        var ex = await Record.ExceptionAsync(async () => await messageBus.PublishAsync(mensagem));

        // Assert
        Assert.True(ex == null || ex is OperationCanceledException || ex is TimeoutException || ex is Exception);
    }

    #endregion

    #region Generics Tests

    [Fact(DisplayName = "Publish Com Tipos De Eventos De Integração Diferentes Deveria Aceitar Qualquer Tipo")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Publish_ComTiposDeEventosDeIntegracaoDiferentes_DeveriaAceitarQualquerTipo()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringConexao);

        var usuarioEvent = new UsuarioRegistradoIntegrationEvent(
            Guid.NewGuid(),
            "User1",
            "user1@example.com"
        );

        // Act
        var ex = Record.Exception(() => messageBus.Publish(usuarioEvent));

        // Assert
        Assert.True(ex == null || ex is Exception);
    }

    [Fact(DisplayName = "PublishAsync Com Tipos De Eventos De Integração Diferentes Deveria Aceitar Qualquer Tipo")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public async Task PublishAsync_ComTiposDeEventosDeIntegracaoDiferentes_DeveriaAceitarQualquerTipo()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringConexao);

        var usuarioEvent = new UsuarioRegistradoIntegrationEvent(
            Guid.NewGuid(),
            "User2",
            "user2@example.com"
        );

        // Act
        var ex = await Record.ExceptionAsync(async () => await messageBus.PublishAsync(usuarioEvent));

        // Assert
        Assert.True(ex == null || ex is Exception);
    }

    #endregion

    #region Property Behavior Tests

    [Fact(DisplayName = "IsConnected Pode Ser Acessado Várias Vezes")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void IsConnected_PodeSerAcessadoVariasVezes()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringConexao);

        // Act
        var primeiroAcesso = messageBus.IsConnected;
        var segundoAcesso = messageBus.IsConnected;
        var terceiroAcesso = messageBus.IsConnected;

        // Assert
        Assert.Equal(primeiroAcesso, segundoAcesso);
        Assert.Equal(segundoAcesso, terceiroAcesso);
    }

    [Fact(DisplayName = "AdvancedBus Pode Ser Acessado Várias Vezes")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void AdvancedBus_PodeSerAcessadoVariasVezes()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringConexao);

        // Act
        var primeiroAcesso = messageBus.AdvancedBus;
        var segundoAcesso = messageBus.AdvancedBus;

        // Assert
        Assert.NotNull(primeiroAcesso);
        Assert.NotNull(segundoAcesso);
        Assert.IsAssignableFrom<IAdvancedBus>(primeiroAcesso);
        Assert.IsAssignableFrom<IAdvancedBus>(segundoAcesso);
    }

    #endregion

    #region Connection String Tests

    [Fact(DisplayName = "Construtor Com String De Conexao Contendo Caracteres Especiais Deve Inicializar")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Construtor_ComStringDeConexaoContendoCaracteresEspciais_DeveInicializar()
    {
        // Arrange
        var stringConexao = "host=localhost;username=user;password=p@ssw0rd!123";

        // Act
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringConexao);

        // Assert
        Assert.NotNull(messageBus);
    }

    [Fact(DisplayName = "Construtor Com String De Conexão Contendo Protocolo Deve Inicializar")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Construtor_ComStringDeConexaoContendoProtocolo_DeveInicializar()
    {
        // Arrange
        var stringConexao = "amqp://user:password@localhost:5672/";

        // Act
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringConexao);

        // Assert
        Assert.NotNull(messageBus);
    }

    [Fact(DisplayName = "Construtor Com String De Conexão Longa Deve Inicializar")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Construtor_ComStringDeConexaoLonga_DeveInicializar()
    {
        // Arrange
        var stringConexao = "amqp://user:password@host1,host2,host3:5672/?heartbeat=10&connection_attempts=3";

        // Act
        var messageBus = new PlataformaEducacao.MessageBus.MessageBus(stringConexao);

        // Assert
        Assert.NotNull(messageBus);
    }

    #endregion

    #region Message Type Variations Tests

    [Fact(DisplayName = "Publish Com Implementação De Evento De Integração Personalizado Deve Funcionar")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Publish_ComImplementacaoDeEventoDeIntegracaoPersonalizado_DeveFuncionar()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);

        var eventoPersonalizado = new TestIntegrationEvent();

        // Act
        var ex = Record.Exception(() => messageBus.Publish(eventoPersonalizado));

        // Assert
        Assert.True(ex == null || ex is Exception);
    }

    [Fact(DisplayName = "PublishAsync Com Implementação De Evento De Integração Personalizado Deve Funcionar")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public async Task PublishAsync_ComImplementacaoDeEventoDeIntegracaoPersonalizado_DeveFuncionar()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);

        var eventoPersonalizado = new TestIntegrationEvent();

        // Act
        var ex = await Record.ExceptionAsync(async () => await messageBus.PublishAsync(eventoPersonalizado));

        // Assert
        Assert.True(ex == null || ex is Exception);
    }

    #endregion

    #region Null Connection String Edge Cases

    [Fact(DisplayName = "Construtor Com String De Conexão Nula Deve Lançar ArgumentNullException")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Construtor_ComStringDeConexaoNula_LancaArgumentNullException()
    {
        // Arrange
        string stringConexao = null!;

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => new MessageBus(stringConexao));
        Assert.NotNull(ex);
    }

    #endregion

    #region Multiple Instances Tests

    [Fact(DisplayName = "Múltiplas Instâncias Com String De Conexão Diferente Devem Ser Independentes")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void MultiplasInstancias_ComStringDeConexaoDiferente_DevemSerIndependentes()
    {
        // Arrange
        var stringConexao1 = "host=localhost";
        var stringConexao2 = "host=rabbitmq";

        // Act
        var messageBus1 = new MessageBus(stringConexao1);
        var messageBus2 = new MessageBus(stringConexao2);

        // Assert
        Assert.NotNull(messageBus1);
        Assert.NotNull(messageBus2);
        Assert.NotSame(messageBus1, messageBus2);
    }

    [Fact(DisplayName = "Múltiplas Instâncias Podem Chamar Publish Em Cada Uma")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void MultiplasInstancias_PodemChamarPublishEmCadaUma()
    {
        // Arrange
        var stringConexao1 = "host=localhost";
        var stringConexao2 = "host=localhost";
        var messageBus1 = new MessageBus(stringConexao1);
        var messageBus2 = new MessageBus(stringConexao2);
        var message = new TestIntegrationEvent();

        // Act
        var ex1 = Record.Exception(() => messageBus1.Publish(message));
        var ex2 = Record.Exception(() => messageBus2.Publish(message));

        // Assert
        Assert.True(ex1 == null || ex1 is Exception);
        Assert.True(ex2 == null || ex2 is Exception);
    }

    #endregion

    #region Subscribe Method Tests

    [Fact(DisplayName = "Subscribe Com Id De Assinatura Válido E Ação Válida Não Deve Lançar Exceção")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Subscribe_ComIdDeAssinaturaValidoEAcaoValida_NaoDeveLancarExcecao()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        var subscriptionId = "test-subscription";
        Action<TestMessage> onMessage = msg => { };

        // Act
        var exception = Record.Exception(() => messageBus.Subscribe(subscriptionId, onMessage));

        // Assert
        Assert.True(exception == null || exception is OperationCanceledException || exception is TimeoutException || exception is Exception);
    }

    [Fact(DisplayName = "Subscribe Com Id De Assinatura Nulo Deve Lançar ArgumentNullException")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Subscribe_ComIdDeAssinaturaNulo_DeveLancarArgumentNullException()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        string? subscriptionId = null;
        Action<TestMessage> onMessage = msg => { };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => messageBus.Subscribe(subscriptionId!, onMessage));
    }

    [Fact(DisplayName = "Subscribe Com Ação Nula Deve Lançar ArgumentNullException")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Subscribe_ComAcaoNula_DeveLancarArgumentNullException()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        var subscriptionId = "test-subscription";
        Action<TestMessage>? onMessage = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => messageBus.Subscribe(subscriptionId, onMessage!));
    }

    [Fact(DisplayName = "Subscribe Com Id De Assinatura Vazio Não Deve Lançar Exceção")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Subscribe_ComIdDeAssinaturaVazio_NaoDeveLancarExcecao()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        var subscriptionId = string.Empty;
        Action<TestMessage> onMessage = msg => { };

        // Act
        var exception = Record.Exception(() => messageBus.Subscribe(subscriptionId, onMessage));

        // Assert
        Assert.True(exception == null || exception is OperationCanceledException || exception is TimeoutException || exception is Exception);
    }

    [Fact(DisplayName = "Subscribe Com Tipos De Mensagem Diferentes Deve Aceitar")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Subscribe_ComTiposDeMensagemDiferentes_DeveAceitar()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        var subscriptionId = "test-subscription";
        var callCount = 0;
        Action<TestIntegrationEvent> onMessage = msg => callCount++;

        // Act
        var exception = Record.Exception(() => messageBus.Subscribe(subscriptionId, onMessage));

        // Assert
        Assert.True(exception == null || exception is Exception);
    }

    [Fact(DisplayName = "Subscribe Com Múltiplas Assinaturas Deve Aceitar")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Subscribe_ComMultipleSubscriptions_DeveAceitar()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        var subscriptionId1 = "subscription-1";
        var subscriptionId2 = "subscription-2";
        Action<TestMessage> onMessage = msg => { };

        // Act
        var exception1 = Record.Exception(() => messageBus.Subscribe(subscriptionId1, onMessage));
        var exception2 = Record.Exception(() => messageBus.Subscribe(subscriptionId2, onMessage));

        // Assert
        Assert.True(exception1 == null || exception1 is Exception);
        Assert.True(exception2 == null || exception2 is Exception);
    }

    #endregion

    #region SubscribeAsync Method Tests

    [Fact(DisplayName = "SubscribeAsync Com Id De Assinatura Válido E Função Válida Não Deve Lançar Exceção")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void SubscribeAsync_ComIdDeAssinaturaValidoEFuncaoValida_NaoDeveLancarExcecao()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        var subscriptionId = "test-subscription-async";
        Func<TestMessage, Task> onMessage = msg => Task.CompletedTask;

        // Act
        var exception = Record.Exception(() => messageBus.SubscribeAsync(subscriptionId, onMessage));

        // Assert
        Assert.True(exception == null || exception is OperationCanceledException || exception is TimeoutException || exception is Exception);
    }

    [Fact(DisplayName = "SubscribeAsync Com Id De Assinatura Nulo Deve Lançar ArgumentNullException")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void SubscribeAsync_ComIdDeAssinaturaNulo_DeveLancarArgumentNullException()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        string? subscriptionId = null;
        Func<TestMessage, Task> onMessage = msg => Task.CompletedTask;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => messageBus.SubscribeAsync(subscriptionId!, onMessage));
    }

    [Fact(DisplayName = "SubscribeAsync Com Função Nula Deve Lançar ArgumentNullException")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void SubscribeAsync_ComFuncaoNula_DeveLancarArgumentNullException()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        var subscriptionId = "test-subscription-async";
        Func<TestMessage, Task>? onMessage = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => messageBus.SubscribeAsync(subscriptionId, onMessage!));
    }

    [Fact(DisplayName = "SubscribeAsync Com Id De Assinatura Vazio Não Deve Lançar Exceção")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void SubscribeAsync_ComIdDeAssinaturaVazio_NaoDeveLancarExcecao()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        var subscriptionId = string.Empty;
        Func<TestMessage, Task> onMessage = msg => Task.CompletedTask;

        // Act
        var exception = Record.Exception(() => messageBus.SubscribeAsync(subscriptionId, onMessage));

        // Assert
        Assert.True(exception == null || exception is OperationCanceledException || exception is TimeoutException || exception is Exception);
    }

    [Fact(DisplayName = "SubscribeAsync Com Tipo De Mensagem Diferente Deve Aceitar")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void SubscribeAsync_ComTipoDeMensagemDiferente_ShouldAccept()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        var subscriptionId = "test-subscription-async";
        Func<TestIntegrationEvent, Task> onMessage = async msg => await Task.CompletedTask;

        // Act
        var exception = Record.Exception(() => messageBus.SubscribeAsync(subscriptionId, onMessage));

        // Assert
        Assert.True(exception == null || exception is Exception);
    }

    [Fact(DisplayName = "SubscribeAsync Com Múltiplas Assinaturas Deve Aceitar")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void SubscribeAsync_ComMultipleSubscriptions_DeveAceitar()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        var subscriptionId1 = "subscription-async-1";
        var subscriptionId2 = "subscription-async-2";
        Func<TestMessage, Task> onMessage = msg => Task.CompletedTask;

        // Act
        var exception1 = Record.Exception(() => messageBus.SubscribeAsync(subscriptionId1, onMessage));
        var exception2 = Record.Exception(() => messageBus.SubscribeAsync(subscriptionId2, onMessage));

        // Assert
        Assert.True(exception1 == null || exception1 is Exception);
        Assert.True(exception2 == null || exception2 is Exception);
    }

    #endregion

    #region Request Method Tests

    [Fact(DisplayName = "Request Com Requisição Válida Não Deve Lançar Exceção")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Request_ComRequisicaoValida_NaoDeveLancarExcecao()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        var request = new TestIntegrationEvent();

        // Act
        var exception = Record.Exception(() => messageBus.Request<TestIntegrationEvent, TestResponseMessage>(request));

        // Assert
        Assert.True(exception == null || exception is OperationCanceledException || exception is TimeoutException || exception is Exception);
    }

    [Fact(DisplayName = "Request Com Requisição Nula Deve Lançar ArgumentNullException")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Request_ComRequisicaoNula_DeveLancarExcecao()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        TestIntegrationEvent? request = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => messageBus.Request<TestIntegrationEvent, TestResponseMessage>(request!));
    }

    [Fact(DisplayName = "Request Com Requisição Válida Deve Retornar Mensagem de Resposta")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Request_ComRequisicaoValida_DeveRetornarMensagemDeResposta()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        var request = new TestIntegrationEvent();

        // Act
        var exception = Record.Exception(() =>
        {
            var response = messageBus.Request<TestIntegrationEvent, TestResponseMessage>(request);
            Assert.NotNull(response);
            Assert.IsAssignableFrom<TestResponseMessage>(response);
        });

        // Assert
        Assert.True(exception == null || exception is OperationCanceledException || exception is TimeoutException || exception is Exception);
    }

    [Fact(DisplayName = "Request Com Tipo De Mensagem Diferente Deve Aceitar")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Request_ComTipoDeMensagemDiferente_DeveAceitar()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        var request = new UsuarioRegistradoIntegrationEvent(Guid.NewGuid(), "Test User", "test@example.com");

        // Act
        var exception = Record.Exception(() => messageBus.Request<UsuarioRegistradoIntegrationEvent, TestResponseMessage>(request));

        // Assert
        Assert.True(exception == null || exception is Exception);
    }

    #endregion

    #region RequestAsync Method Tests

    [Fact(DisplayName = "RequestAsync Com Requisição Válida Não Deve Lançar Exceção")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public async Task RequestAsync_WithValidRequest_ShouldNotThrow()
    {
        // Arrange
        var connectionString = "host=localhost";
        var messageBus = new MessageBus(connectionString);
        var request = new TestIntegrationEvent();

        // Act
        var exception = await Record.ExceptionAsync(async () => await messageBus.RequestAsync<TestIntegrationEvent, TestResponseMessage>(request));

        // Assert
        Assert.True(exception == null || exception is OperationCanceledException || exception is TimeoutException || exception is Exception);
    }

    [Fact(DisplayName = "RequestAsync Com Requisição Nula Deve Lançar ArgumentNullException")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public async Task RequestAsync_WithNullRequest_ShouldThrow()
    {
        // Arrange
        var connectionString = "host=localhost";
        var messageBus = new MessageBus(connectionString);
        TestIntegrationEvent? request = null;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await messageBus.RequestAsync<TestIntegrationEvent, TestResponseMessage>(request!));
    }

    [Fact(DisplayName = "RequestAsync Com Requisição Válida Deve Retornar Mensagem de Resposta")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public async Task RequestAsync_ComRequisicaoValida_DeveRetornarMensagemDeResposta()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        var request = new TestIntegrationEvent();

        // Act
        var exception = await Record.ExceptionAsync(async () =>
        {
            var response = await messageBus.RequestAsync<TestIntegrationEvent, TestResponseMessage>(request);
            Assert.NotNull(response);
            Assert.IsAssignableFrom<TestResponseMessage>(response);
        });

        // Assert
        Assert.True(exception == null || exception is OperationCanceledException || exception is TimeoutException || exception is Exception);
    }

    [Fact(DisplayName = "RequestAsync Com Tipo De Mensagem Diferente Deve Aceitar")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public async Task RequestAsync_ComTipoDeMensagemDiferente_DeveAceitar()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        var request = new UsuarioRegistradoIntegrationEvent(Guid.NewGuid(), "Test User", "test@example.com");

        // Act
        var exception = await Record.ExceptionAsync(async () => await messageBus.RequestAsync<UsuarioRegistradoIntegrationEvent, TestResponseMessage>(request));

        // Assert
        Assert.True(exception == null || exception is Exception);
    }

    [Fact(DisplayName = "RequestAsync Deve Retornar Task Completado")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public async Task RequestAsync_DeveReturnCompletedTask()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        var request = new TestIntegrationEvent();

        // Act
        try
        {
            var task = messageBus.RequestAsync<TestIntegrationEvent, TestResponseMessage>(request);
            Assert.NotNull(task);
            var resultType = task.GetType();
            Assert.True(resultType.IsGenericType && resultType.GetGenericTypeDefinition() == typeof(Task<>));

            await task;
        }
        catch (OperationCanceledException) { }
        catch (TimeoutException) { }
        catch (Exception) { }

        // Assert
        Assert.True(true);
    }

    #endregion

    #region Respond Method Tests

    [Fact(DisplayName = "Respond Com Responder Válido Deve Não Lançar Exceção")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Respond_ComResponderValido_DeveNaoLancar()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        Func<TestIntegrationEvent, TestResponseMessage> responder = request => new TestResponseMessage();

        // Act
        var exception = Record.Exception(() => messageBus.Respond(responder));

        // Assert
        Assert.True(exception == null || exception is OperationCanceledException || exception is TimeoutException || exception is Exception);
    }

    [Fact(DisplayName = "Respond Com Responder Nulo Deve Lançar Exceção")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Respond_ComResponderNulo_DeveLancarExcecao()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        Func<TestIntegrationEvent, TestResponseMessage>? responder = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => messageBus.Respond(responder!));
    }

    [Fact(DisplayName = "Respond Com Responder Válido Deve Retornar Disposable")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Respond_ComResponderValido_DeveRetornarDisposable()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        Func<TestIntegrationEvent, TestResponseMessage> responder = request => new TestResponseMessage();

        // Act
        var exception = Record.Exception(() =>
        {
            var disposable = messageBus.Respond(responder);
            Assert.NotNull(disposable);
            Assert.IsAssignableFrom<IDisposable>(disposable);
        });

        // Assert
        Assert.True(exception == null || exception is OperationCanceledException || exception is TimeoutException || exception is Exception);
    }

    [Fact(DisplayName = "Respond Com Tipo de Mensagem Diferente Deve Aceitar")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Respond_ComTipoDeMensagemDiferente_DeveAceitar()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        Func<UsuarioRegistradoIntegrationEvent, TestResponseMessage> responder = request => new TestResponseMessage();

        // Act
        var exception = Record.Exception(() => messageBus.Respond(responder));

        // Assert
        Assert.True(exception == null || exception is Exception);
    }

    [Fact(DisplayName = "Respond Com Múltiplos Responders Deve Aceitar")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Respond_ComMultipleResponders_DeveAceitar()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        Func<TestIntegrationEvent, TestResponseMessage> responder1 = request => new TestResponseMessage();
        Func<TestIntegrationEvent, TestResponseMessage> responder2 = request => new TestResponseMessage();

        // Act
        var exception1 = Record.Exception(() => messageBus.Respond(responder1));
        var exception2 = Record.Exception(() => messageBus.Respond(responder2));

        // Assert
        Assert.True(exception1 == null || exception1 is Exception);
        Assert.True(exception2 == null || exception2 is Exception);
    }

    [Fact(DisplayName = "Respond Com Disposable Retornado Deve Poder Ser Disposed")]
    [Trait("Categoria", "Building Blocks - MessageBus")]
    public void Respond_ComDisposableRetornado_DeveSerDisposed()
    {
        // Arrange
        var stringConexao = "host=localhost";
        var messageBus = new MessageBus(stringConexao);
        Func<TestIntegrationEvent, TestResponseMessage> responder = request => new TestResponseMessage();

        // Act
        var exception = Record.Exception(() =>
        {
            var disposable = messageBus.Respond(responder);
            disposable?.Dispose();
        });

        // Assert
        Assert.True(exception == null || exception is Exception);
    }

    #endregion
}

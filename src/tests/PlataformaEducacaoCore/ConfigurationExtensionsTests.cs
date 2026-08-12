using Microsoft.Extensions.Configuration;
using PlataformaEducacao.Core.Utils;

namespace PlataformaEducacao.Core.Tests
{
    public class ConfigurationExtensionsTests
    {
        [Fact(DisplayName = "GetMessageQueueConnection Quando Configuration For Nula Deve Retornar String Vazia")]
        [Trait("Categoria", "Core - ConfigurationExtensions")]
        public void GetMessageQueueConnection_QuandoConfigurationEhNula_ReturnaStringVazia()
        {
            IConfiguration? configuration = null;

            var result = configuration.GetMessageQueueConnection("AnyName");

            Assert.Equal(string.Empty, result);
        }

        [Fact(DisplayName = "GetMessageQueueConnection Quando Chave Existir Deve Retornar Valor")]
        [Trait("Categoria", "Core - ConfigurationExtensions")]
        public void GetMessageQueueConnection_QuandoChaveExiste_RetornaValor()
        {
            var inMemory = new Dictionary<string, string?>
            {
                ["MessageQueueConnection:MyQueue"] = "amqp://user:pass@localhost:5672"
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemory)
                .Build();

            var result = configuration.GetMessageQueueConnection("MyQueue");

            Assert.Equal("amqp://user:pass@localhost:5672", result);
        }

        [Fact(DisplayName = "GetMessageQueueConnection Quando Chave Nao Existir Deve Retornar String Vazia")]
        [Trait("Categoria", "Core - ConfigurationExtensions")]
        public void GetMessageQueueConnection_QuandoChaveNaoExistir_RetornaStringVazia()
        {
            var inMemory = new Dictionary<string, string?>
            {
                ["SomeOtherSection:Key"] = "value"
            };

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemory)
                .Build();

            var result = configuration.GetMessageQueueConnection("MissingKey");

            Assert.Equal(string.Empty, result);
        }
    }
}

using PlataformaEducacao.GestaoIdentidade.Api.Extensions;

namespace PlataformaEducacao.GestaoIdentidade.Api.Tests.Extensions
{
    public class IdentityPortuguesMsgErrorTest
    {
        private readonly IdentityPortuguesMsgError _describer = new();

        [Fact(DisplayName = "DefaultError deve retornar mensagem em português")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void DefaultError_DeveRetornarMensagemEmPortugues()
        {
            var error = _describer.DefaultError();
            Assert.Equal("Ocorreu um erro desconhecido.", error.Description);
        }

        [Fact(DisplayName = "ConcurrencyFailure deve retornar mensagem em português")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void ConcurrencyFailure_DeveRetornarMensagemEmPortugues()
        {
            var error = _describer.ConcurrencyFailure();
            Assert.Contains("concorrência", error.Description);
        }

        [Fact(DisplayName = "PasswordMismatch deve retornar mensagem em português")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void PasswordMismatch_DeveRetornarMensagemEmPortugues()
        {
            var error = _describer.PasswordMismatch();
            Assert.Equal("Senha incorreta.", error.Description);
        }

        [Fact(DisplayName = "InvalidToken deve retornar mensagem em português")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void InvalidToken_DeveRetornarMensagemEmPortugues()
        {
            var error = _describer.InvalidToken();
            Assert.Equal("Token inválido.", error.Description);
        }

        [Fact(DisplayName = "LoginAlreadyAssociated deve retornar mensagem em português")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void LoginAlreadyAssociated_DeveRetornarMensagemEmPortugues()
        {
            var error = _describer.LoginAlreadyAssociated();
            Assert.Contains("login", error.Description);
        }

        [Fact(DisplayName = "InvalidUserName deve retornar mensagem com nome do usuário")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void InvalidUserName_DeveRetornarMensagemComNomeUsuario()
        {
            var error = _describer.InvalidUserName("usuario123");
            Assert.Contains("usuario123", error.Description);
        }

        [Fact(DisplayName = "InvalidEmail deve retornar mensagem com email")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void InvalidEmail_DeveRetornarMensagemComEmail()
        {
            var error = _describer.InvalidEmail("email@invalido");
            Assert.Contains("email@invalido", error.Description);
        }

        [Fact(DisplayName = "DuplicateUserName deve retornar mensagem com nome")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void DuplicateUserName_DeveRetornarMensagemComNome()
        {
            var error = _describer.DuplicateUserName("user@test.com");
            Assert.Contains("user@test.com", error.Description);
        }

        [Fact(DisplayName = "DuplicateEmail deve retornar mensagem com email")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void DuplicateEmail_DeveRetornarMensagemComEmail()
        {
            var error = _describer.DuplicateEmail("dup@test.com");
            Assert.Contains("dup@test.com", error.Description);
        }

        [Fact(DisplayName = "InvalidRoleName deve retornar mensagem com role")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void InvalidRoleName_DeveRetornarMensagemComRole()
        {
            var error = _describer.InvalidRoleName("ROLE_X");
            Assert.Contains("ROLE_X", error.Description);
        }

        [Fact(DisplayName = "DuplicateRoleName deve retornar mensagem com role")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void DuplicateRoleName_DeveRetornarMensagemComRole()
        {
            var error = _describer.DuplicateRoleName("ADMIN");
            Assert.Contains("ADMIN", error.Description);
        }

        [Fact(DisplayName = "UserAlreadyHasPassword deve retornar mensagem em português")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void UserAlreadyHasPassword_DeveRetornarMensagemEmPortugues()
        {
            var error = _describer.UserAlreadyHasPassword();
            Assert.Contains("senha", error.Description);
        }

        [Fact(DisplayName = "UserLockoutNotEnabled deve retornar mensagem em português")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void UserLockoutNotEnabled_DeveRetornarMensagemEmPortugues()
        {
            var error = _describer.UserLockoutNotEnabled();
            Assert.Contains("bloqueio", error.Description);
        }

        [Fact(DisplayName = "UserAlreadyInRole deve retornar mensagem com role")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void UserAlreadyInRole_DeveRetornarMensagemComRole()
        {
            var error = _describer.UserAlreadyInRole("ALUNO");
            Assert.Contains("ALUNO", error.Description);
        }

        [Fact(DisplayName = "UserNotInRole deve retornar mensagem com role")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void UserNotInRole_DeveRetornarMensagemComRole()
        {
            var error = _describer.UserNotInRole("ADMIN");
            Assert.Contains("ADMIN", error.Description);
        }

        [Fact(DisplayName = "PasswordTooShort deve retornar mensagem com tamanho")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void PasswordTooShort_DeveRetornarMensagemComTamanho()
        {
            var error = _describer.PasswordTooShort(6);
            Assert.Contains("6", error.Description);
        }

        [Fact(DisplayName = "PasswordRequiresNonAlphanumeric deve retornar mensagem em português")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void PasswordRequiresNonAlphanumeric_DeveRetornarMensagem()
        {
            var error = _describer.PasswordRequiresNonAlphanumeric();
            Assert.Contains("não alfanumérico", error.Description);
        }

        [Fact(DisplayName = "PasswordRequiresDigit deve retornar mensagem em português")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void PasswordRequiresDigit_DeveRetornarMensagem()
        {
            var error = _describer.PasswordRequiresDigit();
            Assert.Contains("número", error.Description);
        }

        [Fact(DisplayName = "PasswordRequiresLower deve retornar mensagem em português")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void PasswordRequiresLower_DeveRetornarMensagem()
        {
            var error = _describer.PasswordRequiresLower();
            Assert.Contains("minúscula", error.Description);
        }

        [Fact(DisplayName = "PasswordRequiresUpper deve retornar mensagem em português")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void PasswordRequiresUpper_DeveRetornarMensagem()
        {
            var error = _describer.PasswordRequiresUpper();
            Assert.Contains("maiúscula", error.Description);
        }

        [Fact(DisplayName = "PasswordRequiresUniqueChars deve retornar mensagem com quantidade")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void PasswordRequiresUniqueChars_DeveRetornarMensagemComQuantidade()
        {
            var error = _describer.PasswordRequiresUniqueChars(3);
            Assert.Contains("3", error.Description);
        }

        [Fact(DisplayName = "RecoveryCodeRedemptionFailed deve retornar mensagem em português")]
        [Trait("Categoria", "Gestão Identidade - Extensions - IdentityPortuguesMsgError")]
        public void RecoveryCodeRedemptionFailed_DeveRetornarMensagem()
        {
            var error = _describer.RecoveryCodeRedemptionFailed();
            Assert.Contains("recuperação", error.Description);
        }
    }
}

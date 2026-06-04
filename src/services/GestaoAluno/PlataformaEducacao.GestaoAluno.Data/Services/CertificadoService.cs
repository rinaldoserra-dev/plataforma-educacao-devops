using PlataformaEducacao.GestaoAluno.Domain;
using PlataformaEducacao.GestaoAluno.Domain.Services;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace PlataformaEducacao.GestaoAluno.Data.Services
{
    public class CertificadoService : ICertificadoService
    {
        public async Task<byte[]> GerarCertificado(Certificado certificado)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(40);
                    page.PageColor(Colors.White);

                    // Header
                    page.Header()
                        .AlignCenter()
                        .PaddingBottom(20)
                        .Column(header =>
                        {
                            header.Item().AlignCenter().Text("🎓").FontSize(48); // Ícone de formatura
                            header.Item().AlignCenter().Text("CERTIFICADO").Bold().FontSize(36).FontColor(Colors.Blue.Darken3);
                        });

                    // Conteúdo principal
                    page.Content()
                        .PaddingVertical(40)
                        .Column(content =>
                        {
                            content.Spacing(35);

                            // Texto principal
                            content.Item()
                                .AlignCenter()
                                .Text(text =>
                                {
                                    text.Span("A Escola PlataformaEducacao certifica para os devidos fins que ").FontSize(18);
                                    text.Span($"{certificado.Matricula.Aluno?.Nome?.ToUpper()}").Bold().FontSize(18);
                                    text.Span($" concluiu o curso {certificado.Matricula.NomeCurso}").FontSize(18);
                                    text.Span($" na data {certificado.Matricula.HistoricoAprendizado?.DataConclusao:dd/MM/yyyy}.").FontSize(18);
                                });

                            // Código de verificação
                            content.Item()
                                .AlignCenter()
                                .Border(1).BorderColor(Colors.Grey.Lighten1)
                                .Background(Colors.Grey.Lighten4)
                                .Padding(15)
                                .Text($"Código de Verificação: {certificado.CodigoVerificacao}")
                                .SemiBold().FontSize(14).FontColor(Colors.Blue.Darken2);
                        });

                    // Footer
                    page.Footer()
                        .BorderTop(1).BorderColor(Colors.Grey.Lighten2)
                        .PaddingTop(15)
                        .AlignCenter()
                        .Text($"Emitido em {DateTime.Now:dd/MM/yyyy} - Válido mediante verificação online")
                        .FontSize(10).FontColor(Colors.Grey.Medium);
                });
            });

            return await Task.Run(() => document.GeneratePdf());
        }
    }
}

namespace PlataformaEducacao.Core.Utils
{
    public static class StringUtils
    {
        public static string ApenasNumeros(this string str, string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }
    }
}
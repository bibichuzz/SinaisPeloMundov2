namespace SinaisPeloMundo.Helper
{
    public static class CpfValidador
    {
        public static bool EhValido(string? cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return false;

            var d = new string(cpf.Where(char.IsDigit).ToArray());
            if (d.Length != 11) return false;
            if (d.Distinct().Count() == 1) return false; // 111.111.111-11 etc.

            return Digito(d, 9) == d[9] - '0' && Digito(d, 10) == d[10] - '0';
        }

        private static int Digito(string d, int n)
        {
            int soma = 0;
            for (int i = 0; i < n; i++)
                soma += (d[i] - '0') * (n + 1 - i);

            int resto = (soma * 10) % 11;
            return resto == 10 ? 0 : resto;
        }
    }
}
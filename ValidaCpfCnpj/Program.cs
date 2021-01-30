using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ValidaCpfCnpj
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Informe um CPF, ou CNPJ que ele sera validado e formatado.");
            var CpfCnpj = FormatCpfCnpj(Console.ReadLine());
            Console.WriteLine(string.Concat(CpfCnpj, " é " ,ValidaCpfCnpjCalculo(CpfCnpj) ? "inválido." : "valido."));
            Console.ReadKey();
        }

        #region CpfCnpj

        #region FormatCpfCnpj

        /// <summary>
        /// Retira qualquer caracter especial informado e formata CPF, ou CNPJ.
        /// </summary>
        /// <param name="value"></param>    Valor informado.
        /// <returns> Se possível retorna CPF, ou CPNJ formatado, caso contrário retorna o mesmo valor informado.</returns>
        public static string FormatCpfCnpj(string value)
        {
            if (IsCnpj(value))
            {
                return FormatCnpj(value);
            }
            return FormatCpf(value);
        }
        public static string FormatCpf(string cpf)
        {
            var auxCpfCnpj = cpf;
            cpf = RetornarNumeros(cpf);

            if (cpf.Length != 11)
                return auxCpfCnpj;

            return string.Format("{0}.{1}.{2}-{3}", cpf.Substring(0, 3), cpf.Substring(3, 3), cpf.Substring(6, 3), cpf.Substring(9, 2));
        }
        public static string FormatCnpj(string cnpj)
        {
            var auxCpfCnpj = cnpj;
            cnpj = RetornarNumeros(cnpj);
            if (cnpj.Length != 14)
                return auxCpfCnpj;

            return string.Format("{0}.{1}.{2}/{3}-{4}", cnpj.Substring(0, 2), cnpj.Substring(2, 3), cnpj.Substring(5, 3), cnpj.Substring(8, 4), cnpj.Substring(12, 2));
        }

        #endregion

        #region ValidaCpfCnpj

        /// <summary>
        /// Validação para CPF e CNPJ
        /// </summary>
        /// <param name="cpfCnpj"></param>  Valor informado pelo usuário.
        /// <param name="isRequired"></param>   Informa se é obrigatorio.
        /// <returns> Retorna True se for inválido.</returns>
        public static bool ValidaCpfCnpjCalculo(string cpfCnpj, bool? isRequired = false)
        {
            if (string.IsNullOrEmpty(cpfCnpj))
                return isRequired.Value;

            if (IsCnpj(cpfCnpj))
            {
                return !ValidarCnpj(cpfCnpj);
            }
            return !ValidarCpf(cpfCnpj);
        }
        private static bool ValidarCpf(string numeroCpf)
        {
            if (ValidaFormatCpf(numeroCpf))
                return false;

            var numeroValido = RetornarNumeros(numeroCpf);
            if (numeroValido.Length != 11)
                return false;

            if (new string(numeroValido[0], numeroValido.Length) == numeroValido) return false;

            var somaPrimeiroDigito = default(int);
            var somaSegundoDigito = default(int);

            somaPrimeiroDigito = SomarNumeroCpf(numeroValido, somaPrimeiroDigito, true);
            somaSegundoDigito = SomarNumeroCpf(numeroValido, somaSegundoDigito, false);

            int primeiroDigito = ValorDigitoCpfCnpj(somaPrimeiroDigito);
            int segundoDigito = ValorDigitoCpfCnpj(somaSegundoDigito);

            return numeroCpf.Substring(numeroCpf.Length - 2, 2).Equals(string.Concat(primeiroDigito.ToString(), segundoDigito.ToString()));
        }
        private static bool ValidaFormatCpf(string numeroCpf)
        {
            var regex = new Regex(@"^((\d{3}).(\d{3}).(\d{3})-(\d{2}))*$");

            Match match = regex.Match(numeroCpf);

            return !match.Success;
        }
        private static int SomarNumeroCpf(string numeroValido, int somaDigito, bool isPrimeiroDigitoVerificador)
        {
            Array.ForEach<int>(Enumerable.Range(0, numeroValido.Length - (isPrimeiroDigitoVerificador ? 2 : 1)).ToArray(), (posicao) =>
            {
                somaDigito += ((int)Char.GetNumericValue(numeroValido[posicao]) * (12 - (isPrimeiroDigitoVerificador ? 2 : 1) - posicao));
            });
            return somaDigito;
        }
        private static bool ValidarCnpj(string numeroCnpj)
        {
            if (ValidaFormatCnpj(numeroCnpj))
                return false;

            var numeroValido = RetornarNumeros(numeroCnpj);
            if (numeroValido.Length != 14)
                return false;

            if (new string(numeroValido[0], numeroValido.Length) == numeroValido) return false;

            var somaPrimeiroDigito = default(int);
            var somaSegundoDigito = default(int);

            somaPrimeiroDigito = SomarNumeroCnpj(numeroValido, somaPrimeiroDigito, true);
            somaSegundoDigito = SomarNumeroCnpj(numeroValido, somaSegundoDigito, false);

            int primeiroDigito = ValorDigitoCpfCnpj(somaPrimeiroDigito);
            int segundoDigito = ValorDigitoCpfCnpj(somaSegundoDigito);

            return numeroCnpj.Substring(numeroCnpj.Length - 2, 2).Equals(string.Concat(primeiroDigito.ToString(), segundoDigito.ToString()));
        }
        private static bool ValidaFormatCnpj(string numeroCnpj)
        {
            var regex = new Regex(@"^((\d{2}).(\d{3}).(\d{3})/(\d{4})-(\d{2}))*$");

            var match = regex.Match(numeroCnpj);

            return !match.Success;
        }
        private static int SomarNumeroCnpj(string numeroValido, int somaDigito, bool isPrimeiroDigitoVerificador)
        {
            var sequencia = new List<int> { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            Array.ForEach<int>(Enumerable.Range(0, numeroValido.Length - (isPrimeiroDigitoVerificador ? 2 : 1)).ToArray(), (posicao) =>
            {
                somaDigito += ((int)Char.GetNumericValue(numeroValido[posicao]) * (sequencia[(isPrimeiroDigitoVerificador ? 1 : 0) + posicao]));
            });
            return somaDigito;
        }
        private static int ValorDigitoCpfCnpj(int somaDigito)
        {
            var digito = ((somaDigito * 10) % 11);
            return digito > 9 ? 0 : digito;
        }

        #endregion

        public static bool IsCnpj(string cpfCnpj)
        {
            cpfCnpj = RetornarNumeros(cpfCnpj);
            if (cpfCnpj.Length == 14)
                return true;
            return false;
        }

        #endregion

        public static string RetornarNumeros(string value)
        {
            return Regex.Replace(value, @"[^0-9]", "");
        }
    }
}

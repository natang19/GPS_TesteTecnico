﻿using Cnpj.Business.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cnpj.Business.Validacoes
{
    public static class ValidacaoCnpj
    {
        public const int TamanhoCnpj = 14;

        public static bool Validar(string cpnj)
        {
            if (!TemTamanhoValido(cpnj)) return false;
            return !TemDigitosRepetidos(cpnj) && TemDigitosValidos(cpnj);
        }

        private static bool TemTamanhoValido(string valor)
        {
            return valor.Length == TamanhoCnpj;
        }

        private static bool TemDigitosRepetidos(string valor)
        {
            string[] invalidNumbers =
            {
                "00000000000000",
                "11111111111111",
                "22222222222222",
                "33333333333333",
                "44444444444444",
                "55555555555555",
                "66666666666666",
                "77777777777777",
                "88888888888888",
                "99999999999999"
            };
            return invalidNumbers.Contains(valor);
        }

        private static bool TemDigitosValidos(string valor)
        {
            var number = valor.Substring(0, TamanhoCnpj - 2);

            var digitoVerificador = new DigitoVerificador(number)
                .ComMultiplicadoresDeAte(2, 9)
                .Substituindo("0", 10, 11);
            var firstDigit = digitoVerificador.CalculaDigito();
            digitoVerificador.AddDigito(firstDigit);
            var secondDigit = digitoVerificador.CalculaDigito();

            return string.Concat(firstDigit, secondDigit) == valor.Substring(TamanhoCnpj - 2, 2);
        }
    }

    public class DigitoVerificador
    {
        private string _numero;
        private const int Modulo = 11;
        private readonly List<int> _multiplicadores = new List<int> { 2, 3, 4, 5, 6, 7, 8, 9 };
        private readonly IDictionary<int, string> _substituicoes = new Dictionary<int, string>();
        private bool _complementarDoModulo = true;

        public DigitoVerificador(string numero)
        {
            _numero = numero;
        }

        public DigitoVerificador ComMultiplicadoresDeAte(int primeiroMultiplicador, int ultimoMultiplicador)
        {
            _multiplicadores.Clear();
            for (var i = primeiroMultiplicador; i <= ultimoMultiplicador; i++)
                _multiplicadores.Add(i);

            return this;
        }

        public DigitoVerificador Substituindo(string substituto, params int[] digitos)
        {
            foreach (var i in digitos)
            {
                _substituicoes[i] = substituto;
            }
            return this;
        }

        public void AddDigito(string digito)
        {
            _numero = string.Concat(_numero, digito);
        }

        public string CalculaDigito()
        {
            return !(_numero.Length > 0) ? "" : GetDigitSum();
        }

        private string GetDigitSum()
        {
            var soma = 0;
            for (int i = _numero.Length - 1, m = 0; i >= 0; i--)
            {
                var produto = (int)char.GetNumericValue(_numero[i]) * _multiplicadores[m];
                soma += produto;

                if (++m >= _multiplicadores.Count) m = 0;
            }

            var mod = (soma % Modulo);
            var resultado = _complementarDoModulo ? Modulo - mod : mod;

            return _substituicoes.ContainsKey(resultado) ? _substituicoes[resultado] : resultado.ToString();
        }
    }
}

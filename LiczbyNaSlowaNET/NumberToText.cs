﻿
// Copyright (c) 2014 Przemek Walkowski

using Ninject;
using System.Collections.Generic;

namespace LiczbyNaSlowaNET
{
    internal enum phase { beforeComma = 1, afterComma };

    public enum Currency { None, PL };

    public static class NumberToText
    {
        private static IKernel kernel;

        static NumberToText()
        {
            kernel = new StandardKernel();

            kernel.Bind(typeof(IDictionaries)).To<PolishDictionary>();
            kernel.Bind<CommonAlgorithm>().ToSelf();
            kernel.Bind<CurrencyAlgorithm>().ToSelf();
            kernel.Bind<NumberToTextOptions>().ToSelf();
        }
        
        /// <summary>
        /// Convert number into words.
        /// </summary>
        /// <param name="number">Number to convert</param>
        /// <returns>The words describe number</returns>
        public static string Convert(int number, Currency currency)
        {
            var options = kernel.Get<NumberToTextOptions>();

            options.curency = currency;

            return CommonConver(new int[] { number }, options);
        }

        public static string Convert(int number, NumberToTextOptions options)
        {
            return CommonConver(new int[] { number }, options);
        }

        public static string Convert(decimal number, Currency currency = Currency.None)
        {
            var options = kernel.Get<NumberToTextOptions>();

            options.curency = currency;

            return CommonConver(PrepareNumbers(number), options);
        }

        public static string Convert(decimal number, NumberToTextOptions options)
        {
            return CommonConver(PrepareNumbers(number), options);
        }

        private static string CommonConver(IEnumerable<int> numbers, NumberToTextOptions options)
        {
            var algorithm = GetAlgorithm(options.curency);

            algorithm.Dictionaries = options.Dictionary ?? kernel.Get<IDictionaries>(); 
            algorithm.Numbers = numbers;
            algorithm.Options = options;

            return algorithm.Build();
        }

        private static IAlgorithm GetAlgorithm(Currency currency)
        {
           switch (currency)
            {
                case Currency.PL:
                    return kernel.Get<CurrencyAlgorithm>();
                default:
                    return kernel.Get<CommonAlgorithm>();
            }
        }

        private static IEnumerable<int> PrepareNumbers(decimal numbers)
       {
            var splitNumber = numbers.ToString().Replace('.', '@').Replace(',', '@').Split('@');

            if (splitNumber.Length > 1 && splitNumber[1].Length == 1)
            {
                splitNumber[1] += "0";
            }
            
            var allNumbers = new List<int>();

            for (int i = 0; i < splitNumber.Length; i++)
            {
                int intNumber;

                if (int.TryParse(splitNumber[i], out intNumber))
                {
                    allNumbers.Add(intNumber);
                }
            }

            return allNumbers;
        }

    }
}
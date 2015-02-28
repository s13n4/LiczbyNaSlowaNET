﻿
// Copyright (c) 2014 Przemek Walkowski

using Ninject;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LiczbyNaSlowaNET
{
    public static class NumberToText
    {
        public enum Currency { None, PL };

        private static IKernel kernel;

        static NumberToText()
        {
            kernel = new StandardKernel();

            kernel.Bind(typeof(IDictionaries)).To<Dictionaries>();

        }

        private static IConverterBuldier GetConverterBuldier(Currency currency)
        {
            switch (currency)
            {
                case Currency.PL:
                    return kernel.Get<CurrencyConvertAlgorithm>();
                default:
                    return kernel.Get<ConverterAlgorithm>();
            }
        }

        private static string CommonConver(int[] numbers, Currency currency = Currency.None)
        {
            var converterBuldier = GetConverterBuldier(currency);

            converterBuldier.Numbers = numbers;

            var commonConverter = new CommonConverter(converterBuldier);

            return commonConverter.Convert();
        }

        /// <summary>
        /// Convert number into words.
        /// </summary>
        /// <param name="number">Number to convert</param>
        /// <returns>The words describe number</returns>
        public static string Convert(int number, Currency currency)
        {
            return CommonConver(new int[] { number }, currency);
        }

        public static string Convert(decimal number, Currency currency = Currency.None)
        {
            var splitNumber = number.ToString().Replace('.','@').Replace(',','@').Split('@');

            var allNumbers = new List<int>();

            for (int i = 0; i < splitNumber.Length; i++)
            {
                int intNumber;

                if (int.TryParse(splitNumber[i], out intNumber))
                {
                    allNumbers.Add(intNumber);
                }
            }

            return CommonConver(allNumbers.ToArray(), currency);
        }
    }
}
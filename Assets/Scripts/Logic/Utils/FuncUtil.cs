using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using System;

namespace Logic
{
    public static class FuncUtil
    {
        public static string EnumToString<T>(T type) => System.Enum.GetName(typeof(T), type);
        public static int GetEnumCount(this Type enumType)
        {
            return Enum.GetValues(enumType).Length;
        }

        public static int GetRandomRange(this int maxNumber)
        {
            return UnityEngine.Random.Range(0, maxNumber);
        }

        public static int GetIncludeNegativeRandomRange(this int maxNumber)
        {
            var isPositiveNumber = 2.GetRandomRange() == 0 ? true : false;


            return UnityEngine.Random.Range(0, isPositiveNumber ? maxNumber : -maxNumber);
        }

        public static float GetRandomRange(this float maxNumber)
        {
            return UnityEngine.Random.Range(0, maxNumber);
        }


        public static int GetRandomRange(int min, int max, bool includeMax = false)
        {
            return UnityEngine.Random.Range(min, includeMax ? max : max + 1);
        }

        public static float GetRandomRange(float min, float max, bool includeMax = false)
        {
            return UnityEngine.Random.Range(min, includeMax ? max : max + 1);
        }

        public static T GetRandomEnumValue<T>() where T : Enum
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException();

            return (T)Enum.GetValues(typeof(T))
                .OfType<Enum>()
                .OrderBy(e => Guid.NewGuid())
                .FirstOrDefault();
        }
        public static T GetRandomEnumValue<T>(T exceptEnumValue) where T : Enum
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException();

            return (T)Enum
                .GetValues(typeof(T))
                .OfType<Enum>()
                .OrderBy(e => Guid.NewGuid())
                .FirstOrDefault(e => !e.Equals(exceptEnumValue));
        }
        public static T GetRandomEnumValue<T>(T exceptEnumValue,T mustExceptEnumValue) where T : Enum
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException();

            return (T)Enum
                .GetValues(typeof(T))
                .OfType<Enum>()
                .Where(e => !e.Equals(mustExceptEnumValue))
                .OrderBy(e => Guid.NewGuid())
                .FirstOrDefault(e => !e.Equals(exceptEnumValue));
        }
    }
}
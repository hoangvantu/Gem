﻿using System;
using System.Linq;
using System.Collections.Generic;
using Gem.Network.Extensions;

namespace Gem.Network.Builders
{
    public class DynamicPropertyInfo
    {
        public Type PropertyType { get; set; }

        public string PropertyName { get; set; }

        public const string PropertyPrefix = "pprefix";

        #region Static Helpers

        /// <summary>
        /// Sets up a generic list of <see cref="DynamicPropertyInfo"/>
        /// </summary>
        /// <param name="types">The types of property info</param>
        /// <param name="propertyPrefix">The prefix of the type's names</param>
        /// <returns>A list of <see cref="DynamicPropertyInfo"/></returns>
        public static List<DynamicPropertyInfo> GetPropertyInfo(Type[] types)
        {
            var propertyInfo = new List<DynamicPropertyInfo>();

            for (int i = 0; i < types.Count(); i++)
            {
                propertyInfo.Add(new DynamicPropertyInfo
                {
                    PropertyName = PropertyPrefix + i,
                    PropertyType = types[i]
                });
            }

            return propertyInfo;
        }


        public static IEnumerable<DynamicPropertyInfo> GetPropertyTypesAndNames<T>()
            where T : new()
        {
            return new T().GetType()
                      .GetProperties()
                      .Select(x => new DynamicPropertyInfo
                      {
                          PropertyName = x.Name,
                          PropertyType = x.PropertyType
                      });
        }

        public static DynamicPropertyInfo GetPropertyInfo(Type types, int order)
        {
                return new DynamicPropertyInfo
                {
                    PropertyName = PropertyPrefix + order,
                    PropertyType = types
                };
        }
        
        /// <summary>
        /// Matches the primitive types to their string representation
        /// </summary>
        private static Dictionary<Type, string> PrimitiveTypesAndAliases = new Dictionary<Type, string>()
            {
                 {typeof(Byte),"byte"},
                 {typeof(SByte),"sbyte"},
                 {typeof(Int32),"int"},
                 {typeof(UInt32),"uint"},
                 {typeof(Int16),"short"},
                 {typeof(UInt16),"ushort"},
                 {typeof(Int64),"long"},
                 {typeof(UInt64),"ulong"},
                 {typeof(Single),"float"},
                 {typeof(Double),"double"},
                 {typeof(Char),"char"},
                 {typeof(Boolean),"bool"},
                 {typeof(String),"string"},
                 {typeof(Decimal),"decimal"}
            };

        /// <summary>
        /// This is used in the constructor body of CsScriptPOCOBuilder
        /// </summary>
        private static Dictionary<Type, string> DecodeInfo = new Dictionary<Type, string>()
            {
                 {typeof(Byte),"ReadByte"},
                 {typeof(SByte),"ReadSByte"},
                 {typeof(Int32),"ReadInt32"},
                 {typeof(UInt32),"ReadUInt32"},
                 {typeof(Int16),"ReadInt16"},
                 {typeof(UInt16),"ReadUInt16"},
                 {typeof(Int64),"ReadInt64"},
                 {typeof(UInt64),"ReadUInt64"},
                 {typeof(Single),"ReadFloat"},
                 {typeof(Double),"ReadDouble"},
                //{typeof(Char),"char"},
                 {typeof(Boolean),"ReadBoolean"},
                 {typeof(String),"ReadString"},
                //{typeof(Decimal),"decimal"}
            };

        public static string GetPrimitiveTypeAlias(Type primitiveType)
        {
            if (PrimitiveTypesAndAliases.ContainsKey(primitiveType))
            {
                return PrimitiveTypesAndAliases[primitiveType];
            }
            else
            {
                throw new InvalidOperationException("Unsupported type");
            }
        }

        public static string GetDecodePrefix(Type primitiveType)
        {
            if (DecodeInfo.ContainsKey(primitiveType))
            {
                return DecodeInfo[primitiveType];
            }
            else
            {
                throw new InvalidOperationException("Unsupported type for decoding");
            }
        }

        #endregion
    }
}

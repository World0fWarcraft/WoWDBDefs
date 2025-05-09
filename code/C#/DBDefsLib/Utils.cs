﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static DBDefsLib.Structs;

namespace DBDefsLib
{
    public static class Utils
    {
        [Obsolete("Use of the Utils.ParseBuild() method is deprecated, it will be removed in an upcoming version. Use Build(build) instead.")]
        public static Build ParseBuild(string build)
        {
            return new Build(build);
        }

        [Obsolete("Use of the Utils.BuildToString() method is deprecated, it will be removed in an upcoming version. Use Build.ToString() instead.")]
        public static string BuildToString(Build build)
        {
            return build.ToString();
        }

        public static bool GetVersionDefinitionByLayoutHash(DBDefinition definition, string layoutHash, out VersionDefinitions? versionToUse)
        {
            foreach (var versionDefinition in definition.versionDefinitions)
            {
                if (versionDefinition.layoutHashes.Contains(layoutHash))
                {
                    versionToUse = versionDefinition;
                    return true;
                }
            }

            versionToUse = null;
            return false;
        }

        public static bool GetVersionDefinitionByBuild(DBDefinition definition, Build build, out VersionDefinitions? versionToUse)
        {
            foreach (var versionDefinition in definition.versionDefinitions)
            {
                if (versionDefinition.builds.Contains(build))
                {
                    versionToUse = versionDefinition;
                    return true;
                }

                foreach (var range in versionDefinition.buildRanges)
                {
                    if (range.Contains(build))
                    {
                        versionToUse = versionDefinition;
                        return true;
                    }
                }
            }

            versionToUse = null;
            return false;
        }

        public static string NormalizeColumn(string col, bool fixFirst = true)
        {
            var thingsToUpperCase = new List<string> { "ID", "WMO" };

            // ugh
            var filteredOut = new List<string> { "mid" };

            var cleaned = col;

            foreach (var thingToUpperCase in thingsToUpperCase)
            {
                if (filteredOut.Contains(col))
                {
                    continue;
                }

                if (cleaned.StartsWith(thingToUpperCase, StringComparison.CurrentCultureIgnoreCase))
                {
                    cleaned = thingToUpperCase + cleaned.Substring(thingToUpperCase.Length);
                }

                if (cleaned.EndsWith(thingToUpperCase, StringComparison.CurrentCultureIgnoreCase))
                {
                    cleaned = cleaned.Substring(0, cleaned.Length - thingToUpperCase.Length) + thingToUpperCase;
                }
            }

            if (cleaned.EndsWith("_"))
            {
                cleaned = cleaned.Substring(0, cleaned.Length - 1);
            }

            if (fixFirst)
            {
                var arr = cleaned.ToCharArray();
                arr[0] = char.ToUpper(arr[0]);
                cleaned = new string(arr);
            }

            return cleaned;
        }

        public static List<string> ReadLines(this StreamReader reader)
        {
            var list = new List<string>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                list.Add(line);
            }

            return list;
        }
    }
    public static class BinaryWriterExtensions
    {
        public static void WriteStringBlockString(this BinaryWriter bw, Dictionary<string, int> stringBlock, string stringToWrite)
        {
            if (string.IsNullOrEmpty(stringToWrite))
            {
                bw.Write(-1);
                return;
            }

            if (stringBlock.TryGetValue(stringToWrite, out var stringIndex))
                bw.Write(stringIndex);
            else
                throw new KeyNotFoundException("String not found in stringblock");
        }
    }
}

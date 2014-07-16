﻿namespace Appccelerate.Version
{
    using System;

    public class TeamCity
    {
        public static void WriteSetParameterMessage(string name, string value, Action<string> log)
        {
            string escapedValue = EscapeValue(value);

            log(string.Format("##teamcity[setParameter name='Appccelerate.Version.{0}' value='{1}']", name, escapedValue));
            log(string.Format("##teamcity[setParameter name='system.Appccelerate.Version.{0}' value='{1}']", name, escapedValue));

        }

        public static void WriteSetVersionMessage(string versionToUseForBuildNumber, Action<string> log)
        {
            log(string.Format("##teamcity[buildNumber '{0}']", EscapeValue(versionToUseForBuildNumber)));
        }

        private static string EscapeValue(string value)
        {
            if (value == null)
            {
                return null;
            }

            // List of escape values from http://confluence.jetbrains.com/display/TCD8/Build+Script+Interaction+with+TeamCity
            value = value.Replace("|", "||");
            value = value.Replace("'", "|'");
            value = value.Replace("[", "|[");
            value = value.Replace("]", "|]");
            value = value.Replace("\r", "|r");
            value = value.Replace("\n", "|n");

            return value;
        }
    }
}
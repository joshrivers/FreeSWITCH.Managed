using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace FreeSWITCH.Managed
{
    public static class CommandStringExtensions
    {
        public static string[] FreeSWITCHCommandParse(this string command)
        {
            char[] spaceArray = new[] { ' ' };
            if (string.IsNullOrEmpty(command))
            {
                return null;
            }
            var args = command.Split(spaceArray, 2, StringSplitOptions.RemoveEmptyEntries);
            if (args.Length == 0 || string.IsNullOrEmpty(args[0]) || string.IsNullOrEmpty(args[0].Trim()))
            {
                return null;
            }
            if (args.Length == 1)
            {
                return new[] { args[0], "" };
            }
            else
            {
                return args;
            }
        }
    }
}

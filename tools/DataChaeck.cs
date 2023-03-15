using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RD91S.tools
{
    internal class DataChaeck
    {
        /// <summary>
        /// 判断是否为电话号
        /// </summary>
        /// <param name="str_handset"></param>
        /// <returns></returns>
        public static bool isHandset(string str_handset)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(str_handset, @"^(13[0-9]|15[012356789]|17[013678]|18[0-9]|14[57]|19[89]|166)[0-9]{8}");
        }
    }
}

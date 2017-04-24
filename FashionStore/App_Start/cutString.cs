using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace FashionStore.App_Start
{
    public class cutString
    {
        private static string[] _cutString(string source)
        {
            string des = Regex.Replace(source, "<.*?>", string.Empty);
            des = Regex.Replace(des, "&nbsp;", "; ");
            return Regex.Split(des, " ");
        }

        public static string SubString(string source, int number)
        {
            string[] str = _cutString(source);
            string des = str[0];
            int length = str.Length >= number ? number : str.Length;
            for (int i = 1; i < length; i++)
            {
                des += " " + str[i];
            }
            return des;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace VMTips_2022.Models
{
    public partial class User
    {
        public class EmailValidation
        {
            public static Regex emailRegEx = new Regex(@"^(([^<>()[\]\\.,;:\s@\""]+"
                                                       + @"(\.[^<>()[\]\\.,;:\s@\""]+)*)|(\"".+\""))@"
                                                       + @"((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}"
                                                       + @"\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+"
                                                       + @"[a-zA-Z]{2,}))$");  //(@"\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b");

            public static bool isValidEmail(string emailAddress)
            {
                if (emailAddress != null)
                    return emailRegEx.IsMatch(emailAddress);
                else
                    return false;
            }
        }
    }
}
namespace HR.Services
{
    public class validationBLL
    {
        // Chech if string value is Null or Empty
        public static bool IsNullOrEmpty(string val)
        {
            if (String.IsNullOrEmpty(val))
            {
                return true;
            }
            return false;
        }

        // Validate if string value is Null or Empty then Return Zero of type string
        public static string IsNullOrEmptyReturnZero(string val)
        {
            if (String.IsNullOrEmpty(val))
            {
                return "0";
            }
            return val;
        }

        // Validate if string value is Null or Empty then Return User Choice of type string
        public static string IsNullOrEmptyReturnUserChoice(string val, string UserChoice)
        {
            if (String.IsNullOrEmpty(val))
            {
                return UserChoice;
            }
            return val;
        }

        // Validate Decimal Values with Alternate options 
        // 5 Overloaded Methods
        public static bool IsDecimalNotNull(decimal? val)
        {
            try
            {

                if (val >= 0 || val < 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }


        }
        public static bool IsDecimalNotNull(string val)
        {
            try
            {
                // if Empty string return false
                if (String.IsNullOrEmpty(val))
                {
                    return false;
                }

                decimal x = Convert.ToDecimal(val);
                if (x >= 0 || x < 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                return false;
            }


        }
        public static decimal IsDecimalNotNull(decimal? val, decimal userVal)
        {
            try
            {

                if (Convert.ToDecimal(val) >= 0 || Convert.ToDecimal(val) < 0)
                {
                    return (decimal)val;
                }
                else
                {
                    return userVal;
                }

            }
            catch (Exception ex)
            {
                return userVal;
            }


        }
        public static decimal IsDecimalNotNull(string val, decimal userVal)
        {
            try
            {
                // if null return 2nd Value
                if (String.IsNullOrEmpty(val))
                {
                    return userVal;
                }

                if (Convert.ToDecimal(val) >= 0 || Convert.ToDecimal(val) < 0)
                {
                    return Convert.ToDecimal(val);
                }
                else
                {
                    return userVal;
                }

            }
            catch (Exception ex)
            {
                return userVal;
            }


        }

        // Date Validation for Null Value
        public static DateTime? isDateNotNull(string dt)
        {
            if (string.IsNullOrEmpty(dt) || !DateTime.TryParse(dt, out DateTime parsedDate))
            {
                return null; // Return null if invalid or empty
            }

            return parsedDate.Date; // Return only the date part
        }

        // Check long  & Int32 Value is Not null or empty or invalid and Give User Choice 
        // to get as desired value if null or invalid value 
        // 3 Overloaded methods available
        public static bool IsNumber(String str)
        {
            try
            {
                // if null return False
                if (String.IsNullOrEmpty(str))
                {
                    return false;
                }
                Double.Parse(str);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static long IsNumber(String str, long UserChoice)
        {
            try
            {
                // if null return 2nd Value
                if (String.IsNullOrEmpty(str))
                {
                    return UserChoice;
                }

                long.Parse(str);
                return long.Parse(str);
            }
            catch (Exception)
            {
                return UserChoice;
            }
        }

        public static Int32 IsNumber(String str, Int32 UserChoice)
        {
            try
            {
                // if null return 2nd Value
                if (String.IsNullOrEmpty(str))
                {
                    return UserChoice;
                }

                Int32.Parse(str);
                return Int32.Parse(str);
            }
            catch (Exception)
            {
                return UserChoice;
            }
        }


        // Check Bool Value is Not null or empty or invalid 3 Methods
        public static bool IsBoolNotNull(bool? str)
        {
            try
            {
                Convert.ToBoolean(str);
                return (bool)str;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool IsBoolNotNull(string str)
        {
            try
            {
                bool x = Convert.ToBoolean(str);
                return x;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool IsBoolNotNull(string str, bool UserChoice)
        {
            try
            {
                // if null return 2nd Value
                if (String.IsNullOrEmpty(str))
                {
                    return UserChoice;
                }


                bool x = Convert.ToBoolean(str);
                return x;
            }
            catch (Exception)
            {
                return UserChoice;
            }
        }

        // testing Function

        //public static long test(EmployeeDetail ed)
        //{
        //    //  decimal? x = (decimal)ed.BasicPay;
        //    string st = ed.Status;
        //    bool v = false;
        //    // st = v.ToString();
        //      return validationBLL.IsNumber(st,99); 
        //}
    }
}


using Newtonsoft.Json;
using System;
namespace NanaFoodWeb.Convert
{
    public class ConvertHelper
    {
        public List<T> ConvertDynamicToList<T>(dynamic dynamicData)
        {
            List<T> dataList = new List<T>();
            try
            {
                string stringData = System.Convert.ToString(dynamicData);
                dataList = JsonConvert.DeserializeObject<List<T>>(stringData);
            }
            catch (Exception)
            {

            }
            

            return dataList;
        }

        public bool ConvertDynamicToT<T>(dynamic dynamicData, out T data)
        {
            data = default; // Initialize out parameter
            try
            {
                string stringData = System.Convert.ToString(dynamicData);
                data = JsonConvert.DeserializeObject<T>(stringData);
                return true;
            }
            catch (Exception ex)
            {
                // Log exception or handle it as necessary
                Console.WriteLine($"Error converting dynamic data to {typeof(T)}: {ex.Message}");
                return false; // Return false if there is an error
            }
        }
        public bool TryParseDynamicToInt(dynamic dynamicValue, out int result)
        {
            // Initialize result to 0
            result = 0;

            try
            {
                // Attempt to convert dynamicValue to int
                if (dynamicValue is int)
                {
                    result = dynamicValue;
                    return true;
                }

                if (dynamicValue is string)
                {
                    return int.TryParse(dynamicValue, out result);
                }

                if (dynamicValue is double || dynamicValue is float || dynamicValue is decimal)
                {
                    result = System.Convert.ToInt32(dynamicValue);
                    return true;
                }

                // Convert dynamicValue to string and try parsing
                return int.TryParse(System.Convert.ToString(dynamicValue), out result);
            }
            catch
            {
                // Handle conversion failure
                return false;
            }
        }

        public bool TryParseDynamicToString(dynamic dynamicValue, out string result)
        {
            // Initialize result to 0
            result = string.Empty;

            try
            {


                if (dynamicValue is string)
                {
                    result = dynamicValue;
                    return true;
                    
                }

                if (dynamicValue is int || dynamicValue is double || dynamicValue is float || dynamicValue is decimal)
                {
                    result = System.Convert.ToString(dynamicValue);
                    return true;
                }

                // Convert dynamicValue to string and try parsing
                return System.Convert.ToString(dynamicValue);
            }
            catch
            {
                // Handle conversion failure
                return false;
            }
        }
    }
}

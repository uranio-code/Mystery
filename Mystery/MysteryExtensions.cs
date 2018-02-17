using Mystery.Json;
using Mystery.Register;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public static class MysteryExtensions
{
    public static void AddRange<T>(this ICollection<T> coll, IEnumerable<T> new_elements )
    {
        foreach (var ele in new_elements)
            coll.Add(ele);
    }

    //clone an object using json
    public static T Clone<T>(this T current_object) where T : new()
    {
        if (current_object == null)
            return default(T);
        var converter = current_object.getGlobalObject<MysteryJsonConverter>();
        return converter.readJson<T>(converter.getJson(current_object));
    }

    /// <summary>
    /// convert bytes into string
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static string getString(this byte[] bytes)
    {
        return System.Text.Encoding.UTF8.GetString(bytes);
    }

    /// <summary>
    /// convert string into bytes
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static byte[] getBytes(this string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            byte[] result = {

            };
            return result;
        }
        return System.Text.Encoding.UTF8.GetBytes(input);
    }

    /// <summary>
    /// convert bytes into string
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static string getBase64(this byte[] bytes)
    {
        return System.Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// cover string into bytes
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static byte[] getBytesBase64(this string input)
    {
        return System.Convert.FromBase64String(input);
    }

    /// <summary>
    /// it does not include hour and minutes
    /// </summary>
    /// <param name="the_date"></param>
    /// <returns></returns>
    /// <remarks></remarks>
    public static string getNiceString(this System.DateTime the_date)
    {
        return the_date.ToString("dd MMM yyyy");
    }

    /// <summary>
    /// return a nice way get the money
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public static string getNiceMoneyString(this double money)
    {
        return money.getNicePrice().ToString("C2");
    }


    /// <summary>
    /// it does round cents down
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public static double getNicePrice(this double price)
    {
        return Math.Round(price, 2);
    }

    /// <summary>
    /// it does round cents down
    /// </summary>
    /// <returns></returns>
    /// <remarks></remarks>
    public static string Tiny(this Guid guid)
    {
        return guid.ToByteArray().getBase64().Replace("=","").Replace("+","-").Replace("/","_");
    }

    public static Guid fromTiny(this string tiny_guid)
    {
        if (string.IsNullOrEmpty(tiny_guid))
            return Guid.Empty;

        if (tiny_guid.Length != 22) return Guid.Empty;
        tiny_guid = tiny_guid.Replace("-", "+").Replace("_", "/") + "==";
        try
        {
            var bites = tiny_guid.getBytesBase64();
            return new Guid(bites);
        }
        catch {
            return Guid.Empty;
        }
    }

    public static log4net.ILog log(this object current_object) {
        return log4net.LogManager.GetLogger(current_object.GetType());
    }


    public static T Execute<T>(this Task<T> task) {
        try
        {
            task.Wait();
        }
        catch (Exception ex) {
            if (ex.InnerException != null)
                throw ex.InnerException;
            else
                throw;
        }
        
        return task.Result;
    }
    


}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class Extension {


    public static IEnumerable<T> ExecuteAction<T>(this IEnumerable<T> list, params Action<T>[] method)
    {
        foreach (var item in list)
        {
            foreach (var executeMethod in method)
            {
                executeMethod?.Invoke(item);
            }
        }
        return list;
    }
}


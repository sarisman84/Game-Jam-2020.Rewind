using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Vector3 = UnityEngine.Vector3;

public class ExtendedVector3 {


    public static Vector3 RandomPositionWithinRange(Vector3 origin, float radius)
    {
        Vector3 result = Vector3.zero;

        result = origin + new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius), Random.Range(-radius, radius));


        return result;
    }
}


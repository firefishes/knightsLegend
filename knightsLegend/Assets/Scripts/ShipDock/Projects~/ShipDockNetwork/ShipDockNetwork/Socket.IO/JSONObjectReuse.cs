#define _SHIPDOCK_POOLING

#define PRETTY		//Comment out when you no longer need to read JSON to disable pretty Print system-wide
//Using doubles will cause errors in VectorTemplates.cs; Unity speaks floats
#define USEFLOAT	//Use floats for numbers instead of doubles	(enable if you're getting too many significant digits in string output)
//#define POOLING	//Currently using a build setting for this one (also it's experimental)

#if SHIPDOCK_POOLING
using ShipDock.Interfaces;
using ShipDock.Pooling;
#endif
using System.Collections.Generic;

public class JSONObjectReuse
#if SHIPDOCK_POOLING
    : IPoolable, IDispose
#endif
{
    public JSONObjectReuse()
    {
        JSONData = new JSONObject();
    }

    public JSONObjectReuse(JSONObject.Type t)
    {
        JSONData = new JSONObject(t);
    }

    public JSONObjectReuse(bool b)
    {
        JSONData = new JSONObject(b);
    }

#if USEFLOAT
	public JSONObjectReuse(float f)
    {
        JSONData = new JSONObject(f);
    }
#else
    public JSONObjectReuse(double d)
    {
        JSONData = new JSONObject(d);
    }
#endif

    public JSONObjectReuse(Dictionary<string, string> dic)
    {
        JSONData = new JSONObject(dic);
    }

    public JSONObjectReuse(Dictionary<string, JSONObject> dic)
    {
        JSONData = new JSONObject(dic);
    }

    public JSONObjectReuse(JSONObject[] objs)
    {
        JSONData = new JSONObject(objs);
    }

    public void Reinit()
    {
        if (JSONData == null)
        {
            JSONData = new JSONObject();
        }
    }

    public void Revert()
    {
        if (JSONData != null)
        {
            JSONData.Clear();
        }
    }

    public void Dispose()
    {
        Revert();
        JSONData = null;
    }

    public void ToPool()
    {
#if SHIPDOCK_POOLING
        Pooling<JSONObjectReuse>.To(this);
#endif
    }

    public JSONObject JSONData { get; private set; }
}
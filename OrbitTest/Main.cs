using System;
using Orbit;
using Orbit.BusinessLayer.Entities;

namespace OrbitTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			double lat = 50.951533;
			double lon = 6.91728;
			
			Sun sun = new Sun ();
			SunPosition sunPos = sun.position (lat, lon, 1, true, DateTime.Now);
			
			Console.WriteLine ("Sun azimuth: " + sunPos.azimuth.ToString () + " and elavation: " + sunPos.elevation.ToString ());
			
			Weather weather = new Weather ();
			WeatherCondition cond = weather.CurrentConditions (lat, lon);
			
			if (cond != null)
			{
				Console.WriteLine(cond.Condition);
			}
		}
	}
}


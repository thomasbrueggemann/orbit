#include <iostream>

namespace Orbit
{
	// SUN POSITION
	class SunPosition
	{
	public:
		double eqTime;
		double solarDec;
		double elevation;
		double azimuth;
		double coszen;
	};

	// SUN
	class Sun
	{
	public:
		// Position
		static SunPosition Position(double latitude, double longitude, int zone, bool daySavings, DateTime time)
		{
			int daylightSavingTime = 0;
			SunPosition resultPosition();

			// All latitudes between 89.8 and 90 S\n will be set to -89.8.
			if ((latitude >= -90) && (latitude < -89.8))
			{
				latitude = -89.8;
			}

			// All latitudes between 89.8 and 90 N\n will be set to 89.8
			if ((latitude <= 90) && (latitude > 89.8))
			{
				latitude = 89.8;
			}

			// The offset must be between -12.5 and 12. Setting "Off-Set"=0
			if (zone > 12 || zone < -12.5)
			{
				zone = 0;
			}

			// if daySaving is true = 60, else = 0
			if (daySavings)
			{
				daylightSavingTime = 60;
			}

			int ss = time.Second;
			int mm = time.Minute;
			int hh = time.Hour - (daylightSavingTime / 60);

			while (hh > 23)
			{
				hh -= 24;
			}

			// timenow is GMT time for calculation, in hours since 0Z
			int timenow = hh + mm / 60 + ss / 3600 + zone;
		};
	};
}

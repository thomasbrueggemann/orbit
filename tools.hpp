#include <iostream>
#include <cmath>

namespace Orbit
{
	// TOOLS
	class Tools
	{
	public:

		// CALC JD
		static double calcJD (int year, int month, int day)
		{
			if (month <= 2) {
				year -= 1;
				month += 12;
			}

			double A = floor(year/100.0);
			double B = 2 - A + floor(A/4);

			return floor(365.25*(year + 4716)) + floor(30.6001*(month+1)) + day + B - 1524.5;
		};

		// CALC DAY OF YEAR
		static double calcDayOfYear(int mn, int dy, bool lpyr)
		{
			int k = (lpyr ? 1 : 2);
			return floor((275 * mn)/9.0) - k * floor((mn + 9.0)/12.0) + dy -30;
		};

		// CALC DAY OF WEEK
		static std::string calcDayOfWeek (double juld)
		{
			double A = (juld + 1.5) % 7;
			return (A == 0) ? "Sunday" : (A == 1) ? "Monday" : (A == 2) ? "Tuesday" : (A == 3) ? "Wednesday" : (A == 4) ? "Thursday" : (A == 5) ? "Friday" : "Saturday";
		};

		// IS LEAP YEAR
		static bool isLeapYear (int yr)
		{
			return ((yr % 4 == 0 && yr % 100 != 0) || yr % 400 == 0);
		};

		// CALC TIME JULIAN CENT
		static double calcTimeJulianCent(double jd)
		{
			return (jd - 2451545.0)/36525.0;
		};

		// RAD TO DEG
		static double radToDeg(double angleRad)
		{
			return (180.0 * angleRad / M_PI);
		};

		// DEG TO RAD
		static double degToRad(double angleDeg)
		{
			return (M_PI * angleDeg / 180.0);
	    };
	}
}

#include <iostream>
#include <cmath>

#include "datetime.hpp"
#include "Tools.hpp"

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
	private:
		// CALC GEOM MEAN ANOMALY SUN
		// Calculate the Geometric Mean Anomaly of the Sun
		static double calcGeomMeanAnomalySun(double t)
		{
			return 357.52911 + t * (35999.05029 - 0.0001537 * t);
		};

		// CALC SUN EQ OF CENTER
		// Calculate the equation of center for the sun
		static double calcSunEqOfCenter(double t)
		{
			double m = Sun.calcGeomMeanAnomalySun(t);

			double mrad = Tools.degToRad(m);
			double sinm = sin(mrad);
			double sin2m = sin(mrad+mrad);
			double sin3m = sin(mrad+mrad+mrad);

			return sinm * (1.914602 - t * (0.004817 + 0.000014 * t)) + sin2m * (0.019993 - 0.000101 * t) + sin3m * 0.000289;
		};

		// CALC SUN TRUE ANOMALY
		// Calculate the true anamoly of the su
		static double calcSunTrueAnomaly(double t)
		{
			var m = Sun.calcGeomMeanAnomalySun (t);
			var c = Sun.calcSunEqOfCenter (t);

			return m + c; // in degrees
		};

		// CALC ECCENTRICITY EARTH ORBIT
		// Calculate the eccentricity of earth's orbit
		static double calcEccentricityEarthOrbit(double t)
		{
			return 0.016708634 - t * (0.000042037 + 0.0000001267 * t);
		};

		// CALC SUN RAD VECTOR
		// Calculate the distance to the sun in AU
		static double calcSunRadVector(double t)
		{
			double v = Sun.calcSunTrueAnomaly(t);
			double e = Sun.calcEccentricityEarthOrbit(t);

			return (1.000001018 * (1 - e * e)) / (1 + e * cos(degToRad(v))); // in AUs
		};

		// CALC GEOM MEAN LONG SUN
		// Calculate the Geometric Mean Longitude of the Sun
		static double calcGeomMeanLongSun(double t)
		{
			double L0 = 280.46646 + t * (36000.76983 + 0.0003032 * t);
			while(L0 > 360.0)
			{
				L0 -= 360.0;
			}
			while(L0 < 0.0)
			{
				L0 += 360.0;
			}
			return L0;		// in degrees
		};

		// CALC SUN TRUE LONG
		// Calculate the true longitude of the sun
		static double calcSunTrueLong(double t)
		{
			double l0 = Sun.calcGeomMeanLongSun(t);
			double c = Sun.calcSunEqOfCenter(t);

			return l0 + c; // in degrees
		};

		// CALC SUN APPARENT LONG
		// Calculate the apparent longitude of the sun
		static double calcSunApparentLong(double t)
		{
			double o = Sun.calcSunTrueLong (t);

			double omega = 125.04 - 1934.136 * t;
			return o - 0.00569 - 0.00478 * sin (Tools.degToRad (omega)); // in degrees
		};

		// CALC MEAN OBLIQUITY OF ECLIPTIC
		// Calculate the mean obliquity of the ecliptic
		static double calcMeanObliquityOfEcliptic(double t)
		{
			double seconds = 21.448 - t*(46.8150 + t*(0.00059 - t*(0.001813)));
			return 23.0 + (26.0 + (seconds/60.0))/60.0; // in degrees
		};

		// CALC OBLIQUITY CORRECTION
		// Calculate the corrected obliquity of the ecliptic
		static double calcObliquityCorrection(double t)
		{
			double e0 = Sun.calcMeanObliquityOfEcliptic(t);

			double omega = 125.04 - 1934.136 * t;
			return e0 + 0.00256 * cos(Tools.degToRad(omega)); // in degrees
		};

		// CALC SUN RT ASCENSION
		// Calculate the right ascension of the sun
		static double calcSunRtAscension(double t)
		{
			double e = Sun.calcObliquityCorrection(t);
			double lambda = Sun.calcSunApparentLong(t);

			double tananum = (cos(Tools.degToRad(e)) * sin(Tools.degToRad(lambda)));
			double tanadenom = (cos(Tools.degToRad(lambda)));
			return Tools.radToDeg(atan2(tananum, tanadenom)); // in degrees
		};

		// CALC SUN DECLINATION
		// Calculate the declination of the sun
		static double calcSunDeclination (double t)
		{
			double e = Sun.calcObliquityCorrection (t);
			double lambda = Sun.calcSunApparentLong (t);

			double sint = sin (Tools.degToRad (e)) * sin (Tools.degToRad (lambda));
			return Tools.radToDeg (asin (sint)); // in degrees
		};

		// CALC EQUATION OF TIME
		// Calculate the difference between true solar time and mean solar time
		static double calcEquationOfTime(double t)
		{
			double epsilon = Sun.calcObliquityCorrection(t);
			double l0 = Sun.calcGeomMeanLongSun(t);
			double e = Sun.calcEccentricityEarthOrbit(t);
			double m = Sun.calcGeomMeanAnomalySun(t);

			var y = tan(Tools.degToRad(epsilon)/2.0);
			y *= y;

			double sin2l0 = sin(2.0 * Tools.degToRad(l0));
			double sinm   = sin(Tools.degToRad(m));
			double cos2l0 = cos(2.0 * Tools.degToRad(l0));
			double sin4l0 = sin(4.0 * Tools.degToRad(l0));
			double sin2m  = sin(2.0 * Tools.degToRad(m));

			double Etime = y * sin2l0 - 2.0 * e * sinm + 4.0 * e * y * sinm * cos2l0
					- 0.5 * y * y * sin4l0 - 1.25 * e * e * sin2m;

			return Tools.radToDeg(Etime)*4.0;	// in minutes of time
		};

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


			double JD = Tools.calcJD (time.Year, time.Month, time.Day);
			double T = Tools.calcTimeJulianCent (JD + timenow / 24.0);

			double theta = Sun.calcSunDeclination (T);
			double Etime = Sun.calcEquationOfTime (T);

			double eqTime = Etime;
			double solarDec = theta;

			resultPosition.eqTime = (floor (100 * eqTime)) / 100;
			resultPosition.solarDec = (floor (100 * (solarDec))) / 100;

			double solarTimeFix = eqTime - 4.0 * longitude + 60.0 * zone;
			double trueSolarTime = hh * 60.0 + mm + ss / 60.0 + solarTimeFix;
			// in minutes

			while (trueSolarTime > 1440) {
				trueSolarTime -= 1440;
			}

			//var hourAngle = calcHourAngle(timenow, longitude, eqTime);
			double hourAngle = trueSolarTime / 4.0 - 180.0;
			// Thanks to Louis Schwarzmayr for finding our error,
			// and providing the following 4 lines to fix it:
			if (hourAngle < -180) {
				hourAngle += 360.0;
			}

			double haRad = Tools.degToRad (hourAngle);

			double csz = sin (Tools.degToRad (latitude)) * sin (Tools.degToRad (solarDec)) + cos (Tools.degToRad (latitude)) * cos (Tools.degToRad (solarDec)) * cos (haRad);
			if (csz > 1.0) {
				csz = 1.0;
			} else if (csz < -1.0) {
				csz = -1.0;
			}

			double zenith = Tools.radToDeg (acos (csz));

			double azDenom = (cos (Tools.degToRad (latitude)) * sin (Tools.degToRad (zenith)));
			double azimuth;
			if (abs (azDenom) > 0.001) {
				double azRad = ((sin (Tools.degToRad (latitude)) * cos (Tools.degToRad (zenith))) - sin (Tools.degToRad (solarDec))) / azDenom;
				if (abs (azRad) > 1.0) {
					if (azRad < 0) {
						azRad = -1.0;
					} else {
						azRad = 1.0;
					}
				}

				azimuth = 180.0 - Tools.radToDeg (acos (azRad));

				if (hourAngle > 0.0) {
					azimuth = -azimuth;
				}
			} else {
				if (latitude > 0.0) {
					azimuth = 180.0;
				} else {
					azimuth = 0.0;
				}
			}
			if (azimuth < 0.0) {
				azimuth += 360.0;
			}

			double exoatmElevation = 90.0 - zenith;
			double refractionCorrection;
			if (exoatmElevation > 85.0) {
				refractionCorrection = 0.0;
			} else {
				double te = tan (Tools.degToRad (exoatmElevation));
				if (exoatmElevation > 5.0) {
					refractionCorrection = 58.1 / te - 0.07 / (te * te * te) + 0.000086 / (te * te * te * te * te);
				} else if (exoatmElevation > -0.575) {
					refractionCorrection = 1735.0 + exoatmElevation * (-518.2 + exoatmElevation * (103.4 + exoatmElevation * (-12.79 + exoatmElevation * 0.711)));
				} else {
					refractionCorrection = -20.774 / te;
				}
				refractionCorrection = refractionCorrection / 3600.0;
			}

			double solarZen = zenith - refractionCorrection;

			if (solarZen < 108.0) {
				// astronomical twilight
				resultPosition.azimuth = (floor (100.0 * azimuth)) / 100.0;
				resultPosition.elevation = (floor (100 * (90.0 - solarZen))) / 100;
				if (solarZen < 90.0) {
					resultPosition.coszen = (floor (10000.0 * (cos (Tools.degToRad (solarZen))))) / 10000.0;
				} else {
					resultPosition.coszen = 0.0;
				}
			} else {
				// do not report az & el after astro twilight
				resultPosition.azimuth = -1;
				resultPosition.elevation = -1;
				resultPosition.coszen = 0.0;
			}

			return resultPosition;
		};
	};
}

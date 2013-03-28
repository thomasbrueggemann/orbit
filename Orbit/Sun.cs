using System;

using Orbit.BusinessLayer.Entities;
using Orbit.BusinessLayer.Controller;

namespace Orbit
{
	public class Sun
	{
		/// <summary>
		/// Calculates the curretn sun position
		/// </summary>
		/// <param name="latitude">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="longitude">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="zone">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="daySavings">
		/// A <see cref="System.Boolean"/>
		/// </param>
		/// <param name="time">
		/// A <see cref="DateTime"/>
		/// </param>
		/// <returns>
		/// A <see cref="SunPosition"/>
		/// </returns>
		public SunPosition position (double latitude, double longitude, int zone, bool daySavings, DateTime time)
		{
			int daylightSavingTime = 0;
			SunPosition resultPosition = new SunPosition ();
			
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
			
			Tools tools = new Tools ();
			
			double JD = tools.calcJD (time.Year, time.Month, time.Day);
			double T = tools.calcTimeJulianCent (JD + timenow / 24.0);

			double theta = tools.calcSunDeclination (T);
			double Etime = tools.calcEquationOfTime (T);
			
			double eqTime = Etime;
			double solarDec = theta;
			
			resultPosition.eqTime = (Math.Floor (100 * eqTime)) / 100;
			resultPosition.solarDec = (Math.Floor (100 * (solarDec))) / 100;
			
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
			
			double haRad = tools.degToRad (hourAngle);
			
			double csz = Math.Sin (tools.degToRad (latitude)) * Math.Sin (tools.degToRad (solarDec)) + Math.Cos (tools.degToRad (latitude)) * Math.Cos (tools.degToRad (solarDec)) * Math.Cos (haRad);
			if (csz > 1.0) {
				csz = 1.0;
			} else if (csz < -1.0) {
				csz = -1.0;
			}
			
			double zenith = tools.radToDeg (Math.Acos (csz));
			
			double azDenom = (Math.Cos (tools.degToRad (latitude)) * Math.Sin (tools.degToRad (zenith)));
			double azimuth;
			if (Math.Abs (azDenom) > 0.001) {
				double azRad = ((Math.Sin (tools.degToRad (latitude)) * Math.Cos (tools.degToRad (zenith))) - Math.Sin (tools.degToRad (solarDec))) / azDenom;
				if (Math.Abs (azRad) > 1.0) {
					if (azRad < 0) {
						azRad = -1.0;
					} else {
						azRad = 1.0;
					}
				}
				
				azimuth = 180.0 - tools.radToDeg (Math.Acos (azRad));
				
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
				double te = Math.Tan (tools.degToRad (exoatmElevation));
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
				resultPosition.azimuth = (Math.Floor (100.0 * azimuth)) / 100.0;
				resultPosition.elevation = (Math.Floor (100 * (90.0 - solarZen))) / 100;
				if (solarZen < 90.0) {
					resultPosition.coszen = (Math.Floor (10000.0 * (Math.Cos (tools.degToRad (solarZen))))) / 10000.0;
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
		}	
	}
}


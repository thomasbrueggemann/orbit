using System;

namespace Orbit.BusinessLayer.Controller
{
	public class Tools
	{
		
		//***********************************************************************/
		//* Name:    calcJD									                    */
		//* Type:    Function									                */
		//* Purpose: Julian day from calendar day						        */
		//* Arguments:										                    */
		//*   year : 4 digit year								                */
		//*   month: January = 1								                */
		//*   day  : 1 - 31									                    */
		//* Return value:										                */
		//*   The Julian day corresponding to the date					        */
		//* Note:											                    */
		//*   Number is returned for start of day.  Fractional days should be	*/
		//*   added later.									                    */
		//***********************************************************************/

			public double calcJD (int year, int month, int day)
			{
				if (month <= 2) {
					year -= 1;
					month += 12;
				}
				
				double A = Math.Floor(year/100.0);
				double B = 2 - A + Math.Floor(A/4);
		
				double JD = Math.Floor(365.25*(year + 4716)) + Math.Floor(30.6001*(month+1)) + day + B - 1524.5;
				return JD;
			}
		
		//***********************************************************************/
		//* Name:    calcDayOfYear								                */
		//* Type:    Function									                */
		//* Purpose: Finds numerical day-of-year from mn, day and lp year info  */
		//* Arguments:										                    */
		//*   month: January = 1								                */
		//*   day  : 1 - 31									                    */
		//*   lpyr : 1 if leap year, 0 if not						            */
		//* Return value:										                */
		//*   The numerical day of year							                */
		//***********************************************************************/
		
			public double calcDayOfYear(int mn, int dy, bool lpyr) 
			{
				int k = (lpyr ? 1 : 2);
				double doy = Math.Floor((275 * mn)/9.0) - k * Math.Floor((mn + 9.0)/12.0) + dy -30;
				
			    return doy;
			}
		
		
		//***********************************************************************/
		//* Name:    calcDayOfWeek		                 						*/
		//* Type:    Function			                						*/
		//* Purpose: Derives weekday from Julian Day	        				*/
		//* Arguments:			                    					 		*/
		//*   juld : Julian Day						                 			*/
		//* Return value:					                 					*/
		//*   String containing name of weekday	            					*/
		//***********************************************************************/
		
			public string calcDayOfWeek (double juld)
			{
				double A = (juld + 1.5) % 7;
				string DOW = (A == 0) ? "Sunday" : (A == 1) ? "Monday" : (A == 2) ? "Tuesday" : (A == 3) ? "Wednesday" : (A == 4) ? "Thursday" : (A == 5) ? "Friday" : "Saturday";
				
			    return DOW;
			}
		
		
		//	'isLeapYear' returns '1' if the yr is a leap year, '0' if it is not.

			public bool isLeapYear (int yr)
			{
				return ((yr % 4 == 0 && yr % 100 != 0) || yr % 400 == 0);
			}
		
		//***********************************************************************/
		//* Name:    calcTimeJulianCent							                */
		//* Type:    Function									                */
		//* Purpose: convert Julian Day to centuries since J2000.0.		     	*/
		//* Arguments:										                    */
		//*   jd : the Julian Day to convert	            					*/
		//* Return value:				                						*/
		//*   the T value corresponding to the Julian Day       				*/
		//***********************************************************************/
		
			public double calcTimeJulianCent(double jd)
			{
				var T = (jd - 2451545.0)/36525.0;
				return T;
			}
		
		// Convert radian angle to degrees

			public double radToDeg(double angleRad) 
			{
				return (180.0 * angleRad / Math.PI);
			}
		
		//*********************************************************************/
		
		// Convert degree angle to radians
		
			public double degToRad(double angleDeg) 
			{
				return (Math.PI * angleDeg / 180.0);
		    }
		
		//***********************************************************************/
		//* Name:    calGeomAnomalySun							*/
		//* Type:    Function									*/
		//* Purpose: calculate the Geometric Mean Anomaly of the Sun		*/
		//* Arguments:										*/
		//*   t : number of Julian centuries since J2000.0				*/
		//* Return value:										*/
		//*   the Geometric Mean Anomaly of the Sun in degrees			*/
		//***********************************************************************/
		
			public double calcGeomMeanAnomalySun(double t)
			{
				double M = 357.52911 + t * (35999.05029 - 0.0001537 * t);
				return M;		// in degrees
			}
		
		//***********************************************************************/
		//* Name:    calcSunEqOfCenter							*/
		//* Type:    Function									*/
		//* Purpose: calculate the equation of center for the sun			*/
		//* Arguments:										*/
		//*   t : number of Julian centuries since J2000.0				*/
		//* Return value:										*/
		//*   in degrees										*/
		//***********************************************************************/
		
		
			public double calcSunEqOfCenter(double t)
			{
				double m = this.calcGeomMeanAnomalySun(t);
		
				double mrad = this.degToRad(m);
				double sinm = Math.Sin(mrad);
				double sin2m = Math.Sin(mrad+mrad);
				double sin3m = Math.Sin(mrad+mrad+mrad);
		
				double C = sinm * (1.914602 - t * (0.004817 + 0.000014 * t)) + sin2m * (0.019993 - 0.000101 * t) + sin3m * 0.000289;
				return C;		// in degrees
			}
		
		//***********************************************************************/
		//* Name:    calcSunTrueAnomaly							*/
		//* Type:    Function									*/
		//* Purpose: calculate the true anamoly of the sun				*/
		//* Arguments:										*/
		//*   t : number of Julian centuries since J2000.0				*/
		//* Return value:										*/
		//*   sun's true anamoly in degrees							*/
		//***********************************************************************/
		
			public double calcSunTrueAnomaly (double t)
			{
				var m = this.calcGeomMeanAnomalySun (t);
				var c = this.calcSunEqOfCenter (t);
		
				double v = m + c;
				return v;
				// in degrees
			}
		
		//***********************************************************************/
		//* Name:    calcEccentricityEarthOrbit						*/
		//* Type:    Function									*/
		//* Purpose: calculate the eccentricity of earth's orbit			*/
		//* Arguments:										*/
		//*   t : number of Julian centuries since J2000.0				*/
		//* Return value:										*/
		//*   the unitless eccentricity							*/
		//***********************************************************************/
		
		
			public double calcEccentricityEarthOrbit(double t)
			{
				double e = 0.016708634 - t * (0.000042037 + 0.0000001267 * t);
				return e;		// unitless
			}
		
		//***********************************************************************/
		//* Name:    calcSunRadVector			              					*/
		//* Type:    Function			                  						*/
		//* Purpose: calculate the distance to the sun in AU	     			*/
		//* Arguments:									                     	*/
		//*   t : number of Julian centuries since J2000.0		         		*/
		//* Return value:									                	*/
		//*   sun radius vector in AUs						                	*/
		//***********************************************************************/
		
			public double calcSunRadVector(double t)
			{
				double v = this.calcSunTrueAnomaly(t);
				double e = this.calcEccentricityEarthOrbit(t);
		 
				double R = (1.000001018 * (1 - e * e)) / (1 + e * Math.Cos(degToRad(v)));
				return R;		// in AUs
			}
		
		//***********************************************************************/
		//* Name:    calGeomMeanLongSun							*/
		//* Type:    Function									*/
		//* Purpose: calculate the Geometric Mean Longitude of the Sun		*/
		//* Arguments:										*/
		//*   t : number of Julian centuries since J2000.0				*/
		//* Return value:										*/
		//*   the Geometric Mean Longitude of the Sun in degrees			*/
		//***********************************************************************/
		
			public double calcGeomMeanLongSun(double t)
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
			}
		
		//***********************************************************************/
		//* Name:    calcSunTrueLong								*/
		//* Type:    Function									*/
		//* Purpose: calculate the true longitude of the sun				*/
		//* Arguments:										*/
		//*   t : number of Julian centuries since J2000.0				*/
		//* Return value:										*/
		//*   sun's true longitude in degrees						*/
		//***********************************************************************/
		
		
			public double calcSunTrueLong(double t)
			{
				double l0 = this.calcGeomMeanLongSun(t);
				double c = this.calcSunEqOfCenter(t);
		
				double O = l0 + c;
				return O;		// in degrees
			}
		
		//***********************************************************************/
		//* Name:    calcSunApparentLong							*/
		//* Type:    Function									*/
		//* Purpose: calculate the apparent longitude of the sun			*/
		//* Arguments:										*/
		//*   t : number of Julian centuries since J2000.0				*/
		//* Return value:										*/
		//*   sun's apparent longitude in degrees						*/
		//***********************************************************************/
		
			public double calcSunApparentLong (double t)
			{
				double o = this.calcSunTrueLong (t);
		
				double omega = 125.04 - 1934.136 * t;
				double lambda = o - 0.00569 - 0.00478 * Math.Sin (this.degToRad (omega));
				return lambda;
				// in degrees
			}
		
		//***********************************************************************/
		//* Name:    calcMeanObliquityOfEcliptic						*/
		//* Type:    Function									*/
		//* Purpose: calculate the mean obliquity of the ecliptic			*/
		//* Arguments:										*/
		//*   t : number of Julian centuries since J2000.0				*/
		//* Return value:										*/
		//*   mean obliquity in degrees							*/
		//***********************************************************************/
		
			public double calcMeanObliquityOfEcliptic(double t)
			{
				double seconds = 21.448 - t*(46.8150 + t*(0.00059 - t*(0.001813)));
				double e0 = 23.0 + (26.0 + (seconds/60.0))/60.0;
				return e0;		// in degrees
			}
		
		//***********************************************************************/
		//* Name:    calcObliquityCorrection				            		*/
		//* Type:    Function								                	*/
		//* Purpose: calculate the corrected obliquity of the ecliptic	     	*/
		//* Arguments:										                    */
		//*   t : number of Julian centuries since J2000.0		        		*/
		//* Return value:								                		*/
		//*   corrected obliquity in degrees			            			*/
		//***********************************************************************/
		
			public double calcObliquityCorrection(double t)
			{
				double e0 = this.calcMeanObliquityOfEcliptic(t);
		
				double omega = 125.04 - 1934.136 * t;
				double e = e0 + 0.00256 * Math.Cos(this.degToRad(omega));
				return e;		// in degrees
			}
		
		//***********************************************************************/
		//* Name:    calcSunRtAscension							                */
		//* Type:    Function									  				*/
		//* Purpose: calculate the right ascension of the sun	     			*/
		//* Arguments:									                    	*/
		//*   t : number of Julian centuries since J2000.0	        			*/
		//* Return value:									                	*/
		//*   sun's right ascension in degrees									*/
		//***********************************************************************/
		
			public double calcSunRtAscension(double t)
			{
				var e = this.calcObliquityCorrection(t);
				var lambda = this.calcSunApparentLong(t);
		 
				var tananum = (Math.Cos(this.degToRad(e)) * Math.Sin(degToRad(lambda)));
				var tanadenom = (Math.Cos(degToRad(lambda)));
				var alpha = radToDeg(Math.Atan2(tananum, tanadenom));
				return alpha;		// in degrees
			}
		
		//***********************************************************************/
		//* Name:    calcSunDeclination							*/
		//* Type:    Function									*/
		//* Purpose: calculate the declination of the sun				*/
		//* Arguments:										*/
		//*   t : number of Julian centuries since J2000.0				*/
		//* Return value:										*/
		//*   sun's declination in degrees							*/
		//***********************************************************************/
		
			public double calcSunDeclination (double t)
			{
				double e = this.calcObliquityCorrection (t);
				double lambda = this.calcSunApparentLong (t);
		
				double sint = Math.Sin (this.degToRad (e)) * Math.Sin (this.degToRad (lambda));
				double theta = radToDeg (Math.Asin (sint));
				return theta;
				// in degrees
			}
		
		//***********************************************************************/
		//* Name:    calcEquationOfTime							*/
		//* Type:    Function									*/
		//* Purpose: calculate the difference between true solar time and mean	*/
		//*		solar time									*/
		//* Arguments:										*/
		//*   t : number of Julian centuries since J2000.0				*/
		//* Return value:										*/
		//*   equation of time in minutes of time						*/
		//***********************************************************************/
		
			public double calcEquationOfTime(double t)
			{
				double epsilon = this.calcObliquityCorrection(t);
				double l0 = this.calcGeomMeanLongSun(t);
				double e = this.calcEccentricityEarthOrbit(t);
				double m = this.calcGeomMeanAnomalySun(t);
		
				var y = Math.Tan(this.degToRad(epsilon)/2.0);
				y *= y;
		
				double sin2l0 = Math.Sin(2.0 * this.degToRad(l0));
				double sinm   = Math.Sin(this.degToRad(m));
				double cos2l0 = Math.Cos(2.0 * this.degToRad(l0));
				double sin4l0 = Math.Sin(4.0 * this.degToRad(l0));
				double sin2m  = Math.Sin(2.0 * this.degToRad(m));
		
				double Etime = y * sin2l0 - 2.0 * e * sinm + 4.0 * e * y * sinm * cos2l0
						- 0.5 * y * y * sin4l0 - 1.25 * e * e * sin2m;
		
				return radToDeg(Etime)*4.0;	// in minutes of time
			}
	}
}


using System;
using System.Net;
using System.Web;
using System.Xml;

using Orbit.BusinessLayer.Entities;

namespace Orbit
{
	public class Weather
	{	
		/// <summary>
		/// Gets the weather conditions to a lat/lon value
		/// </summary>
		/// <param name="lat">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <param name="lon">
		/// A <see cref="System.Double"/>
		/// </param>
		/// <returns>
		/// A <see cref="WeatherCondition"/>
		/// </returns>
		public WeatherCondition CurrentConditions (double lat, double lon)
		{
			String sKoord = ",,," + Convert.ToInt32(lat * 1000000).ToString() + "," +  Convert.ToInt32(lon * 1000000).ToString();

			return this.CurrentConditions(sKoord);
		}
		
		/// <summary>
		/// Gets the weather condtions to a location string
		/// </summary>
		/// <param name="place">
		/// A <see cref="String"/>
		/// </param>
		/// <returns>
		/// A <see cref="WeatherCondition"/>
		/// </returns>
		public WeatherCondition CurrentConditions (String location)
		{
			HttpWebRequest GoogleRequest;
			HttpWebResponse GoogleResponse = null;
			XmlDocument GoogleXMLdoc = null;
			
			WeatherCondition cond = new WeatherCondition ();
			
	        try 
	        {
				GoogleRequest = (HttpWebRequest)WebRequest.Create ("http://www.google.com/ig/api?weather=" + string.Format (location));
				GoogleResponse = (HttpWebResponse)GoogleRequest.GetResponse ();
				GoogleXMLdoc = new XmlDocument ();
				GoogleXMLdoc.Load (GoogleResponse.GetResponseStream ());
			
	            XmlNode root = GoogleXMLdoc.DocumentElement;
			
	            XmlNodeList nodeList1 = root.SelectNodes ("weather/forecast_information");
				
				
				cond.City = nodeList1.Item (0).SelectSingleNode ("city").Attributes["data"].InnerText;
			
				// current conditions
				XmlNodeList nodeList = root.SelectNodes ("weather/current_conditions");
				
				cond.Temperature = Convert.ToDouble (nodeList.Item (0).SelectSingleNode ("temp_c").Attributes["data"].InnerText);
				cond.Condition = nodeList.Item (0).SelectSingleNode ("condition").Attributes["data"].InnerText;
				cond.WindCondition = nodeList.Item (0).SelectSingleNode ("wind_condition").Attributes["data"].InnerText;
				cond.Humidity = nodeList.Item (0).SelectSingleNode ("humidity").Attributes["data"].InnerText;
	        } 
	        catch (Exception) 
	        {
				cond = null;
	        } 
	        finally 
	        {
				GoogleResponse.Close ();
			} 
			
			return cond;
		}
	}
}


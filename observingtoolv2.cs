using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            //user input
            double latitude = 38.0406;
            double longitude = 84.5037;
            int N = 322;
            int UT_offset = -4;
            double RA_hours = 5;
            double RA_minutes = 0;
            double RA_seconds = 0;
            double DEC_degrees = 5;
            double DEC_minutes = 0;
            double DEC_seconds = 0;

            //query at a certain time
            double query_Hours_Local = 8;
            double query_local_minutes = 36;

            //Calculate RA, DEC, and cosh
            double RA = (CalculateRA(RA_hours, RA_minutes, RA_seconds));
            double DEC = (CalculateDEC(DEC_degrees, DEC_minutes, DEC_seconds));
            double cosh = (-Math.Tan((Math.PI / 180) * latitude) * Math.Tan((Math.PI / 180) * DEC));

            if (cosh > 1) // Object never rises
            {
                Console.WriteLine("This object at your latitude will never be in the sky");
                Console.ReadLine();
                double Altitude_TransitAlt = (180 / Math.PI) * (Math.Asin((Math.Sin((Math.PI / 180) * latitude) * Math.Sin((Math.PI / 180) * DEC)) + (Math.Cos((Math.PI / 180) * latitude) * Math.Cos((Math.PI / 180) * DEC))));
                double Z_TransitAlt = 90 - Altitude_TransitAlt;
                double Air_Mass_TransitAlt = (1 / Math.Cos((Math.PI / 180) * Z_TransitAlt));
                string Full_Local_Transit_Time = CalculateFullLocalTransitTime(RA, longitude, N, UT_offset);
                Console.WriteLine("The object at your latitude is always in the sky");
                Console.WriteLine("Local Transit time is " + Full_Local_Transit_Time);
                Console.WriteLine("Altitude at Transit is " + Convert.ToString(Altitude_TransitAlt) + " Degrees");
                Console.WriteLine("Air Mass at Transit is " + Convert.ToString(Air_Mass_TransitAlt));
                Console.ReadLine();
            }
            else if (cosh < -1) // Object never sets
            {
                double Altitude_TransitAlt = (180 / Math.PI) * (Math.Asin((Math.Sin((Math.PI / 180) * latitude) * Math.Sin((Math.PI / 180) * DEC)) + (Math.Cos((Math.PI / 180) * latitude) * Math.Cos((Math.PI / 180) * DEC))));
                double Z_TransitAlt = 90 - Altitude_TransitAlt;
                double Air_Mass_TransitAlt = (1 / Math.Cos((Math.PI / 180) * Z_TransitAlt));
                string Full_Local_Transit_Time = CalculateFullLocalTransitTime(RA, longitude, N, UT_offset);
                Console.WriteLine("The object at your latitude is always in the sky");
                Console.WriteLine("Local Transit time is " + Full_Local_Transit_Time);
                Console.WriteLine("Altitude at Transit is " + Convert.ToString(Altitude_TransitAlt) + " Degrees");
                Console.WriteLine("Air Mass at Transit is " + Convert.ToString(Air_Mass_TransitAlt));
                Console.ReadLine();
            }
            else // Object rises and sets
            {
                double Altitude_TransitAlt = (180 / Math.PI) * (Math.Asin((Math.Sin((Math.PI / 180) * latitude) * Math.Sin((Math.PI / 180) * DEC)) + (Math.Cos((Math.PI / 180) * latitude) * Math.Cos((Math.PI / 180) * DEC))));
                double Z_TransitAlt = 90 - Altitude_TransitAlt;
                double Air_Mass_TransitAlt = (1 / Math.Cos((Math.PI / 180) * Z_TransitAlt));
                double h = (180 / Math.PI) * Math.Acos(cosh);
                double Rise = RA - h;
                double Set = RA + h;
                string Full_Local_Transit_Time = CalculateFullLocalTransitTime(RA, longitude, N, UT_offset);
                string Full_Local_Rise_Time = ConvertRiseOrSetToLocalTime(Rise, cosh, RA, longitude, N, UT_offset);
                string Full_Local_Set_Time = ConvertRiseOrSetToLocalTime(Set, cosh, RA, longitude, N, UT_offset);
                Console.WriteLine("Altitude at Transit is " + Convert.ToString(Altitude_TransitAlt) + " Degrees");
                Console.WriteLine("Air Mass at Transit is " + Convert.ToString(Air_Mass_TransitAlt));
                Console.WriteLine("Local Rise time is " + Full_Local_Rise_Time);
                Console.WriteLine("Local Transit time is " + Full_Local_Transit_Time);
                Console.WriteLine("Local Set time is " + Full_Local_Set_Time);
                Console.ReadLine();
            }


        }


        static double CalculateRA(double RA_hours, double RA_minutes, double RA_seconds)
        {
            double RA = ((15 * RA_hours) + (15 * RA_minutes / 60) + (15 * RA_seconds / 3600));

            return RA;
        }

        static double CalculateDEC(double DEC_degrees, double DEC_minutes, double DEC_seconds)
        {
            double DEC;

            if (DEC_degrees < 0)
            {
                DEC = (DEC_degrees - (DEC_minutes / 60) - (DEC_seconds / 3600));
            }
            else
            {
                DEC = (DEC_degrees + (DEC_minutes / 60) + (DEC_seconds / 3600));
            }

            return DEC;
        }

        static string CalculateFullLocalTransitTime(double RA, double longitude, int N, int UT_offset)
        {
            double TransitAlt_Time_Hours_LST = RA / 15;
            double TransitAlt_Time_Hours_GMST = TransitAlt_Time_Hours_LST + (longitude / 15);
            TransitAlt_Time_Hours_GMST = TwentyFourHourConversion(TransitAlt_Time_Hours_GMST);

            double TransitAlt_Time_Hours_UT = (TransitAlt_Time_Hours_GMST - 6.656306 - (0.0657098242 * (N))) / 1.0027379093;
            TransitAlt_Time_Hours_UT = TwentyFourHourConversion(TransitAlt_Time_Hours_UT);

            double TransitAlt_Time_Hours_Local = TransitAlt_Time_Hours_UT + UT_offset;
            TransitAlt_Time_Hours_Local = TwentyFourHourConversion(TransitAlt_Time_Hours_Local);

            double TransitAlt_Time_Hours_Local_Floored = Math.Floor(TransitAlt_Time_Hours_Local);
            double TransitAlt_Time_Minutes_Local = (TransitAlt_Time_Hours_Local - TransitAlt_Time_Hours_Local_Floored) * 60;
            double TransitAlt_Time_Minutes_Local_Floored = Math.Floor(TransitAlt_Time_Minutes_Local);
            string TransitAlt_Time_Minutes_Local_Floored_String;

            if (TransitAlt_Time_Minutes_Local_Floored < 10)
            {
                TransitAlt_Time_Minutes_Local_Floored_String = "0" + Convert.ToString(TransitAlt_Time_Minutes_Local_Floored);
            }

            else
            {
                TransitAlt_Time_Minutes_Local_Floored_String = Convert.ToString(TransitAlt_Time_Minutes_Local_Floored);
            }

            string Full_Local_Transit_Time;

            if (TransitAlt_Time_Hours_Local_Floored == 0)
            {
                Full_Local_Transit_Time = "12:" + TransitAlt_Time_Minutes_Local_Floored_String + " AM";
            }

            else if (TransitAlt_Time_Hours_Local_Floored >= 1 && TransitAlt_Time_Hours_Local_Floored < 12)
            {
                Full_Local_Transit_Time = Convert.ToString(TransitAlt_Time_Hours_Local_Floored) + ":" + TransitAlt_Time_Minutes_Local_Floored_String + " AM";
            }

            else if (TransitAlt_Time_Hours_Local_Floored == 12)
            {
                Full_Local_Transit_Time = "12:" + TransitAlt_Time_Minutes_Local_Floored_String + " PM";
            }

            else
            {
                TransitAlt_Time_Hours_Local_Floored -= 12;
                Full_Local_Transit_Time = Convert.ToString(TransitAlt_Time_Hours_Local_Floored) + ":" + TransitAlt_Time_Minutes_Local_Floored_String + " PM";
            }

            return Full_Local_Transit_Time;
        }

        static string ConvertRiseOrSetToLocalTime(double Time_Degrees, double cosh, double RA, double longitude, int N, int UT_offset)
        {

            if (Time_Degrees < 0)
            {
                Time_Degrees += 360;
            }
            if (Time_Degrees >= 360)
            {
                Time_Degrees -= 360;
            }

            double Time_LST_Hours = Time_Degrees / 15;
            double Time_LST_Minutes = (Time_LST_Hours - Math.Floor(Time_LST_Hours)) * 60;

            double Time_Hours_GMST = Time_LST_Hours + (longitude / 15);
            Time_Hours_GMST = TwentyFourHourConversion(Time_Hours_GMST);

            double Time_Hours_UT = (Time_Hours_GMST - 6.656306 - (0.0657098242 * (N))) / 1.0027379093;
            Time_Hours_UT = TwentyFourHourConversion(Time_Hours_UT);

            double Time_Hours_Local = Time_Hours_UT + UT_offset;
            Time_Hours_Local = TwentyFourHourConversion(Time_Hours_Local);

            double Time_Hours_Local_Floored = Math.Floor(Time_Hours_Local);
            double Time_Minutes_Local = (Time_Hours_Local - Time_Hours_Local_Floored) * 60;
            double Time_Minutes_Local_Floored = Math.Floor(Time_Minutes_Local);
            string Time_Minutes_Local_Floored_String;

            if (Time_Minutes_Local_Floored < 10)
            {
                Time_Minutes_Local_Floored_String = "0" + Convert.ToString(Time_Minutes_Local_Floored);
            }

            else
            {
                Time_Minutes_Local_Floored_String = Convert.ToString(Time_Minutes_Local_Floored);
            }

            string Full_Local_Time;

            if (Time_Hours_Local_Floored == 0)
            {
                Full_Local_Time = "12:" + Time_Minutes_Local_Floored_String + " AM";
            }

            else if (Time_Hours_Local_Floored >= 1 && Time_Hours_Local_Floored < 12)
            {
                Full_Local_Time = Convert.ToString(Time_Hours_Local_Floored) + ":" + Time_Minutes_Local_Floored_String + " AM";
            }

            else if (Time_Hours_Local_Floored == 12)
            {
                Full_Local_Time = "12:" + Time_Minutes_Local_Floored_String + " PM";
            }

            else
            {
                Time_Hours_Local_Floored -= 12;
                Full_Local_Time = Convert.ToString(Time_Hours_Local_Floored) + ":" + Time_Minutes_Local_Floored_String + " PM";
            }

            return Full_Local_Time;

        }

        static double TwentyFourHourConversion(double Hours)
        {
            if (Hours < 0)
            {
                Hours += 24;
            }
            if (Hours >= 24)
            {
                Hours -= 24;
            }

            return Hours;
        }
    }
}

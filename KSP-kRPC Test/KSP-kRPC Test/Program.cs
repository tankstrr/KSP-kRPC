using System;
using KRPC.Client;
using KRPC.Client.Services.SpaceCenter;

class Program
{
    public static void Main()
    {
        var conn = new Connection("TSA Mission Control");
        var spaceCenter = conn.SpaceCenter();
        var vessel = spaceCenter.ActiveVessel;

        var ut = conn.AddStream(() => conn.SpaceCenter().UT);
        var met = conn.AddStream(() => conn.SpaceCenter().ActiveVessel.MET);
        var flight = vessel.Flight();
        var altitude = conn.AddStream(() => flight.MeanAltitude);
        var apoapsis = conn.AddStream(() => vessel.Orbit.ApoapsisAltitude);
        var periapsis = conn.AddStream(() => vessel.Orbit.PeriapsisAltitude);
        var inc = conn.AddStream(() => vessel.Orbit.Inclination);
        var ecc = conn.AddStream(() => vessel.Orbit.Eccentricity);
        var name = conn.AddStream(() => vessel.Name);
        var elec = conn.AddStream(() => vessel.Resources.Amount("ElectricCharge"));
        var ox = conn.AddStream(() => vessel.Resources.Amount("Oxidizer"));
        var lf = conn.AddStream(() => vessel.Resources.Amount("LiquidFuel"));
        var throt = conn.AddStream(() => vessel.Control.Throttle);

        Console.CursorVisible = false;
        Console.ForegroundColor = ConsoleColor.Green;
        float elecOld = 0;
        while (true)
        {
            TimeSpan timeM = TimeSpan.FromSeconds(met.Get());
            //TimeSpan kerbalYear = new TimeSpan(426, 0, 0, 0);
            //TimeSpan timeU = TimeSpan.FromSeconds(ut.Get()) + kerbalYear;

            //here backslash is must to tell that colon is
            //not the part of format, it just a character that we want in output
            string metF = timeM.ToString(@"ddd\:hh\:mm\:ss\.fff");
            //string utF = timeU.ToString(@"ddd\:hh\:mm\:ss\.fff");
            float elecNew = elec.Get();
            float elecRate = (elecOld - elecNew) * 10;

            Console.SetCursorPosition(0, 0);
            Console.WriteLine("MET: {0}      VES: {1}", metF, name.Get());
            //Console.WriteLine(" UT: {0}", utF);
            //Console.WriteLine(" UT: {0}", TimeZoneInfo.ConvertTimeFromUtc(ut.Get()));
            Console.WriteLine("");
            Console.WriteLine("APO: {0,15:n0} m", apoapsis.Get());
            Console.WriteLine("PER: {0,15:n0} m", periapsis.Get());
            Console.WriteLine("ALT: {0,15:n0} m", altitude.Get());
            Console.WriteLine("ECC: {0,15:n5}", ecc.Get());
            Console.WriteLine("INC: {0,15:n5}", inc.Get() * (180 / Math.PI));
            Console.WriteLine("");
            Console.WriteLine("THRO: {0,15:n2} %", throt.Get() * 100 );
            Console.WriteLine("");
            Console.WriteLine("ELEC: {0,15:n3} {1,10:n3}", elec.Get(), elecRate);
            Console.WriteLine("OXID: {0,15:n3}", ox.Get());
            Console.WriteLine("LIFU: {0,15:n3}", lf.Get());

            elecOld = elecNew;

            System.Threading.Thread.Sleep(100);
        }
    }
}
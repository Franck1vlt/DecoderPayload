using Payload.Model.Platform.Lorawan.Device.MileSight;
using System;

namespace DecoderPayload
{
    class Program
    {
        static void Main(string[] args)
        {
            // Exemple de payload en bytes
            byte[] payload1 = { 0x01, 0x75, 0x5C, 0x03, 0x67, 0x01, 0x01, 0x04, 0x82, 0x44, 0x08, 0x05, 0x00, 0x01 };
            byte[] payload2 = { 0x83, 0x67, 0xE8, 0x00, 0x01, 0x84, 0x82, 0x41, 0x06, 0x01, 0x01 };
            byte[] payload3 = { 0x06, 0x88, 0x36, 0xBF, 0x77, 0x01, 0xF0, 0x00, 0x09, 0x07, 0x22 };

            // Décode les payloads
            var decodedData1 = EM400PayloadDecoder.Milesight(payload1);
            var decodedData2 = EM400PayloadDecoder.Milesight(payload2);
            var decodedData3 = EM400PayloadDecoder.Milesight(payload3);

            // Affiche les résultats
            Console.WriteLine("Payload 1:");
            PrintDecodedResult(decodedData1);
            Console.WriteLine();

            Console.WriteLine("Payload 2:");
            PrintDecodedResult(decodedData2);
            Console.WriteLine();

            Console.WriteLine("Payload 3:");
            PrintDecodedResult(decodedData3);
        }

        static void PrintDecodedResult(EM400PayloadDecoder.DecodedData decoded)
        {
            Console.WriteLine($"Battery: {decoded.Battery}%");
            Console.WriteLine($"Temperature: {decoded.Temperature}°C");
            Console.WriteLine($"Temperature with Abnormal: {decoded.TemperatureAbnormal}");
            Console.WriteLine($"Distance: {decoded.Distance}mm");
            Console.WriteLine($"Distance Alarming: {decoded.DistanceAlarming}");
            Console.WriteLine($"Position: {decoded.Position}");
            Console.WriteLine($"Longitude: {decoded.Longitude}");
            Console.WriteLine($"Latitude: {decoded.Latitude}");
            Console.WriteLine($"Motion Status: {decoded.MotionStatus}");
            Console.WriteLine($"Geofence Status: {decoded.GeofenceStatus}");
        }
    }
}

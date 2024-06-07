using System;

namespace Payload.Model.Platform.Lorawan.Device.MileSight
{
    public class EM400PayloadDecoder
    {
        #region Fields
        public class DecodedData
        {
            #region Methods

            public int? Battery { get; set; }
            public double? Temperature { get; set; }
            public bool? TemperatureAbnormal { get; set; }
            public int? Distance { get; set; }
            public bool? DistanceAlarming { get; set; }
            public double? Longitude { get; set; }
            public double? Latitude { get; set; }
            public string MotionStatus { get; set; }
            public string GeofenceStatus { get; set; }
            public string Position { get; set; }

            #endregion
        }

        #endregion

        #region Methods

        public void DecodeUplink(byte[] input)
        {
            var dd = Milesight(input);
            /*return new GenericModel { Battery = dd.Battery, Distance = dd.Distance };*/
        }

        public static DecodedData Milesight(byte[] bytes)
        {
            var decoded = new DecodedData();

            for (var i = 0; i < bytes.Length - 1;)
            {
                var channelId = bytes[i++];
                var channelType = bytes[i++];

                if (i >= bytes.Length)
                {
                    // Vérifier si nous avons dépassé la longueur du tableau
                    break;
                }

                if (channelId == 0x01 && channelType == 0x75) // BATTERY
                {
                    decoded.Battery = bytes[i];
                    i += 1;
                }
                else if (channelId == 0x03 && channelType == 0x67) // TEMPERATURE
                {
                    if (i + 1 < bytes.Length)
                    {
                        decoded.Temperature = ReadUInt16LE(bytes, i) / 10.0;
                        i += 2;
                    }
                }
                // DISTANCE
                else if (channelId == 0x04 && channelType == 0x82)
                {
                    decoded.Distance = ReadUInt16LE(bytes, i);
                    i += 2;
                }
                // POSITION
                else if (channelId == 0x05 && channelType == 0x00)
                {
                    decoded.Position = bytes[i] == 0 ? "normal" : "tilt";
                    i += 1;
                }
                // LOCATION
                else if (channelId == 0x06 && channelType == 0x88)
                {
                    // Convertir la longitude
                    int longitudeRaw = (bytes[i + 3] << 24) | (bytes[i + 2] << 16) | (bytes[i + 1] << 8) | bytes[i];
                    decoded.Longitude = longitudeRaw / 1e6;

                    // Convertir la latitude
                    int latitudeRaw = (bytes[i + 7] << 24) | (bytes[i + 6] << 16) | (bytes[i + 5] << 8) | bytes[i + 4];
                    decoded.Latitude = latitudeRaw / 1e6;

                    i += 8;

                    var status = bytes[i++];
                    string[] motionStatuses = { "unknown", "start", "moving", "stop" };
                    decoded.MotionStatus = motionStatuses[status & 0x03];
                    string[] geofenceStatuses = { "inside", "outside", "unset", "unknown" };
                    decoded.GeofenceStatus = geofenceStatuses[status >> 4];
                }
                // TEMPERATURE WITH ABNORMAL
                else if (channelId == 0x83 && channelType == 0x67)
                {
                    decoded.Temperature = ReadUInt16LE(bytes, i) / 10.0;
                    i += 2;
                    decoded.TemperatureAbnormal = bytes[i++] != 0;
                }
                // DISTANCE WITH ALARMING
                else if (channelId == 0x84 && channelType == 0x82)
                {
                    decoded.Distance = ReadUInt16LE(bytes, i);
                    i += 2;
                    decoded.DistanceAlarming = bytes[i++] != 0;
                }

                else
                {
                    // Si un canal n'est pas pris en charge, sortir de la boucle
                    break;
                }
            }

            return decoded;
        }


        private static int ReadUInt16LE(byte[] bytes, int startIndex)
        {
            return (bytes[startIndex + 1] << 8) + bytes[startIndex];
        }

        private static int ReadInt16LE(byte[] bytes, int startIndex)
        {
            return (short)((bytes[startIndex + 1] << 8) + bytes[startIndex]);
        }
    }

    #endregion
}

using System;

namespace Payload.Model.Platform.Lorawan.Device.MileSight
{
    public class EM500UDLPayloadDecoder
    {
        public class DecodedData
        {
            public int? Battery { get; set; }
            public int? Distance { get; set; }
        }

        public void DecodeUplink(byte[] input)
        {
            var dd = Milesight(input);
            /*return new GenericModel { Battery = dd.Battery, Distance = dd.Distance };*/
        }

        public static DecodedData Milesight(byte[] bytes)
        {
            var decoded = new DecodedData();

            for (var i = 0; i < bytes.Length;)
            {
                var channelId = bytes[i++];
                var channelType = bytes[i++];

                if (channelId == 0x01 && channelType == 0x75) // BATTERY
                {
                    decoded.Battery = bytes[i];
                    i += 1;
                }
                else if (channelId == 0x03 && channelType == 0x82) // DISTANCE
                {
                    decoded.Distance = ReadUInt16LE(bytes, i);
                    i += 2;
                }
                else
                {
                    break;
                }
            }

            return decoded;
        }

        private static int ReadUInt16LE(byte[] bytes, int startIndex)
        {
            return (bytes[startIndex + 1] << 8) + bytes[startIndex];
        }
    }
}

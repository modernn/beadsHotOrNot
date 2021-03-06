﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeadedStream_HON
{
    public class SensorSpacing
    {
        public string Units { get; set; }
        public string text { get; set; }
    }

    public class OwdDS28EC20
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string Family { get; set; }
        public string ROMId { get; set; }
        public string Health { get; set; }
        public string Channel { get; set; }
        public string Key1 { get; set; }
        public string Value1 { get; set; }
        public string Key2 { get; set; }
        public string Value2 { get; set; }
        public string Key3 { get; set; }
        public string Value3 { get; set; }
        public string Key4 { get; set; }
        public string Value4 { get; set; }
        public string Key5 { get; set; }
        public string Value5 { get; set; }
        public string Key6 { get; set; }
        public string Value6 { get; set; }
        public string Key7 { get; set; }
        public string Value7 { get; set; }
        public string Key8 { get; set; }
        public string Value8 { get; set; }
        public string Key9 { get; set; }
        public string Value9 { get; set; }
        public string Key10 { get; set; }
        public string Value10 { get; set; }
        public string Key11 { get; set; }
        public string Value11 { get; set; }
        public string Key12 { get; set; }
        public string Value12 { get; set; }
        public string Key13 { get; set; }
        public string Value13 { get; set; }
        public string Key14 { get; set; }
        public string Value14 { get; set; }
        public string Key15 { get; set; }
        public string Value15 { get; set; }
        public string Key16 { get; set; }
        public string Value16 { get; set; }
        public string Key17 { get; set; }
        public string Value17 { get; set; }
        public string Key18 { get; set; }
        public string Value18 { get; set; }
        public string Key19 { get; set; }
        public string Value19 { get; set; }
        public string Key20 { get; set; }
        public string Value20 { get; set; }
    }

public class OwdDS18B20
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public string SensorID { get; set; }
        public string Health { get; set; }
        public string TemperatureRaw { get; set; }
        public string TemperatureCalibrated { get; set; }
        public string CalByte1 { get; set; }
        public string CalByte2 { get; set; }
        public string PowerSource { get; set; }
        public SensorSpacing SensorSpacing { get; set; }
    }

    public class DevicesDetailResponse
    {
        [JsonConverter(typeof(MyConverter))]
        public string PollCount { get; set; }
        [JsonConverter(typeof(MyConverter))]
        public string DevicesConnected { get; set; }
        [JsonConverter(typeof(MyConverter))]
        public string LoopTime { get; set; }
        [JsonConverter(typeof(MyConverter))]
        public string DevicesConnectedChannel1 { get; set; }
        [JsonConverter(typeof(MyConverter))]
        public string DataErrorsChannel1 { get; set; }
        [JsonConverter(typeof(MyConverter))]
        public string VoltageChannel1 { get; set; }
        [JsonConverter(typeof(MyConverter))]
        public string VoltagePower { get; set; }
        [JsonConverter(typeof(MyConverter))]
        public string DeviceName { get; set; }
        [JsonConverter(typeof(MyConverter))]
        public string HostName { get; set; }
        [JsonConverter(typeof(MyConverter))]
        public string MACAddress { get; set; }
        [JsonConverter(typeof(MyConverter))]
        public string DateTime { get; set; }
        public OwdDS28EC20 owd_DS28EC20 { get; set; }
        public List<OwdDS18B20> owd_DS18B20 { get; set; }
    }
    public class RootObject
    {
        public DevicesDetailResponse DevicesDetailResponse { get; set; }
    }

    public class MyConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                JArray array = JArray.Load(reader);
                
                return "0";
                //return serializer.Deserialize(reader, objectType);
            }

            return reader.Value.ToString();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

}

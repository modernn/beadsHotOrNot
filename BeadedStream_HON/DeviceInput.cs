using Newtonsoft.Json;
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
   public string PollCount { get; set; }
    public string DevicesConnected { get; set; }
    public string LoopTime { get; set; }
    public string DevicesConnectedChannel1 { get; set; }
    public string DataErrorsChannel1 { get; set; }
    public string VoltageChannel1 { get; set; }
    public string VoltagePower { get; set; }
    public string DeviceName { get; set; }
    public string HostName { get; set; }
    public string MACAddress { get; set; }
    public string DateTime { get; set; }
    public List<OwdDS18B20> owd_DS18B20 { get; set; }
}
public class RootObject
{
    public DevicesDetailResponse DevicesDetailResponse { get; set; }
}

}

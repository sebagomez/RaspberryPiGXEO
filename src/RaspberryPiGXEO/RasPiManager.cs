using System;
using System.Collections.Generic;
using Windows.Devices.Enumeration;
using Windows.Devices.Gpio;
using Windows.Devices.Spi;

namespace RaspberryPiGXEO
{
	public class RasPiManager
	{
		static GpioController s_gpio;
		static Dictionary<int, GpioPin> s_coll;

		/*RaspBerry Pi2  Parameters*/
		static readonly string SPI_CONTROLLER_NAME = "SPI0";  /* For Raspberry Pi 2, use SPI0                             */
		static readonly int SPI_CHIP_SELECT_LINE = 0;       /* Line 0 maps to physical pin number 24 on the Rpi2        */

		static byte[] readBuffer = new byte[3]; /*this is defined to hold the output data*/
		static byte[] writeBuffer = new byte[3] { 0x01, 0x80, 0x00 }; //00000001 10000000 00000000

		static private SpiDevice SpiDisplay;

		static RasPiManager()
		{
			s_gpio = GpioController.GetDefault();
			s_coll = new Dictionary<int, GpioPin>();

			InitSPI();
		}

		private static async void InitSPI()
		{
			var settings = new SpiConnectionSettings(SPI_CHIP_SELECT_LINE);
			settings.ClockFrequency = 500000;// 10000000;
			settings.Mode = SpiMode.Mode0; //Mode3;

			string spiAqs = SpiDevice.GetDeviceSelector(SPI_CONTROLLER_NAME);
			var deviceInfo = await DeviceInformation.FindAllAsync(spiAqs);
			SpiDisplay = await SpiDevice.FromIdAsync(deviceInfo[0].Id, settings);
		}

		private static int ConvertToInt(byte[] data)
		{
			int result = data[1] & 0x03; //note we are using the second byte here instead of the first
			result <<= 8;
			result += data[2]; //note we are using the third byte here instead of the second
			return result;
		}

		public static void TurnOn(int pinNumber)
		{
			SwitchPin(pinNumber, GpioPinValue.High);
		}

		public static void TurnOff(int pinNumber)
		{
			SwitchPin(pinNumber, GpioPinValue.Low);
		}

		public static int ReadSPI0()
		{
			SpiDisplay.TransferFullDuplex(writeBuffer, readBuffer);
			return ConvertToInt(readBuffer);
		}

		private static void SwitchPin(int pinNumber, GpioPinValue value)
		{
			GetPin(pinNumber).Write(value);
		}

		static GpioPin GetPin(int pinNumber)
		{
			if (!s_coll.ContainsKey(pinNumber))
			{
				GpioPin pin = s_gpio.OpenPin(pinNumber);
				pin.Write(GpioPinValue.Low);
				pin.SetDriveMode(GpioPinDriveMode.Output);

				s_coll[pinNumber] = pin;
			}
			return s_coll[pinNumber];

		}
	}
}

using RaspberryPiGXEO;

namespace RPGXEOWinRTWrapper
{
	public sealed class RasPiManagerWrapper
    {
		public static void TurnOn(int pinNumber)
		{
			RasPiManager.TurnOn(pinNumber);
		}

		public static void TurnOff(int pinNumber)
		{
			RasPiManager.TurnOff(pinNumber);
		}

		public static int ReadSPIO()
		{
			return RasPiManager.ReadSPI0();
		}
	}
}

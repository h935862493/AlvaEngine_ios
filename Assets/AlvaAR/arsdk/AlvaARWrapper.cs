using System;
using System.Runtime.InteropServices;

namespace arsdk
{
    internal static class AlvaARWrapper
	{
		private static IAlvaARWrapper sWrapper;

		private static IAlvaARWrapper sCamIndependentWrapper;

        public static IAlvaARWrapper Instance
		{
			get
			{
				if (AlvaARWrapper.sWrapper == null)
				{
                    AlvaARWrapper.CreateRuntimeInstance();
				}
				return AlvaARWrapper.sWrapper;
			}
		}

		public static IAlvaARWrapper CamIndependentInstance
		{
			get
			{
				if (AlvaARWrapper.sCamIndependentWrapper == null)
				{
					AlvaARWrapper.sCamIndependentWrapper = AlvaARWrapper.Instance;

				}
				return AlvaARWrapper.sCamIndependentWrapper;
			}
		}

		public static void CreateRuntimeInstance()
		{
            AlvaARWrapper.sWrapper = new AlvaARNativeWrapper();
        }

		private static void CreateCamIndependentInstance()
		{
            AlvaARWrapper.sCamIndependentWrapper = new AlvaARNativeWrapper();
        }

		public static void SetImplementation(IAlvaARWrapper implementation)
		{
			AlvaARWrapper.sWrapper = implementation;
		}
	}
}

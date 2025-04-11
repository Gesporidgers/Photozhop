using System;
using System.Collections.Generic;

namespace Photozhop
{
	public class PPO
	{
		public delegate byte OperationDelegate(byte a, byte b, int opacity = 1);
		public string Name { get; set; }
		public OperationDelegate ByteOperation;

		public static List<PPO> getPPO()
		{
			return new List<PPO>(){
			new PPO()
			{
				Name = "Normal",
				ByteOperation = (a,b,o) => (byte)(a*o)
			},
			new PPO()
			{
				Name = "Sum",
				ByteOperation = (a,b,o) => (byte)(a+b)
			},
			new PPO()
			{
				Name = "Multiply",
				ByteOperation = (a,b,o) => (byte)(a*b/255.0)
			},
			new PPO()
			{
				Name = "Substract",
				ByteOperation = (a,b,o) => (byte)(a-b)
			},
			new PPO()
			{
				Name = "Divide",
				ByteOperation = (a,b,o) => (byte)(1.0*b/a)
			},
			new PPO()
			{
				Name = "Max",
				ByteOperation = (a,b,o) => Math.Max(a,b)
			},
			new PPO()
			{
				Name = "Min",
				ByteOperation = (a,b,o) => Math.Min(a,b)
			},
			new PPO()
			{
				Name = "Avg",
				ByteOperation = (a,b,o) => (byte)(a+b/2)
			}
		};
		}
	}
}

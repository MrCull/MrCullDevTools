﻿using System;
using Spruce.Schema;

namespace MrCullDevTools.Infrastructure.Data.Schema
{
	/// <summary>
	/// These are db objects that don't really fit anywhere else. They are core to the system as a whole
	/// </summary>
	public class GlobalSection : ISchemaSection
	{
		public Type[] Tables
		{
			get
			{
				return new Type[]{};
			}
		}

		public ScriptedObject[] ScriptedObjects
		{
			get
			{
				return new ScriptedObject[]
				{
				};
			}
		}
	}
}
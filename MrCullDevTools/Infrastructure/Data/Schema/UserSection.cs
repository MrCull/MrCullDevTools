using System;
using MrCullDevTools.Models.Users;
using Spruce.Schema;

namespace MrCullDevTools.Infrastructure.Data.Schema
{
	public class UserSection : ISchemaSection
	{
		public Type[] Tables
		{
			get
			{
				return new []
				{
					typeof(User),
					typeof(PasswordRetrieval),
				};
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
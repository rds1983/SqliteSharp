// Generated by Hebron at 4/3/2022 2:10:11 AM

using System;
using System.Runtime.InteropServices;
using Hebron.Runtime;

namespace SqliteSharp
{
	unsafe partial class Sqlite
	{
		public static Table tableOfTrigger(Trigger pTrigger)
		{
			return sqlite3HashFind(&pTrigger.pTabSchema.tblHash, pTrigger.table);
		}
	}
}
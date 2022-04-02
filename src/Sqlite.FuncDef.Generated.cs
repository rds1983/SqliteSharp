// Generated by Hebron at 4/3/2022 2:10:11 AM

using System;
using System.Runtime.InteropServices;
using Hebron.Runtime;

namespace SqliteSharp
{
	unsafe partial class Sqlite
	{
		public static int matchQuality(FuncDef p, int nArg, byte enc)
		{
			int match = 0;
			if (p.nArg != nArg)
			{
				if ((nArg) == (-2))
					return (int)(((p.xSFunc) == (null)) ? 0 : 6);
				if ((p.nArg) >= (0))
					return (int)(0);
			}

			if ((p.nArg) == (nArg))
			{
				match = (int)(4);
			}
			else
			{
				match = (int)(1);
			}

			if ((enc) == (p.funcFlags & 0x0003))
			{
				match += (int)(2);
			}
			else if ((enc & p.funcFlags & 2) != 0)
			{
				match += (int)(1);
			}

			return (int)(match);
		}
		public static void sqlite3InsertBuiltinFuncs(FuncDef aDef, int nDef)
		{
			int i = 0;
			for (i = (int)(0); (i) < (nDef); i++)
			{
				FuncDef pOther;
				sbyte* zName = aDef[i].zName;
				int nName = (int)(sqlite3Strlen30(zName));
				int h = (int)(((zName[0]) + (nName)) % 23);
				pOther = sqlite3FunctionSearch((int)(h), zName);
				if ((pOther) != null)
				{
					aDef[i].pNext = pOther.pNext;
					pOther.pNext = aDef[i];
				}
				else
				{
					aDef[i].pNext = null;
					aDef[i].u.pHash = sqlite3BuiltinFunctions.a[h];
					sqlite3BuiltinFunctions.a[h] = aDef[i];
				}
			}
		}
	}
}
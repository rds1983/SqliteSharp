// Generated by Hebron at 4/3/2022 2:10:11 AM

using System;
using System.Runtime.InteropServices;
using Hebron.Runtime;

namespace SqliteSharp
{
	unsafe partial class Sqlite
	{
		public static void corruptSchema(InitData pData, sbyte** azObj, sbyte* zExtra)
		{
			sqlite3 db = pData.db;
			if ((db.mallocFailed) != 0)
			{
				pData.rc = (int)(7);
			}
			else if (pData.pzErrMsg[0] != null)
			{
			}
			else if ((pData.mInitFlags & (0x0003)) != 0)
			{
				*pData.pzErrMsg = sqlite3MPrintf(db, "error in %s %s after %s: %s", azObj[0], azObj[1], corruptSchema_azAlterType[(pData.mInitFlags & 0x0003) - 1], zExtra);
				pData.rc = (int)(1);
			}
			else if ((db.flags & 0x00000001) != 0)
			{
				pData.rc = (int)(sqlite3CorruptError((int)(133217)));
			}
			else
			{
				sbyte* z;
				sbyte* zObj = (azObj[1]) != 0 ? azObj[1] : "?";
				z = sqlite3MPrintf(db, "malformed database schema (%s)", zObj);
				if (((zExtra) != null) && ((zExtra[0]) != 0))
					z = sqlite3MPrintf(db, "%z - %s", z, zExtra);
				*pData.pzErrMsg = z;
				pData.rc = (int)(sqlite3CorruptError((int)(133224)));
			}
		}
	}
}
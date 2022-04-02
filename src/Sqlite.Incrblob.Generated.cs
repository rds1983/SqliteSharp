// Generated by Hebron at 4/3/2022 2:10:11 AM

using System;
using System.Runtime.InteropServices;
using Hebron.Runtime;

namespace SqliteSharp
{
	unsafe partial class Sqlite
	{
		public static int blobSeekToRow(Incrblob p, long iRow, sbyte** pzErr)
		{
			int rc = 0;
			sbyte* zErr = null;
			Vdbe v = (Vdbe)(p.pStmt);
			v.aMem[1].flags = (ushort)(0x0004);
			v.aMem[1].u.i = (long)(iRow);
			if ((v.pc) > (4))
			{
				v.pc = (int)(4);
				rc = (int)(sqlite3VdbeExec(v));
			}
			else
			{
				rc = (int)(sqlite3_step(p.pStmt));
			}

			if ((rc) == (100))
			{
				VdbeCursor pC = v.apCsr[0];
				uint type = 0;
				type = (uint)((pC.nHdrParsed) > (p.iCol) ? pC.aType[p.iCol] : 0);
				if ((type) < (12))
				{
					zErr = sqlite3MPrintf(p.db, "cannot open value of type %s", (type) == (0) ? "null" : (type) == (7) ? "real" : "integer");
					rc = (int)(1);
					sqlite3_finalize(p.pStmt);
					p.pStmt = null;
				}
				else
				{
					p.iOffset = (int)(pC.aType[p.iCol + pC.nField]);
					p.nByte = (int)(sqlite3VdbeSerialTypeLen((uint)(type)));
					p.pCsr = pC.uc.pCursor;
					sqlite3BtreeIncrblobCursor(p.pCsr);
				}
			}

			if ((rc) == (100))
			{
				rc = (int)(0);
			}
			else if ((p.pStmt) != null)
			{
				rc = (int)(sqlite3_finalize(p.pStmt));
				p.pStmt = null;
				if ((rc) == (0))
				{
					zErr = sqlite3MPrintf(p.db, "no such rowid: %lld", (long)(iRow));
					rc = (int)(1);
				}
				else
				{
					zErr = sqlite3MPrintf(p.db, "%s", sqlite3_errmsg(p.db));
				}
			}

			*pzErr = zErr;
			return (int)(rc);
		}
	}
}
// Generated by Hebron at 4/3/2022 2:10:11 AM

using System;
using System.Runtime.InteropServices;
using Hebron.Runtime;

namespace SqliteSharp
{
	unsafe partial class Sqlite
	{
		public static KeyInfo sqlite3KeyInfoRef(KeyInfo p)
		{
			if ((p) != null)
			{
				p.nRef++;
			}

			return p;
		}
		public static void sqlite3KeyInfoUnref(KeyInfo p)
		{
			if ((p) != null)
			{
				p.nRef--;
				if ((p.nRef) == (0))
					sqlite3DbFreeNN(p.db, p);
			}
		}
		public static UnpackedRecord sqlite3VdbeAllocUnpackedRecord(KeyInfo pKeyInfo)
		{
			UnpackedRecord p;
			int nByte = 0;
			nByte = (int)((((sizeof(UnpackedRecord)) + 7) & ~7) + sizeof(sqlite3_value) * (pKeyInfo.nKeyField + 1));
			p = (UnpackedRecord)(sqlite3DbMallocRaw(pKeyInfo.db, (ulong)(nByte)));
			if (p == null)
				return null;
			p.aMem = (sqlite3_value)(&((sbyte*)(p))[(((sizeof(UnpackedRecord)) + 7) & ~7)]);
			p.pKeyInfo = pKeyInfo;
			p.nField = (ushort)(pKeyInfo.nKeyField + 1);
			return p;
		}
		public static void sqlite3VdbeRecordUnpack(KeyInfo pKeyInfo, int nKey, void* pKey, UnpackedRecord p)
		{
			byte* aKey = (byte*)(pKey);
			uint d = 0;
			uint idx = 0;
			ushort u = 0;
			uint szHdr = 0;
			sqlite3_value pMem = p.aMem;
			p.default_rc = (sbyte)(0);
			idx = (uint)((byte)(((*(aKey)) < ((byte)(0x80))) ? ((szHdr) = ((uint)(*(aKey)))), 1 :  sqlite3GetVarint32((aKey), &(szHdr)) ) ) ;
			d = (uint)(szHdr);
			u = (ushort)(0);
			while (((idx) < (szHdr)) && ((d) <= ((uint)(nKey))))
			{
				uint serial_type = 0;
				idx += (uint)((byte)(((*(&aKey[idx])) < ((byte)(0x80))) ? ((serial_type) = ((uint)(*(&aKey[idx])))), 1 : 
        sqlite3GetVarint32((&aKey[idx]), &(serial_type)) ) )
        ;
				pMem.enc = (byte)(pKeyInfo.enc);
				pMem.db = pKeyInfo.db;
				pMem.szMalloc = (int)(0);
				pMem.z = null;
				sqlite3VdbeSerialGet(&aKey[d], (uint)(serial_type), pMem);
				d += (uint)(sqlite3VdbeSerialTypeLen((uint)(serial_type)));
				pMem++;
				if ((++u) >= (p.nField))
					break;
			}

			if (((d) > ((uint)(nKey))) && ((u) != 0))
			{
				sqlite3VdbeMemSetNull(pMem - 1);
			}

			p.nField = (ushort)(u);
		}
	}
}
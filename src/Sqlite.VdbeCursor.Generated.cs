// Generated by Hebron at 4/3/2022 2:10:11 AM

using System;
using System.Runtime.InteropServices;
using Hebron.Runtime;

namespace SqliteSharp
{
	unsafe partial class Sqlite
	{
		public static int handleMovedCursor(VdbeCursor p)
		{
			int isDifferentRow = 0; int rc = 0;
			rc = (int)(sqlite3BtreeCursorRestore(p.uc.pCursor, &isDifferentRow));
			p.cacheStatus = (uint)(0);
			if ((isDifferentRow) != 0)
				p.nullRow = (byte)(1);
			return (int)(rc);
		}
		public static int sqlite3VdbeCursorMoveto(VdbeCursor pp, uint* piCol)
		{
			VdbeCursor p = pp;
			if ((p.deferredMoveto) != 0)
			{
				uint iMap = 0;
				if ((((p.ub.aAltMap) != null) && ((iMap = (uint)(p.ub.aAltMap[1 + *piCol])) > (0))) && (p.nullRow == 0))
				{
					pp = p.pAltCursor;
					*piCol = (uint)(iMap - 1);
					return (int)(0);
				}

				return (int)(sqlite3VdbeFinishMoveto(p));
			}

			if ((sqlite3BtreeCursorHasMoved(p.uc.pCursor)) != 0)
			{
				return (int)(handleMovedCursor(p));
			}

			return (int)(0);
		}
		public static int sqlite3VdbeCursorRestore(VdbeCursor p)
		{
			if ((sqlite3BtreeCursorHasMoved(p.uc.pCursor)) != 0)
			{
				return (int)(handleMovedCursor(p));
			}

			return (int)(0);
		}
		public static int sqlite3VdbeFinishMoveto(VdbeCursor p)
		{
			int res = 0; int rc = 0;
			rc = (int)(sqlite3BtreeTableMoveto(p.uc.pCursor, (long)(p.movetoTarget), (int)(0), &res));
			if ((rc) != 0)
				return (int)(rc);
			if (res != 0)
				return (int)(sqlite3CorruptError((int)(83317)));
			p.deferredMoveto = (byte)(0);
			p.cacheStatus = (uint)(0);
			return (int)(0);
		}
		public static int sqlite3VdbeSorterCompare(VdbeCursor pCsr, sqlite3_value pVal, int nKeyCol, int* pRes)
		{
			VdbeSorter pSorter;
			UnpackedRecord r2;
			KeyInfo pKeyInfo;
			int i = 0;
			void* pKey;
			int nKey = 0;
			pSorter = pCsr.uc.pSorter;
			r2 = pSorter.pUnpacked;
			pKeyInfo = pCsr.pKeyInfo;
			if ((r2) == (null))
			{
				r2 = pSorter.pUnpacked = sqlite3VdbeAllocUnpackedRecord(pKeyInfo);
				if ((r2) == (null))
					return (int)(7);
				r2.nField = (ushort)(nKeyCol);
			}

			pKey = vdbeSorterRowkey(pSorter, &nKey);
			sqlite3VdbeRecordUnpack(pKeyInfo, (int)(nKey), pKey, r2);
			for (i = (int)(0); (i) < (nKeyCol); i++)
			{
				if ((r2.aMem[i].flags & 0x0001) != 0)
				{
					*pRes = (int)(-1);
					return (int)(0);
				}
			}

			*pRes = (int)(sqlite3VdbeRecordCompare((int)(pVal.n), pVal.z, r2));
			return (int)(0);
		}
		public static int sqlite3VdbeSorterRewind(VdbeCursor pCsr, int* pbEof)
		{
			VdbeSorter pSorter;
			int rc = (int)(0);
			pSorter = pCsr.uc.pSorter;
			if ((pSorter.bUsePMA) == (0))
			{
				if ((pSorter.list.pList) != null)
				{
					*pbEof = (int)(0);
					rc = (int)(vdbeSorterSort(pSorter.aTask[0], &pSorter.list));
				}
				else
				{
					*pbEof = (int)(1);
				}

				return (int)(rc);
			}

			rc = (int)(vdbeSorterFlushPMA(pSorter));
			rc = (int)(vdbeSorterJoinAll(pSorter, (int)(rc)));
			if ((rc) == (0))
			{
				rc = (int)(vdbeSorterSetupMerge(pSorter));
				*pbEof = (int)(0);
			}

			return (int)(rc);
		}
		public static int sqlite3VdbeSorterRowkey(VdbeCursor pCsr, sqlite3_value pOut)
		{
			VdbeSorter pSorter;
			void* pKey;
			int nKey = 0;
			pSorter = pCsr.uc.pSorter;
			pKey = vdbeSorterRowkey(pSorter, &nKey);
			if ((sqlite3VdbeMemClearAndResize(pOut, (int)(nKey))) != 0)
			{
				return (int)(7);
			}

			pOut.n = (int)(nKey);
			((pOut).flags = (ushort)(((pOut).flags & ~(0xc1bf | 0x4000)) | 0x0010));
			CRuntime.memcpy(pOut.z, pKey, (ulong)(nKey));
			return (int)(0);
		}
		public static int sqlite3VdbeSorterWrite(VdbeCursor pCsr, sqlite3_value pVal)
		{
			VdbeSorter pSorter;
			int rc = (int)(0);
			SorterRecord* pNew;
			int bFlush = 0;
			int nReq = 0;
			int nPMA = 0;
			int t = 0;
			pSorter = pCsr.uc.pSorter;
			t = (int)((uint)(*((byte*)(&pVal.z[1]))));
			if ((t) >= (0x80))
				sqlite3GetVarint32(((byte*)(&pVal.z[1])), (uint*)(&(t)));
			if ((((t) > (0)) && ((t) < (10))) && (t != 7))
			{
				pSorter.typeMask &= (byte)(0x01);
			}
			else if (((t) > (10)) && ((t & 0x01) != 0))
			{
				pSorter.typeMask &= (byte)(0x02);
			}
			else
			{
				pSorter.typeMask = (byte)(0);
			}

			nReq = (int)(pVal.n + sizeof(SorterRecord));
			nPMA = (int)(pVal.n + sqlite3VarintLen((ulong)(pVal.n)));
			if ((pSorter.mxPmaSize) != 0)
			{
				if ((pSorter.list.aMemory) != null)
				{
					bFlush = (int)(((pSorter.iMemory) != 0) && ((pSorter.iMemory + nReq) > (pSorter.mxPmaSize)) ? 1 : 0);
				}
				else
				{
					bFlush = (int)(((pSorter.list.szPMA) > (pSorter.mxPmaSize)) || (((pSorter.list.szPMA) > (pSorter.mnPmaSize)) && ((sqlite3HeapNearlyFull()) != 0)) ? 1 : 0);
				}

				if ((bFlush) != 0)
				{
					rc = (int)(vdbeSorterFlushPMA(pSorter));
					pSorter.list.szPMA = (int)(0);
					pSorter.iMemory = (int)(0);
				}
			}

			pSorter.list.szPMA += (int)(nPMA);
			if ((nPMA) > (pSorter.mxKeysize))
			{
				pSorter.mxKeysize = (int)(nPMA);
			}

			if ((pSorter.list.aMemory) != null)
			{
				int nMin = (int)(pSorter.iMemory + nReq);
				if ((nMin) > (pSorter.nMemory))
				{
					byte* aNew;
					long nNew = (long)(2 * (long)(pSorter.nMemory));
					int iListOff = (int)(-1);
					if ((pSorter.list.pList) != null)
					{
						iListOff = (int)((byte*)(pSorter.list.pList) - pSorter.list.aMemory);
					}

					while ((nNew) < (nMin))
					{
						nNew = (long)(nNew * 2);
					}

					if ((nNew) > (pSorter.mxPmaSize))
						nNew = (long)(pSorter.mxPmaSize);
					if ((nNew) < (nMin))
						nNew = (long)(nMin);
					aNew = sqlite3Realloc(pSorter.list.aMemory, (ulong)(nNew));
					if (aNew == null)
						return (int)(7);
					if ((iListOff) >= (0))
					{
						pSorter.list.pList = (SorterRecord*)(&aNew[iListOff]);
					}

					pSorter.list.aMemory = aNew;
					pSorter.nMemory = (int)(nNew);
				}

				pNew = (SorterRecord*)(&pSorter.list.aMemory[pSorter.iMemory]);
				pSorter.iMemory += (int)(((nReq) + 7) & ~7);
				if ((pSorter.list.pList) != null)
				{
					pNew->u.iNext = ((int)((byte*)(pSorter.list.pList) - pSorter.list.aMemory));
				}
			}
			else
			{
				pNew = (SorterRecord*)(sqlite3Malloc((ulong)(nReq)));
				if ((pNew) == (null))
				{
					return (int)(7);
				}

				pNew->u.pNext = pSorter.list.pList;
			}

			CRuntime.memcpy(((void*)((pNew) + 1)), pVal.z, (ulong)(pVal.n));
			pNew->nVal = (int)(pVal.n);
			pSorter.list.pList = pNew;
			return (int)(rc);
		}
	}
}
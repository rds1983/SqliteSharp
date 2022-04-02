// Generated by Hebron at 4/3/2022 2:10:11 AM

using System;
using System.Runtime.InteropServices;
using Hebron.Runtime;

namespace SqliteSharp
{
	unsafe partial class Sqlite
	{
		public static int numberOfCachePages(PCache p)
		{
			if ((p.szCache) >= (0))
			{
				return (int)(p.szCache);
			}
			else
			{
				long n = 0;
				n = (long)((-1024 * (long)(p.szCache)) / (p.szPage + p.szExtra));
				if ((n) > (1000000000))
					n = (long)(1000000000);
				return (int)(n);
			}
		}
		public static PgHdr pcacheFetchFinishWithInit(PCache pCache, uint pgno, sqlite3_pcache_page* pPage)
		{
			PgHdr pPgHdr;
			pPgHdr = (PgHdr)(pPage->pExtra);
			CRuntime.memset(pPgHdr.pDirty, (int)(0), (ulong)(sizeof(PgHdr) - ((ulong)((0).pDirty))));
			pPgHdr.pPage = pPage;
			pPgHdr.pData = pPage->pBuf;
			pPgHdr.pExtra = (void*)(pPgHdr[1]);
			CRuntime.memset(pPgHdr.pExtra, (int)(0), (ulong)(8));
			pPgHdr.pCache = pCache;
			pPgHdr.pgno = (uint)(pgno);
			pPgHdr.flags = (ushort)(0x001);
			return sqlite3PcacheFetchFinish(pCache, (uint)(pgno), pPage);
		}
		public static void sqlite3PcacheCleanAll(PCache pCache)
		{
			PgHdr p;
			while ((p = pCache.pDirty) != null)
			{
				sqlite3PcacheMakeClean(p);
			}
		}
		public static void sqlite3PcacheClear(PCache pCache)
		{
			sqlite3PcacheTruncate(pCache, (uint)(0));
		}
		public static void sqlite3PcacheClearSyncFlags(PCache pCache)
		{
			PgHdr p;
			for (p = pCache.pDirty; p; p = p.pDirtyNext)
			{
				p.flags &= (ushort)(~0x008);
			}

			pCache.pSynced = pCache.pDirtyTail;
		}
		public static void sqlite3PcacheClearWritable(PCache pCache)
		{
			PgHdr p;
			for (p = pCache.pDirty; p; p = p.pDirtyNext)
			{
				p.flags &= (ushort)(~(0x008 | 0x004));
			}

			pCache.pSynced = pCache.pDirtyTail;
		}
		public static void sqlite3PcacheClose(PCache pCache)
		{
			sqlite3Config.pcache2.xDestroy(pCache.pCache);
		}
		public static PgHdr sqlite3PcacheDirtyList(PCache pCache)
		{
			PgHdr p;
			for (p = pCache.pDirty; p; p = p.pDirtyNext)
			{
				p.pDirty = p.pDirtyNext;
			}

			return pcacheSortDirtyList(pCache.pDirty);
		}
		public static PgHdr sqlite3PcacheFetchFinish(PCache pCache, uint pgno, sqlite3_pcache_page* pPage)
		{
			PgHdr pPgHdr;
			pPgHdr = (PgHdr)(pPage->pExtra);
			if (pPgHdr.pPage == null)
			{
				return pcacheFetchFinishWithInit(pCache, (uint)(pgno), pPage);
			}

			pCache.nRefSum++;
			pPgHdr.nRef++;
			return pPgHdr;
		}
		public static int sqlite3PcacheFetchStress(PCache pCache, uint pgno, sqlite3_pcache_page** ppPage)
		{
			PgHdr pPg;
			if ((pCache.eCreate) == (2))
				return (int)(0);
			if ((sqlite3PcachePagecount(pCache)) > (pCache.szSpill))
			{
				for (pPg = pCache.pSynced; ((pPg) != null) && (((pPg.nRef) != 0) || ((pPg.flags & 0x008) != 0)); pPg = pPg.pDirtyPrev)
				{
				}

				pCache.pSynced = pPg;
				if (pPg == null)
				{
					for (pPg = pCache.pDirtyTail; ((pPg) != null) && ((pPg.nRef) != 0); pPg = pPg.pDirtyPrev)
					{
					}
				}

				if ((pPg) != null)
				{
					int rc = 0;
					rc = (int)(pCache.xStress(pCache.pStress, pPg));
					if ((rc != 0) && (rc != 5))
					{
						return (int)(rc);
					}
				}
			}

			*ppPage = sqlite3Config.pcache2.xFetch(pCache.pCache, (uint)(pgno), (int)(2));
			return (int)((*ppPage) == (null) ? 7 : 0);
		}
		public static int sqlite3PcachePagecount(PCache pCache)
		{
			return (int)(sqlite3Config.pcache2.xPagecount(pCache.pCache));
		}
		public static int sqlite3PCachePercentDirty(PCache pCache)
		{
			PgHdr pDirty;
			int nDirty = (int)(0);
			int nCache = (int)(numberOfCachePages(pCache));
			for (pDirty = pCache.pDirty; pDirty; pDirty = pDirty.pDirtyNext)
			{
				nDirty++;
			}

			return (int)((nCache) != 0 ? (int)(((long)(nDirty) * 100) / nCache) : 0);
		}
		public static int sqlite3PcacheRefCount(PCache pCache)
		{
			return (int)(pCache.nRefSum);
		}
		public static void sqlite3PcacheSetCachesize(PCache pCache, int mxPage)
		{
			pCache.szCache = (int)(mxPage);
			sqlite3Config.pcache2.xCachesize(pCache.pCache, (int)(numberOfCachePages(pCache)));
		}
		public static int sqlite3PcacheSetPageSize(PCache pCache, int szPage)
		{
			if ((pCache.szPage) != 0)
			{
				sqlite3_pcache* pNew;
				pNew = sqlite3Config.pcache2.xCreate((int)(szPage), (int)(pCache.szExtra + (((sizeof(PgHdr)) + 7) & ~7)), (int)(pCache.bPurgeable));
				if ((pNew) == (null))
					return (int)(7);
				sqlite3Config.pcache2.xCachesize(pNew, (int)(numberOfCachePages(pCache)));
				if ((pCache.pCache) != null)
				{
					sqlite3Config.pcache2.xDestroy(pCache.pCache);
				}

				pCache.pCache = pNew;
				pCache.szPage = (int)(szPage);
			}

			return (int)(0);
		}
		public static int sqlite3PcacheSetSpillsize(PCache p, int mxPage)
		{
			int res = 0;
			if ((mxPage) != 0)
			{
				if ((mxPage) < (0))
				{
					mxPage = ((int)((-1024 * (long)(mxPage)) / (p.szPage + p.szExtra)));
				}

				p.szSpill = (int)(mxPage);
			}

			res = (int)(numberOfCachePages(p));
			if ((res) < (p.szSpill))
				res = (int)(p.szSpill);
			return (int)(res);
		}
		public static void sqlite3PcacheShrink(PCache pCache)
		{
			sqlite3Config.pcache2.xShrink(pCache.pCache);
		}
		public static void sqlite3PcacheTruncate(PCache pCache, uint pgno)
		{
			if ((pCache.pCache) != null)
			{
				PgHdr p;
				PgHdr pNext;
				for (p = pCache.pDirty; p; p = pNext)
				{
					pNext = p.pDirtyNext;
					if ((p.pgno) > (pgno))
					{
						sqlite3PcacheMakeClean(p);
					}
				}

				if (((pgno) == (0)) && ((pCache.nRefSum) != 0))
				{
					sqlite3_pcache_page* pPage1;
					pPage1 = sqlite3Config.pcache2.xFetch(pCache.pCache, (uint)(1), (int)(0));
					if ((pPage1) != null)
					{
						CRuntime.memset(pPage1->pBuf, (int)(0), (ulong)(pCache.szPage));
						pgno = (uint)(1);
					}
				}

				sqlite3Config.pcache2.xTruncate(pCache.pCache, (uint)(pgno + 1));
			}
		}
	}
}
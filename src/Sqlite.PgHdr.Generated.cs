// Generated by Hebron at 4/3/2022 2:10:11 AM

using System;
using System.Runtime.InteropServices;
using Hebron.Runtime;

namespace SqliteSharp
{
	unsafe partial class Sqlite
	{
		public static MemPage btreePageFromDbPage(PgHdr pDbPage, uint pgno, BtShared pBt)
		{
			MemPage pPage = (MemPage)(sqlite3PagerGetExtra(pDbPage));
			if (pgno != pPage.pgno)
			{
				pPage.aData = sqlite3PagerGetData(pDbPage);
				pPage.pDbPage = pDbPage;
				pPage.pBt = pBt;
				pPage.pgno = (uint)(pgno);
				pPage.hdrOffset = (byte)((pgno) == (1) ? 100 : 0);
			}

			return pPage;
		}
		public static int pager_write(PgHdr pPg)
		{
			Pager pPager = pPg.pPager;
			int rc = (int)(0);
			if ((pPager.eState) == (2))
			{
				rc = (int)(pager_open_journal(pPager));
				if (rc != 0)
					return (int)(rc);
			}

			sqlite3PcacheMakeDirty(pPg);
			if ((pPager.pInJournal != null) && ((sqlite3BitvecTestNotNull(pPager.pInJournal, (uint)(pPg.pgno))) == (0)))
			{
				if ((pPg.pgno) <= (pPager.dbOrigSize))
				{
					rc = (int)(pagerAddPageToRollbackJournal(pPg));
					if (rc != 0)
					{
						return (int)(rc);
					}
				}
				else
				{
					if (pPager.eState != 4)
					{
						pPg.flags |= (ushort)(0x008);
					}
				}
			}

			pPg.flags |= (ushort)(0x004);
			if ((pPager.nSavepoint) > (0))
			{
				rc = (int)(subjournalPageIfRequired(pPg));
			}

			if ((pPager.dbSize) < (pPg.pgno))
			{
				pPager.dbSize = (uint)(pPg.pgno);
			}

			return (int)(rc);
		}
		public static void pager_write_changecounter(PgHdr pPg)
		{
			uint change_counter = 0;
			if (((pPg) == (null)))
				return;
			change_counter = (uint)(sqlite3Get4byte((byte*)(pPg.pPager.dbFileVers)) + 1);
			sqlite3Put4byte((byte*)((sbyte*)(pPg.pData)) + 24, (uint)(change_counter));
			sqlite3Put4byte((byte*)((sbyte*)(pPg.pData)) + 92, (uint)(change_counter));
			sqlite3Put4byte((byte*)((sbyte*)(pPg.pData)) + 96, (uint)(3038002));
		}
		public static int pagerAddPageToRollbackJournal(PgHdr pPg)
		{
			Pager pPager = pPg.pPager;
			int rc = 0;
			uint cksum = 0;
			sbyte* pData2;
			long iOff = (long)(pPager.journalOff);
			pData2 = pPg.pData;
			cksum = (uint)(pager_cksum(pPager, (byte*)(pData2)));
			pPg.flags |= (ushort)(0x008);
			rc = (int)(write32bits(pPager.jfd, (long)(iOff), (uint)(pPg.pgno)));
			if (rc != 0)
				return (int)(rc);
			rc = (int)(sqlite3OsWrite(pPager.jfd, pData2, (int)(pPager.pageSize), (long)(iOff + 4)));
			if (rc != 0)
				return (int)(rc);
			rc = (int)(write32bits(pPager.jfd, (long)(iOff + pPager.pageSize + 4), (uint)(cksum)));
			if (rc != 0)
				return (int)(rc);
			pPager.journalOff += (long)(8 + pPager.pageSize);
			pPager.nRec++;
			rc = (int)(sqlite3BitvecSet(pPager.pInJournal, (uint)(pPg.pgno)));
			rc |= (int)(addToSavepointBitvecs(pPager, (uint)(pPg.pgno)));
			return (int)(rc);
		}
		public static void pageReinit(PgHdr pData)
		{
			MemPage pPage;
			pPage = (MemPage)(sqlite3PagerGetExtra(pData));
			if ((pPage.isInit) != 0)
			{
				pPage.isInit = (byte)(0);
				if ((sqlite3PagerPageRefcount(pData)) > (1))
				{
					btreeInitPage(pPage);
				}
			}
		}
		public static void pagerReleaseMapPage(PgHdr pPg)
		{
			Pager pPager = pPg.pPager;
			pPager.nMmapOut--;
			pPg.pDirty = pPager.pMmapFreelist;
			pPager.pMmapFreelist = pPg;
			sqlite3OsUnfetch(pPager.fd, (long)((long)(pPg.pgno - 1) * pPager.pageSize), pPg.pData);
		}
		public static int pagerWriteLargeSector(PgHdr pPg)
		{
			int rc = (int)(0);
			uint nPageCount = 0;
			uint pg1 = 0;
			int nPage = (int)(0);
			int ii = 0;
			int needSync = (int)(0);
			Pager pPager = pPg.pPager;
			uint nPagePerSector = (uint)(pPager.sectorSize / pPager.pageSize);
			pPager.doNotSpill |= (byte)(0x04);
			pg1 = (uint)(((pPg.pgno - 1) & ~(nPagePerSector - 1)) + 1);
			nPageCount = (uint)(pPager.dbSize);
			if ((pPg.pgno) > (nPageCount))
			{
				nPage = (int)((pPg.pgno - pg1) + 1);
			}
			else if ((pg1 + nPagePerSector - 1) > (nPageCount))
			{
				nPage = (int)(nPageCount + 1 - pg1);
			}
			else
			{
				nPage = (int)(nPagePerSector);
			}

			for (ii = (int)(0); ((ii) < (nPage)) && ((rc) == (0)); ii++)
			{
				uint pg = (uint)(pg1 + ii);
				PgHdr pPage;
				if (((pg) == (pPg.pgno)) || (sqlite3BitvecTest(pPager.pInJournal, (uint)(pg)) == 0))
				{
					if (pg != ((uint)((sqlite3PendingByte / ((pPager).pageSize)) + 1)))
					{
						rc = (int)(sqlite3PagerGet(pPager, (uint)(pg), pPage, (int)(0)));
						if ((rc) == (0))
						{
							rc = (int)(pager_write(pPage));
							if ((pPage.flags & 0x008) != 0)
							{
								needSync = (int)(1);
							}

							sqlite3PagerUnrefNotNull(pPage);
						}
					}
				}
				else if ((pPage = sqlite3PagerLookup(pPager, (uint)(pg))) != null)
				{
					if ((pPage.flags & 0x008) != 0)
					{
						needSync = (int)(1);
					}

					sqlite3PagerUnrefNotNull(pPage);
				}
			}

			if (((rc) == (0)) && ((needSync) != 0))
			{
				for (ii = (int)(0); (ii) < (nPage); ii++)
				{
					PgHdr pPage = sqlite3PagerLookup(pPager, (uint)(pg1 + ii));
					if ((pPage) != null)
					{
						pPage.flags |= (ushort)(0x008);
						sqlite3PagerUnrefNotNull(pPage);
					}
				}
			}

			pPager.doNotSpill &= (byte)(~0x04);
			return (int)(rc);
		}
		public static void pcacheManageDirtyList(PgHdr pPage, byte addRemove)
		{
			PCache p = pPage.pCache;
			if ((addRemove & 1) != 0)
			{
				if ((p.pSynced) == (pPage))
				{
					p.pSynced = pPage.pDirtyPrev;
				}

				if ((pPage.pDirtyNext) != null)
				{
					pPage.pDirtyNext.pDirtyPrev = pPage.pDirtyPrev;
				}
				else
				{
					p.pDirtyTail = pPage.pDirtyPrev;
				}

				if ((pPage.pDirtyPrev) != null)
				{
					pPage.pDirtyPrev.pDirtyNext = pPage.pDirtyNext;
				}
				else
				{
					p.pDirty = pPage.pDirtyNext;
					if ((p.pDirty) == (null))
					{
						p.eCreate = (byte)(2);
					}
				}
			}

			if ((addRemove & 2) != 0)
			{
				pPage.pDirtyPrev = null;
				pPage.pDirtyNext = p.pDirty;
				if ((pPage.pDirtyNext) != null)
				{
					pPage.pDirtyNext.pDirtyPrev = pPage;
				}
				else
				{
					p.pDirtyTail = pPage;
					if ((p.bPurgeable) != 0)
					{
						p.eCreate = (byte)(1);
					}
				}

				p.pDirty = pPage;
				if ((p.pSynced == null) && ((0) == (pPage.flags & 0x008)))
				{
					p.pSynced = pPage;
				}
			}
		}
		public static PgHdr pcacheMergeDirtyList(PgHdr pA, PgHdr pB)
		{
			PgHdr result = new PgHdr(); PgHdr pTail;
			pTail = result;
			for (; ; )
			{
				if ((pA.pgno) < (pB.pgno))
				{
					pTail.pDirty = pA;
					pTail = pA;
					pA = pA.pDirty;
					if ((pA) == (null))
					{
						pTail.pDirty = pB;
						break;
					}
				}
				else
				{
					pTail.pDirty = pB;
					pTail = pB;
					pB = pB.pDirty;
					if ((pB) == (null))
					{
						pTail.pDirty = pA;
						break;
					}
				}
			}

			return result.pDirty;
		}
		public static PgHdr pcacheSortDirtyList(PgHdr pIn)
		{
			var a = new PgHdr[32]; a[0] = new PgHdr(); a[1] = new PgHdr(); a[2] = new PgHdr(); a[3] = new PgHdr(); a[4] = new PgHdr(); a[5] = new PgHdr(); a[6] = new PgHdr(); a[7] = new PgHdr(); a[8] = new PgHdr(); a[9] = new PgHdr(); a[10] = new PgHdr(); a[11] = new PgHdr(); a[12] = new PgHdr(); a[13] = new PgHdr(); a[14] = new PgHdr(); a[15] = new PgHdr(); a[16] = new PgHdr(); a[17] = new PgHdr(); a[18] = new PgHdr(); a[19] = new PgHdr(); a[20] = new PgHdr(); a[21] = new PgHdr(); a[22] = new PgHdr(); a[23] = new PgHdr(); a[24] = new PgHdr(); a[25] = new PgHdr(); a[26] = new PgHdr(); a[27] = new PgHdr(); a[28] = new PgHdr(); a[29] = new PgHdr(); a[30] = new PgHdr(); a[31] = new PgHdr(); PgHdr p;
			int i = 0;
			CRuntime.memset(a, (int)(0), (ulong)(32 * sizeof(PgHdr)));
			while ((pIn) != null)
			{
				p = pIn;
				pIn = p.pDirty;
				p.pDirty = null;
				for (i = (int)(0); ((i) < (32 - 1)); i++)
				{
					if ((a[i]) == (null))
					{
						a[i] = p;
						break;
					}
					else
					{
						p = pcacheMergeDirtyList(a[i], p);
						a[i] = null;
					}
				}

				if (((i) == (32 - 1)))
				{
					a[i] = pcacheMergeDirtyList(a[i], p);
				}
			}

			p = a[0];
			for (i = (int)(1); (i) < (32); i++)
			{
				if ((a[i]) == (null))
					continue;
				p = p ? pcacheMergeDirtyList(p, a[i]) : a[i];
			}

			return p;
		}
		public static void pcacheUnpin(PgHdr p)
		{
			if ((p.pCache.bPurgeable) != 0)
			{
				sqlite3Config.pcache2.xUnpin(p.pCache.pCache, p.pPage, (int)(0));
			}
		}
		public static int readDbPage(PgHdr pPg)
		{
			Pager pPager = pPg.pPager;
			int rc = (int)(0);
			uint iFrame = (uint)(0);
			if (((pPager).pWal != null))
			{
				rc = (int)(sqlite3WalFindFrame(pPager.pWal, (uint)(pPg.pgno), &iFrame));
				if ((rc) != 0)
					return (int)(rc);
			}

			if ((iFrame) != 0)
			{
				rc = (int)(sqlite3WalReadFrame(pPager.pWal, (uint)(iFrame), (int)(pPager.pageSize), pPg.pData));
			}
			else
			{
				long iOffset = (long)((pPg.pgno - 1) * pPager.pageSize);
				rc = (int)(sqlite3OsRead(pPager.fd, pPg.pData, (int)(pPager.pageSize), (long)(iOffset)));
				if ((rc) == (10 | (2 << 8)))
				{
					rc = (int)(0);
				}
			}

			if ((pPg.pgno) == (1))
			{
				if ((rc) != 0)
				{
					CRuntime.memset(pPager.dbFileVers, (int)(0xff), (ulong)(16 * sizeof(sbyte)));
				}
				else
				{
					byte* dbFileVers = &((byte*)(pPg.pData))[24];
					CRuntime.memcpy(&pPager.dbFileVers, dbFileVers, (ulong)(16 * sizeof(sbyte)));
				}
			}

			return (int)(rc);
		}
		public static void sqlite3PagerDontWrite(PgHdr pPg)
		{
			Pager pPager = pPg.pPager;
			if (((pPager.tempFile == 0) && ((pPg.flags & 0x002) != 0)) && ((pPager.nSavepoint) == (0)))
			{
				pPg.flags |= (ushort)(0x010);
				pPg.flags &= (ushort)(~0x004);
			}
		}
		public static int sqlite3PagerPageRefcount(PgHdr pPage)
		{
			return (int)(sqlite3PcachePageRefcount(pPage));
		}
		public static void sqlite3PagerRef(PgHdr pPg)
		{
			sqlite3PcacheRef(pPg);
		}
		public static void sqlite3PagerRekey(PgHdr pPg, uint iNew, ushort flags)
		{
			pPg.flags = (ushort)(flags);
			sqlite3PcacheMove(pPg, (uint)(iNew));
		}
		public static void sqlite3PagerUnref(PgHdr pPg)
		{
			if ((pPg) != null)
				sqlite3PagerUnrefNotNull(pPg);
		}
		public static void sqlite3PagerUnrefNotNull(PgHdr pPg)
		{
			if ((pPg.flags & 0x020) != 0)
			{
				pagerReleaseMapPage(pPg);
			}
			else
			{
				sqlite3PcacheRelease(pPg);
			}
		}
		public static void sqlite3PagerUnrefPageOne(PgHdr pPg)
		{
			Pager pPager;
			pPager = pPg.pPager;
			sqlite3PcacheRelease(pPg);
			pagerUnlockIfUnused(pPager);
		}
		public static int sqlite3PagerWrite(PgHdr pPg)
		{
			Pager pPager = pPg.pPager;
			if (((pPg.flags & 0x004) != 0) && ((pPager.dbSize) >= (pPg.pgno)))
			{
				if ((pPager.nSavepoint) != 0)
					return (int)(subjournalPageIfRequired(pPg));
				return (int)(0);
			}
			else if ((pPager.errCode) != 0)
			{
				return (int)(pPager.errCode);
			}
			else if ((pPager.sectorSize) > ((uint)(pPager.pageSize)))
			{
				return (int)(pagerWriteLargeSector(pPg));
			}
			else
			{
				return (int)(pager_write(pPg));
			}
		}
		public static void sqlite3PcacheDrop(PgHdr p)
		{
			if ((p.flags & 0x002) != 0)
			{
				pcacheManageDirtyList(p, (byte)(1));
			}

			p.pCache.nRefSum--;
			sqlite3Config.pcache2.xUnpin(p.pCache.pCache, p.pPage, (int)(1));
		}
		public static void sqlite3PcacheMakeClean(PgHdr p)
		{
			pcacheManageDirtyList(p, (byte)(1));
			p.flags &= (ushort)(~(0x002 | 0x008 | 0x004));
			p.flags |= (ushort)(0x001);
			if ((p.nRef) == (0))
			{
				pcacheUnpin(p);
			}
		}
		public static void sqlite3PcacheMakeDirty(PgHdr p)
		{
			if ((p.flags & (0x001 | 0x010)) != 0)
			{
				p.flags &= (ushort)(~0x010);
				if ((p.flags & 0x001) != 0)
				{
					p.flags ^= (ushort)(0x002 | 0x001);
					pcacheManageDirtyList(p, (byte)(2));
				}
			}
		}
		public static void sqlite3PcacheMove(PgHdr p, uint newPgno)
		{
			PCache pCache = p.pCache;
			sqlite3Config.pcache2.xRekey(pCache.pCache, p.pPage, (uint)(p.pgno), (uint)(newPgno));
			p.pgno = (uint)(newPgno);
			if (((p.flags & 0x002) != 0) && ((p.flags & 0x008) != 0))
			{
				pcacheManageDirtyList(p, (byte)(3));
			}
		}
		public static int sqlite3PcachePageRefcount(PgHdr p)
		{
			return (int)(p.nRef);
		}
		public static void sqlite3PcacheRef(PgHdr p)
		{
			p.nRef++;
			p.pCache.nRefSum++;
		}
		public static void sqlite3PcacheRelease(PgHdr p)
		{
			p.pCache.nRefSum--;
			if ((--p.nRef) == (0))
			{
				if ((p.flags & 0x001) != 0)
				{
					pcacheUnpin(p);
				}
				else
				{
					pcacheManageDirtyList(p, (byte)(3));
				}
			}
		}
		public static int subjournalPage(PgHdr pPg)
		{
			int rc = (int)(0);
			Pager pPager = pPg.pPager;
			if (pPager.journalMode != 2)
			{
				rc = (int)(openSubJournal(pPager));
				if ((rc) == (0))
				{
					void* pData = pPg.pData;
					long offset = (long)((long)(pPager.nSubRec) * (4 + pPager.pageSize));
					sbyte* pData2;
					pData2 = pData;
					rc = (int)(write32bits(pPager.sjfd, (long)(offset), (uint)(pPg.pgno)));
					if ((rc) == (0))
					{
						rc = (int)(sqlite3OsWrite(pPager.sjfd, pData2, (int)(pPager.pageSize), (long)(offset + 4)));
					}
				}
			}

			if ((rc) == (0))
			{
				pPager.nSubRec++;
				rc = (int)(addToSavepointBitvecs(pPager, (uint)(pPg.pgno)));
			}

			return (int)(rc);
		}
		public static int subjournalPageIfRequired(PgHdr pPg)
		{
			if ((subjRequiresPage(pPg)) != 0)
			{
				return (int)(subjournalPage(pPg));
			}
			else
			{
				return (int)(0);
			}
		}
		public static int subjRequiresPage(PgHdr pPg)
		{
			Pager pPager = pPg.pPager;
			PagerSavepoint* p;
			uint pgno = (uint)(pPg.pgno);
			int i = 0;
			for (i = (int)(0); (i) < (pPager.nSavepoint); i++)
			{
				p = &pPager.aSavepoint[i];
				if (((p->nOrig) >= (pgno)) && ((0) == (sqlite3BitvecTestNotNull(p->pInSavepoint, (uint)(pgno)))))
				{
					for (i = (int)(i + 1); (i) < (pPager.nSavepoint); i++)
					{
						pPager.aSavepoint[i].bTruncateOnRelease = (int)(0);
					}

					return (int)(1);
				}
			}

			return (int)(0);
		}
	}
}
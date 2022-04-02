// Generated by Hebron at 4/3/2022 2:10:11 AM

using System;
using System.Runtime.InteropServices;
using Hebron.Runtime;

namespace SqliteSharp
{
	unsafe partial class Sqlite
	{
		public static void codeDeferredSeek(WhereInfo pWInfo, Index pIdx, int iCur, int iIdxCur)
		{
			Parse pParse = pWInfo.pParse;
			Vdbe v = pParse.pVdbe;
			pWInfo.bDeferredSeek = (uint)(1);
			sqlite3VdbeAddOp3(v, (int)(140), (int)(iIdxCur), (int)(0), (int)(iCur));
			if (((pWInfo.wctrlFlags & 0x0020) != 0) && ((((pParse).pToplevel ? (pParse).pToplevel : (pParse)).writeMask) == (0)))
			{
				int i = 0;
				Table pTab = pIdx.pTable;
				uint* ai = (uint*)(sqlite3DbMallocZero(pParse.db, (ulong)(sizeof(uint) * (pTab.nCol + 1))));
				if ((ai) != null)
				{
					ai[0] = (uint)(pTab.nCol);
					for (i = (int)(0); (i) < (pIdx.nColumn - 1); i++)
					{
						int x1 = 0;
						int x2 = 0;
						x1 = (int)(pIdx.aiColumn[i]);
						x2 = (int)(sqlite3TableColumnToStorage(pTab, (short)(x1)));
						if ((x1) >= (0))
							ai[x2 + 1] = (uint)(i + 1);
					}

					sqlite3VdbeChangeP4(v, (int)(-1), (sbyte*)(ai), (int)(-15));
				}
			}
		}
		public static void sqlite3ConstructBloomFilter(WhereInfo pWInfo, int iLevel, WhereLevel pLevel, ulong notReady)
		{
			int addrOnce = 0;
			int addrTop = 0;
			int addrCont = 0;
			WhereTerm pTerm;
			WhereTerm pWCEnd;
			Parse pParse = pWInfo.pParse;
			Vdbe v = pParse.pVdbe;
			WhereLoop pLoop = pLevel.pWLoop;
			int iCur = 0;
			addrOnce = (int)(sqlite3VdbeAddOp0(v, (int)(17)));
			do
			{
				SrcItem pItem;
				Table pTab;
				ulong sz = 0;
				sqlite3WhereExplainBloomFilter(pParse, pWInfo, pLevel);
				addrCont = (int)(sqlite3VdbeMakeLabel(pParse));
				iCur = (int)(pLevel.iTabCur);
				pLevel.regFilter = (int)(++pParse.nMem);
				pItem = pWInfo.pTabList.a[pLevel.iFrom];
				pTab = pItem.pTab;
				sz = (ulong)(sqlite3LogEstToInt((short)(pTab.nRowLogEst)));
				if ((sz) < (10000))
				{
					sz = (ulong)(10000);
				}
				else if ((sz) > (10000000))
				{
					sz = (ulong)(10000000);
				}

				sqlite3VdbeAddOp2(v, (int)(76), (int)(sz), (int)(pLevel.regFilter));
				addrTop = (int)(sqlite3VdbeAddOp1(v, (int)(38), (int)(iCur)));
				pWCEnd = pWInfo.sWC.a[pWInfo.sWC.nTerm];
				for (pTerm = pWInfo.sWC.a; (pTerm) < (pWCEnd); pTerm++)
				{
					Expr pExpr = pTerm.pExpr;
					if (((pTerm.wtFlags & 0x0002) == (0)) && ((sqlite3ExprIsTableConstant(pExpr, (int)(iCur))) != 0))
					{
						sqlite3ExprIfFalse(pParse, pTerm.pExpr, (int)(addrCont), (int)(0x10));
					}
				}

				if ((pLoop.wsFlags & 0x00000100) != 0)
				{
					int r1 = (int)(sqlite3GetTempReg(pParse));
					sqlite3VdbeAddOp2(v, (int)(134), (int)(iCur), (int)(r1));
					sqlite3VdbeAddOp4Int(v, (int)(178), (int)(pLevel.regFilter), (int)(0), (int)(r1), (int)(1));
					sqlite3ReleaseTempReg(pParse, (int)(r1));
				}
				else
				{
					Index pIdx = pLoop.u.btree.pIndex;
					int n = (int)(pLoop.u.btree.nEq);
					int r1 = (int)(sqlite3GetTempRange(pParse, (int)(n)));
					int jj = 0;
					for (jj = (int)(0); (jj) < (n); jj++)
					{
						int iCol = (int)(pIdx.aiColumn[jj]);
						sqlite3ExprCodeGetColumnOfTable(v, pIdx.pTable, (int)(iCur), (int)(iCol), (int)(r1 + jj));
					}

					sqlite3VdbeAddOp4Int(v, (int)(178), (int)(pLevel.regFilter), (int)(0), (int)(r1), (int)(n));
					sqlite3ReleaseTempRange(pParse, (int)(r1), (int)(n));
				}

				sqlite3VdbeResolveLabel(v, (int)(addrCont));
				sqlite3VdbeAddOp2(v, (int)(5), (int)(pLevel.iTabCur), (int)(addrTop + 1));
				sqlite3VdbeJumpHere(v, (int)(addrTop));
				pLoop.wsFlags &= (uint)(~0x00400000);
				if ((((pParse.db).dbOptFlags & (0x00100000)) != 0))
					break;
				while ((++iLevel) < (pWInfo.nLevel))
				{
					SrcItem pTabItem;
					pLevel = pWInfo.a[iLevel];
					pTabItem = pWInfo.pTabList.a[pLevel.iFrom];
					if ((pTabItem.fg.jointype & 0x0008) != 0)
						continue;
					pLoop = pLevel.pWLoop;
					if (((pLoop) == (null)))
						continue;
					if ((pLoop.prereq & notReady) != 0)
						continue;
					if ((pLoop.wsFlags & (0x00400000 | 0x00000004)) == (0x00400000))
					{
						break;
					}
				}
			}
			while ((iLevel) < (pWInfo.nLevel));
			sqlite3VdbeJumpHere(v, (int)(addrOnce));
		}
		public static int sqlite3WhereBreakLabel(WhereInfo pWInfo)
		{
			return (int)(pWInfo.iBreak);
		}
		public static int sqlite3WhereContinueLabel(WhereInfo pWInfo)
		{
			return (int)(pWInfo.iContinue);
		}
		public static void sqlite3WhereEnd(WhereInfo pWInfo)
		{
			Parse pParse = pWInfo.pParse;
			Vdbe v = pParse.pVdbe;
			int i = 0;
			WhereLevel pLevel;
			WhereLoop pLoop;
			SrcList pTabList = pWInfo.pTabList;
			sqlite3 db = pParse.db;
			int iEnd = (int)(sqlite3VdbeCurrentAddr(v));
			for (i = (int)(pWInfo.nLevel - 1); (i) >= (0); i--)
			{
				int addr = 0;
				pLevel = pWInfo.a[i];
				pLoop = pLevel.pWLoop;
				if (pLevel.op != 182)
				{
					int addrSeek = (int)(0);
					Index pIdx;
					int n = 0;
					if (((((((pWInfo.eDistinct) == (2)) && ((i) == (pWInfo.nLevel - 1))) && ((pLoop.wsFlags & 0x00000200) != 0)) && (((pIdx = pLoop.u.btree.pIndex).hasStat1) != 0)) && ((n = (int)(pLoop.u.btree.nDistinctCol)) > (0))) && ((pIdx.aiRowLogEst[n]) >= (36)))
					{
						int r1 = (int)(pParse.nMem + 1);
						int j = 0;
						int op = 0;
						for (j = (int)(0); (j) < (n); j++)
						{
							sqlite3VdbeAddOp3(v, (int)(93), (int)(pLevel.iIdxCur), (int)(j), (int)(r1 + j));
						}

						pParse.nMem += (int)(n + 1);
						op = (int)((pLevel.op) == (4) ? 23 : 26);
						addrSeek = (int)(sqlite3VdbeAddOp4Int(v, (int)(op), (int)(pLevel.iIdxCur), (int)(0), (int)(r1), (int)(n)));
						sqlite3VdbeAddOp2(v, (int)(11), (int)(1), (int)(pLevel.p2));
					}

					sqlite3VdbeResolveLabel(v, (int)(pLevel.addrCont));
					sqlite3VdbeAddOp3(v, (int)(pLevel.op), (int)(pLevel.p1), (int)(pLevel.p2), (int)(pLevel.p3));
					sqlite3VdbeChangeP5(v, (ushort)(pLevel.p5));
					if ((pLevel.regBignull) != 0)
					{
						sqlite3VdbeResolveLabel(v, (int)(pLevel.addrBignull));
						sqlite3VdbeAddOp2(v, (int)(60), (int)(pLevel.regBignull), (int)(pLevel.p2 - 1));
					}

					if ((addrSeek) != 0)
						sqlite3VdbeJumpHere(v, (int)(addrSeek));
				}
				else
				{
					sqlite3VdbeResolveLabel(v, (int)(pLevel.addrCont));
				}

				if (((pLoop.wsFlags & 0x00000800) != 0) && ((pLevel.u._in_.nIn) > (0)))
				{
					InLoop* pIn;
					int j = 0;
					sqlite3VdbeResolveLabel(v, (int)(pLevel.addrNxt));
					for (j = (int)(pLevel.u._in_.nIn), pIn = &pLevel.u._in_.aInLoop[j - 1]; (j) > (0); j--, pIn--)
					{
						sqlite3VdbeJumpHere(v, (int)(pIn->addrInTop + 1));
						if (pIn->eEndLoopOp != 182)
						{
							if ((pIn->nPrefix) != 0)
							{
								int bEarlyOut = (int)(((pLoop.wsFlags & 0x00000400) == (0)) && ((pLoop.wsFlags & 0x00040000) != 0) ? 1 : 0);
								if ((pLevel.iLeftJoin) != 0)
								{
									sqlite3VdbeAddOp2(v, (int)(27), (int)(pIn->iCur), (int)(sqlite3VdbeCurrentAddr(v) + 2 + bEarlyOut));
								}

								if ((bEarlyOut) != 0)
								{
									sqlite3VdbeAddOp4Int(v, (int)(28), (int)(pLevel.iIdxCur), (int)(sqlite3VdbeCurrentAddr(v) + 2), (int)(pIn->iBase), (int)(pIn->nPrefix));
									sqlite3VdbeJumpHere(v, (int)(pIn->addrInTop + 1));
								}
							}

							sqlite3VdbeAddOp2(v, (int)(pIn->eEndLoopOp), (int)(pIn->iCur), (int)(pIn->addrInTop));
						}

						sqlite3VdbeJumpHere(v, (int)(pIn->addrInTop - 1));
					}
				}

				sqlite3VdbeResolveLabel(v, (int)(pLevel.addrBrk));
				if ((pLevel.addrSkip) != 0)
				{
					sqlite3VdbeGoto(v, (int)(pLevel.addrSkip));
					sqlite3VdbeJumpHere(v, (int)(pLevel.addrSkip));
					sqlite3VdbeJumpHere(v, (int)(pLevel.addrSkip - 2));
				}

				if ((pLevel.addrLikeRep) != 0)
				{
					sqlite3VdbeAddOp2(v, (int)(60), (int)(pLevel.iLikeRepCntr >> 1), (int)(pLevel.addrLikeRep));
				}

				if ((pLevel.iLeftJoin) != 0)
				{
					int ws = (int)(pLoop.wsFlags);
					addr = (int)(sqlite3VdbeAddOp1(v, (int)(49), (int)(pLevel.iLeftJoin)));
					if ((ws & 0x00000040) == (0))
					{
						sqlite3VdbeAddOp1(v, (int)(135), (int)(pLevel.iTabCur));
					}

					if (((ws & 0x00000200) != 0) || (((ws & 0x00002000) != 0) && ((pLevel.u.pCoveringIdx) != null)))
					{
						if ((ws & 0x00002000) != 0)
						{
							Index pIx = pLevel.u.pCoveringIdx;
							int iDb = (int)(sqlite3SchemaToIndex(db, pIx.pSchema));
							sqlite3VdbeAddOp3(v, (int)(100), (int)(pLevel.iIdxCur), (int)(pIx.tnum), (int)(iDb));
							sqlite3VdbeSetP4KeyInfo(pParse, pIx);
						}

						sqlite3VdbeAddOp1(v, (int)(135), (int)(pLevel.iIdxCur));
					}

					if ((pLevel.op) == (67))
					{
						sqlite3VdbeAddOp2(v, (int)(12), (int)(pLevel.p1), (int)(pLevel.addrFirst));
					}
					else
					{
						sqlite3VdbeGoto(v, (int)(pLevel.addrFirst));
					}

					sqlite3VdbeJumpHere(v, (int)(addr));
				}
			}

			sqlite3VdbeResolveLabel(v, (int)(pWInfo.iBreak));
			for (i = (int)(0), pLevel = pWInfo.a; (i) < (pWInfo.nLevel); i++, pLevel++)
			{
				int k = 0;
				int last = 0;
				VdbeOp* pOp;
				VdbeOp* pLastOp;
				Index pIdx = null;
				SrcItem pTabItem = pTabList.a[pLevel.iFrom];
				Table pTab = pTabItem.pTab;
				pLoop = pLevel.pWLoop;
				if ((pTabItem.fg.viaCoroutine) != 0)
				{
					translateColumnToCopy(pParse, (int)(pLevel.addrBody), (int)(pLevel.iTabCur), (int)(pTabItem.regResult), (int)(0));
					continue;
				}

				if ((pLoop.wsFlags & (0x00000200 | 0x00000040)) != 0)
				{
					pIdx = pLoop.u.btree.pIndex;
				}
				else if ((pLoop.wsFlags & 0x00002000) != 0)
				{
					pIdx = pLevel.u.pCoveringIdx;
				}

				if (((pIdx) != null) && (db.mallocFailed == 0))
				{
					if (((pWInfo.eOnePass) == (0)) || (!(((pIdx.pTable).tabFlags & 0x00000080) == (0))))
					{
						last = (int)(iEnd);
					}
					else
					{
						last = (int)(pWInfo.iEndWhere);
					}

					k = (int)(pLevel.addrBody + 1);
					pOp = sqlite3VdbeGetOp(v, (int)(k));
					pLastOp = pOp + (last - k);
					do
					{
						if (pOp->p1 != pLevel.iTabCur)
						{
						}
						else if ((pOp->opcode) == (93))
						{
							int x = (int)(pOp->p2);
							if (!(((pTab).tabFlags & 0x00000080) == (0)))
							{
								Index pPk = sqlite3PrimaryKeyIndex(pTab);
								x = (int)(pPk.aiColumn[x]);
							}
							else
							{
								x = (int)(sqlite3StorageColumnToTable(pTab, (short)(x)));
							}

							x = (int)(sqlite3TableColumnToIndex(pIdx, (short)(x)));
							if ((x) >= (0))
							{
								pOp->p2 = (int)(x);
								pOp->p1 = (int)(pLevel.iIdxCur);
							}
							else
							{
							}
						}
						else if ((pOp->opcode) == (134))
						{
							pOp->p1 = (int)(pLevel.iIdxCur);
							pOp->opcode = (byte)(141);
						}
						else if ((pOp->opcode) == (22))
						{
							pOp->p1 = (int)(pLevel.iIdxCur);
						}
					}
					while ((++pOp) < (pLastOp));
				}
			}

			if ((pWInfo.pExprMods) != null)
				whereUndoExprMods(pWInfo);
			pParse.nQueryLoop = (uint)(pWInfo.savedNQueryLoop);
			whereInfoFree(db, pWInfo);
			return;
		}
		public static int sqlite3WhereIsDistinct(WhereInfo pWInfo)
		{
			return (int)(pWInfo.eDistinct);
		}
		public static int sqlite3WhereIsOrdered(WhereInfo pWInfo)
		{
			return (int)(pWInfo.nOBSat);
		}
		public static int sqlite3WhereIsSorted(WhereInfo pWInfo)
		{
			return (int)(pWInfo.sorted);
		}
		public static int sqlite3WhereOkOnePass(WhereInfo pWInfo, int* aiCur)
		{
			CRuntime.memcpy(aiCur, pWInfo.aiCurOnePass, (ulong)(sizeof(int) * 2));
			return (int)(pWInfo.eOnePass);
		}
		public static int sqlite3WhereOrderByLimitOptLabel(WhereInfo pWInfo)
		{
			WhereLevel pInner;
			if (pWInfo.bOrderedInnerLoop == 0)
			{
				return (int)(pWInfo.iContinue);
			}

			pInner = pWInfo.a[pWInfo.nLevel - 1];
			return (int)(pInner.addrNxt);
		}
		public static short sqlite3WhereOutputRowCount(WhereInfo pWInfo)
		{
			return (short)(pWInfo.nRowOut);
		}
		public static int sqlite3WhereUsesDeferredSeek(WhereInfo pWInfo)
		{
			return (int)(pWInfo.bDeferredSeek);
		}
		public static void whereCheckIfBloomFilterIsUseful(WhereInfo pWInfo)
		{
			int i = 0;
			short nSearch = 0;
			nSearch = (short)(pWInfo.a[0].pWLoop.nOut);
			for (i = (int)(1); (i) < (pWInfo.nLevel); i++)
			{
				WhereLoop pLoop = pWInfo.a[i].pWLoop;
				uint reqFlags = (uint)(0x00800000 | 0x00000001);
				if (((pLoop.wsFlags & reqFlags) == (reqFlags)) && ((pLoop.wsFlags & (0x00000100 | 0x00000200)) != 0))
				{
					SrcItem pItem = pWInfo.pTabList.a[pLoop.iTab];
					Table pTab = pItem.pTab;
					pTab.tabFlags |= (uint)(0x00000100);
					if (((nSearch) > (pTab.nRowLogEst)) && ((pTab.tabFlags & 0x00000010) != 0))
					{
						pLoop.wsFlags |= (uint)(0x00400000);
						pLoop.wsFlags &= (uint)(~0x00000040);
					}
				}

				nSearch += (short)(pLoop.nOut);
			}
		}
		public static ulong whereOmitNoopJoin(WhereInfo pWInfo, ulong notReady)
		{
			int i = 0;
			ulong tabUsed = 0;
			tabUsed = (ulong)(sqlite3WhereExprListUsage(&pWInfo.sMaskSet, pWInfo.pResultSet));
			if ((pWInfo.pOrderBy) != null)
			{
				tabUsed |= (ulong)(sqlite3WhereExprListUsage(&pWInfo.sMaskSet, pWInfo.pOrderBy));
			}

			for (i = (int)(pWInfo.nLevel - 1); (i) >= (1); i--)
			{
				WhereTerm pTerm;
				WhereTerm pEnd;
				SrcItem pItem;
				WhereLoop pLoop;
				pLoop = pWInfo.a[i].pWLoop;
				pItem = pWInfo.pTabList.a[pLoop.iTab];
				if ((pItem.fg.jointype & 0x0008) == (0))
					continue;
				if (((pWInfo.wctrlFlags & 0x0100) == (0)) && ((pLoop.wsFlags & 0x00001000) == (0)))
				{
					continue;
				}

				if ((tabUsed & pLoop.maskSelf) != 0)
					continue;
				pEnd = pWInfo.sWC.a[pWInfo.sWC.nTerm];
				for (pTerm = pWInfo.sWC.a; (pTerm) < (pEnd); pTerm++)
				{
					if ((pTerm.prereqAll & pLoop.maskSelf) != 0)
					{
						if ((!(((pTerm.pExpr).flags & (0x000001)) != 0)) || (pTerm.pExpr.w.iRightJoinTable != pItem.iCursor))
						{
							break;
						}
					}
				}

				if ((pTerm) < (pEnd))
					continue;
				notReady &= (ulong)(~pLoop.maskSelf);
				for (pTerm = pWInfo.sWC.a; (pTerm) < (pEnd); pTerm++)
				{
					if ((pTerm.prereqAll & pLoop.maskSelf) != 0)
					{
						pTerm.wtFlags |= (ushort)(0x0004);
					}
				}

				if (i != pWInfo.nLevel - 1)
				{
					int nByte = (int)((pWInfo.nLevel - 1 - i) * sizeof(WhereLevel));
					CRuntime.memmove(pWInfo.a[i], pWInfo.a[i + 1], (ulong)(nByte));
				}

				pWInfo.nLevel--;
			}

			return (ulong)(notReady);
		}
		public static sbyte wherePathSatisfiesOrderBy(WhereInfo pWInfo, ExprList pOrderBy, WherePath pPath, ushort wctrlFlags, ushort nLoop, WhereLoop pLast, ulong* pRevMask)
		{
			byte revSet = 0;
			byte rev = 0;
			byte revIdx = 0;
			byte isOrderDistinct = 0;
			byte distinctColumns = 0;
			byte isMatch = 0;
			ushort eqOpMask = 0;
			ushort nKeyCol = 0;
			ushort nColumn = 0;
			ushort nOrderBy = 0;
			int iLoop = 0;
			int i = 0; int j = 0;
			int iCur = 0;
			int iColumn = 0;
			WhereLoop pLoop = null;
			WhereTerm pTerm;
			Expr pOBExpr;
			CollSeq pColl;
			Index pIndex;
			sqlite3 db = pWInfo.pParse.db;
			ulong obSat = (ulong)(0);
			ulong obDone = 0;
			ulong orderDistinctMask = 0;
			ulong ready = 0;
			if (((nLoop) != 0) && (((db).dbOptFlags & (0x00000040)) != 0))
				return (sbyte)(0);
			nOrderBy = (ushort)(pOrderBy.nExpr);
			if ((nOrderBy) > (((int)(sizeof(ulong) * 8)) - 1))
				return (sbyte)(0);
			isOrderDistinct = (byte)(1);
			obDone = (ulong)((((ulong)(1)) << (nOrderBy)) - 1);
			orderDistinctMask = (ulong)(0);
			ready = (ulong)(0);
			eqOpMask = (ushort)(0x0002 | 0x0080 | 0x0100);
			if ((wctrlFlags & (0x0800 | 0x0002 | 0x0001)) != 0)
			{
				eqOpMask |= (ushort)(0x0001);
			}

			for (iLoop = (int)(0); (((isOrderDistinct) != 0) && ((obSat) < (obDone))) && ((iLoop) <= (nLoop)); iLoop++)
			{
				if ((iLoop) > (0))
					ready |= (ulong)(pLoop.maskSelf);
				if ((iLoop) < (nLoop))
				{
					pLoop = pPath.aLoop[iLoop];
					if ((wctrlFlags & 0x0800) != 0)
						continue;
				}
				else
				{
					pLoop = pLast;
				}

				if ((pLoop.wsFlags & 0x00000400) != 0)
				{
					if (((pLoop.u.vtab.isOrdered) != 0) && ((wctrlFlags & 0x0080) == (0)))
					{
						obSat = (ulong)(obDone);
					}

					break;
				}
				else if ((wctrlFlags & 0x0080) != 0)
				{
					pLoop.u.btree.nDistinctCol = (ushort)(0);
				}

				iCur = (int)(pWInfo.pTabList.a[pLoop.iTab].iCursor);
				for (i = (int)(0); (i) < (nOrderBy); i++)
				{
					if (((((ulong)(1)) << (i)) & obSat) != 0)
						continue;
					pOBExpr = sqlite3ExprSkipCollateAndLikely(pOrderBy.a[i].pExpr);
					if (((pOBExpr) == (null)))
						continue;
					if ((pOBExpr.op != 167) && (pOBExpr.op != 169))
						continue;
					if (pOBExpr.iTable != iCur)
						continue;
					pTerm = sqlite3WhereFindTerm(pWInfo.sWC, (int)(iCur), (int)(pOBExpr.iColumn), (ulong)(~ready), (uint)(eqOpMask), null);
					if ((pTerm) == (null))
						continue;
					if ((pTerm.eOperator) == (0x0001))
					{
						for (j = (int)(0); ((j) < (pLoop.nLTerm)) && (pTerm != pLoop.aLTerm[j]); j++)
						{
						}

						if ((j) >= (pLoop.nLTerm))
							continue;
					}

					if (((pTerm.eOperator & (0x0002 | 0x0080)) != 0) && ((pOBExpr.iColumn) >= (0)))
					{
						Parse pParse = pWInfo.pParse;
						CollSeq pColl1 = sqlite3ExprNNCollSeq(pParse, pOrderBy.a[i].pExpr);
						CollSeq pColl2 = sqlite3ExprCompareCollSeq(pParse, pTerm.pExpr);
						if (((pColl2) == (null)) || ((sqlite3StrICmp(pColl1.zName, pColl2.zName)) != 0))
						{
							continue;
						}
					}

					obSat |= (ulong)(((ulong)(1)) << (i));
				}

				if ((pLoop.wsFlags & 0x00001000) == (0))
				{
					if ((pLoop.wsFlags & 0x00000100) != 0)
					{
						pIndex = null;
						nKeyCol = (ushort)(0);
						nColumn = (ushort)(1);
					}
					else if (((pIndex = pLoop.u.btree.pIndex) == (null)) || ((pIndex.bUnordered) != 0))
					{
						return (sbyte)(0);
					}
					else
					{
						nKeyCol = (ushort)(pIndex.nKeyCol);
						nColumn = (ushort)(pIndex.nColumn);
						isOrderDistinct = (byte)(((pIndex).onError != 0) && ((pLoop.wsFlags & 0x00008000) == (0)));
					}

					rev = (byte)(revSet = (byte)(0));
					distinctColumns = (byte)(0);
					for (j = (int)(0); (j) < (nColumn); j++)
					{
						byte bOnce = (byte)(1);
						if (((j) < (pLoop.u.btree.nEq)) && ((j) >= (pLoop.nSkip)))
						{
							ushort eOp = (ushort)(pLoop.aLTerm[j].eOperator);
							if ((eOp & eqOpMask) != 0)
							{
								if ((eOp & (0x0100 | 0x0080)) != 0)
								{
									isOrderDistinct = (byte)(0);
								}

								continue;
							}
							else if ((eOp & 0x0001) != 0)
							{
								Expr pX = pLoop.aLTerm[j].pExpr;
								for (i = (int)(j + 1); (i) < (pLoop.u.btree.nEq); i++)
								{
									if ((pLoop.aLTerm[i].pExpr) == (pX))
									{
										bOnce = (byte)(0);
										break;
									}
								}
							}
						}

						if ((pIndex) != null)
						{
							iColumn = (int)(pIndex.aiColumn[j]);
							revIdx = (byte)(pIndex.aSortOrder[j] & 0x01);
							if ((iColumn) == (pIndex.pTable.iPKey))
								iColumn = (int)(-1);
						}
						else
						{
							iColumn = (int)(-1);
							revIdx = (byte)(0);
						}

						if ((isOrderDistinct) != 0)
						{
							if ((((iColumn) >= (0)) && ((j) >= (pLoop.u.btree.nEq))) && ((pIndex.pTable.aCol[iColumn].notNull) == (0)))
							{
								isOrderDistinct = (byte)(0);
							}

							if ((iColumn) == (-2))
							{
								isOrderDistinct = (byte)(0);
							}
						}

						isMatch = (byte)(0);
						for (i = (int)(0); ((bOnce) != 0) && ((i) < (nOrderBy)); i++)
						{
							if (((((ulong)(1)) << (i)) & obSat) != 0)
								continue;
							pOBExpr = sqlite3ExprSkipCollateAndLikely(pOrderBy.a[i].pExpr);
							if (((pOBExpr) == (null)))
								continue;
							if ((wctrlFlags & (0x0040 | 0x0080)) == (0))
								bOnce = (byte)(0);
							if ((iColumn) >= (-1))
							{
								if ((pOBExpr.op != 167) && (pOBExpr.op != 169))
									continue;
								if (pOBExpr.iTable != iCur)
									continue;
								if (pOBExpr.iColumn != iColumn)
									continue;
							}
							else
							{
								Expr pIdxExpr = pIndex.aColExpr.a[j].pExpr;
								if ((sqlite3ExprCompareSkip(pOBExpr, pIdxExpr, (int)(iCur))) != 0)
								{
									continue;
								}
							}

							if (iColumn != (-1))
							{
								pColl = sqlite3ExprNNCollSeq(pWInfo.pParse, pOrderBy.a[i].pExpr);
								if (sqlite3StrICmp(pColl.zName, pIndex.azColl[j]) != 0)
									continue;
							}

							if ((wctrlFlags & 0x0080) != 0)
							{
								pLoop.u.btree.nDistinctCol = (ushort)(j + 1);
							}

							isMatch = (byte)(1);
							break;
						}

						if (((isMatch) != 0) && ((wctrlFlags & 0x0040) == (0)))
						{
							if ((revSet) != 0)
							{
								if ((rev ^ revIdx) != (pOrderBy.a[i].sortFlags & 0x01))
								{
									isMatch = (byte)(0);
								}
							}
							else
							{
								rev = (byte)(revIdx ^ (pOrderBy.a[i].sortFlags & 0x01));
								if ((rev) != 0)
									*pRevMask |= (ulong)(((ulong)(1)) << (iLoop));
								revSet = (byte)(1);
							}
						}

						if (((isMatch) != 0) && ((pOrderBy.a[i].sortFlags & 0x02) != 0))
						{
							if ((j) == (pLoop.u.btree.nEq))
							{
								pLoop.wsFlags |= (uint)(0x00080000);
							}
							else
							{
								isMatch = (byte)(0);
							}
						}

						if ((isMatch) != 0)
						{
							if ((iColumn) == (-1))
							{
								distinctColumns = (byte)(1);
							}

							obSat |= (ulong)(((ulong)(1)) << (i));
						}
						else
						{
							if (((j) == (0)) || ((j) < (nKeyCol)))
							{
								isOrderDistinct = (byte)(0);
							}

							break;
						}
					}

					if ((distinctColumns) != 0)
					{
						isOrderDistinct = (byte)(1);
					}
				}

				if ((isOrderDistinct) != 0)
				{
					orderDistinctMask |= (ulong)(pLoop.maskSelf);
					for (i = (int)(0); (i) < (nOrderBy); i++)
					{
						Expr p;
						ulong mTerm = 0;
						if (((((ulong)(1)) << (i)) & obSat) != 0)
							continue;
						p = pOrderBy.a[i].pExpr;
						mTerm = (ulong)(sqlite3WhereExprUsage(&pWInfo.sMaskSet, p));
						if (((mTerm) == (0)) && (sqlite3ExprIsConstant(p) == 0))
							continue;
						if ((mTerm & ~orderDistinctMask) == (0))
						{
							obSat |= (ulong)(((ulong)(1)) << (i));
						}
					}
				}
			}

			if ((obSat) == (obDone))
				return (sbyte)(nOrderBy);
			if (isOrderDistinct == 0)
			{
				for (i = (int)(nOrderBy - 1); (i) > (0); i--)
				{
					ulong m = (ulong)(((i) < ((int)(sizeof(ulong) * 8))) ? (((ulong)(1)) << (i)) - 1 : 0);
					if ((obSat & m) == (m))
						return (sbyte)(i);
				}

				return (sbyte)(0);
			}

			return (sbyte)(-1);
		}
		public static int wherePathSolver(WhereInfo pWInfo, short nRowEst)
		{
			int mxChoice = 0;
			int nLoop = 0;
			Parse pParse;
			sqlite3 db;
			int iLoop = 0;
			int ii = 0; int jj = 0;
			int mxI = (int)(0);
			int nOrderBy = 0;
			short mxCost = (short)(0);
			short mxUnsorted = (short)(0);
			int nTo = 0; int nFrom = 0;
			WherePath aFrom;
			WherePath aTo;
			WherePath pFrom;
			WherePath pTo;
			WhereLoop pWLoop;
			WhereLoop pX;
			short* aSortCost = null;
			sbyte* pSpace;
			int nSpace = 0;
			pParse = pWInfo.pParse;
			db = pParse.db;
			nLoop = (int)(pWInfo.nLevel);
			mxChoice = (int)(((nLoop) <= (1)) ? 1 : ((nLoop) == (2) ? 5 : 10));
			if (((pWInfo.pOrderBy) == (null)) || ((nRowEst) == (0)))
			{
				nOrderBy = (int)(0);
			}
			else
			{
				nOrderBy = (int)(pWInfo.pOrderBy.nExpr);
			}

			nSpace = (int)((sizeof(WherePath) + sizeof(WhereLoop) * nLoop) * mxChoice * 2);
			nSpace += (int)(sizeof(short) * nOrderBy);
			pSpace = sqlite3DbMallocRawNN(db, (ulong)(nSpace));
			if ((pSpace) == (null))
				return (int)(7);
			aTo = (WherePath)(pSpace);
			aFrom = aTo[mxChoice];
			CRuntime.memset(aFrom, (int)(0), (ulong)(sizeof(WherePath)));
			pX = (WhereLoop)(aFrom[mxChoice]);
			for (ii = (int)(mxChoice * 2), pFrom = aTo; (ii) > (0); ii--, pFrom++, pX += nLoop)
			{
				pFrom.aLoop = pX;
			}

			if ((nOrderBy) != 0)
			{
				aSortCost = (short*)(pX);
				CRuntime.memset(aSortCost, (int)(0), (ulong)(sizeof(short) * nOrderBy));
			}

			aFrom[0].nRow = (short)((pParse.nQueryLoop) < (48) ? (pParse.nQueryLoop) : (48));
			nFrom = (int)(1);
			if ((nOrderBy) != 0)
			{
				aFrom[0].isOrdered = (sbyte)((nLoop) > (0) ? -1 : nOrderBy);
			}

			for (iLoop = (int)(0); (iLoop) < (nLoop); iLoop++)
			{
				nTo = (int)(0);
				for (ii = (int)(0), pFrom = aFrom; (ii) < (nFrom); ii++, pFrom++)
				{
					for (pWLoop = pWInfo.pLoops; pWLoop; pWLoop = pWLoop.pNextLoop)
					{
						short nOut = 0;
						short rCost = 0;
						short rUnsorted = 0;
						sbyte isOrdered = (sbyte)(pFrom.isOrdered);
						ulong maskNew = 0;
						ulong revMask = (ulong)(0);
						if ((pWLoop.prereq & ~pFrom.maskLoop) != 0)
							continue;
						if ((pWLoop.maskSelf & pFrom.maskLoop) != 0)
							continue;
						if (((pWLoop.wsFlags & 0x00004000) != 0) && ((pFrom.nRow) < (3)))
						{
							continue;
						}

						rUnsorted = (short)(sqlite3LogEstAdd((short)(pWLoop.rSetup), (short)(pWLoop.rRun + pFrom.nRow)));
						rUnsorted = (short)(sqlite3LogEstAdd((short)(rUnsorted), (short)(pFrom.rUnsorted)));
						nOut = (short)(pFrom.nRow + pWLoop.nOut);
						maskNew = (ulong)(pFrom.maskLoop | pWLoop.maskSelf);
						if ((isOrdered) < (0))
						{
							isOrdered = (sbyte)(wherePathSatisfiesOrderBy(pWInfo, pWInfo.pOrderBy, pFrom, (ushort)(pWInfo.wctrlFlags), (ushort)(iLoop), pWLoop, &revMask));
						}
						else
						{
							revMask = (ulong)(pFrom.revLoop);
						}

						if (((isOrdered) >= (0)) && ((isOrdered) < (nOrderBy)))
						{
							if ((aSortCost[isOrdered]) == (0))
							{
								aSortCost[isOrdered] = (short)(whereSortingCost(pWInfo, (short)(nRowEst), (int)(nOrderBy), (int)(isOrdered)));
							}

							rCost = (short)(sqlite3LogEstAdd((short)(rUnsorted), (short)(aSortCost[isOrdered])) + 5);
						}
						else
						{
							rCost = (short)(rUnsorted);
							rUnsorted -= (short)(2);
						}

						for (jj = (int)(0), pTo = aTo; (jj) < (nTo); jj++, pTo++)
						{
							if (((pTo.maskLoop) == (maskNew)) && (((pTo.isOrdered ^ isOrdered) & 0x80) == (0)))
							{
								break;
							}
						}

						if ((jj) >= (nTo))
						{
							if (((nTo) >= (mxChoice)) && (((rCost) > (mxCost)) || (((rCost) == (mxCost)) && ((rUnsorted) >= (mxUnsorted)))))
							{
								continue;
							}

							if ((nTo) < (mxChoice))
							{
								jj = (int)(nTo++);
							}
							else
							{
								jj = (int)(mxI);
							}

							pTo = aTo[jj];
						}
						else
						{
							if (((pTo.rCost) < (rCost)) || (((pTo.rCost) == (rCost)) && (((pTo.nRow) < (nOut)) || (((pTo.nRow) == (nOut)) && ((pTo.rUnsorted) <= (rUnsorted))))))
							{
								continue;
							}
						}

						pTo.maskLoop = (ulong)(pFrom.maskLoop | pWLoop.maskSelf);
						pTo.revLoop = (ulong)(revMask);
						pTo.nRow = (short)(nOut);
						pTo.rCost = (short)(rCost);
						pTo.rUnsorted = (short)(rUnsorted);
						pTo.isOrdered = (sbyte)(isOrdered);
						CRuntime.memcpy(pTo.aLoop, pFrom.aLoop, (ulong)(sizeof(WhereLoop) * iLoop));
						pTo.aLoop[iLoop] = pWLoop;
						if ((nTo) >= (mxChoice))
						{
							mxI = (int)(0);
							mxCost = (short)(aTo[0].rCost);
							mxUnsorted = (short)(aTo[0].nRow);
							for (jj = (int)(1), pTo = aTo[1]; (jj) < (mxChoice); jj++, pTo++)
							{
								if (((pTo.rCost) > (mxCost)) || (((pTo.rCost) == (mxCost)) && ((pTo.rUnsorted) > (mxUnsorted))))
								{
									mxCost = (short)(pTo.rCost);
									mxUnsorted = (short)(pTo.rUnsorted);
									mxI = (int)(jj);
								}
							}
						}
					}
				}

				pFrom = aTo;
				aTo = aFrom;
				aFrom = pFrom;
				nFrom = (int)(nTo);
			}

			if ((nFrom) == (0))
			{
				sqlite3ErrorMsg(pParse, "no query solution");
				sqlite3DbFreeNN(db, pSpace);
				return (int)(1);
			}

			pFrom = aFrom;
			for (ii = (int)(1); (ii) < (nFrom); ii++)
			{
				if ((pFrom.rCost) > (aFrom[ii].rCost))
					pFrom = aFrom[ii];
			}

			for (iLoop = (int)(0); (iLoop) < (nLoop); iLoop++)
			{
				WhereLevel pLevel = pWInfo.a[iLoop];
				pLevel.pWLoop = pWLoop = pFrom.aLoop[iLoop];
				pLevel.iFrom = (byte)(pWLoop.iTab);
				pLevel.iTabCur = (int)(pWInfo.pTabList.a[pLevel.iFrom].iCursor);
			}

			if (((((pWInfo.wctrlFlags & 0x0100) != 0) && ((pWInfo.wctrlFlags & 0x0080) == (0))) && ((pWInfo.eDistinct) == (0))) && ((nRowEst) != 0))
			{
				ulong notUsed = 0;
				int rc = (int)(wherePathSatisfiesOrderBy(pWInfo, pWInfo.pResultSet, pFrom, (ushort)(0x0080), (ushort)(nLoop - 1), pFrom.aLoop[nLoop - 1], &notUsed));
				if ((rc) == (pWInfo.pResultSet.nExpr))
				{
					pWInfo.eDistinct = (byte)(2);
				}
			}

			pWInfo.bOrderedInnerLoop = (uint)(0);
			if ((pWInfo.pOrderBy) != null)
			{
				if ((pWInfo.wctrlFlags & 0x0080) != 0)
				{
					if ((pFrom.isOrdered) == (pWInfo.pOrderBy.nExpr))
					{
						pWInfo.eDistinct = (byte)(2);
					}
				}
				else
				{
					pWInfo.nOBSat = (sbyte)(pFrom.isOrdered);
					pWInfo.revMask = (ulong)(pFrom.revLoop);
					if ((pWInfo.nOBSat) <= (0))
					{
						pWInfo.nOBSat = (sbyte)(0);
						if ((nLoop) > (0))
						{
							uint wsFlags = (uint)(pFrom.aLoop[nLoop - 1].wsFlags);
							if (((wsFlags & 0x00001000) == (0)) && ((wsFlags & (0x00000100 | 0x00000004)) != (0x00000100 | 0x00000004)))
							{
								ulong m = (ulong)(0);
								int rc = (int)(wherePathSatisfiesOrderBy(pWInfo, pWInfo.pOrderBy, pFrom, (ushort)(0x0800), (ushort)(nLoop - 1), pFrom.aLoop[nLoop - 1], &m));
								if ((rc) == (pWInfo.pOrderBy.nExpr))
								{
									pWInfo.bOrderedInnerLoop = (uint)(1);
									pWInfo.revMask = (ulong)(m);
								}
							}
						}
					}
					else if ((((nLoop) != 0) && ((pWInfo.nOBSat) == (1))) && ((pWInfo.wctrlFlags & (0x0001 | 0x0002)) != 0))
					{
						pWInfo.bOrderedInnerLoop = (uint)(1);
					}
				}

				if ((((pWInfo.wctrlFlags & 0x0200) != 0) && ((pWInfo.nOBSat) == (pWInfo.pOrderBy.nExpr))) && ((nLoop) > (0)))
				{
					ulong revMask = (ulong)(0);
					int nOrder = (int)(wherePathSatisfiesOrderBy(pWInfo, pWInfo.pOrderBy, pFrom, (ushort)(0), (ushort)(nLoop - 1), pFrom.aLoop[nLoop - 1], &revMask));
					if ((nOrder) == (pWInfo.pOrderBy.nExpr))
					{
						pWInfo.sorted = (uint)(1);
						pWInfo.revMask = (ulong)(revMask);
					}
				}
			}

			pWInfo.nRowOut = (short)(pFrom.nRow);
			sqlite3DbFreeNN(db, pSpace);
			return (int)(0);
		}
		public static short whereSortingCost(WhereInfo pWInfo, short nRow, int nOrderBy, int nSorted)
		{
			short rScale = 0; short rSortCost = 0;
			rScale = (short)(sqlite3LogEst((ulong)((nOrderBy - nSorted) * 100 / nOrderBy)) - 66);
			rSortCost = (short)(nRow + rScale + 16);
			if (((pWInfo.wctrlFlags & 0x4000) != 0) && ((pWInfo.iLimit) < (nRow)))
			{
				nRow = (short)(pWInfo.iLimit);
			}
			else if ((pWInfo.wctrlFlags & 0x0100) != 0)
			{
				if ((nRow) > (10))
				{
					nRow -= (short)(10);
				}
			}

			rSortCost += (short)(estLog((short)(nRow)));
			return (short)(rSortCost);
		}
		public static void whereUndoExprMods(WhereInfo pWInfo)
		{
			while ((pWInfo.pExprMods) != null)
			{
				WhereExprMod p = pWInfo.pExprMods;
				pWInfo.pExprMods = p.pNext;
				CRuntime.memcpy(p.pExpr, p.orig, (ulong)(sizeof(Expr)));
				sqlite3DbFree(pWInfo.pParse.db, p);
			}
		}
	}
}
// Generated by Hebron at 4/3/2022 2:10:11 AM

using System;
using System.Runtime.InteropServices;
using Hebron.Runtime;

namespace SqliteSharp
{
	unsafe partial class Sqlite
	{
		public static byte binaryCompareP5(Expr pExpr1, Expr pExpr2, int jumpIfNull)
		{
			byte aff = (byte)(sqlite3ExprAffinity(pExpr2));
			aff = (byte)((byte)(sqlite3CompareAffinity(pExpr1, (sbyte)(aff))) | (byte)(jumpIfNull));
			return (byte)(aff);
		}
		public static sbyte comparisonAffinity(Expr pExpr)
		{
			sbyte aff = 0;
			aff = (sbyte)(sqlite3ExprAffinity(pExpr.pLeft));
			if ((pExpr.pRight) != null)
			{
				aff = (sbyte)(sqlite3CompareAffinity(pExpr.pRight, (sbyte)(aff)));
			}
			else if ((((pExpr).flags & 0x000800) != 0))
			{
				aff = (sbyte)(sqlite3CompareAffinity(pExpr.x.pSelect.pEList.a[0].pExpr, (sbyte)(aff)));
			}
			else if ((aff) == (0))
			{
				aff = (sbyte)(0x41);
			}

			return (sbyte)(aff);
		}
		public static int dupedExprNodeSize(Expr p, int flags)
		{
			int nByte = (int)(dupedExprStructSize(p, (int)(flags)) & 0xfff);
			if ((!(((p).flags & (0x000400)) != 0)) && ((p.u.zToken) != null))
			{
				nByte += (int)((CRuntime.strlen(p.u.zToken) & 0x3fffffff) + 1);
			}

			return (int)(((nByte) + 7) & ~7);
		}
		public static int dupedExprSize(Expr p, int flags)
		{
			int nByte = (int)(0);
			if ((p) != null)
			{
				nByte = (int)(dupedExprNodeSize(p, (int)(flags)));
				if ((flags & 0x0001) != 0)
				{
					nByte += (int)(dupedExprSize(p.pLeft, (int)(flags)) + dupedExprSize(p.pRight, (int)(flags)));
				}
			}

			return (int)(nByte);
		}
		public static int dupedExprStructSize(Expr p, int flags)
		{
			int nSize = 0;
			if ((((0) == (flags)) || ((p.op) == (178))) || (((p).flags & (0x1000000)) != 0))
			{
				nSize = (int)(sizeof(Expr));
			}
			else
			{
				if (((p.pLeft) != null) || ((p.x.pList) != null))
				{
					nSize = (int)(((ulong)(&((0).iTable))) | 0x002000);
				}
				else
				{
					nSize = (int)(((ulong)((0).pLeft)) | 0x004000);
				}
			}

			return (int)(nSize);
		}
		public static int exprIsConst(Expr p, int initFlag, int iCur)
		{
			Walker w = new Walker();
			w.eCode = (ushort)(initFlag);
			w.xExprCallback = exprNodeIsConstant;
			w.xSelectCallback = sqlite3SelectWalkFail;
			w.u.iCur = (int)(iCur);
			sqlite3WalkExpr(w, p);
			return (int)(w.eCode);
		}
		public static int exprIsDeterministic(Expr p)
		{
			Walker w = new Walker();
			CRuntime.memset(w, (int)(0), (ulong)(sizeof(Walker)));
			w.eCode = (ushort)(1);
			w.xExprCallback = exprNodeIsDeterministic;
			w.xSelectCallback = sqlite3SelectWalkFail;
			sqlite3WalkExpr(w, p);
			return (int)(w.eCode);
		}
		public static int exprProbability(Expr p)
		{
			double r = (double)(-1.0);
			if (p.op != 153)
				return (int)(-1);
			sqlite3AtoF(p.u.zToken, &r, (int)(sqlite3Strlen30(p.u.zToken)), (byte)(1));
			if ((r) > (1.0))
				return (int)(-1);
			return (int)(r * 134217728.0);
		}
		public static void exprSetHeight(Expr p)
		{
			int nHeight = (int)(p.pLeft ? p.pLeft.nHeight : 0);
			if (((p.pRight) != null) && ((p.pRight.nHeight) > (nHeight)))
				nHeight = (int)(p.pRight.nHeight);
			if ((((p).flags & 0x000800) != 0))
			{
				heightOfSelect(p.x.pSelect, &nHeight);
			}
			else if ((p.x.pList) != null)
			{
				heightOfExprList(p.x.pList, &nHeight);
				p.flags |= (uint)((0x000100 | 0x200000 | 0x000004) & sqlite3ExprListFlags(p.x.pList));
			}

			p.nHeight = (int)(nHeight + 1);
		}
		public static int exprStructSize(Expr p)
		{
			if ((((p).flags & (0x004000)) != 0))
				return (int)((ulong)((0).pLeft));
			if ((((p).flags & (0x002000)) != 0))
				return (int)((ulong)(&((0).iTable)));
			return (int)(sizeof(Expr));
		}
		public static void exprToRegister(Expr pExpr, int iReg)
		{
			Expr p = sqlite3ExprSkipCollateAndLikely(pExpr);
			if (((p) == (null)))
				return;
			p.op2 = (byte)(p.op);
			p.op = (byte)(176);
			p.iTable = (int)(iReg);
			(p).flags &= (uint)(~(0x001000));
		}
		public static void heightOfExpr(Expr p, int* pnHeight)
		{
			if ((p) != null)
			{
				if ((p.nHeight) > (*pnHeight))
				{
					*pnHeight = (int)(p.nHeight);
				}
			}
		}
		public static void incrAggFunctionDepth(Expr pExpr, int N)
		{
			if ((N) > (0))
			{
				Walker w = new Walker();
				CRuntime.memset(w, (int)(0), (ulong)(sizeof(Walker)));
				w.xExprCallback = incrAggDepth;
				w.u.n = (int)(N);
				sqlite3WalkExpr(w, pExpr);
			}
		}
		public static Select isCandidateForInOpt(Expr pX)
		{
			Select p;
			SrcList pSrc;
			ExprList pEList;
			Table pTab;
			int i = 0;
			if (!(((pX).flags & 0x000800) != 0))
				return null;
			if ((((pX).flags & (0x000020)) != 0))
				return null;
			p = pX.x.pSelect;
			if ((p.pPrior) != null)
				return null;
			if ((p.selFlags & (0x0000001 | 0x0000008)) != 0)
			{
				return null;
			}

			if ((p.pLimit) != null)
				return null;
			if ((p.pWhere) != null)
				return null;
			pSrc = p.pSrc;
			if (pSrc.nSrc != 1)
				return null;
			if ((pSrc.a[0].pSelect) != null)
				return null;
			pTab = pSrc.a[0].pTab;
			if ((((pTab).eTabType) == (1)))
				return null;
			pEList = p.pEList;
			for (i = (int)(0); (i) < (pEList.nExpr); i++)
			{
				Expr pRes = pEList.a[i].pExpr;
				if (pRes.op != 167)
					return null;
			}

			return p;
		}
		public static sbyte sqlite3CompareAffinity(Expr pExpr, sbyte aff2)
		{
			sbyte aff1 = (sbyte)(sqlite3ExprAffinity(pExpr));
			if (((aff1) > (0x40)) && ((aff2) > (0x40)))
			{
				if (((aff1) >= (0x43)) || ((aff2) >= (0x43)))
				{
					return (sbyte)(0x43);
				}
				else
				{
					return (sbyte)(0x41);
				}
			}
			else
			{
				return (sbyte)(((aff1) <= (0x40) ? aff2 : aff1) | 0x40);
			}
		}
		public static void sqlite3DequoteExpr(Expr p)
		{
			p.flags |= (uint)((p.u.zToken[0]) == (34) ? 0x4000000 | 0x000040 : 0x4000000);
			sqlite3Dequote(p.u.zToken);
		}
		public static sbyte sqlite3ExprAffinity(Expr pExpr)
		{
			int op = 0;
			while ((((pExpr).flags & (0x001000 | 0x020000)) != 0))
			{
				pExpr = pExpr.pLeft;
			}

			op = (int)(pExpr.op);
			if ((op) == (176))
				op = (int)(pExpr.op2);
			if (((op) == (167)) || ((op) == (169)))
			{
				if ((pExpr.y.pTab) != null)
				{
					return (sbyte)(sqlite3TableColumnAffinity(pExpr.y.pTab, (int)(pExpr.iColumn)));
				}
			}

			if ((op) == (138))
			{
				return (sbyte)(sqlite3ExprAffinity(pExpr.x.pSelect.pEList.a[0].pExpr));
			}

			if ((op) == (36))
			{
				return (sbyte)(sqlite3AffinityType(pExpr.u.zToken, null));
			}

			if ((op) == (178))
			{
				return (sbyte)(sqlite3ExprAffinity(pExpr.pLeft.x.pSelect.pEList.a[pExpr.iColumn].pExpr));
			}

			if ((op) == (177))
			{
				return (sbyte)(sqlite3ExprAffinity(pExpr.x.pList.a[0].pExpr));
			}

			return (sbyte)(pExpr.affExpr);
		}
		public static int sqlite3ExprCanBeNull(Expr p)
		{
			byte op = 0;
			while (((p.op) == (174)) || ((p.op) == (173)))
			{
				p = p.pLeft;
			}

			op = (byte)(p.op);
			if ((op) == (176))
				op = (byte)(p.op2);
			switch (op)
			{
				case 155:
				case 117:
				case 153:
				case 154:
					return (int)(0);
				case 167:
					;
					return (((((p).flags & (0x100000)) != 0) || ((p.y.pTab) == (null))) || ((((p.iColumn) >= (0)) && (p.y.pTab.aCol != null)) && ((p.y.pTab.aCol[p.iColumn].notNull) == (0))) ? 1 : 0);
				default:
					return (int)(1);
			}
		}
		public static ulong sqlite3ExprColUsed(Expr pExpr)
		{
			int n = 0;
			Table pExTab;
			n = (int)(pExpr.iColumn);
			pExTab = pExpr.y.pTab;
			if (((pExTab.tabFlags & 0x00000060) != 0) && ((pExTab.aCol[n].colFlags & 0x0060) != 0))
			{
				return (ulong)((pExTab.nCol) >= ((int)(sizeof(ulong) * 8)) ? ((ulong)(-1)) : (((ulong)(1)) << (pExTab.nCol)) - 1);
			}
			else
			{
				if ((n) >= ((int)(sizeof(ulong) * 8)))
					n = (int)(((int)(sizeof(ulong) * 8)) - 1);
				return (ulong)(((ulong)(1)) << n);
			}
		}
		public static int sqlite3ExprCompareSkip(Expr pA, Expr pB, int iTab)
		{
			return (int)(sqlite3ExprCompare(null, sqlite3ExprSkipCollateAndLikely(pA), sqlite3ExprSkipCollateAndLikely(pB), (int)(iTab)));
		}
		public static int sqlite3ExprCoveredByIndex(Expr pExpr, int iCur, Index pIdx)
		{
			Walker w = new Walker();
			IdxCover xcov = new IdxCover();
			CRuntime.memset(w, (int)(0), (ulong)(sizeof(Walker)));
			xcov.iCur = (int)(iCur);
			xcov.pIdx = pIdx;
			w.xExprCallback = exprIdxCover;
			w.u.pIdxCover = xcov;
			sqlite3WalkExpr(w, pExpr);
			return (int)(!w.eCode);
		}
		public static int sqlite3ExprIdToTrueFalse(Expr pExpr)
		{
			uint v = 0;
			if ((!(((pExpr).flags & (0x4000000 | 0x000400)) != 0)) && ((v = (uint)(sqlite3IsTrueOrFalse(pExpr.u.zToken))) != 0))
			{
				pExpr.op = (byte)(170);
				(pExpr).flags |= (uint)(v);
				return (int)(1);
			}

			return (int)(0);
		}
		public static int sqlite3ExprImpliesNonNullRow(Expr p, int iTab)
		{
			Walker w = new Walker();
			p = sqlite3ExprSkipCollateAndLikely(p);
			if ((p) == (null))
				return (int)(0);
			if ((p.op) == (51))
			{
				p = p.pLeft;
			}
			else
			{
				while ((p.op) == (44))
				{
					if ((sqlite3ExprImpliesNonNullRow(p.pLeft, (int)(iTab))) != 0)
						return (int)(1);
					p = p.pRight;
				}
			}

			w.xExprCallback = impliesNotNullRow;
			w.xSelectCallback = null;
			w.xSelectCallback2 = null;
			w.eCode = (ushort)(0);
			w.u.iCur = (int)(iTab);
			sqlite3WalkExpr(w, p);
			return (int)(w.eCode);
		}
		public static int sqlite3ExprIsConstant(Expr p)
		{
			return (int)(exprIsConst(p, (int)(1), (int)(0)));
		}
		public static int sqlite3ExprIsConstantNotJoin(Expr p)
		{
			return (int)(exprIsConst(p, (int)(2), (int)(0)));
		}
		public static int sqlite3ExprIsConstantOrFunction(Expr p, byte isInit)
		{
			return (int)(exprIsConst(p, (int)(4 + isInit), (int)(0)));
		}
		public static int sqlite3ExprIsInteger(Expr p, int* pValue)
		{
			int rc = (int)(0);
			if (((p) == (null)))
				return (int)(0);
			if ((p.flags & 0x000400) != 0)
			{
				*pValue = (int)(p.u.iValue);
				return (int)(1);
			}

			switch (p.op)
			{
				case 174:
					{
						rc = (int)(sqlite3ExprIsInteger(p.pLeft, pValue));
						break;
					}

				case 173:
					{
						int v = (int)(0);
						if ((sqlite3ExprIsInteger(p.pLeft, &v)) != 0)
						{
							*pValue = (int)(-v);
							rc = (int)(1);
						}

						break;
					}

				default:
					break;
			}

			return (int)(rc);
		}
		public static int sqlite3ExprIsTableConstant(Expr p, int iCur)
		{
			return (int)(exprIsConst(p, (int)(3), (int)(iCur)));
		}
		public static int sqlite3ExprIsVector(Expr pExpr)
		{
			return ((sqlite3ExprVectorSize(pExpr)) > (1) ? 1 : 0);
		}
		public static int sqlite3ExprNeedsNoAffinityChange(Expr p, sbyte aff)
		{
			byte op = 0;
			int unaryMinus = (int)(0);
			if ((aff) == (0x41))
				return (int)(1);
			while (((p.op) == (174)) || ((p.op) == (173)))
			{
				if ((p.op) == (173))
					unaryMinus = (int)(1);
				p = p.pLeft;
			}

			op = (byte)(p.op);
			if ((op) == (176))
				op = (byte)(p.op2);
			switch (op)
			{
				case 155:
					{
						return ((aff) >= (0x43) ? 1 : 0);
					}

				case 153:
					{
						return ((aff) >= (0x43) ? 1 : 0);
					}

				case 117:
					{
						return ((unaryMinus == 0) && ((aff) == (0x42)) ? 1 : 0);
					}

				case 154:
					{
						return (int)(!unaryMinus);
					}

				case 167:
					{
						return (((aff) >= (0x43)) && ((p.iColumn) < (0)) ? 1 : 0);
					}

				default:
					{
						return (int)(0);
					}
			}
		}
		public static int sqlite3ExprReferencesUpdatedColumn(Expr pExpr, int* aiChng, int chngRowid)
		{
			Walker w = new Walker();
			CRuntime.memset(w, (int)(0), (ulong)(sizeof(Walker)));
			w.eCode = (ushort)(0);
			w.xExprCallback = checkConstraintExprNode;
			w.u.aiCol = aiChng;
			sqlite3WalkExpr(w, pExpr);
			if (chngRowid == 0)
			{
				w.eCode &= (ushort)(~0x02);
			}

			return (int)(w.eCode != 0);
		}
		public static Expr sqlite3ExprSimplifiedAndOr(Expr pExpr)
		{
			if (((pExpr.op) == (44)) || ((pExpr.op) == (43)))
			{
				Expr pRight = sqlite3ExprSimplifiedAndOr(pExpr.pRight);
				Expr pLeft = sqlite3ExprSimplifiedAndOr(pExpr.pLeft);
				if ((((pLeft).flags & (0x000001 | 0x10000000)) == (0x10000000)) || (((pRight).flags & (0x000001 | 0x20000000)) == (0x20000000)))
				{
					pExpr = (pExpr.op) == (44) ? pRight : pLeft;
				}
				else if ((((pRight).flags & (0x000001 | 0x10000000)) == (0x10000000)) || (((pLeft).flags & (0x000001 | 0x20000000)) == (0x20000000)))
				{
					pExpr = (pExpr.op) == (44) ? pLeft : pRight;
				}
			}

			return pExpr;
		}
		public static Expr sqlite3ExprSkipCollate(Expr pExpr)
		{
			while (((pExpr) != null) && (((pExpr).flags & (0x001000)) != 0))
			{
				pExpr = pExpr.pLeft;
			}

			return pExpr;
		}
		public static Expr sqlite3ExprSkipCollateAndLikely(Expr pExpr)
		{
			while (((pExpr) != null) && (((pExpr).flags & (0x001000 | 0x040000)) != 0))
			{
				if ((((pExpr).flags & (0x040000)) != 0))
				{
					pExpr = pExpr.x.pList.a[0].pExpr;
				}
				else
				{
					pExpr = pExpr.pLeft;
				}
			}

			return pExpr;
		}
		public static int sqlite3ExprTruthValue(Expr pExpr)
		{
			pExpr = sqlite3ExprSkipCollate(pExpr);
			return ((pExpr.u.zToken[4]) == (0) ? 1 : 0);
		}
		public static int sqlite3ExprVectorSize(Expr pExpr)
		{
			byte op = (byte)(pExpr.op);
			if ((op) == (176))
				op = (byte)(pExpr.op2);
			if ((op) == (177))
			{
				return (int)(pExpr.x.pList.nExpr);
			}
			else if ((op) == (138))
			{
				return (int)(pExpr.x.pSelect.pEList.nExpr);
			}
			else
			{
				return (int)(1);
			}
		}
		public static int sqlite3IndexAffinityOk(Expr pExpr, sbyte idx_affinity)
		{
			sbyte aff = (sbyte)(comparisonAffinity(pExpr));
			if ((aff) < (0x42))
			{
				return (int)(1);
			}

			if ((aff) == (0x42))
			{
				return ((idx_affinity) == (0x42) ? 1 : 0);
			}

			return (((idx_affinity) >= (0x43)) ? 1 : 0);
		}
		public static int sqlite3InRhsIsConstant(Expr pIn)
		{
			Expr pLHS;
			int res = 0;
			pLHS = pIn.pLeft;
			pIn.pLeft = null;
			res = (int)(sqlite3ExprIsConstant(pIn));
			pIn.pLeft = pLHS;
			return (int)(res);
		}
		public static void sqlite3SetJoinExpr(Expr p, int iTable)
		{
			while ((p) != null)
			{
				(p).flags |= (uint)(0x000001);
				p.w.iRightJoinTable = (int)(iTable);
				if ((p.op) == (172))
				{
					if ((p.x.pList) != null)
					{
						int i = 0;
						for (i = (int)(0); (i) < (p.x.pList.nExpr); i++)
						{
							sqlite3SetJoinExpr(p.x.pList.a[i].pExpr, (int)(iTable));
						}
					}
				}

				sqlite3SetJoinExpr(p.pLeft, (int)(iTable));
				p = p.pRight;
			}
		}
		public static void sqlite3StringToId(Expr p)
		{
			if ((p.op) == (117))
			{
				p.op = (byte)(59);
			}
			else if (((p.op) == (113)) && ((p.pLeft.op) == (117)))
			{
				p.pLeft.op = (byte)(59);
			}
		}
		public static Expr sqlite3VectorFieldSubexpr(Expr pVector, int i)
		{
			if ((sqlite3ExprIsVector(pVector)) != 0)
			{
				if (((pVector.op) == (138)) || ((pVector.op2) == (138)))
				{
					return pVector.x.pSelect.pEList.a[i].pExpr;
				}
				else
				{
					return pVector.x.pList.a[i].pExpr;
				}
			}

			return pVector;
		}
		public static void transferJoinMarkings(Expr pDerived, Expr pBase)
		{
			if ((pDerived) != null)
			{
				pDerived.flags |= (uint)(pBase.flags & 0x000001);
				pDerived.w.iRightJoinTable = (int)(pBase.w.iRightJoinTable);
			}
		}
		public static void unsetJoinExpr(Expr p, int iTable)
		{
			while ((p) != null)
			{
				if ((((p).flags & (0x000001)) != 0) && (((iTable) < (0)) || ((p.w.iRightJoinTable) == (iTable))))
				{
					(p).flags &= (uint)(~(0x000001));
				}

				if (((p.op) == (167)) && ((p.iTable) == (iTable)))
				{
					(p).flags &= (uint)(~(0x100000));
				}

				if ((p.op) == (172))
				{
					if ((p.x.pList) != null)
					{
						int i = 0;
						for (i = (int)(0); (i) < (p.x.pList.nExpr); i++)
						{
							unsetJoinExpr(p.x.pList.a[i].pExpr, (int)(iTable));
						}
					}
				}

				unsetJoinExpr(p.pLeft, (int)(iTable));
				p = p.pRight;
			}
		}
		public static void updateRangeAffinityStr(Expr pRight, int n, sbyte* zAff)
		{
			int i = 0;
			for (i = (int)(0); (i) < (n); i++)
			{
				Expr p = sqlite3VectorFieldSubexpr(pRight, (int)(i));
				if (((sqlite3CompareAffinity(p, (sbyte)(zAff[i]))) == (0x41)) || ((sqlite3ExprNeedsNoAffinityChange(p, (sbyte)(zAff[i]))) != 0))
				{
					zAff[i] = (sbyte)(0x41);
				}
			}
		}
		public static void whereApplyPartialIndexConstraints(Expr pTruth, int iTabCur, WhereClause pWC)
		{
			int i = 0;
			WhereTerm pTerm;
			while ((pTruth.op) == (44))
			{
				whereApplyPartialIndexConstraints(pTruth.pLeft, (int)(iTabCur), pWC);
				pTruth = pTruth.pRight;
			}

			for (i = (int)(0), pTerm = pWC.a; (i) < (pWC.nTerm); i++, pTerm++)
			{
				Expr pExpr;
				if ((pTerm.wtFlags & 0x0004) != 0)
					continue;
				pExpr = pTerm.pExpr;
				if ((sqlite3ExprCompare(null, pExpr, pTruth, (int)(iTabCur))) == (0))
				{
					pTerm.wtFlags |= (ushort)(0x0004);
				}
			}
		}
		public static Expr whereRightSubexprIsColumn(Expr p)
		{
			p = sqlite3ExprSkipCollateAndLikely(p.pRight);
			if (((p != null) && ((p.op) == (167))) && (!(((p).flags & (0x000008)) != 0)))
			{
				return p;
			}

			return null;
		}
	}
}
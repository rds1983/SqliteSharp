// Generated by Hebron at 4/3/2022 2:10:11 AM

using System;
using System.Runtime.InteropServices;
using Hebron.Runtime;

namespace SqliteSharp
{
	unsafe partial class Sqlite
	{
		public static void constInsert(WhereConst pConst, Expr pColumn, Expr pValue, Expr pExpr)
		{
			int i = 0;
			if ((((pColumn).flags & (0x000008)) != 0))
				return;
			if (sqlite3ExprAffinity(pValue) != 0)
				return;
			if (sqlite3IsBinary(sqlite3ExprCompareCollSeq(pConst.pParse, pExpr)) == 0)
			{
				return;
			}

			for (i = (int)(0); (i) < (pConst.nConst); i++)
			{
				Expr pE2 = pConst.apExpr[i * 2];
				if (((pE2.iTable) == (pColumn.iTable)) && ((pE2.iColumn) == (pColumn.iColumn)))
				{
					return;
				}
			}

			if ((sqlite3ExprAffinity(pColumn)) == (0x41))
			{
				pConst.bHasAffBlob = (int)(1);
			}

			pConst.nConst++;
			pConst.apExpr = sqlite3DbReallocOrFree(pConst.pParse.db, pConst.apExpr, (ulong)(pConst.nConst * 2 * sizeof(Expr)));
			if ((pConst.apExpr) == (null))
			{
				pConst.nConst = (int)(0);
			}
			else
			{
				pConst.apExpr[pConst.nConst * 2 - 2] = pColumn;
				pConst.apExpr[pConst.nConst * 2 - 1] = pValue;
			}
		}
		public static void findConstInWhere(WhereConst pConst, Expr pExpr)
		{
			Expr pRight; Expr pLeft;
			if (((pExpr) == (null)))
				return;
			if ((((pExpr).flags & (0x000001)) != 0))
				return;
			if ((pExpr.op) == (44))
			{
				findConstInWhere(pConst, pExpr.pRight);
				findConstInWhere(pConst, pExpr.pLeft);
				return;
			}

			if (pExpr.op != 53)
				return;
			pRight = pExpr.pRight;
			pLeft = pExpr.pLeft;
			if (((pRight.op) == (167)) && ((sqlite3ExprIsConstant(pLeft)) != 0))
			{
				constInsert(pConst, pRight, pLeft, pExpr);
			}

			if (((pLeft.op) == (167)) && ((sqlite3ExprIsConstant(pRight)) != 0))
			{
				constInsert(pConst, pLeft, pRight, pExpr);
			}
		}
		public static int propagateConstantExprRewriteOne(WhereConst pConst, Expr pExpr, int bIgnoreAffBlob)
		{
			int i = 0;
			if ((pConst.pOomFault[0]) != 0)
				return (int)(1);
			if (pExpr.op != 167)
				return (int)(0);
			if ((((pExpr).flags & (0x000008 | 0x000001)) != 0))
			{
				return (int)(0);
			}

			for (i = (int)(0); (i) < (pConst.nConst); i++)
			{
				Expr pColumn = pConst.apExpr[i * 2];
				if ((pColumn) == (pExpr))
					continue;
				if (pColumn.iTable != pExpr.iTable)
					continue;
				if (pColumn.iColumn != pExpr.iColumn)
					continue;
				if (((bIgnoreAffBlob) != 0) && ((sqlite3ExprAffinity(pColumn)) == (0x41)))
				{
					break;
				}

				pConst.nChng++;
				(pExpr).flags &= (uint)(~(0x800000));
				(pExpr).flags |= (uint)(0x000008);
				pExpr.pLeft = sqlite3ExprDup(pConst.pParse.db, pConst.apExpr[i * 2 + 1], (int)(0));
				if ((pConst.pParse.db.mallocFailed) != 0)
					return (int)(1);
				break;
			}

			return (int)(1);
		}
	}
}
// Generated by Hebron at 4/3/2022 2:10:11 AM

using System;
using System.Runtime.InteropServices;
using Hebron.Runtime;

namespace SqliteSharp
{
	unsafe partial class Sqlite
	{
		public static int sqlite3FixExpr(DbFixer pFix, Expr pExpr)
		{
			return (int)(sqlite3WalkExpr(pFix.w, pExpr));
		}
		public static void sqlite3FixInit(DbFixer pFix, Parse pParse, int iDb, sbyte* zType, Token* pName)
		{
			sqlite3 db = pParse.db;
			pFix.pParse = pParse;
			pFix.zDb = db.aDb[iDb].zDbSName;
			pFix.pSchema = db.aDb[iDb].pSchema;
			pFix.zType = zType;
			pFix.pName = pName;
			pFix.bTemp = (byte)((iDb) == (1));
			pFix.w.pParse = pParse;
			pFix.w.xExprCallback = fixExprCb;
			pFix.w.xSelectCallback = fixSelectCb;
			pFix.w.xSelectCallback2 = sqlite3WalkWinDefnDummyCallback;
			pFix.w.walkerDepth = (int)(0);
			pFix.w.eCode = (ushort)(0);
			pFix.w.u.pFix = pFix;
		}
		public static int sqlite3FixSelect(DbFixer pFix, Select pSelect)
		{
			return (int)(sqlite3WalkSelect(pFix.w, pSelect));
		}
		public static int sqlite3FixSrcList(DbFixer pFix, SrcList pList)
		{
			int res = (int)(0);
			if ((pList) != null)
			{
				Select s = new Select();
				CRuntime.memset(s, (int)(0), (ulong)(sizeof(Select)));
				s.pSrc = pList;
				res = (int)(sqlite3WalkSelect(pFix.w, s));
			}

			return (int)(res);
		}
		public static int sqlite3FixTriggerStep(DbFixer pFix, TriggerStep pStep)
		{
			while ((pStep) != null)
			{
				if (((((sqlite3WalkSelect(pFix.w, pStep.pSelect)) != 0) || ((sqlite3WalkExpr(pFix.w, pStep.pWhere)) != 0)) || ((sqlite3WalkExprList(pFix.w, pStep.pExprList)) != 0)) || ((sqlite3FixSrcList(pFix, pStep.pFrom)) != 0))
				{
					return (int)(1);
				}

				{
					Upsert pUp;
					for (pUp = pStep.pUpsert; pUp; pUp = pUp.pNextUpsert)
					{
						if (((((sqlite3WalkExprList(pFix.w, pUp.pUpsertTarget)) != 0) || ((sqlite3WalkExpr(pFix.w, pUp.pUpsertTargetWhere)) != 0)) || ((sqlite3WalkExprList(pFix.w, pUp.pUpsertSet)) != 0)) || ((sqlite3WalkExpr(pFix.w, pUp.pUpsertWhere)) != 0))
						{
							return (int)(1);
						}
					}
				}

				pStep = pStep.pNext;
			}

			return (int)(0);
		}
	}
}
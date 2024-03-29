// Generated by Hebron at 4/3/2022 2:10:11 AM

using System;
using System.Runtime.InteropServices;
using Hebron.Runtime;

namespace SqliteSharp
{
	unsafe partial class Sqlite
	{
		public static int jsonEachClose(sqlite3_vtab_cursor cur)
		{
			JsonEachCursor p = (JsonEachCursor)(cur);
			jsonEachCursorReset(p);
			sqlite3_free(cur);
			return (int)(0);
		}
		public static int jsonEachColumn(sqlite3_vtab_cursor cur, sqlite3_context ctx, int i)
		{
			JsonEachCursor p = (JsonEachCursor)(cur);
			JsonNode* pThis = &p.sParse.aNode[p.i];
			switch (i)
			{
				case 0:
					{
						if ((p.i) == (0))
							break;
						if ((p.eType) == (7))
						{
							jsonReturn(pThis, ctx, null);
						}
						else if ((p.eType) == (6))
						{
							uint iKey = 0;
							if ((p.bRecursive) != 0)
							{
								if ((p.iRowid) == (0))
									break;
								iKey = (uint)(p.sParse.aNode[p.sParse.aUp[p.i]].u.iKey);
							}
							else
							{
								iKey = (uint)(p.iRowid);
							}

							sqlite3_result_int64(ctx, (long)(iKey));
						}

						break;
					}

				case 1:
					{
						if ((pThis->jnFlags & 0x40) != 0)
							pThis++;
						jsonReturn(pThis, ctx, null);
						break;
					}

				case 2:
					{
						if ((pThis->jnFlags & 0x40) != 0)
							pThis++;
						sqlite3_result_text(ctx, jsonType[pThis->eType], (int)(-1), null);
						break;
					}

				case 3:
					{
						if ((pThis->jnFlags & 0x40) != 0)
							pThis++;
						if ((pThis->eType) >= (6))
							break;
						jsonReturn(pThis, ctx, null);
						break;
					}

				case 4:
					{
						sqlite3_result_int64(ctx, (long)((long)(p.i) + ((pThis->jnFlags & 0x40) != 0)));
						break;
					}

				case 5:
					{
						if (((p.i) > (p.iBegin)) && ((p.bRecursive) != 0))
						{
							sqlite3_result_int64(ctx, (long)(p.sParse.aUp[p.i]));
						}

						break;
					}

				case 6:
					{
						JsonString x = new JsonString();
						jsonInit(x, ctx);
						if ((p.bRecursive) != 0)
						{
							jsonEachComputePath(p, x, (uint)(p.i));
						}
						else
						{
							if ((p.zRoot) != null)
							{
								jsonAppendRaw(x, p.zRoot, (uint)((int)(CRuntime.strlen(p.zRoot))));
							}
							else
							{
								jsonAppendChar(x, (sbyte)(36));
							}

							if ((p.eType) == (6))
							{
								jsonPrintf((int)(30), x, "[%d]", (uint)(p.iRowid));
							}
							else if ((p.eType) == (7))
							{
								jsonPrintf((int)(pThis->n), x, ".%.*s", (uint)(pThis->n - 2), pThis->u.zJContent + 1);
							}
						}

						jsonResult(x);
						break;
					}

				case 7:
					{
						if ((p.bRecursive) != 0)
						{
							JsonString x = new JsonString();
							jsonInit(x, ctx);
							jsonEachComputePath(p, x, (uint)(p.sParse.aUp[p.i]));
							jsonResult(x);
							break;
						}
					}

				default:
					{
						sbyte* zRoot = p.zRoot;
						if ((zRoot) == (null))
							zRoot = "$";
						sqlite3_result_text(ctx, zRoot, (int)(-1), null);
						break;
					}

				case 8:
					{
						sqlite3_result_text(ctx, p.sParse.zJson, (int)(-1), null);
						break;
					}
			}

			return (int)(0);
		}
		public static int jsonEachEof(sqlite3_vtab_cursor cur)
		{
			JsonEachCursor p = (JsonEachCursor)(cur);
			return ((p.i) >= (p.iEnd) ? 1 : 0);
		}
		public static int jsonEachFilter(sqlite3_vtab_cursor cur, int idxNum, sbyte* idxStr, int argc, sqlite3_value argv)
		{
			JsonEachCursor p = (JsonEachCursor)(cur);
			sbyte* z;
			sbyte* zRoot = null;
			long n = 0;
			jsonEachCursorReset(p);
			if ((idxNum) == (0))
				return (int)(0);
			z = (sbyte*)(sqlite3_value_text(argv[0]));
			if ((z) == (null))
				return (int)(0);
			n = (long)(sqlite3_value_bytes(argv[0]));
			p.zJson = sqlite3_malloc64((ulong)(n + 1));
			if ((p.zJson) == (null))
				return (int)(7);
			CRuntime.memcpy(p.zJson, z, (ulong)((ulong)(n) + 1));
			if ((jsonParse(&p.sParse, null, p.zJson)) != 0)
			{
				int rc = (int)(7);
				if ((p.sParse.oom) == (0))
				{
					sqlite3_free(cur.pVtab.zErrMsg);
					cur.pVtab.zErrMsg = sqlite3_mprintf("malformed JSON");
					if ((cur.pVtab.zErrMsg) != null)
						rc = (int)(1);
				}

				jsonEachCursorReset(p);
				return (int)(rc);
			}
			else if (((p.bRecursive) != 0) && ((jsonParseFindParents(&p.sParse)) != 0))
			{
				jsonEachCursorReset(p);
				return (int)(7);
			}
			else
			{
				JsonNode* pNode = null;
				if ((idxNum) == (3))
				{
					sbyte* zErr = null;
					zRoot = (sbyte*)(sqlite3_value_text(argv[1]));
					if ((zRoot) == (null))
						return (int)(0);
					n = (long)(sqlite3_value_bytes(argv[1]));
					p.zRoot = sqlite3_malloc64((ulong)(n + 1));
					if ((p.zRoot) == (null))
						return (int)(7);
					CRuntime.memcpy(p.zRoot, zRoot, (ulong)((ulong)(n) + 1));
					if (zRoot[0] != 36)
					{
						zErr = zRoot;
					}
					else
					{
						pNode = jsonLookupStep(&p.sParse, (uint)(0), p.zRoot + 1, null, &zErr);
					}

					if ((zErr) != null)
					{
						sqlite3_free(cur.pVtab.zErrMsg);
						cur.pVtab.zErrMsg = jsonPathSyntaxError(zErr);
						jsonEachCursorReset(p);
						return (int)((cur.pVtab.zErrMsg) != 0 ? 1 : 7);
					}
					else if ((pNode) == (null))
					{
						return (int)(0);
					}
				}
				else
				{
					pNode = p.sParse.aNode;
				}

				p.iBegin = (uint)(p.i = (uint)((int)(pNode - p.sParse.aNode)));
				p.eType = (byte)(pNode->eType);
				if ((p.eType) >= (6))
				{
					pNode->u.iKey = (uint)(0);
					p.iEnd = (uint)(p.i + pNode->n + 1);
					if ((p.bRecursive) != 0)
					{
						p.eType = (byte)(p.sParse.aNode[p.sParse.aUp[p.i]].eType);
						if (((p.i) > (0)) && ((p.sParse.aNode[p.i - 1].jnFlags & 0x40) != 0))
						{
							p.i--;
						}
					}
					else
					{
						p.i++;
					}
				}
				else
				{
					p.iEnd = (uint)(p.i + 1);
				}
			}

			return (int)(0);
		}
		public static int jsonEachNext(sqlite3_vtab_cursor cur)
		{
			JsonEachCursor p = (JsonEachCursor)(cur);
			if ((p.bRecursive) != 0)
			{
				if ((p.sParse.aNode[p.i].jnFlags & 0x40) != 0)
					p.i++;
				p.i++;
				p.iRowid++;
				if ((p.i) < (p.iEnd))
				{
					uint iUp = (uint)(p.sParse.aUp[p.i]);
					JsonNode* pUp = &p.sParse.aNode[iUp];
					p.eType = (byte)(pUp->eType);
					if ((pUp->eType) == (6))
					{
						if ((iUp) == (p.i - 1))
						{
							pUp->u.iKey = (uint)(0);
						}
						else
						{
							pUp->u.iKey++;
						}
					}
				}
			}
			else
			{
				switch (p.eType)
				{
					case 6:
						{
							p.i += (uint)(jsonNodeSize(&p.sParse.aNode[p.i]));
							p.iRowid++;
							break;
						}

					case 7:
						{
							p.i += (uint)(1 + jsonNodeSize(&p.sParse.aNode[p.i + 1]));
							p.iRowid++;
							break;
						}

					default:
						{
							p.i = (uint)(p.iEnd);
							break;
						}
				}
			}

			return (int)(0);
		}
		public static int jsonEachRowid(sqlite3_vtab_cursor cur, long* pRowid)
		{
			JsonEachCursor p = (JsonEachCursor)(cur);
			*pRowid = (long)(p.iRowid);
			return (int)(0);
		}
		public static int pragmaVtabClose(sqlite3_vtab_cursor cur)
		{
			PragmaVtabCursor pCsr = (PragmaVtabCursor)(cur);
			pragmaVtabCursorClear(pCsr);
			sqlite3_free(pCsr);
			return (int)(0);
		}
		public static int pragmaVtabColumn(sqlite3_vtab_cursor pVtabCursor, sqlite3_context ctx, int i)
		{
			PragmaVtabCursor pCsr = (PragmaVtabCursor)(pVtabCursor);
			PragmaVtab pTab = (PragmaVtab)(pVtabCursor.pVtab);
			if ((i) < (pTab.iHidden))
			{
				sqlite3_result_value(ctx, sqlite3_column_value(pCsr.pPragma, (int)(i)));
			}
			else
			{
				sqlite3_result_text(ctx, pCsr.azArg[i - pTab.iHidden], (int)(-1), ((Void(Void * ))(-1)));
			}

			return (int)(0);
		}
		public static int pragmaVtabEof(sqlite3_vtab_cursor pVtabCursor)
		{
			PragmaVtabCursor pCsr = (PragmaVtabCursor)(pVtabCursor);
			return (((pCsr.pPragma) == (null)) ? 1 : 0);
		}
		public static int pragmaVtabFilter(sqlite3_vtab_cursor pVtabCursor, int idxNum, sbyte* idxStr, int argc, sqlite3_value argv)
		{
			PragmaVtabCursor pCsr = (PragmaVtabCursor)(pVtabCursor);
			PragmaVtab pTab = (PragmaVtab)(pVtabCursor.pVtab);
			int rc = 0;
			int i = 0; int j = 0;
			sqlite3_str acc = new sqlite3_str();
			sbyte* zSql;
			pragmaVtabCursorClear(pCsr);
			j = (int)((pTab.pName->mPragFlg & 0x20) != 0 ? 0 : 1);
			for (i = (int)(0); (i) < (argc); i++, j++)
			{
				sbyte* zText = (sbyte*)(sqlite3_value_text(argv[i]));
				if ((zText) != null)
				{
					pCsr.azArg[j] = sqlite3_mprintf("%s", zText);
					if ((pCsr.azArg[j]) == (null))
					{
						return (int)(7);
					}
				}
			}

			sqlite3StrAccumInit(acc, null, null, (int)(0), (int)(pTab.db.aLimit[1]));
			sqlite3_str_appendall(acc, "PRAGMA ");
			if ((pCsr.azArg[1]) != null)
			{
				sqlite3_str_appendf(acc, "%Q.", pCsr.azArg[1]);
			}

			sqlite3_str_appendall(acc, pTab.pName->zName);
			if ((pCsr.azArg[0]) != null)
			{
				sqlite3_str_appendf(acc, "=%Q", pCsr.azArg[0]);
			}

			zSql = sqlite3StrAccumFinish(acc);
			if ((zSql) == (null))
				return (int)(7);
			rc = (int)(sqlite3_prepare_v2(pTab.db, zSql, (int)(-1), &pCsr.pPragma, null));
			sqlite3_free(zSql);
			if (rc != 0)
			{
				pTab._base_.zErrMsg = sqlite3_mprintf("%s", sqlite3_errmsg(pTab.db));
				return (int)(rc);
			}

			return (int)(pragmaVtabNext(pVtabCursor));
		}
		public static int pragmaVtabNext(sqlite3_vtab_cursor pVtabCursor)
		{
			PragmaVtabCursor pCsr = (PragmaVtabCursor)(pVtabCursor);
			int rc = (int)(0);
			pCsr.iRowid++;
			if (100 != sqlite3_step(pCsr.pPragma))
			{
				rc = (int)(sqlite3_finalize(pCsr.pPragma));
				pCsr.pPragma = null;
				pragmaVtabCursorClear(pCsr);
			}

			return (int)(rc);
		}
		public static int pragmaVtabRowid(sqlite3_vtab_cursor pVtabCursor, long* p)
		{
			PragmaVtabCursor pCsr = (PragmaVtabCursor)(pVtabCursor);
			*p = (long)(pCsr.iRowid);
			return (int)(0);
		}
	}
}
//******************************************************************************************//
// Copyright 2023,	Patrice Charbonneau														//
//					a.k.a. Sigma3Wolf														//
//					Iid: [80a11b77a52aa9eeed80c9d37dcb7878519289d40beeddb40bb23a60d2711963]	//
//					All rights reserved.													//
//																							//
// This source code is licensed under the [BSD 3-Clause "New" or "Revised" License]			//
// found in the LICENSE file in the root directory of this source tree.						//
//******************************************************************************************//
//**      If you modify this file, you MUST rename it to exclude the version number.      **//
//******************************************************************************************//

//v1.01 - 2019-12-25:	Initial release;
//						BREAKING CODE - Mid() function was working by Index (C#) Instead of Position (Vb6);
//						Fix order comparaison in Left() and Right() to evaluate IsNullOrEmpty() first;
//v1.02 - 2019-12-29:	Add ExtractXmlValue();
//v1.03 - 2020-01-02:	add NewGuidEx();
//v1.04	- 2020-05-13:	Add internal modifier;
//v1.05 - 2020-05-14:	Add ToEx internal class;
//v1.06 - 2020-05-17:	Add ToBytes();
//v1.07 - 2020-05-17:	.ToInt32 now support HEX string;
//v1.08 - 2020-08-28:	Fix a bug in ToDateTime() when getting Time position;
//						ren ToDateStr() to ToSqlString() for less confusion;
//						ren ToDateTime() to SqlStringToDateTime() for less confusion;
//						2019-12-25 change to MID function, broke the SqlStringToDateTime() function using Mid() - FIXED;
//v1.09 - 2020-08-29:	add new LEFT/RIGHT function to make Length parameter optional;
//v1.10 - 2021-02-28:	add new HexEncode(), HexDecode(); update var name in Base64Encode(), Base64Decode();
//v1.11 - 2021-03-05:	cosmetic: fix some unnecessary declaration;
//v1.12 - 2021-03-27:	Fix an issue with InstrRev() startposition where -1 were not taking into account;
//v1.13 - 2021-05-09:	Use of PrototypeOmega namespace; simplify NewGuidEx(); add ExtractPath();
//v1.14 - 2021-09-10:	Use of inline declaration;
//v1.15 - 2021-09-22:	Add PascalCase() function;
//v1.16 - 2021-09-26:	remove secondary namespace [vcxStringEx];
//v1.17 - 2022-01-03:	Adapt to FrameNetCore6;
//v2.00 - 2022-01-03:	Adapt to FrameNetCore6;
//v2.01 - 2022-02-19:	Add ToEx.ToPhone;
//v2.02 - 2023-06-18:	Compatible with Vs2019;
//v2.03 - 2023-08-26:	Move Base64 function to another module;
//v2.04 - 2023-09-24:	fix Mid function (was returning value for index 1 when asked for index 0)
//v2.05 - 2023-10-01:	add RemoveAccents(); change Copyright style header;
//v3.00 - 2023-10-15:	change public to internal; Add AttemptSetTextClipBoard(); Add InternationalId;
//						AppEx dependant;
//v3.01 - 2023-11-03:	removed AttemptSetTextClipBoard() because Clipboard() is already handled in vcxAppEx_v3.01;
//v3.02 - 2024-01-22:	Change Mid function. StartPos is now always valid and default to 0;
//v3.03 - 2024-08-13:	mainly cosmetic, minor logic fix; remove internal modifier for Web;

//Variable declaration
using System;
using System.Text;
using System.Globalization;

namespace PrototypeOmega {
	public static class StringEx {
		internal static string RemoveAccents(this string text) {
			StringBuilder sbReturn = new StringBuilder();
			var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
			foreach (char letter in arrayText) {
				if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
					sbReturn.Append(letter);
			}
			return sbReturn.ToString();
		}

		public static string Left(string pstrString, int plngLength) {
			string strRet = "";

			if (!string.IsNullOrEmpty(pstrString)) {
				if (plngLength > 0) {
					if (pstrString.Length <= plngLength) {
						strRet = pstrString;
					} else {
						strRet = pstrString.Substring(0, plngLength);
					}
				}
			}

			return strRet;
		} //Left

		public static bool Left(string pstrString, string pstrLeft, bool pblnCaseInsensitive = false) {
			bool blnRetValue = false;

			if (!string.IsNullOrEmpty(pstrString)) {
				string strLeft = pstrLeft;
				int lngLength = strLeft.Length;
				if (lngLength > 0) {
					if (pstrString.Length >= lngLength) {

						string strExtract = pstrString.Substring(0, lngLength);
						if (pblnCaseInsensitive == true) {
							strLeft = strLeft.ToLower();
							strExtract = strExtract.ToLower();
						}

						if (strExtract == strLeft) {
							blnRetValue = true;
						}
					}
				}
			}

			return blnRetValue;
		} //Left

		public static string Right(string pstrString, int plngLength) {
			string strRet = "";

			if (!string.IsNullOrEmpty(pstrString)) {
				if (plngLength > 0) {
					if (pstrString.Length <= plngLength) {
						strRet = pstrString;
					} else {
						strRet = pstrString.Substring(pstrString.Length - plngLength, plngLength);
					}
				}
			}

			return strRet;
		} //Right

		public static bool Right(string pstrString, string pstrRight, bool pblnCaseInsensitive = false) {
			bool blnRetValue = false;

			if (!string.IsNullOrEmpty(pstrString)) {
				string strRight = pstrRight;
				int lngLength = strRight.Length;
				if (lngLength > 0) {
					if (pstrString.Length >= lngLength) {
						string strExtract = pstrString.Substring(pstrString.Length - lngLength, lngLength);
						if (pblnCaseInsensitive == true) {
							strRight = strRight.ToLower();
							strExtract = strExtract.ToLower();
						}

						if (strExtract == strRight) {
							blnRetValue = true;
						}
					}
				}
			}

			return blnRetValue;
		} //Right

		public static string Mid(string pstrString, int plngStartPos, int plngLength = -1) {
			string strRet = "";

			if (!string.IsNullOrEmpty(pstrString)) {
				if (plngStartPos <= pstrString.Length) {
					//from here, we work with index instead of position (C# convention)
					int lngStartIndex = IntNotBelow(plngStartPos, 1) - 1;

					if (lngStartIndex < pstrString.Length) {
						int lngMaxLength = LengthFromIndex(pstrString, lngStartIndex);
						int lngLength;

						if (plngLength < 0) {
							//return all available caracter
							lngLength = lngMaxLength;
						} else {
							//return available caracter up to lngMaxLength
							lngLength = IntNotHigher(plngLength, lngMaxLength);
						}

						if (lngLength > 0) {
							strRet = pstrString.Substring(lngStartIndex, lngLength);
						}
					}
				}
			}

			return strRet;
		} //Mid

		public static int Instr(string pstrString, string pstrChar, int plngStart = 1) {
			int lngRet = -1;

			if (pstrChar.Length > 0) {
				if (plngStart < 1) {
					plngStart = 1;
				}

				if (plngStart <= pstrString.Length) {
					lngRet = pstrString.IndexOf(pstrChar, plngStart - 1);
					if (lngRet != -1) {
						lngRet = (lngRet + 1);
					}
				}
			}

			return lngRet;
		} //Instr

		public static int InstrRev(string pstrString, string pstrChar, int plngStart = -1) {
			int lngRet = -1;

			if (plngStart < -1) {
				plngStart = -1;
			}

			if (pstrChar.Length > 0) {
				if ((plngStart > pstrString.Length) || (plngStart == -1)) {
					plngStart = pstrString.Length;
				}

				if (plngStart >= 1) {
					int lngIndex = pstrString.LastIndexOf(pstrChar, plngStart - 1);
					if (lngIndex > -1) {
						lngRet = (lngIndex + 1);
					}
				}
			}

			return lngRet;
		} //InstrRev

		public static string PascalCase(string pstrData) {
			string strRet = StringEx.Left(pstrData, 1).ToUpper() + StringEx.Mid(pstrData, 2).ToLower();

			return strRet;
		}

		//Extract a path from full filename
		public static string ExtractPath(string pstrPath) {
			string strRet = pstrPath;

			int lngPos = InstrRev(pstrPath, @"\");
			if (lngPos > 0) {
				strRet = Left(pstrPath, (lngPos - 1));
			}

			return strRet;
		}

		//Extract the String content of the first node provided
		public static string ExtractXmlValue(string pxmlData, string pstrNodeName) {
			string strRet = "";
			int lngInstr;

			lngInstr = Instr(pxmlData, "<" + pstrNodeName + ">");
			if (lngInstr == 0) {
				lngInstr = Instr(pxmlData, "<" + pstrNodeName + " ");
			}

			if (lngInstr != 0) {
				int lngInstr2;
				lngInstr2 = Instr(pxmlData, "</" + pstrNodeName + ">");
				if (lngInstr2 != 0) {
					//Ok Find Begin of String
					lngInstr = Instr(pxmlData, ">", lngInstr + 1) + 1;
					strRet = Mid(pxmlData, lngInstr, lngInstr2 - lngInstr);
				}
			}

			//using System.Xml.Linq;
			//XDocument xmlDoc = XDocument.Parse(typPostResult.mstrRetData);
			////XElement xmlDoc = XElement.Parse(typPostResult.mstrRetData);
			//XElement xmlSubmission = xmlDoc.Element("mt-submission-response");
			//XElement xmlMessages = xmlSubmission.Element("messages");
			//XElement xmlMessage = xmlMessages.Element("message");
			//XElement xmlStatus = xmlMessage.Element("status");
			return strRet;
		}

		/* *************************************************************************************************** */
		private static int LengthFromIndex(string pstrString, int plngIndex) {
			int lngRet = 0;

			if (pstrString != null) {
				if (pstrString.Length > 0) {
					int lngIndex = IntNotBelow(plngIndex, 0);
					int lngLength = pstrString.Length;

					if (lngIndex < lngLength) {
						lngRet = (lngLength - lngIndex);
					}
				}
			}

			return lngRet;
		} //LengthFromIndex

		//Return value not under plngMinValue
		private static int IntNotBelow(int plngValue, int plngMinValue) {
			int lngRet = plngValue;

			if (lngRet < plngMinValue) {
				lngRet = plngMinValue;
			}

			return lngRet;
		} //intNotBelow

		//Return value not over plngMaxValue
		private static int IntNotHigher(int plngValue, int plngMaxValue) {
			int lngRet = plngValue;

			if (lngRet > plngMaxValue) {
				lngRet = plngMaxValue;
			}

			return lngRet;
		} //intNotHigher
		/* *************************************************************************************************** */

		public static string NewGuidEx() {
			string strRet;
			Guid objGuid = Guid.NewGuid();
			//string strGuid = objGuid.ToString();
			//strRet = StringEx.Left(strGuid, 8);
			//strRet += StringEx.Mid(strGuid, 10, 4);
			//strRet += StringEx.Mid(strGuid, 15, 4);
			//strRet += StringEx.Mid(strGuid, 20, 4);
			//strRet += StringEx.Mid(strGuid, 25);

			strRet = objGuid.ToString("N");

			return strRet;
		}

		public static string HexEncode(string pstrPlainText) {
			string strRet;
			StringBuilder sb = new StringBuilder();

			byte[] bytes = Encoding.Unicode.GetBytes(pstrPlainText);
			foreach (byte t in bytes) {
				sb.Append(t.ToString("X2"));
			}
			strRet = sb.ToString();

			return strRet; // returns: "48656C6C6F20776F726C64" for "Hello world"
		}

		public static string HexDecode(string pstrHexData) {
			string strRet;
			byte[] bytes = new byte[pstrHexData.Length / 2];
			for (int i = 0; i < bytes.Length; i++) {
				bytes[i] = Convert.ToByte(pstrHexData.Substring(i * 2, 2), 16);
			}
			strRet = Encoding.Unicode.GetString(bytes);

			return strRet;
		}
	} //StringEx

	public static class ToEx {
		public static byte[] ToBytes(this string pstrData) {
			byte[] bytBytes;

			//byte[] bytes = Encoding.ASCII.GetBytes(pstrData);
			//string strData = Encoding.ASCII.GetString(bytes);
			bytBytes = Encoding.ASCII.GetBytes(pstrData);

			return bytBytes;
		}

		//Convert String to Int32 or null if fail
		public static int ToInt32(this string pstrInt32) {
			int lngRet = -1;

			try {
				lngRet = int.Parse(pstrInt32);
			} catch {
			}

			return lngRet;
		}

		public static int HexToInt32(this string pstrInt32) {
			int lngRet = -1;
			try {
				lngRet = int.Parse(pstrInt32, System.Globalization.NumberStyles.HexNumber);
			} catch {
			}

			return lngRet;
		}

		//String To Boolean
		public static bool ToBoolean(this string pstrValue) {
			if (bool.TryParse(pstrValue, out bool blnRetValue) == false) {
				blnRetValue = false;
			}
			return blnRetValue;
		}

		//SqlString "mm/dd/yyyy Hh:Nn:Ss" to DateTime
		public static DateTime SqlStringToDateTime(this string pstrSqlDate) {
			DateTime datRetValue = new DateTime();

			string strTextDate = pstrSqlDate + "";
			string strDate;
			string strTime;

			bool blnSuccess;
			string strTmp;
			int intYear = 0;
			int intDay = 0;
			int intMinute = 0;
			int intSecond = 0;

			int lngInstr = StringEx.Instr(strTextDate, " ");
			if (lngInstr != -1) {
				strDate = StringEx.Left(strTextDate, lngInstr - 1);
				strTime = StringEx.Mid(strTextDate, lngInstr + 1);
			} else {
				strDate = strTextDate;
				strTime = "00:00:00";
			}

			//Reconstruct Date
			blnSuccess = false;
			strTmp = StringEx.Left(strDate, 2);
			if (int.TryParse(strTmp, out int intMonth)) {
				strTmp = StringEx.Mid(strDate, 4, 2);
				if (int.TryParse(strTmp, out intDay)) {
					strTmp = StringEx.Right(strDate, 4);
					if (int.TryParse(strTmp, out intYear)) {
						blnSuccess = true;
					}
				}
			}

			if (blnSuccess == true) {
				//Reconstruct Time
				blnSuccess = false;
				strTmp = StringEx.Left(strTime, 2);
				if (int.TryParse(strTmp, out int intHour)) {
					strTmp = StringEx.Mid(strTime, 4, 2);
					if (int.TryParse(strTmp, out intMinute)) {
						strTmp = StringEx.Right(strTime, 2);
						if (int.TryParse(strTmp, out intSecond)) {
							blnSuccess = true;
						}
					}
				}

				if (blnSuccess == true) {
					datRetValue = new DateTime(intYear, intMonth, intDay, intHour, intMinute, intSecond);
				} else {
					//Parse only Date
					//AppEx.Debug.Print("ERROR PARSING DATE 2");
					datRetValue = new DateTime(intYear, intMonth, intDay);
				}
			} else {
				//AppEx.Debug.Print("ERROR PARSING DATE 1");
			}

			return datRetValue;
		} //DateFromText

		//DateTime to SqlString "mm/dd/yyyy Hh:Nn:Ss"
		public static string ToSqlString(this DateTime pdatSql) {
			string strRet = "";
			string strTmp;

			int lngYear = pdatSql.Year;
			int lngMonth = pdatSql.Month;
			int lngDay = pdatSql.Day;

			// mm/dd/yyyy Hh:Nn:Ss
			strTmp = "0" + lngMonth.ToString();
			strRet = (strRet + StringEx.Right(strTmp, 2)) + "/";

			strTmp = "0" + lngDay.ToString();
			strRet = (strRet + StringEx.Right(strTmp, 2)) + "/";

			strTmp = lngYear.ToString();
			strRet = (strRet + strTmp) + " ";

			// mm/dd/yyyy Hh:Nn:Ss
			int lngHour = pdatSql.Hour;
			int lngMinute = pdatSql.Minute;
			int lngSecond = pdatSql.Second;

			strTmp = "0" + lngHour.ToString();
			strRet = (strRet + StringEx.Right(strTmp, 2)) + ":";

			strTmp = "0" + lngMinute.ToString();
			strRet = (strRet + StringEx.Right(strTmp, 2)) + ":";

			strTmp = "0" + lngSecond.ToString();
			strRet = (strRet + StringEx.Right(strTmp, 2));

			return strRet;
		}

		//return a 10 digit string
		public static string ToPhone(string strData, bool pblnFormat = false) {
			string strRet = "";

			foreach (char c in strData) {
				if (char.IsDigit(c) == true) {
					strRet = strRet + c.ToString();
				}
			}
			strRet = StringEx.Left(strRet, 10);

			if (pblnFormat == true) {
				strRet = "(" + StringEx.Left(strRet, 3) + ") " + StringEx.Mid(strRet, 4, 3) + "-" + StringEx.Right(strRet, 4);
			}

			return strRet;
		}
	}
}

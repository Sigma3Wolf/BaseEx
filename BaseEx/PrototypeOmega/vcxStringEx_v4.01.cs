//*****************************************************************************************//
// Copyright 2023, Patrice Charbonneau                                                     //
//                 a.k.a. Genetic Wolf                                                     //
//                 Iid: [80a11b77a52aa9eeed80c9d37dcb7878519289d40beeddb40bb23a60d2711963] //
//                 All rights reserved.                                                    //
//                                                                                         //
// This source code is licensed under the [BSD 3-Clause "New" or "Revised" License]        //
// found in the LICENSE file in the root directory of this source tree.                    //
//*****************************************************************************************//
//**      If you modify this file, you MUST rename it to exclude the version number.     **//
//*****************************************************************************************//
// USAGE: Extension to manipulate string the easy way as Vb6 did

// v4.00 - 2024-11-18:  Integreate Left, Right and Mid as static method using [this];
// v4.01 - 2024-11-29:  Remove unused using;

//Variable declaration
using System;
using System.Text;
using System.Globalization;

namespace PrototypeOmega {
	public static class StringEx {
        public static string PascalCaseEx(this string pstrData) {
            string strRet = StringEx.Left(pstrData, 1).ToUpper() + StringEx.Mid(pstrData, 2).ToLower();
            return strRet;
        }

        public static string RemoveAccentsEx(this string text) {
            StringBuilder sbReturn = new StringBuilder();
            var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (char letter in arrayText) {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark) {
                    sbReturn.Append(letter);
                }
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

        public static string LeftEx(this string pstrString, int plngLength) {
            string strRet = StringEx.Left(pstrString, plngLength);
            return strRet;
        } //LeftEx

        //Compare Left of string with another string
        public static bool LeftEx(this string pstrString, string pstrLeft, bool pblnCaseInsensitive = false, bool pblnRemoveAccents = false) {
			bool blnRet = false;

			if (!string.IsNullOrEmpty(pstrString)) {
				string strPart = pstrLeft;
				int lngPart = strPart.Length;
				if (lngPart > 0) {
					if (pstrString.Length >= lngPart) {
						string strExtract = pstrString.Substring(0, lngPart);
						if (pblnCaseInsensitive == true) {
							strPart = strPart.ToLower();
							strExtract = strExtract.ToLower();
						}

                        if (pblnRemoveAccents == true) {
                            strPart = StringEx.RemoveAccentsEx(strPart);
                            strExtract = StringEx.RemoveAccentsEx(strExtract);
                        }

						if (strExtract == strPart) {
							blnRet = true;
						}
					}
				}
			}

			return blnRet;
        } //LeftEx

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

        public static string RightEx(this string pstrString, int plngLength) {
            string strRet = StringEx.Right(pstrString, plngLength);
            return strRet;
        } //RightEx

        //Compare Right of string with another string
        public static bool RightEx(this string pstrString, string pstrRight, bool pblnCaseInsensitive = false, bool pblnRemoveAccents = false) {
			bool blnRet = false;

			if (!string.IsNullOrEmpty(pstrString)) {
				string strPart = pstrRight;
				int lngPart = strPart.Length;
				if (lngPart > 0) {
					if (pstrString.Length >= lngPart) {
						string strExtract = pstrString.Substring(pstrString.Length - lngPart, lngPart);
						if (pblnCaseInsensitive == true) {
							strPart = strPart.ToLower();
							strExtract = strExtract.ToLower();
						}

                        if (pblnRemoveAccents == true) {
                            strPart = StringEx.RemoveAccentsEx(strPart);
                            strExtract = StringEx.RemoveAccentsEx(strExtract);
                        }

                        if (strExtract == strPart) {
							blnRet = true;
						}
					}
				}
			}

			return blnRet;
        } //RightEx

        //plngStartPos is [BASE 1]
        public static string Mid(string pstrString, int plngStartPos, int plngLength = -1) {
            string strRet = "";

            if (!string.IsNullOrEmpty(pstrString)) {
                if (plngStartPos <= pstrString.Length) {
                    //from here, we work with index instead of position (C# convention)
                    int lngStartIndex = StringEx.IntNotBelow(plngStartPos, 1) - 1;

                    if (lngStartIndex < pstrString.Length) {
                        int lngMaxLength = StringEx.LengthFromIndex(pstrString, lngStartIndex);
                        int lngLength;

                        if (plngLength < 0) {
                            //return all available caracter
                            lngLength = lngMaxLength;
                        } else {
                            //return available caracter up to lngMaxLength
                            lngLength = StringEx.IntNotHigher(plngLength, lngMaxLength);
                        }

                        if (lngLength > 0) {
                            strRet = pstrString.Substring(lngStartIndex, lngLength);
                        }
                    }
                }
            }

            return strRet;
        } //Mid

        //plngStartPos is [BASE 1]
        public static string MidEx(this string pstrString, int plngStartPos, int plngLength = -1) {
            string strRet = StringEx.Mid(pstrString, plngStartPos, plngLength);
            return strRet;
        } //MidEx

        //plngStartPos is [BASE 1]
        public static void FixMidEx(ref string pstrString, int plngStartPos, string pstrNewMid, bool pblnAllowOverflow = false) {
            string strLeft = "";
            string strMid = pstrString;
            string strRight = "";

            if ((pstrString.Length > 0) && (pstrNewMid.Length > 0)) {
                int lngMidSize = pstrNewMid.Length;
                if ((plngStartPos + pstrNewMid.Length - 1) > pstrString.Length) {
                    if (pblnAllowOverflow == false) {
                        lngMidSize = pstrString.Length + 1 - plngStartPos;
                    }
                }

                //Left Part
                if (plngStartPos > 1) {
                    strLeft = StringEx.Left(pstrString, plngStartPos - 1);
                }

                //Mid Part
                strMid = StringEx.Left(pstrNewMid, lngMidSize);

                //Right Part
                int lngRemaining = (plngStartPos + pstrNewMid.Length - 1);
                if (pstrString.Length > lngRemaining) {
                    lngRemaining = (pstrString.Length - lngRemaining);
                    strRight = StringEx.Right(pstrString, lngRemaining);
                }
            }

            pstrString = (strLeft + strMid + strRight);
        } //MidEx

        //Compare Mid of string with another string
        //plngStartPos is [BASE 1]
        public static bool MidEx(this string pstrString, int plngStartPos, string pstrMid, bool pblnCaseInsensitive = false, bool pblnRemoveAccents = false) {
            bool blnRet = false;

            if (!string.IsNullOrEmpty(pstrString)) {
                string strPart = pstrMid;
                int lngPart = strPart.Length;
                if (lngPart > 0) {
                    if (pstrString.Length >= lngPart) {
                        string strExtract = StringEx.Mid(pstrString, plngStartPos, lngPart);
                        if (pblnCaseInsensitive == true) {
                            strPart = strPart.ToLower();
                            strExtract = strExtract.ToLower();
                        }

                        if (pblnRemoveAccents == true) {
                            strPart = StringEx.RemoveAccentsEx(strPart);
                            strExtract = StringEx.RemoveAccentsEx(strExtract);
                        }

                        if (strExtract == strPart) {
                            blnRet = true;
                        }
                    }
                }
            }

            return blnRet;
        }

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

        public static int InstrEx(this string pstrString, string pstrChar, int plngStart = 1) {
            int lngRet = StringEx.Instr(pstrString, pstrChar, plngStart);
            return lngRet;
        } //InstrEx

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

        public static int InstrRevEx(this string pstrString, string pstrChar, int plngStart = -1) {
            int lngRet = StringEx.InstrRev(pstrString, pstrChar, plngStart);
            return lngRet;
        } //InstrRevEx

		//Extract a path from full filename (remove ending backslash)
		public static string ExtractPath(string pstrPath) {
			string strRet = pstrPath;

			int lngPos = StringEx.InstrRev(pstrPath, @"\");
			if (lngPos > 0) {
				strRet = StringEx.Left(pstrPath, (lngPos - 1));
			}

			return strRet;
		}

        public static string ExtractPathEx(this string pstrPath) {
            string strRet = StringEx.ExtractPath(pstrPath);
            return strRet;
        }

        //Extract the String content of the first node provided
        public static string ExtractXmlValue(string pxmlData, string pstrNodeName) {
			string strRet = "";
			int lngInstr;

			lngInstr = StringEx.Instr(pxmlData, "<" + pstrNodeName + ">");
			if (lngInstr == 0) {
				lngInstr = StringEx.Instr(pxmlData, "<" + pstrNodeName + " ");
			}

			if (lngInstr != 0) {
				int lngInstr2;
				lngInstr2 = StringEx.Instr(pxmlData, "</" + pstrNodeName + ">");
				if (lngInstr2 != 0) {
					//Ok Find Begin of String
					lngInstr = StringEx.Instr(pxmlData, ">", lngInstr + 1) + 1;
					strRet = StringEx.Mid(pxmlData, lngInstr, lngInstr2 - lngInstr);
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

        public static string ExtractXmlValueEx(this string pxmlData, string pstrNodeName) {
            string strRet = StringEx.ExtractXmlValue(pxmlData, pstrNodeName);
            return strRet;
        }

        public enum EncodingEx {
            Unicode,
            ASCII,
            UTF8,
        }

        //Convert String for BdString
        public static byte[] ToBytesEx(this string pstrData, EncodingEx penmEncoding = EncodingEx.UTF8) {
            byte[] bytBytes;

            switch (penmEncoding) {
                case EncodingEx.Unicode:
                    bytBytes = Encoding.Unicode.GetBytes(pstrData);
                    break;

                case EncodingEx.ASCII:
                    bytBytes = Encoding.ASCII.GetBytes(pstrData);
                    break;

                case EncodingEx.UTF8:
                default:
                    bytBytes = Encoding.UTF8.GetBytes(pstrData);
                    break;
            }
            //string strData = Encoding.ASCII.GetString(bytes);

            return bytBytes;
        }

        ////Convert String to Int32 or null if fail
        //public static int ToInt32Ex(this string pstrInt32) {
        //	int lngRet = -1;

        //	try {
        //		lngRet = int.Parse(pstrInt32);
        //	} catch {
        //	}

        //	return lngRet;
        //}

        //public static int HexToInt32Ex(this string pstrInt32) {
        //	int lngRet = -1;
        //	try {
        //		lngRet = int.Parse(pstrInt32, System.Globalization.NumberStyles.HexNumber);
        //	} catch {
        //	}

        //	return lngRet;
        //}

        //String To Boolean
        public static bool ToBooleanEx(this string pstrValue) {
            //if (bool.TryParse(pstrValue, out bool blnRetValue) == false) {
            //    blnRetValue = false;
            //}
            bool.TryParse(pstrValue, out bool blnRetValue);
            return blnRetValue;
        }

        //SqlString To DateTimeEx
        //SqlString "mm/dd/yyyy Hh:Nn:Ss" to DateTime
        public static DateTime ToDateTimeEx(this string pstrSqlDate) {
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
        public static string ToSqlStringEx(this DateTime pdatSql) {
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
        public static string ToPhoneEx(this string strData, bool pblnFormat = false) {
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

        public static string HexEncodeEx(this string pstrPlainText) {
            string strRet = StringEx.HexEncode(pstrPlainText);
            return strRet;
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

        public static string HexDecodeEx(string pstrHexData) {
            string strRet = StringEx.HexDecode(pstrHexData);
            return strRet;
        }

        /* PRIVATE SECTION *********************************************************************************** */
        private static int LengthFromIndex(string pstrString, int plngIndex) {
			int lngRet = 0;

			if (pstrString != null) {
				if (pstrString.Length > 0) {
					int lngIndex = StringEx.IntNotBelow(plngIndex, 0);
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
    } //StringEx
}

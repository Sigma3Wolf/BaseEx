//*****************************************************************************************//
// Copyright 2023, Patrice Charbonneau                                                     //
//                 a.k.a. Sigma3Wolf                                                       //
//                 Iid: [80a11b77a52aa9eeed80c9d37dcb7878519289d40beeddb40bb23a60d2711963] //
//                 All rights reserved.                                                    //
//                                                                                         //
// This source code is licensed under the [BSD 3-Clause "New" or "Revised" License]        //
// found in the LICENSE file in the root directory of this source tree.                    //
//*****************************************************************************************//
//**      If you modify this file, you MUST rename it to exclude the version number.     **//
//*****************************************************************************************//
// Usage: Convert between different Base (2, 8, 10, 16, ...) and allow math function;

// v2.00 - 2024-10-04:	Breaking change;
//                          replaced Base32 by Base34; remove Base32 ** won't be used for calculation **
//                          replaced Base55 by Base56; remove Base55 ** won't be used for calculation **
// v2.01 - 2024-10-21:  Name change from HexEx to BaseEx
//                      Add ZeroTrim();
//                      BaseExValidated can now group by a number of bit (usefull to convert from one base to another);
// v2.02 - 2024-10-22:  Add StringBase convertion;
// v2.03 - 2024-11-11:  Fix some compatibility issue with .Net 4.8.1;
// v2.04 - 2024-11-12:  Add base30 and base50 (compatible together);
// v2.05 - 2024-12-07:  Fix a type string in the return of ConvertBase2ToBase16();

//Variable declaration
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PrototypeOmega {
    //https://stackoverflow.com/questions/26829414/c-sharp-boxing-wrapper-custom-class-act-as-integer
    public class BaseEx {
        public enum enmBaseEx {
            Base2 = 2,

            Base8 = 8,

            Base10 = 10,

            Base16 = 16,

            Base30 = 30,

            Base35 = 35,

            Base50 = 50,

            Base56 = 56
        }

        private static Random gobjRnd = new Random();
        public readonly enmBaseEx genmBaseExSize;
        private readonly string gstrBaseExCharBank;

        private long glngBaseExValue = 0;
        private string gstrBaseExToString;
        private int glngMaxChar;
        private static Dictionary<string, long> gdicTableValue = CreateDictionary();

        public BaseEx(enmBaseEx penmBaseExSize = enmBaseEx.Base35, long plngValue = 0) {
            this.genmBaseExSize = penmBaseExSize;
            this.gstrBaseExCharBank = CharBank(penmBaseExSize);
            this.glngMaxChar = MaxChar(penmBaseExSize);

            this.glngBaseExValue = Math.Abs(plngValue);
            this.gstrBaseExToString = "";
        }

        public BaseEx(enmBaseEx penmBaseExSize = enmBaseEx.Base35, string pstrtValue = "0") {
            this.genmBaseExSize = penmBaseExSize;
            this.gstrBaseExCharBank = CharBank(penmBaseExSize);
            this.glngMaxChar = MaxChar(penmBaseExSize);

            // Can I do that safely ??
            long lngValue = BaseEx.StringToValue(penmBaseExSize, pstrtValue);
            this.glngBaseExValue = lngValue;
            this.gstrBaseExToString = "";
        }

        public enmBaseEx BaseExBase {
            get {
                return this.genmBaseExSize;
            }
        }

        public string BaseExCharBank {
            get {
                return this.gstrBaseExCharBank;
            }
        }

        private long BaseExValue {
            get {
                return this.glngBaseExValue;
            }

            set {
                long lngValueTmp = Math.Abs(value);
                if (value != glngBaseExValue) {
                    this.glngBaseExValue = value;
                }
            }
        }

        public new string ToString() {
            //use this.genmBaseExSize as output
            this.gstrBaseExToString = ValueToString(this.glngBaseExValue, this.genmBaseExSize);

            return this.gstrBaseExToString;
        }

        public string ToString(enmBaseEx penmBaseExSize) {
            //use this.genmBaseExSize as output
            this.gstrBaseExToString = ValueToString(this.glngBaseExValue, penmBaseExSize);

            return this.gstrBaseExToString;
        }

        public static implicit operator long(BaseEx d) => d.BaseExValue;

        public static implicit operator BaseEx(long b) => new BaseEx(enmBaseEx.Base10, b);
        //public static implicit operator BaseEx(enmBaseExBase a, long b) => new BaseEx(a, b);
        //public static implicit operator BaseEx(this a, long b) => new BaseEx(a, b);
        //public static BaseEx operator +(BaseEx b, byte amount) => new BaseEx(b.BaseExValue + amount);
        //public static BaseEx operator -(BaseEx b, byte amount) => new BaseEx(b.BaseExValue - amount);

        // static ******************************************************************************
        public static string ZeroTrim(string pstrBaseValue) {
            string strRet = "0";

            //Find first non Zero
            for (int i = 0; i < pstrBaseValue.Length; i++) {
                if (pstrBaseValue[i] != '0') {
                    strRet = StringEx.Mid(pstrBaseValue, i + 1);
                    break;
                }
            }

            return strRet;
        }

        public static string CharBank(enmBaseEx enmHexSize) {
            string strRet = "";

            //                ****  Base2  ****
            //                  12
            string cstrBase2 = "01";

            //                ****  Base8  ****
            //                  12345678
            string cstrBase8 = "01234567";

            //                ****  Base10  ****
            //                            1
            //                   1234567890
            string cstrBase10 = "0123456789";

            //                ****  Base16  ****
            //                               1
            //                      1234567890123456
            string cstrBase16 = cstrBase10 + "ABCDEF";

            //                ****  Base30  ****
            //                               1         2         3
            //                      123456789012345678901234567890
            string cstrBase30 = cstrBase10 + "adefghjkmnpqrtuvwxyz";  //compatible with cstrBase50

            //                ****  Base35  ****
            //                               1         2         3
            //                      12345678901234567890123456789012345
            string cstrBase35 = cstrBase10 + "abcdefghijkmnopqrstuvwxyz";

            //                ****  Base50  ****
            //           1         2         3         4         5
            //  12345678901234567890123456789012345678901234567890
            string cstrBase50 = cstrBase30 + "ADEFGHJKMNPQRTUVWXYZ";  //compatible with cstrBase30
            //const string cstrBase50 = cstrBase30 + cstrBase30.ToUpper();

            //                ****  Base56  ****
            //      1         2         3         4         5
            //45678901234567890123456789012345678901234567890123456
            string cstrBase56 = cstrBase35 + "ADEFGHJKLMNPQRTUVWXYZ";

            switch (enmHexSize) {
                case enmBaseEx.Base2:
                    strRet = cstrBase2;
                    break;

                case enmBaseEx.Base8:
                    strRet = cstrBase8;
                    break;

                case enmBaseEx.Base10:
                    strRet = cstrBase10;
                    break;

                case enmBaseEx.Base16:
                    strRet = cstrBase16;
                    break;

                case enmBaseEx.Base30:
                    strRet = cstrBase30;
                    break;

                case enmBaseEx.Base35:
                    //l removed (1)
                    strRet = cstrBase35;
                    break;

                case enmBaseEx.Base50:
                    strRet = cstrBase50;
                    break;

                case enmBaseEx.Base56:
                    //                ****  Base56  ****
                    //                base 35 + Added caracter to achieved Base55
                    //B removed (8)
                    //C removed (c) - from Base 35
                    //I removed (1)
                    //O removed (0)
                    //S removed (5)
                    strRet = cstrBase56;
                    break;
            }

            return strRet;
        }

        //return a random BaseEx
        public static string BaseExRnd(enmBaseEx penmBaseExSize, int plngBaseExNbByte = 4) {
            string strRet = "";
            int lngNbDigit = BaseExFixSize(plngBaseExNbByte);
            string strCharBank = BaseEx.CharBank(penmBaseExSize);

            int lngNewDigit;
            for (int i = 0; i < lngNbDigit; i++) {
                lngNewDigit = gobjRnd.Next(strCharBank.Length);
                strRet = strRet + strCharBank[lngNewDigit];
            }

            return strRet;
        }

        private static int BaseExFixSize(int plngSize) {
            int lngRet = plngSize;

            if (lngRet < 2) {
                lngRet = 2;
            } else if (lngRet > 256) {
                lngRet = 256;
            }

            return lngRet;
        }

        private static string BaseExFixCase(enmBaseEx enmHexSize, string pstrBaseEx) {
            string strRet = pstrBaseEx;

            if ((enmHexSize != enmBaseEx.Base50) && (enmHexSize != enmBaseEx.Base56)) {
                strRet = strRet.ToUpper();
            }

            return strRet;
        }

        public static string BaseExPadLeft(enmBaseEx enmHexSize, string pstrBaseEx, int plngNbByte) {
            string strRet = BaseExFixCase(enmHexSize, pstrBaseEx);
            int lngNbDigit = BaseExFixSize(plngNbByte);

            for (int i = pstrBaseEx.Length; i < lngNbDigit; i++) {
                strRet = "0" + strRet;
            }

            return strRet;
        }

        //This should only be used when user entered their own data, NOT when using a QRCode
        public static string BaseExNormalizeUserEntry(enmBaseEx penmBaseExSize, string pstrBaseEx) {
            string strRet = BaseExFixCase(penmBaseExSize, pstrBaseEx);

            //** AFTER 1.04 breaking changed, this need to be reevaluated !!!

            //if ((penmBaseExSize == enmBaseExBase.Base34) || (penmBaseExSize == enmBaseExBase.Base55)) {
            //    //B removed (8)
            //    strRet = strRet.Replace('B', '8');

            //    //I removed (1)
            //    strRet = strRet.Replace('I', '1');

            //    //O removed (0)
            //    strRet = strRet.Replace('O', '0');

            //    //S removed (5)
            //    strRet = strRet.Replace('S', '5');
            //}

            //if (penmBaseExSize == enmBaseExBase.Base55) {
            //    //c removed (C)
            //    strRet = strRet.Replace('c', 'C');

            //    //l removed (1)
            //    strRet = strRet.Replace('l', '1');

            //    //o removed (0)
            //    strRet = strRet.Replace('o', '0');
            //}

            return strRet;
        }

        //This function is based on string manipulation and DOES NOT include any calculation
        //Base2 vers Base8, on separe la base 2 par groupe de 3 bit, on converti directement en base 8
        public static string ConvertBase2ToBase8(string pstrValue) {
			//string strValidated = BaseEx.BaseExValidated(BaseEx.enmBaseEx.Base2, pstrValue);
			string strValidated = BaseEx.BaseExValidated(BaseEx.enmBaseEx.Base2, pstrValue, 3);

			string strOutput = "Base 2 vers Base 8\r\n";
            strOutput = strOutput + strValidated + "\r\n";
			int lngNbLoop = strValidated.Length / 3;

            string strConverted = "";
            for (int i = 0; i < lngNbLoop; i++) {
                string strGroup = StringEx.Mid(strValidated, (i * 3) + 1, 3);
                long lngValue = BaseEx.StringToValue(BaseEx.enmBaseEx.Base2, strGroup);
                string strDigit = lngValue.ToString();
                strOutput = strOutput + "\r\n" + "[" + strGroup + "] = " + strDigit;
                strConverted = strConverted + strDigit;
            }

            strOutput = strOutput + "\r\n\r\n" + strValidated + "x2 = " + strConverted + "x8";
            return strOutput;
        }

        //This function is based on string manipulation and DOES NOT include any calculation
        //Base8 vers Base2, on place sur 3 bit, chaque bit de base 8
        public static string ConvertBase8ToBase2(string pstrValue) {
			string strValidated = BaseEx.BaseExValidated(BaseEx.enmBaseEx.Base8, pstrValue);

			string strOutput = "Base 8 vers Base 2\r\n";
			strOutput = strOutput + strValidated + "\r\n";

			string strConverted = "";
            foreach (char x in pstrValue) {
                string strBit = x + "";
                long lngValue = BaseEx.StringToValue(BaseEx.enmBaseEx.Base8, strBit);
                BaseEx BaseEx2 = new BaseEx(BaseEx.enmBaseEx.Base2, lngValue);
                string strGroup = BaseEx.BaseExValidated(BaseEx.enmBaseEx.Base2, BaseEx2.ToString(), 3);
                strOutput = strOutput + "\r\n[" + strBit + "] = " + strGroup;
                strConverted = strConverted + strGroup;
            }

            strConverted = BaseEx.ZeroTrim(strConverted);
            strOutput = strOutput + "\r\n\r\n" + pstrValue + "x8 = " + strConverted + "x2";

            return strOutput;
        }

        //This function is based on string manipulation and DOES NOT include any calculation
        //Base2 vers Base16, on separe la base 2 par groupe de 4 bit, on converti directement en base 16
        public static string ConvertBase2ToBase16(string pstrValue) {
			string strValidated = BaseEx.BaseExValidated(BaseEx.enmBaseEx.Base2, pstrValue, 4);

			string strOutput = "Base 2 vers Base 16\r\n";
			strOutput = strOutput + strValidated + "\r\n";
            int lngNbLoop = strValidated.Length / 4;

            string strConverted = "";
            for (int i = 0; i < lngNbLoop; i++) {
                string strGroup = StringEx.Mid(strValidated, (i * 4) + 1, 4);
                long lngValue = BaseEx.StringToValue(BaseEx.enmBaseEx.Base2, strGroup);
                BaseEx baseEx = new BaseEx(BaseEx.enmBaseEx.Base16, lngValue);

                string strDigit = baseEx.ToString();
                strOutput = strOutput + "\r\n" + "[" + strGroup + "] = " + strDigit;
                strConverted = strConverted + strDigit;
            }

            strOutput = strOutput + "\r\n\r\n" + strValidated + "x2 = " + strConverted + "x16";

            return strOutput;
        }

        //This function is based on string manipulation and DOES NOT include any calculation
        //Base16 vers Base8, je viens de trouver la méthode.
        public static string ConvertBase16ToBase8(string pstrValue) {
			string strValidated = BaseEx.BaseExValidated(BaseEx.enmBaseEx.Base16, pstrValue);

			string strOutput = "Base 16 vers Base 8\r\n";
			strOutput = strOutput + strValidated + "\r\n";

			long lngDigit = 0;
            
            string strConverted = "";
            long lngRetenue = 0;
            for (int i = pstrValue.Length; i > 0; i--) {
                String strChar = StringEx.Mid(pstrValue, i, 1);
                String strChar2 = strChar;
                lngDigit = BaseEx.StringToValue(BaseEx.enmBaseEx.Base16, strChar);
                int lngPosValue2 = (pstrValue.Length - i) * 2;
                Debug.WriteLine(lngPosValue2.ToString());
                int lngPosValue = (pstrValue.Length - i);
                lngPosValue = (int)Math.Pow(2, lngPosValue);
                lngDigit = lngDigit * lngPosValue;
                lngDigit = lngDigit + lngRetenue;

                lngRetenue = 0;
                while (lngDigit > 7) {
                    lngDigit = lngDigit - 8;
                    lngRetenue++;
                }
                strChar2 = lngDigit.ToString();

                if (lngPosValue > 0) {
                    strOutput = strOutput + "[" + strChar + "] * " + lngPosValue.ToString() + " = " + strChar2 + " (retenu " + lngRetenue.ToString() + ")\r\n";
                } else {
                    strOutput = strOutput + "[" + strChar + "] = " + strChar2 + " (retenu " + lngRetenue.ToString() + ")\r\n";
                }
                strConverted = strChar2 + strConverted;
            }
            strOutput = strOutput + "\r\n";

            //remaining...
            long lngRemaining = lngRetenue;
			lngDigit = lngRetenue;
            lngRetenue = 0;
            if (lngDigit > 7) {
                strOutput = strOutput + "remaining: [" + lngDigit .ToString() + "]\r\n";

                //possible bug here, could the remaining division higher then 7 ? if so we need another digit
                //but it doesn't seem possible it would mean initial remaining is higher then 77x8. we need check possibility
                lngRetenue = (lngDigit / 8);
                long lngSubstract = lngRetenue * 8;
				//while (lngDigit > 7) {
				//                lngDigit = lngDigit - 8;
				//                lngRetenue++;
				//            }
				//367D50 FAIL
				lngDigit = lngDigit - lngSubstract;

                string strTmp = lngRemaining.ToString();
                if (strTmp.Length < 3) {
                    strTmp = "   " + strTmp;
                    strTmp = strTmp.RightEx(3);
				}
				
				strOutput = strOutput + strTmp + " / 8     = " + lngRetenue.ToString();
				strOutput = strOutput + " => [" + lngRetenue.ToString() + " * 8 = {" + lngSubstract.ToString() + "}]\r\n";

                strOutput = strOutput + strTmp;
                strTmp = lngSubstract.ToString();
				if (strTmp.Length < 3) {
					strTmp = "   " + strTmp;
					strTmp = strTmp.RightEx(3);
				}
				strOutput = strOutput + " - {" + strTmp + "} = " + lngDigit.ToString();
			}
			strConverted = lngRetenue.ToString() + lngDigit.ToString() + strConverted;

            strOutput = strOutput + "\r\n\r\n" + pstrValue + "x16 = " + strConverted + "x8";

            return strOutput;
        }

        public static string BaseExValidated(enmBaseEx penmBaseExSize, string pstrBaseEx, byte pbytGroup = 0) {
            string strBaseEx = string.Empty;

            int lngMaxChar = MaxChar(penmBaseExSize);
            if ((pstrBaseEx.Length > 0) && (pstrBaseEx.Length <= lngMaxChar)) {
                strBaseEx = pstrBaseEx;

                string strCharBank = BaseEx.CharBank(penmBaseExSize);
                string strChar = string.Empty;
                for (int i = 0; i < strBaseEx.Length; i++) {
                    strChar = strBaseEx[i] + "";
                    if (!strCharBank.Contains(strChar)) {
                        strBaseEx = "";
                        break;
                    }
                }
            }

            if (pbytGroup != 0) {
                int lngModulo = strBaseEx.Length % pbytGroup;
                if (lngModulo != 0) {
                    int lngLength = (strBaseEx.Length / pbytGroup) + 1;
                    lngLength = (lngLength * pbytGroup);
                    strBaseEx = BaseExPadLeft(penmBaseExSize, strBaseEx, lngLength);
                }
            }

            return strBaseEx;
        }

        //Maxchar is based on [uint] but we're using [long] for safety
        //Return the maximum number of digit allowed per BaseEx type
        public static byte MaxChar(enmBaseEx penmHexSize) {
            byte lngMaxChar = 10;  //Base50, Base55

            //To improve: We should use a formulae instead, based on uInt
            switch (penmHexSize) {
                case enmBaseEx.Base2:
                    lngMaxChar = 62;
                    break;

                case enmBaseEx.Base8:
                    lngMaxChar = 20;
                    break;

                case enmBaseEx.Base10:
                    lngMaxChar = 18;
                    break;

                case enmBaseEx.Base16:
                    lngMaxChar = 14;
                    break;

                case enmBaseEx.Base30:
                    lngMaxChar = 12;
                    break;

                case enmBaseEx.Base35:
                    lngMaxChar = 12;
                    break;
            }

            return lngMaxChar;
        }

        public static long StringToValue(enmBaseEx penmBaseExSize, string pstrValue) {
            long lngNewValue = 0;

            int lngMaxLength = MaxChar(penmBaseExSize);
            if (pstrValue.Length <= lngMaxLength) {
                string strValue = BaseExValidated(penmBaseExSize, pstrValue);
                if (strValue.Length > 0) {
                    string strCharBank = CharBank(penmBaseExSize);
                    int lngBase = strCharBank.Length;
                    int lngLenght = strValue.Length;

                    for (int i = 0; i < lngLenght; i++) {
                        string strChar = strValue[i].ToString();

                        int lngCharPos = strCharBank.IndexOf(strChar, StringComparison.Ordinal);
                        if (lngCharPos != 0) {
                            int lngExposant = (lngLenght - i) - 1;

                            long lngMultiplicateur = 1;
                            if (lngExposant != 0) {
                                //Since Math.Pow is using double, I'll loop to prevent CPU cycle
                                for (int j = 0; j < lngExposant; j++) {
                                    lngMultiplicateur = checked(lngMultiplicateur * (long)lngBase);
                                }
                            }
                            lngNewValue = checked(lngNewValue + ((long)lngCharPos * lngMultiplicateur));
                        }
                    }
                }
            }

            return lngNewValue;
        }

        //Special function where we create an utility dictionary for digit value for each BaseExType
        private static Dictionary<string, long> CreateDictionary() {
            Dictionary<string, long> dicTableValue = new Dictionary<string, long>();

            //Let's loop through our Enum
            foreach (BaseEx.enmBaseEx objEnum in Enum.GetValues(typeof(BaseEx.enmBaseEx))) {
                string strName = objEnum.ToString();
                byte lngMaxChar = MaxChar(objEnum);

                //Define our byte base
                int lngBase = (int)objEnum;

                //create a dictionary value for each byte position
                //This is what we do:
                //long lngDicVal = (long)Math.Pow((double)lngBase, (double)bytPos);

                long lngDicVal = 1;
                string strDicKey = "";
                for (int bytPos = 1; bytPos <= lngMaxChar; bytPos++) {
                    strDicKey = strName + "-" + bytPos.ToString();
                    dicTableValue.Add(strDicKey, lngDicVal);
                    //Debug.WriteLine(strDicKey + "; " + lngDicVal.ToString());

                    //Prepare Next Value (we could encompass it in a IF but since long don't overflow by design, it's ok)
                    lngDicVal = lngDicVal * lngBase;
                }

                ////We need the last value for dictionary checking in ValueToString()
                //strDicKey = strName + "-" + lngMaxChar.ToString();
                //dicTableValue.Add(strDicKey, lngDicVal);
                //Debug.WriteLine(strDicKey + "; " + lngDicVal.ToString());
            }

            return dicTableValue;
        }

        //10xBase to any BaseEx
        private static string ValueToString(long plngValue, enmBaseEx penmBaseExSize) {
            string strHexTmp = "";
            long lngValue = plngValue;
            string strValue = lngValue.ToString();
            int lngLength = strValue.Length;
            int lngMaxChar = MaxChar(penmBaseExSize);
            string strCharbank = CharBank(penmBaseExSize);
            if (lngLength > lngMaxChar) {
                throw new Exception("Overflow[1] in BaseEx.ValueToString()");
            }

            //Loop through all bit value
            long lngPosValue = 0;
            for (int bytByte = lngMaxChar; bytByte > 0; bytByte--) {
                string strKey = penmBaseExSize.ToString() + "-" + bytByte.ToString();
                bool blnSuccess = gdicTableValue.TryGetValue(strKey, out lngPosValue);

                //would the use of modulo be faster ?
                int lngComponent = (int)(lngValue / lngPosValue);
                long lngComponentValue = (lngComponent * lngPosValue);
                lngValue = lngValue - lngComponentValue;
                string strCharPos = strCharbank[lngComponent].ToString();
                strHexTmp = strHexTmp + strCharPos;
            }
            //strValue = strHexTmp;

            //Trim answer
            bool blnTrim = true;
            strValue = "";
            for (int i = 0; i < strHexTmp.Length; i++) {
                string strChar = strHexTmp[i].ToString();
                if (strChar != "0") {
                    blnTrim = false;
                }

                if (blnTrim == false) {
                    strValue = strValue + strChar;
                }
            }

            if (strValue.Length == 0) {
                strValue = "0";
            }

            return strValue;
        }
    }
}

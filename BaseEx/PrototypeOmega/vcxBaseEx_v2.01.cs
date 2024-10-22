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

// v1.00 - 2024-08-12:	Initial release Hex32 (32 bit);
// v1.01 - 2024-08-20:	Change Hex32PadLeft to public; Minimum size is now 2; byte is used instead of ushort;
//							add Hex16 (regular 16 bit) and Hex55 (55 bit);
// v1.02 - 2024-08-27:	Remove dependency to outside library (include static rnd);
// v1.03 - 2024-09-25:	Updated ToString to allow enmHexExBase argument;
// v2.00 - 2024-10-04:	Breaking change;
//                          replaced Base32 by Base34; remove Base32 ** won't be used for calculation **
//                          replaced Base55 by Base56; remove Base55 ** won't be used for calculation **
// v2.01 - 2024-10-21:  Name change from HexEx to BaseEx
//                      Add ZeroTrim();
//                      BaseExValidated can now group by a number of bit (usefull to convert from one base to another);

//Variable declaration
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;

namespace PrototypeOmega {
    //https://stackoverflow.com/questions/26829414/c-sharp-boxing-wrapper-custom-class-act-as-integer
    public class BaseEx {
        public enum enmBaseEx {
            Base2 = 2,

            Base8 = 8,

            Base10 = 10,

            Base16 = 16,

            [Display(Name = "Base35 (default)")]
            Base35 = 35,

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

            //                      ****  Base2  ****
            //                        12
            const string cstrBase2 = "01";

            //                      ****  Base8  ****
            //                        12345678
            const string cstrBase8 = "01234567";

            //                      ****  Base10  ****
            //                                  1
            //                         1234567890
            const string cstrBase10 = "0123456789";

            //                      ****  Base16  ****
            //                                     1
            //                            1234567890123456
            const string cstrBase16 = cstrBase10 + "ABCDEF";

            //                      ****  Base35  ****
            //                                     1         2         3
            //                            12345678901234567890123456789012345
            const string cstrBase35 = cstrBase10 + "abcdefghijkmnopqrstuvwxyz";

            //                      ****  Base56  ****
            //            1         2         3         4         5
            //   12345678901234567890123456789012345678901234567890123456
            const string cstrBase56 = cstrBase35 + "ADEFGHJKLMNPQRTUVWXYZ";

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

                case enmBaseEx.Base35:
                default:
                    //                ****  Base35  ****
                    //l removed (1)
                    strRet = cstrBase35;
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

            if (enmHexSize != enmBaseEx.Base56) {
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

        public static string BaseExValidated(enmBaseEx penmBaseExSize, string pstrBaseEx, byte pbytGroup = 0) {
            string strBaseEx = string.Empty;

            int lngMaxChar = MaxChar(penmBaseExSize);
            if ((pstrBaseEx.Length > 0) && (pstrBaseEx.Length <= lngMaxChar)) {
                strBaseEx = pstrBaseEx;

                string strCharBank = BaseEx.CharBank(penmBaseExSize);
                char chrChar = '\0';
                for (int i = 0; i < strBaseEx.Length; i++) {
                    chrChar = strBaseEx[i];
                    if (!strCharBank.Contains(chrChar)) {
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
            byte lngMaxChar = 10;  //Base55

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

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

//v1.00 - 2024-08-12:	Initial release Hex32 (32 bit);
//v1.01 - 2024-08-20:	Change Hex32PadLeft to public; Minimum size is now 2; byte is used instead of ushort;
//							add Hex16 (regular 16 bit) and Hex55 (55 bit);
//v1.02 - 2024-08-27:	Remove dependency to outside library (include static rnd);

//Variable declaration
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;

namespace PrototypeOmega {
    //https://stackoverflow.com/questions/26829414/c-sharp-boxing-wrapper-custom-class-act-as-integer
    public class HexEx {
        public enum enmHexExBase {
            Base2 = 2,

            Base8 = 8,

            Base10 = 10,

            Base16 = 16,

            [Display(Name = "Base32 (default)")]
            Base32 = 32,

            Base55 = 55
        }

        private static Random gobjRnd = new Random();
        public readonly enmHexExBase genmHexExSize;
        private readonly string gstrHexExCharBank;

        private long glngHexExValue = 0;
        private string gstrHexExToString;
        private bool gblnHexExChanged;
        private int glngMaxChar;
        private static Dictionary<string, long> gdicTableValue = new();

        public HexEx(enmHexExBase penmHexExSize = enmHexExBase.Base32, long plngValue = 0) {
            gdicTableValue = CreateDictionary();
            this.genmHexExSize = penmHexExSize;
            this.gstrHexExCharBank = CharBank(penmHexExSize);
            this.glngMaxChar = MaxChar(penmHexExSize);

            this.glngHexExValue = Math.Abs(plngValue);
            this.gstrHexExToString = "";
            this.gblnHexExChanged = true;
        }

        public HexEx(enmHexExBase penmHexExSize = enmHexExBase.Base32, string pstrtValue = "0") {
            gdicTableValue = CreateDictionary();
            this.genmHexExSize = penmHexExSize;
            this.gstrHexExCharBank = CharBank(penmHexExSize);
            this.glngMaxChar = MaxChar(penmHexExSize);

            // Can I do that safely ??
            long lngValue = HexEx.StringToValue(penmHexExSize, pstrtValue);
            this.glngHexExValue = lngValue;
            this.gstrHexExToString = "";
            this.gblnHexExChanged = true;
        }

        public enmHexExBase HexExBase {
            get {
                return this.genmHexExSize;
            }
        }

        public string HexExCharBank {
            get {
                return this.gstrHexExCharBank;
            }
        }

        private long HexExValue {
            get {
                return this.glngHexExValue;
            }

            set {
                long lngValueTmp = Math.Abs(value);
                if (value != glngHexExValue) {
                    this.glngHexExValue = value;
                    this.gblnHexExChanged = true;
                }
            }
        }

        public override string ToString() {
            //use this.genmHexExSize as output
            if (this.gblnHexExChanged == true) {
                this.gstrHexExToString = ValueToString(this.glngHexExValue, this.genmHexExSize);
                this.gblnHexExChanged = false;
            }

            return this.gstrHexExToString;
        }

        public static implicit operator long(HexEx d) => d.HexExValue;


        //
        public static implicit operator HexEx(long b) => new HexEx(enmHexExBase.Base10, b);
        
        //public static implicit operator HexEx(long b) => new HexEx(enmHexExBase.Base16, b);
        //public static implicit operator HexEx(long b) => new HexEx(CurrentHexExBase, b);
        //public static implicit operator HexEx(enmHexExBase a, long b) => new HexEx(a, b);


        //public static implicit operator HexEx(long b) => new HexEx(enmHexExBase.Base2, b);
        //public static implicit operator HexEx(long b) => new HexEx(enmHexExBase.Base8, b);

        //public static implicit operator HexEx(long b) => new HexEx(enmHexExBase.Base16, b);

        //public static implicit operator HexEx(this a, long b) => new HexEx(a, b);

        //public static implicit operator HexEx(long b) => new HexEx(enmHexExBase.Base32, b);
        //public static implicit operator HexEx(long b) => new HexEx(enmHexExBase.Base55, b);

        //public static HexEx operator +(HexEx b, byte amount) => new HexEx(b.HexExValue + amount);
        //public static HexEx operator -(HexEx b, byte amount) => new HexEx(b.HexExValue - amount);

        // static ******************************************************************************
        public static string CharBank(enmHexExBase enmHexSize) {
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

            //                      ****  Base32  ****
            //                                     1         2         3
            //                            12345678901234567890123456789012
            const string cstrBase32 = cstrBase10 + "ACDEFGHJKLMNPQRTUVWXYZ";

            //                      ****  Base55  ****
            //               1         2         3         4         5
            //      1234567890123456789012345678901234567890123456789012345
            const string cstrBase55 = cstrBase32 + "abdefghijkmnpqrstuvwxyz";

            switch (enmHexSize) {
                case enmHexExBase.Base2:
                    strRet = cstrBase2;
                    break;

                case enmHexExBase.Base8:
                    strRet = cstrBase8;
                    break;

                case enmHexExBase.Base10:
                    strRet = cstrBase10;
                    break;

                case enmHexExBase.Base16:

                    strRet = cstrBase16;
                    break;

                case enmHexExBase.Base55:
                    //B removed (8)
                    //I removed (1)
                    //O removed (0)
                    //S removed (5)

                    //c removed (C)
                    //l removed (1)
                    //o removed (0)
                    strRet = cstrBase55;
                    break;

                default:
                    //                  ****  Base32  ****
                    //B removed (8)
                    //I removed (1)
                    //O removed (0)
                    //S removed (5)
                    strRet = cstrBase32;
                    break;
            }

            return strRet;
        }

        //return a random HexEx
        public static string HexExRnd(enmHexExBase penmHexExSize, byte pbytHexExNbByte = 4) {
            string strRet = "";
            byte bytNbDigit = HexExFixSize(pbytHexExNbByte);
            string strCharBank = HexEx.CharBank(penmHexExSize);

            int lngNewDigit;
            for (int i = 0; i < bytNbDigit; i++) {
                lngNewDigit = gobjRnd.Next(strCharBank.Length);
                strRet = strRet + strCharBank[lngNewDigit];
            }

            return strRet;
        }

        private static byte HexExFixSize(byte pbytSize) {
            byte bytRet = pbytSize;

            if (bytRet < 2) {
                bytRet = 2;
            }

            return bytRet;
        }

        private static string HexExFixCase(enmHexExBase enmHexSize, string pstrHexEx) {
            string strRet = pstrHexEx;

            if (enmHexSize != enmHexExBase.Base55) {
                strRet = strRet.ToUpper();
            }

            return strRet;
        }

        public static string HexExPadLeft(enmHexExBase enmHexSize, string pstrHexEx, byte pbytNbByte) {
            string strRet = HexExFixCase(enmHexSize, pstrHexEx);
            byte bytNbDigit = HexExFixSize(pbytNbByte);

            for (int i = pstrHexEx.Length; i < bytNbDigit; i++) {
                strRet = "0" + strRet;
            }

            return strRet;
        }

        public static string HexExNormalize(enmHexExBase penmHexExSize, string pstrHexEx) {
            string strRet = HexExFixCase(penmHexExSize, pstrHexEx);

            if ((penmHexExSize == enmHexExBase.Base32) || (penmHexExSize == enmHexExBase.Base55)) {
                //B removed (8)
                strRet = strRet.Replace('B', '8');

                //I removed (1)
                strRet = strRet.Replace('I', '1');

                //O removed (0)
                strRet = strRet.Replace('O', '0');

                //S removed (5)
                strRet = strRet.Replace('S', '5');
            }

            if (penmHexExSize == enmHexExBase.Base55) {
                //c removed (C)
                strRet = strRet.Replace('c', 'C');

                //l removed (1)
                strRet = strRet.Replace('l', '1');

                //o removed (0)
                strRet = strRet.Replace('o', '0');
            }

            return strRet;
        }

        public static string HexExValidated(enmHexExBase penmHexExSize, string pstrHexEx) {
            string strHexEx = string.Empty;

            int lngMaxChar = MaxChar(penmHexExSize);
            if ((pstrHexEx.Length > 0) && (pstrHexEx.Length <= lngMaxChar)) {
                strHexEx = pstrHexEx;

                string strCharBank = HexEx.CharBank(penmHexExSize);
                char chrChar = '\0';
                for (int i = 0; i < strHexEx.Length; i++) {
                    chrChar = strHexEx[i];
                    if (!strCharBank.Contains(chrChar)) {
                        strHexEx = "";
                        break;
                    }
                }
            }

            return strHexEx;
        }

        //Maxchar is based on [uint] but we're using [long] for safety
        //Return the maximum number of digit allowed per HexEx type
        public static byte MaxChar(enmHexExBase penmHexSize) {
            byte lngMaxChar = 10;  //Base55

            switch (penmHexSize) {
                case enmHexExBase.Base2:
                    lngMaxChar = 62;
                    break;

                case enmHexExBase.Base8:
                    lngMaxChar = 20;
                    break;

                case enmHexExBase.Base10:
                    lngMaxChar = 18;
                    break;

                case enmHexExBase.Base16:
                    lngMaxChar = 14;
                    break;

                case enmHexExBase.Base32:
                    lngMaxChar = 12;
                    break;
            }

            return lngMaxChar;
        }

        public static long StringToValue(enmHexExBase penmHexExSize, string pstrValue) {
            long lngNewValue = 0;

            int lngMaxLength = MaxChar(penmHexExSize);
            if (pstrValue.Length <= lngMaxLength) {
                string strValue = HexExValidated(penmHexExSize, pstrValue);
                if (strValue.Length > 0) {
                    string strCharBank = CharBank(penmHexExSize);
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

        //Special function where we create an utility dictionary for digit value for each HexExType
        private static Dictionary<string, long> CreateDictionary() {
            Dictionary<string, long> dicTableValue = new Dictionary<string, long>();

            //Let's loop through our Enum
            foreach (HexEx.enmHexExBase objEnum in Enum.GetValues(typeof(HexEx.enmHexExBase))) {
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
        private static string ValueToString(long plngValue, enmHexExBase penmHexExSize) {
            string strHexTmp = "";
            long lngValue = plngValue;
            string strValue = lngValue.ToString();
            int lngLength = strValue.Length;
            int lngMaxChar = MaxChar(penmHexExSize);
            string strCharbank = CharBank(penmHexExSize);
            if (lngLength > lngMaxChar) {
                throw new Exception("Overflow[1] in HexEx.ValueToString()");
            }

            //Loop through all bit value
            long lngPosValue = 0;
            for (int bytByte = lngMaxChar; bytByte > 0; bytByte--) {
                string strKey = penmHexExSize.ToString() + "-" + bytByte.ToString();
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

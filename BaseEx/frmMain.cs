using System.Diagnostics;
using System.Numerics;
using PrototypeOmega;

namespace HexExExample {
    public partial class frmMain : Form {
        private string strCharBank = "";
        private BaseEx.enmBaseEx enmPreviousBase = BaseEx.enmBaseEx.Base10;
        private BaseEx.enmBaseEx enmCurrentBase = BaseEx.enmBaseEx.Base10;

        public frmMain() {
            InitializeComponent();

            this.radioButton2.Click += this.RadioButtonHex_Click;
            this.radioButton8.Click += this.RadioButtonHex_Click;
            this.radioButton10.Click += this.RadioButtonHex_Click;
            this.radioButton16.Click += this.RadioButtonHex_Click;
            this.radioButton35.Click += this.RadioButtonHex_Click;
            this.radioButton56.Click += this.RadioButtonHex_Click;
            this.cmdRandom.Click += this.CmdRandom_Click;

            this.txtNumber1.KeyPress += this.TxtValue_KeyPress;
            this.txtNumber2.KeyPress += this.TxtValue_KeyPress;
            this.txtNumber1.TextChanged += this.TxtNumber_TextChanged;
            this.txtNumber2.TextChanged += this.TxtNumber_TextChanged;

            this.txtNumber2.BackColor = SystemColors.ButtonFace;
            this.txtResult.BackColor = SystemColors.ButtonFace;

            this.cmdOperation.Click += this.CmdOperation_Click;

            this.txtBase2.Click += this.TxtBase2_Click;
            this.txtBase8.Click += this.TxtBase8_Click;
            this.txtBase16.Click += this.TxtBase16_Click;
            //Base16 vers Base2: Convert en 4 bit from Base16

            //Initial Value status
            this.RadioButtonHex_Click(radioButton10, new EventArgs());
            this.radioButton10.Checked = true;
        }

        private void TxtBase2_Click(object? sender, EventArgs e) {
            if (radioButton8.Checked == true) {
                string strValue = txtBase8.Text;
                string strOutput = "Base 8 vers Base 2";
                //Base8 vers Base2, on place sur 3 bit, chaque bit de base 8

                string strConverted = "";
                foreach (char x in strValue) {
                    string strBit = x + "";
                    long lngValue = BaseEx.StringToValue(BaseEx.enmBaseEx.Base8, strBit);
                    BaseEx BaseEx2 = new BaseEx(BaseEx.enmBaseEx.Base2, lngValue);
                    string strGroup = BaseEx.BaseExValidated(BaseEx.enmBaseEx.Base2, BaseEx2.ToString(), 3);
                    strOutput = strOutput + "\r\n[" + strBit + "] = " + strGroup;
                    strConverted = strConverted + strGroup;
                }

                strConverted = BaseEx.ZeroTrim(strConverted);
                strOutput = strOutput + "\r\n\r\n" + strValue + "x8 = " + strConverted + "x2";
                this.txtProof.Text = strOutput;
            }

        }

        private void TxtBase8_Click(object? sender, EventArgs e) {
            if (radioButton2.Checked == true) {
                string strValue = txtBase2.Text;
                string strOutput = "Base 2 vers Base 8\r\n";
                //Base2 vers Base8, on separe la base 2 par groupe de 3 bit, on converti directement en base 8

                string strValidated = BaseEx.BaseExValidated(BaseEx.enmBaseEx.Base2, strValue, 3);
                strOutput = strOutput + strValidated;
                strOutput = strOutput + "\r\n";
                int lngNbLoop = strValidated.Length / 3;

                string strConverted = "";
                for (int i = 0; i < lngNbLoop; i++) {
                    string strGroup = StringEx.Mid(strValidated, (i * 3) + 1, 3);
                    long lngValue = BaseEx.StringToValue(BaseEx.enmBaseEx.Base2, strGroup);
                    string strDigit = lngValue.ToString();
                    strOutput = strOutput + "\r\n" + "[" + strGroup + "] = " + strDigit;
                    strConverted = strConverted + strDigit;
                }

                strOutput = strOutput + "\r\n" + strValidated + "x2 = " + strConverted + "x8";
                this.txtProof.Text = strOutput;
            } else if (radioButton16.Checked == true) {
                string strValue = txtBase16.Text;
                string strOutput = "Base 16 vers Base 8\r\n";
                //Base16 vers Base8, je viens de trouver la méthode.

                string strConverted = "";
                long lngRetenue = 0;
                for (int i = strValue.Length; i > 0; i--) {
                    String strChar = StringEx.Mid(strValue, i, 1);
                    String strChar2 = strChar;
                    long lngDigit = BaseEx.StringToValue(BaseEx.enmBaseEx.Base16, strChar);
                    int lngPosValue2 = (strValue.Length - i) * 2;
                    Debug.WriteLine(lngPosValue2.ToString());
                    int lngPosValue = (strValue.Length - i);
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

                if (lngRetenue != 0) {
                    //strOutput = strOutput = "\r\nAdding Retenue";
                    strConverted = lngRetenue.ToString() + strConverted;
                }
                strOutput = strOutput + strValue + "x16 = " + strConverted + "x8";
                this.txtProof.Text = strOutput;
            }
        }

        private void TxtBase16_Click(object? sender, EventArgs e) {
            if (radioButton2.Checked == true) {
                string strValue = txtBase2.Text;
                string strOutput = "Base 2 vers Base 16\r\n";
                //Base2 vers Base16, on separe la base 2 par groupe de 4 bit, on converti directement en base 16

                string strValidated = BaseEx.BaseExValidated(BaseEx.enmBaseEx.Base2, strValue, 4);
                strOutput = strOutput + strValidated;
                strOutput = strOutput + "\r\n";
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

                strOutput = strOutput + "\r\n" + strValidated + "x2 = " + strConverted + "x8";
                this.txtProof.Text = strOutput;
            }
        }

        private void TxtNumber_TextChanged(object? sender, EventArgs e) {
            if (sender != null) {
                TextBox txtNumber = (TextBox)sender;
                string strValidated = BaseEx.BaseExValidated(this.enmCurrentBase, txtNumber.Text);
                if ((strValidated.Length == 0) || (strValidated == "0")) {
                    this.txtNumber1.Text = "0";
                } else {
                    if (strValidated[0] == '0') {
                        strValidated = StringEx.Mid(strValidated, 2);
                        
                        txtNumber.Text = strValidated;
                        txtNumber.SelectionStart = strValidated.Length;
                    } else {
                        txtNumber.Text = strValidated;
                    }
                }

                if (txtNumber.Name == "txtNumber1") {
                    this.UpdateValue();
                }

                this.UpdateOperation();
            }
        }

        private void CmdOperation_Click(object? sender, EventArgs e) {
            if (sender != null) {
                string strOperation = "+-*/";
                int lngIndex = strOperation.IndexOf(cmdOperation.Text);
                lngIndex++;
                if (lngIndex > 3) {
                    lngIndex = 0;
                }
                cmdOperation.Text = strOperation.Substring(lngIndex, 1);
                //MessageBox.Show(txtNumber1.SelectionStart.ToString());
                this.UpdateOperation();
            }
        }

        private void CmdRandom_Click(object? sender, EventArgs e) {
            string strValue = BaseEx.BaseExRnd(this.enmCurrentBase);
            txtNumber1.Text = strValue;
        }

        //Check for allowed character based on the CharBank.
        //if necessary, convert to Uppercase
        private void TxtValue_KeyPress(object? sender, KeyPressEventArgs e) {
            //assume user entered a valid character
            bool blnValid = true;

            switch (e.KeyChar) {
                case (char)8:
                    break;

                default:
                    //Prevent entering invalid character
                    if (!strCharBank.Contains(e.KeyChar)) {
                        if ((this.enmCurrentBase == BaseEx.enmBaseEx.Base16) || (this.enmCurrentBase == BaseEx.enmBaseEx.Base35)) {
                            //Check for LowerCase
                            string strDn = strCharBank.ToLower();
                            if (!strDn.Contains(e.KeyChar)) {
                                blnValid = false;
                            } else {
                                //convert to UpperCase character
                                e.KeyChar = char.Parse(e.KeyChar.ToString().ToUpper());
                            }
                        } else {
                            blnValid = false;
                        }
                    }
                    break;
            }

            if (blnValid == false) {
                e.Handled = true;
            }
        }

        private void RadioButtonHex_Click(object? sender, EventArgs e) {
            if (sender != null) {
                RadioButton objRadio = (RadioButton)sender;
                switch (objRadio.Name) {
                    case "radioButton2":
                        this.enmCurrentBase = BaseEx.enmBaseEx.Base2;
                        break;

                    case "radioButton8":
                        this.enmCurrentBase = BaseEx.enmBaseEx.Base8;
                        break;

                    case "radioButton10":
                        this.enmCurrentBase = BaseEx.enmBaseEx.Base10;
                        break;

                    case "radioButton16":
                        this.enmCurrentBase = BaseEx.enmBaseEx.Base16;
                        break;

                    case "radioButton35":
                        this.enmCurrentBase = BaseEx.enmBaseEx.Base35;
                        break;

                    case "radioButton56":
                    default:
                        this.enmCurrentBase = BaseEx.enmBaseEx.Base56;
                        break;
                }
                this.strCharBank = BaseEx.CharBank(this.enmCurrentBase);
                this.lblBaseX.Text = "Base " + this.strCharBank.Length.ToString();

                if (this.enmPreviousBase != this.enmCurrentBase) {
                    long lngNumber1 = BaseEx.StringToValue(this.enmPreviousBase, this.txtNumber1.Text);
                    long lngNumber2 = BaseEx.StringToValue(this.enmPreviousBase, this.txtNumber2.Text);

                    BaseEx hexEx1 = new BaseEx(this.enmCurrentBase, lngNumber1);
                    BaseEx hexEx2 = new BaseEx(this.enmCurrentBase, lngNumber2);

                    this.txtNumber1.Text = hexEx1.ToString();
                    this.txtNumber2.Text = hexEx2.ToString();
                    this.UpdateOperation();

                    this.UpdateValue();
                }
            }
        }

        private void UpdateOperation() {
            long lngValue1 = BaseEx.StringToValue(this.enmCurrentBase, txtNumber1.Text);
            long lngValue2 = BaseEx.StringToValue(this.enmCurrentBase, txtNumber2.Text);
            long lngValue3 = 0;
            string strAnswer = "";
            BaseEx hexExRet;

            switch (cmdOperation.Text) {
                case "+":
                    lngValue3 = lngValue1 + lngValue2;
                    hexExRet = new BaseEx(this.enmCurrentBase, lngValue3);
                    strAnswer = hexExRet.ToString();
                    break;

                case "-":
                    lngValue3 = lngValue1 - lngValue2;
                    if (lngValue3 < 0) {
                        strAnswer = "< 0";
                    } else {
                        hexExRet = new BaseEx(this.enmCurrentBase, lngValue3);
                        strAnswer = hexExRet.ToString();
                    }
                    break;

                case "*":
                    lngValue3 = lngValue1 * lngValue2;
                    hexExRet = new BaseEx(this.enmCurrentBase, lngValue3);
                    strAnswer = hexExRet.ToString();
                    break;

                case "/":
                default:
                    if (lngValue2 != 0) {
                        lngValue3 = lngValue1 / lngValue2;
                        hexExRet = new BaseEx(this.enmCurrentBase, lngValue3);
                        strAnswer = hexExRet.ToString();
                    } else {
                        strAnswer = "div/0";
                    }
                    break;
            }

            txtResult.Text = strAnswer;
        }

        private void UpdateValue() {
            //string strValidated = this.txtValue.Text;  //big bug in Vs 2022
            string strValidated = BaseEx.BaseExValidated(this.enmCurrentBase, this.txtNumber1.Text);

            if (strValidated.Length == 0) {
                this.txtNumber1.Text = "0";
            } else if (strValidated == "0") {
                this.txtBase2.Text = "0";
                this.txtBase8.Text = "0";
                this.txtBase10.Text = "0";
                this.txtBase16.Text = "0";
                this.txtBase35.Text = "0";
                this.txtBase56.Text = "0";
            } else {
                long lngValue;
                //string strValidated = HexEx.HexExValidated(this.enmCurrentBase, this.txtNumber1.Text);
                lngValue = BaseEx.StringToValue(this.enmCurrentBase, strValidated);
                //switch (this.enmHexExSize) {
                //    case HexEx.HexExBase.Base2:
                //        lngValue = long.Parse(strValidated, System.Globalization.NumberStyles.BinaryNumber);
                //        break;

                //    case HexEx.HexExBase.Base8:
                //        lngValue = 0;
                //        break;

                //    case HexEx.HexExBase.Base10:
                //        lngValue = long.Parse(strValidated);
                //        break;

                //    case HexEx.HexExBase.Base16:
                //        lngValue = long.Parse(strValidated, System.Globalization.NumberStyles.HexNumber);
                //        break;

                //    case HexEx.HexExBase.Base35:
                //        lngValue = 0;
                //        break;

                //    case HexEx.HexExBase.Base56:
                //    default:
                //        lngValue = 0;
                //        break;
                //}

                //lngValue = BaseEx.StringToValue(this.enmCurrentBase, strValidated);
                BaseEx hexEx2 = new BaseEx(BaseEx.enmBaseEx.Base2, lngValue);
                this.txtBase2.Text = hexEx2.ToString();

                BaseEx hexEx8 = new BaseEx(BaseEx.enmBaseEx.Base8, lngValue);
                this.txtBase8.Text = hexEx8.ToString();

                BaseEx hexEx10 = new BaseEx(BaseEx.enmBaseEx.Base10, lngValue);
                this.txtBase10.Text = hexEx10.ToString();

                BaseEx hexEx16 = new BaseEx(BaseEx.enmBaseEx.Base16, lngValue);
                this.txtBase16.Text = hexEx16.ToString();

                BaseEx hexEx35 = new BaseEx(BaseEx.enmBaseEx.Base35, lngValue);
                this.txtBase35.Text = hexEx35.ToString();

                BaseEx hexEx56 = new BaseEx(BaseEx.enmBaseEx.Base56, lngValue);
                this.txtBase56.Text = hexEx56.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            BaseEx hexEx = new(BaseEx.enmBaseEx.Base16, "E82");
            //hexEx++;
            hexEx = hexEx % 22;
            //hexEx = hexEx + 4;
            //hexEx.ToString("-");

            Debug.WriteLine(hexEx.ToString(BaseEx.enmBaseEx.Base2));
            Debug.WriteLine(hexEx.ToString(BaseEx.enmBaseEx.Base8));
            Debug.WriteLine(hexEx.ToString(BaseEx.enmBaseEx.Base10));
            Debug.WriteLine(hexEx.ToString(BaseEx.enmBaseEx.Base16));
            Debug.WriteLine(hexEx.ToString(BaseEx.enmBaseEx.Base35));
            Debug.WriteLine(hexEx.ToString(BaseEx.enmBaseEx.Base56));
        }
    }
}

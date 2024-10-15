using System.Diagnostics;
using PrototypeOmega;

namespace HexExExample {
    public partial class Form1 : Form {
        private string strCharBank = "";
        private HexEx.enmHexExBase enmPreviousBase = HexEx.enmHexExBase.Base10;
        private HexEx.enmHexExBase enmCurrentBase = HexEx.enmHexExBase.Base10;

        public Form1() {
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

            //Initial Value status
            this.RadioButtonHex_Click(radioButton10, new EventArgs());
            this.radioButton10.Checked = true;
        }

        private void TxtNumber_TextChanged(object? sender, EventArgs e) {
            if (sender != null) {
                TextBox txtNumber = (TextBox)sender;
                string strValidated = HexEx.HexExValidated(this.enmCurrentBase, txtNumber.Text);
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
            string strValue = HexEx.HexExRnd(this.enmCurrentBase);
            txtNumber1.Text = strValue;
        }

        private void TxtValue_TextChanged(object? sender, EventArgs e) {
            
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
                        if ((this.enmCurrentBase == HexEx.enmHexExBase.Base16) || (this.enmCurrentBase == HexEx.enmHexExBase.Base35)) {
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
                        this.enmCurrentBase = HexEx.enmHexExBase.Base2;
                        break;

                    case "radioButton8":
                        this.enmCurrentBase = HexEx.enmHexExBase.Base8;
                        break;

                    case "radioButton10":
                        this.enmCurrentBase = HexEx.enmHexExBase.Base10;
                        break;

                    case "radioButton16":
                        this.enmCurrentBase = HexEx.enmHexExBase.Base16;
                        break;

                    case "radioButton35":
                        this.enmCurrentBase = HexEx.enmHexExBase.Base35;
                        break;

                    case "radioButton56":
                    default:
                        this.enmCurrentBase = HexEx.enmHexExBase.Base56;
                        break;
                }
                this.strCharBank = HexEx.CharBank(this.enmCurrentBase);
                this.lblBaseX.Text = "Base " + this.strCharBank.Length.ToString();

                if (this.enmPreviousBase != this.enmCurrentBase) {
                    long lngNumber1 = HexEx.StringToValue(this.enmPreviousBase, this.txtNumber1.Text);
                    long lngNumber2 = HexEx.StringToValue(this.enmPreviousBase, this.txtNumber2.Text);

                    HexEx hexEx1 = new HexEx(this.enmCurrentBase, lngNumber1);
                    HexEx hexEx2 = new HexEx(this.enmCurrentBase, lngNumber2);

                    this.txtNumber1.Text = hexEx1.ToString();
                    this.txtNumber2.Text = hexEx2.ToString();
                    this.UpdateOperation();

                    this.UpdateValue();
                }
            }
        }

        private void UpdateOperation() {
            long lngValue1 = HexEx.StringToValue(this.enmCurrentBase, txtNumber1.Text);
            long lngValue2 = HexEx.StringToValue(this.enmCurrentBase, txtNumber2.Text);
            long lngValue3 = 0;
            string strAnswer = "";
            HexEx hexExRet;

            switch (cmdOperation.Text) {
                case "+":
                    lngValue3 = lngValue1 + lngValue2;
                    hexExRet = new HexEx(this.enmCurrentBase, lngValue3);
                    strAnswer = hexExRet.ToString();
                    break;

                case "-":
                    lngValue3 = lngValue1 - lngValue2;
                    if (lngValue3 < 0) {
                        strAnswer = "< 0";
                    } else {
                        hexExRet = new HexEx(this.enmCurrentBase, lngValue3);
                        strAnswer = hexExRet.ToString();
                    }
                    break;

                case "*":
                    lngValue3 = lngValue1 * lngValue2;
                    hexExRet = new HexEx(this.enmCurrentBase, lngValue3);
                    strAnswer = hexExRet.ToString();
                    break;

                case "/":
                default:
                    if (lngValue2 != 0) {
                        lngValue3 = lngValue1 / lngValue2;
                        hexExRet = new HexEx(this.enmCurrentBase, lngValue3);
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
            string strValidated = HexEx.HexExValidated(this.enmCurrentBase, this.txtNumber1.Text);

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
                lngValue = HexEx.StringToValue(this.enmCurrentBase, strValidated);
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

                HexEx hexEx2 = new HexEx(HexEx.enmHexExBase.Base2, lngValue);
                this.txtBase2.Text = hexEx2.ToString();

                HexEx hexEx8 = new HexEx(HexEx.enmHexExBase.Base8, lngValue);
                this.txtBase8.Text = hexEx8.ToString();

                HexEx hexEx10 = new HexEx(HexEx.enmHexExBase.Base10, lngValue);
                this.txtBase10.Text = hexEx10.ToString();

                HexEx hexEx16 = new HexEx(HexEx.enmHexExBase.Base16, lngValue);
                this.txtBase16.Text = hexEx16.ToString();

                HexEx hexEx35 = new HexEx(HexEx.enmHexExBase.Base35, lngValue);
                this.txtBase35.Text = hexEx35.ToString();

                HexEx hexEx56 = new HexEx(HexEx.enmHexExBase.Base56, lngValue);
                this.txtBase56.Text = hexEx56.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            HexEx hexEx = new(HexEx.enmHexExBase.Base16, "E82");
            //hexEx++;
            hexEx = hexEx % 22;
            //hexEx = hexEx + 4;
            //hexEx.ToString("-");

            Debug.WriteLine(hexEx.ToString(HexEx.enmHexExBase.Base2));
            Debug.WriteLine(hexEx.ToString(HexEx.enmHexExBase.Base8));
            Debug.WriteLine(hexEx.ToString(HexEx.enmHexExBase.Base10));
            Debug.WriteLine(hexEx.ToString(HexEx.enmHexExBase.Base16));
            Debug.WriteLine(hexEx.ToString(HexEx.enmHexExBase.Base35));
            Debug.WriteLine(hexEx.ToString(HexEx.enmHexExBase.Base56));
        }
    }
}

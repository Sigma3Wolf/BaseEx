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
                this.txtProof.Text = BaseEx.ConvertBase8ToBase2(txtBase8.Text);
            }
        }

        private void TxtBase8_Click(object? sender, EventArgs e) {
            if (radioButton2.Checked == true) {
                this.txtProof.Text = BaseEx.ConvertBase2ToBase8(txtBase2.Text);
            } else if (radioButton16.Checked == true) {
                //367D50 FAIL
                this.txtProof.Text = BaseEx.ConvertBase16ToBase8(txtBase16.Text);
            }
        }

        private void TxtBase16_Click(object? sender, EventArgs e) {
            if (radioButton2.Checked == true) {
                this.txtProof.Text = BaseEx.ConvertBase2ToBase16(txtBase2.Text);
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
				this.enmPreviousBase = this.enmCurrentBase;
				BaseEx.enmBaseEx enmNewBase = this.enmCurrentBase;

				string strHighLight = "000000";
                RadioButton objRadio = (RadioButton)sender;
                switch (objRadio.Name) {
                    case "radioButton2":
						enmNewBase = BaseEx.enmBaseEx.Base2;
						//x8; x16 highlight
						strHighLight = "010100";
						break;

                    case "radioButton8":
						enmNewBase = BaseEx.enmBaseEx.Base8;
						//x2 highlight
						strHighLight = "100000";
						break;

                    case "radioButton10":
						enmNewBase = BaseEx.enmBaseEx.Base10;
                        break;

                    case "radioButton16":
						enmNewBase = BaseEx.enmBaseEx.Base16;
						//x8 highlight
						strHighLight = "010000";
						break;

                    case "radioButton35":
						enmNewBase = BaseEx.enmBaseEx.Base35;
                        break;

                    case "radioButton56":
                    default:
						enmNewBase = BaseEx.enmBaseEx.Base56;
                        break;
                }
                this.strCharBank = BaseEx.CharBank(enmNewBase);
                this.lblBaseX.Text = "Base " + this.strCharBank.Length.ToString();

				this.enmCurrentBase = enmNewBase;
				if (this.enmPreviousBase != enmNewBase) {
                    long lngNumber1 = BaseEx.StringToValue(this.enmPreviousBase, this.txtNumber1.Text);
                    long lngNumber2 = BaseEx.StringToValue(this.enmPreviousBase, this.txtNumber2.Text);

                    BaseEx hexEx1 = new BaseEx(enmNewBase, lngNumber1);
                    BaseEx hexEx2 = new BaseEx(enmNewBase, lngNumber2);

                    this.txtNumber1.Text = hexEx1.ToString();
                    this.txtNumber2.Text = hexEx2.ToString();
                    this.UpdateOperation();

                    this.UpdateValue();
                }

                string strChar = "";
				strChar = strHighLight.MidEx(1, 1);
				if (strChar == "1") {
					txtBase2.BackColor = Color.LightGreen;
				} else {
					txtBase2.BackColor = SystemColors.Control;
				}
                
				strChar = strHighLight.MidEx(2, 1);
				if (strChar == "1") {
					txtBase8.BackColor = Color.LightGreen;
				} else {
					txtBase8.BackColor = SystemColors.Control;
				}

				strChar = strHighLight.MidEx(3, 1);
				if (strChar == "1") {
					txtBase10.BackColor = Color.LightGreen;
				} else {
					txtBase10.BackColor = SystemColors.Control;
				}

				strChar = strHighLight.MidEx(4, 1);
				if (strChar == "1") {
					txtBase16.BackColor = Color.LightGreen;
				} else {
					txtBase16.BackColor = SystemColors.Control;
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

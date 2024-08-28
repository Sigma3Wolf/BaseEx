using PrototypeOmega;
using System.Diagnostics;

namespace HexExExample {
    public partial class Form1 : Form {
        private string strCharBank = "";
        private HexEx.enmHexExBase enmHexExSize = HexEx.enmHexExBase.Base10;

        public Form1() {
            InitializeComponent();

            this.radioButton2.Click += this.RadioButtonHex_Click;
            this.radioButton10.Click += this.RadioButtonHex_Click;
            this.radioButton16.Click += this.RadioButtonHex_Click;
            this.radioButton32.Click += this.RadioButtonHex_Click;
            this.radioButton55.Click += this.RadioButtonHex_Click;

            this.txtValue.KeyPress += this.TxtValue_KeyPress;
            this.txtValue.TextChanged += this.TxtValue_TextChanged;

            //Initial Value status
            radioButton10.Checked = true;
            this.strCharBank = HexEx.CharBank(enmHexExSize);
            this.txtValue.Text = "0";
        }

        private void TxtValue_TextChanged(object? sender, EventArgs e) {
            this.UpdateValue();
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
                        if ((this.enmHexExSize == HexEx.enmHexExBase.Base16) || (this.enmHexExSize == HexEx.enmHexExBase.Base32)) {
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
                        this.enmHexExSize = HexEx.enmHexExBase.Base2;
                        break;

                    case "radioButton10":
                        this.enmHexExSize = HexEx.enmHexExBase.Base10;
                        break;

                    case "radioButton16":
                        this.enmHexExSize = HexEx.enmHexExBase.Base16;
                        break;

                    case "radioButton32":
                        this.enmHexExSize = HexEx.enmHexExBase.Base32;
                        break;

                    case "radioButton55":
                    default:
                        this.enmHexExSize = HexEx.enmHexExBase.Base55;
                        break;
                }
                this.strCharBank = HexEx.CharBank(this.enmHexExSize);
                this.txtValue.Text = "0";
                this.UpdateValue();
            }
        }

        private void UpdateValue() {
            //string strValidated = this.txtValue.Text;  //big bug in Vs 2022
            string strValidated = HexEx.HexExValidated(this.enmHexExSize, this.txtValue.Text);

            if (strValidated.Length == 0) {
                this.txtValue.Text = "0";
            } else if (strValidated == "0") {
                this.txtBase2.Text = "0";
                this.txtBase8.Text = "0";
                this.txtBase10.Text = "0";
                this.txtBase16.Text = "0";
                this.txtBase32.Text = "0";
                this.txtBase55.Text = "0";
            } else {
                long lngValue;
                lngValue = HexEx.StringToValue(this.enmHexExSize, strValidated);
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

                //    case HexEx.HexExBase.Base32:
                //        lngValue = 0;
                //        break;

                //    case HexEx.HexExBase.Base55:
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

                HexEx hexEx32 = new HexEx(HexEx.enmHexExBase.Base32, lngValue);
                this.txtBase32.Text = hexEx32.ToString();

                HexEx hexEx55 = new HexEx(HexEx.enmHexExBase.Base55, lngValue);
                this.txtBase55.Text = hexEx55.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            HexEx hexEx = new(HexEx.enmHexExBase.Base32, "3HQ");
            hexEx++;

            //this need to be fixed because hexEx++ doesn'T take into account the BASE of the HexEx object
            //See library line 117:
            // public static implicit operator HexEx(long b) => new HexEx(enmHexExBase.Base32, b);

            Debug.Write(hexEx.ToString());
        }
    }
}

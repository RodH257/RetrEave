using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Retreave.Domain.Services;

namespace SearchTestUi
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnInput_Click(object sender, EventArgs e)
        {
            SearchEngineService service = new SearchEngineService();
            foreach (string result in service.GetUrlsForTerm(this.txtInput.Text))
            {
                this.txtResults.Text += result + Environment.NewLine + Environment.NewLine;
            }
        }
    }
}

//
// FOfficeKey.cs
// This file is part of Ghostscript.NET.Viewer project
//
// Author: Artifex Software Inc.
// Copyright (c) 2026 by Artifex Software Inc. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ghostscript.NET.Viewer
{
    internal sealed class FOfficeKey : Form
    {
        private readonly TextBox _txtKey;
        private bool _continueWithoutKey;

        public FOfficeKey()
        {
            Text = "Office license key";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            ClientSize = new Size(500, 168);
            Font = new Font("Segoe UI", 9F);

            Label lbl = new Label();
            lbl.AutoSize = false;
            lbl.Location = new Point(12, 12);
            lbl.Size = new Size(476, 48);
            lbl.Text = "This is a Microsoft Office file. Enter your Ghostscript.NET.Office license key to view the full document. Without a key, only the first 3 pages are shown.";

            Label lblKey = new Label();
            lblKey.AutoSize = true;
            lblKey.Location = new Point(12, 68);
            lblKey.Text = "License key:";

            _txtKey = new TextBox();
            _txtKey.Location = new Point(12, 88);
            _txtKey.Size = new Size(476, 23);
            _txtKey.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            Button btnOk = new Button();
            btnOk.Text = "OK";
            btnOk.Location = new Point(251, 128);
            btnOk.Size = new Size(75, 25);
            btnOk.Click += BtnOk_Click;

            Button btnRestricted = new Button();
            btnRestricted.Text = "First 3 pages";
            btnRestricted.Location = new Point(332, 128);
            btnRestricted.Size = new Size(87, 25);
            btnRestricted.Click += BtnRestricted_Click;

            Button btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(425, 128);
            btnCancel.Size = new Size(63, 25);

            Controls.Add(lbl);
            Controls.Add(lblKey);
            Controls.Add(_txtKey);
            Controls.Add(btnOk);
            Controls.Add(btnRestricted);
            Controls.Add(btnCancel);

            AcceptButton = btnOk;
            CancelButton = btnCancel;
        }

        public string Key
        {
            get { return _continueWithoutKey ? null : _txtKey.Text.Trim(); }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtKey.Text))
            {
                MessageBox.Show(this,
                    "Please enter a license key, or choose First 3 pages.",
                    Text,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                _txtKey.Focus();
                return;
            }

            _continueWithoutKey = false;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void BtnRestricted_Click(object sender, EventArgs e)
        {
            _continueWithoutKey = true;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

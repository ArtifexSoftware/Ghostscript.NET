// Copyright (C) 2024 Artifex Software, Inc.
//
// This file is part of Ghostscript.NET.
//
// Ghostscript.NET is free software: you can redistribute it and/or modify it 
// under the terms of the GNU Affero General Public License as published by the 
// Free Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// Ghostscript.NET is distributed in the hope that it will be useful, but WITHOUT 
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
// FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more
// details.
//
// You should have received a copy of the GNU Affero General Public License
// along with Ghostscript.NET. If not, see 
// <https://www.gnu.org/licenses/agpl-3.0.en.html>
//
// Alternative licensing terms are available from the licensor.
// For commercial licensing, see <https://www.artifex.com/> or contact
// Artifex Software, Inc., 39 Mesa Street, Suite 108A, San Francisco,
// CA 94129, USA, for further information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Extensions.Logging;
using Ghostscript.NET.PDFA3Converter;

namespace Ghostscript.NET.PDFA3Converter.Samples.ZUGFeRD
{

    public class ZUGFeRDExporter
    {
        protected String? gsDLL = null;
        protected String? sourcePDF = null;
        protected bool noSourceCopy = false;
        protected String profile = "EN16931";
        protected int version = 2;
        protected IExportableTransaction? trans = null;

        public ZUGFeRDExporter(String gsDLL)
        {
            this.gsDLL = gsDLL;
        }
        public ZUGFeRDExporter load(String PDFfilename)
        {
            String basename=Path.GetFileName(PDFfilename);
            this.sourcePDF = Path.GetTempPath() + basename;
            String d1=Path.GetDirectoryName(PDFfilename)+Path.DirectorySeparatorChar;
            String d2=Path.GetTempPath();
            if (d1.Equals(Path.GetTempPath())) {
                noSourceCopy=true;
            } else {
                File.Copy(PDFfilename, sourcePDF);
            }
            return this;
        }
        public ZUGFeRDExporter setTransaction(IExportableTransaction trans)
        {
            this.trans = trans;
            return this;
        }

        public ZUGFeRDExporter setZUGFeRDVersion(int version)
        {
            this.version = version;
            return this;
        }

        public ZUGFeRDExporter setProfile(String profile)
        {
            this.profile = profile;
            return this;
        }


        public void export(String targetFilename)
        {

            Ghostscript.NET.PDFA3Converter.PDFA3Converter pc = new Ghostscript.NET.PDFA3Converter.PDFA3Converter(gsDLL);

            ZUGFeRD2PullProvider zf2p = new ZUGFeRD2PullProvider();
            zf2p.setProfile(Profiles.getByName(profile));
            zf2p.generateXML(trans);
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();

            string tempfilename = Path.GetTempPath() + "\\factur-x.xml";
            File.WriteAllBytes(tempfilename, zf2p.getXML());

            pc.SetZUGFeRDVersion("2.1");
            pc.SetZUGFeRDProfile(Profiles.getByName("EN16931").getXMPName());
            pc.SetEmbeddedXMLFile(tempfilename);
            pc.ConvertToPDFA3(sourcePDF, targetFilename);
            File.Delete(Path.GetTempPath() + "\\factur-x.xml");
            if (!noSourceCopy) {
                File.Delete(sourcePDF);
            }

        }
    }
}

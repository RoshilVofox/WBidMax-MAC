using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace WBid.WBidiPad.Model
{
    [Serializable]
    public class ReportReleases
    {
        public ReportReleases()
        {

        }
        public ReportReleases(ReportReleases reportrelease)
        {
            if (reportrelease != null)
            {
                AllDays = reportrelease.AllDays;
                First = reportrelease.First;
                Last = reportrelease.Last;
                NoMid = reportrelease.NoMid;
                Report = reportrelease.Report;
                Release = reportrelease.Release;
                if (reportrelease.lstParameters != null)
                {
                    lstParameters = new List<ReportRelease>();
                    foreach (ReportRelease item in reportrelease.lstParameters)
                    {
                        lstParameters.Add(new ReportRelease(item));
                    }
                }
            }
        }
        [XmlAttribute("AllDays")]
        public bool AllDays { get; set; }
        [XmlAttribute("First")]
        public bool First { get; set; }
        [XmlAttribute("Last")]
        public bool Last { get; set; }
        [XmlAttribute("NoMid")]
        public bool NoMid { get; set; }
        [XmlAttribute("Report")]
        public int Report { get; set; }
        [XmlAttribute("Release")]
        public int Release { get; set; }

        public List<ReportRelease> lstParameters { get; set; }


    }
    [Serializable]
    public class ReportRelease
    {
        public ReportRelease()
        {

        }
        public ReportRelease(ReportRelease data)
        {
            AllDays = data.AllDays;
            First = data.First;
            Last = data.Last;
            NoMid = data.NoMid;
            Report = data.Report;
            Release = data.Release;
        }
        [XmlAttribute("AllDays")]
        public bool AllDays { get; set; }
        [XmlAttribute("First")]
        public bool First { get; set; }
        [XmlAttribute("Last")]
        public bool Last { get; set; }
        [XmlAttribute("NoMid")]
        public bool NoMid { get; set; }
        [XmlAttribute("Report")]
        public int Report { get; set; }
        [XmlAttribute("Release")]
        public int Release { get; set; }
    }
}

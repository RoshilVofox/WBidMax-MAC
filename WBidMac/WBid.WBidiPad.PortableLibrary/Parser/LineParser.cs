using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
//using WBid.WBidiPad.PortableLibrary.Utility;

namespace WBid.WBidiPad.PortableLibrary.Parser
{
    public class LineParser
    {
        #region Properties
        public Dictionary<string, Line> Lines { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Parse Line file from PS file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="filesToParse"></param>
        /// <returns></returns>
        public Dictionary<string, Line> ParseLines(string fileName, Byte[] filesToParse)
        {
            List<string> lineRecords = new List<string>();
            lineRecords = getLineRecords(filesToParse);
            if (fileName.Substring(0, 1) == "A")
                return Lines = ParseFaLinesFromLineRecords(lineRecords);
            else
                return Lines = ParsePltLinesfromLineRecords(lineRecords);
        }

        private Dictionary<string, Line> ParseFaLinesFromLineRecords(List<string> lineRecords)
        {
            string newLine = string.Empty, oldLine = string.Empty, tripNum = string.Empty;
            int step = 1;
            int reserveSeq = 0;

            Line line = null;
            Trip trip = null;
            DateTime oldDate = new DateTime();
            DateTime newDate = new DateTime();
            Lines = new Dictionary<string, Line>();

            foreach (var record in lineRecords)
            {
                newLine = record.Substring(Defines.GLC_LINENUM - 1, 3);
                if (newLine == oldLine && record.Substring(0, 1) != "T" && record.Substring(0, 1) != "A")
                {
                    if (!line.FAPositions.Contains(GetFaPostion(record)))
                        line.FAPositions.Add(GetFaPostion(record));
                }
                else
                {
                    // C record gives lineNum, tfp, and blockHours.

                    if (record.Substring(0, 1) == "C") // *** I need to deal with A, B, C and D annotation in C record
                    {
                        if (line != null)
                            Lines.Add(line.LineNum.ToString(), line);
                        line = new Line();
                        line.LineNum = Convert.ToInt16(newLine);
                        line.LineDisplay = line.LineNum.ToString();
                        line.Tfp = Convert.ToDecimal(record.Substring(Defines.GLC_CREDIT - 1, 5)) / 100m;
                        line.Block = Convert.ToInt32(record.Substring(Defines.GLC_BLK - 1, 5));
                        line.FAPositions.Add(GetFaPostion(record));
                        oldLine = newLine.ToString();
                    }
                    if (record.Substring(0, 1) == "T")
                    {
                        string pairNum, pairDay;
                        pairNum = record.Substring(Defines.GLT_PAIRING_1 - 1, 4);
                        pairDay = record.Substring(Defines.GLT_STARTDATE_1 - 1, 2);
						string month = record.Substring(Defines.GLT_STARTDATE_1 + 1, 3);
                        pairNum = pairNum + pairDay;
                        pairNum = pairNum.Replace(" ", "0");
                        //if (!line.Pairings.Contains(pairNum))
						if (!line.Pairings.Contains(pairNum) || (month == "MAR" && pairDay == "01" && line.Pairings[line.Pairings.Count - 1] != pairNum))
                            line.Pairings.Add(pairNum);
                        if (record.Substring(31, 1) != " ")
                        {
                            pairNum = record.Substring(Defines.GLT_PAIRING_2 - 1, 4);
                            pairDay = record.Substring(Defines.GLT_STARTDATE_2 - 1, 2);
							month = record.Substring(Defines.GLT_STARTDATE_2 + 1, 3);
                            pairNum = pairNum + pairDay;
                            pairNum = pairNum.Replace(" ", "0");
                            //if (!line.Pairings.Contains(pairNum))
							if (!line.Pairings.Contains(pairNum) || (month == "MAR" && pairDay == "01" && line.Pairings[line.Pairings.Count - 1] != pairNum))
                                line.Pairings.Add(pairNum);
                        }
                        if (record.Substring(50, 1) != " ")
                        {
                            pairNum = record.Substring(Defines.GLT_PAIRING_3 - 1, 4);
                            pairDay = record.Substring(Defines.GLT_STARTDATE_3 - 1, 2);
							month = record.Substring(Defines.GLT_STARTDATE_3 + 1, 3);
                            pairNum = pairNum + pairDay;
                            pairNum = pairNum.Replace(" ", "0");
                            //if (!line.Pairings.Contains(pairNum))
							if (!line.Pairings.Contains(pairNum) || (month == "MAR" && pairDay == "01" && line.Pairings[line.Pairings.Count - 1] != pairNum))
                                line.Pairings.Add(pairNum);
                        }
                    }
                    if (record.Substring(0, 1) == "A")
                    {
                        // spot 1 = "A" is reserve, spot 4-7 = linenum, spot 13-15 = FAR, FPR, FRR, 18-19=Day, 20-22=Month, 23-24=yy, 25-28 = report herb, repeat 18-28 in 29-39 for release

                        DutyPeriod dutyPeriod = new DutyPeriod();
                        line.ReserveLine = true;
                        newDate = DateTime.ParseExact(record.Substring(17, 7), "ddMMMyy", CultureInfo.InvariantCulture);

                        if (newDate.AddDays(-1) != oldDate)
                        {
                            if (trip != null)
                            {
                                line.Pairings.Add(trip.TripNum.Replace(" ", "0"));

                            }
                            reserveSeq++;
                            trip = new Trip();
                            trip.TripNum = record.Substring(12, 3) + reserveSeq.ToString();
                        }

                        dutyPeriod.ShowTime = Convert.ToInt16(record.Substring(24, 2)) * 60 + Convert.ToInt16(record.Substring(26, 2));
                        dutyPeriod.ReleaseTime = Convert.ToInt16(record.Substring(35, 2)) * 60 + Convert.ToInt16(record.Substring(37, 2));
                        dutyPeriod.Tfp = 6.0m;
                       // line.TypeFaReserveLine = record.Substring(12, 3);
                        trip.DutyPeriods.Add(dutyPeriod);
                    }
                }
            }
            Lines.Add(line.LineNum.ToString(), line);
            return Lines;
        }
        #endregion

        #region Private Methods

        private string GetFaPostion(string record)
        {
            string position = record.Substring(Defines.GLC_CREWCODE - 1, 2);
            if (position == "00") return "A";
            if (position == "01") return "B";
            if (position == "02") return "C";
            if (position == "03") return "D";
            return "";
        }

        /// <summary>
        /// convert the byte data for parsed data to list of records( each individual line from LIne file is added to LineRecords)
        /// </summary>
        /// <param name="fileToParse"></param>
        /// <returns></returns>
        /// Frank changed to pubic so can access from SWACredentialViewModel.cs

        public List<string> getLineRecords(Byte[] fileToParse)
        {
            string record;
            List<string> LineRecords = new List<string>();
            Stream stream = new MemoryStream(fileToParse);
            using (StreamReader reader = new StreamReader(stream))
            {
                while (!reader.EndOfStream)
                {
                    record = reader.ReadLine();
                    if (record.Substring(0, 2) != "*E")
                        LineRecords.Add(record);  // each individual line from Trips file is added to TripRecords (rec1,2,3,5,6 etc)
                }
            }
            return LineRecords;
        }

        /// <summary>
        /// Parse lines from line records retirived from ps file.
        /// </summary>
        /// <param name="LineRecords"></param>
        /// <returns></returns>
        private Dictionary<string, Line> ParsePltLinesfromLineRecords(List<string> LineRecords)
        {
            string newLine = string.Empty, oldLine = string.Empty, tripNum = string.Empty;
            int step = 1;

            Line line = null;
            Lines = new Dictionary<string, Line>();

            foreach (var record in LineRecords)
            {
                // handle case where line is on two records
                newLine = record.Substring(0, 6);
                step = 1;
                if (oldLine != newLine)
                {
                    line = new Line();
                    line.LineNum = Convert.ToInt16(record.Substring(0, 6));
                    line.ReserveLine = record.Substring(6, 1) == "R" ? true : false;
                    line.BlankLine = record.Substring(6, 1) == " " ? true : false;
                    if (line.ReserveLine)
                    {
                        line.LineDisplay = line.LineNum.ToString() + "-R";
                    }
                    else if (line.BlankLine)
                    {
                        line.LineDisplay = line.LineNum.ToString() + "-B";
                    }
                    else
                    {
                        line.LineDisplay = line.LineNum.ToString();
                    }
                    while (record.Substring(step * 6 + 8, 2) != " 0" && step < 11)
                    {
                        tripNum = record.Substring(step * 6 + 4, 6);
                        tripNum = tripNum.Replace(" ", "0");// stored as Trip name and Date nnnndd
                        line.Pairings.Add(tripNum);
                        step += 1;
                        var reserveletter = tripNum.Substring(1, 1);
                        if (reserveletter == "W" || reserveletter == "X" || reserveletter == "Y")
                            line.LineDisplay = line.LineNum.ToString() + "-R";

                    }
                    if (record.Length > 80)
                    {
                        line.ETOPS = record.Substring(80, 1).ToLower() == "e" ? true : false;
                        if(line.ETOPS)
                        line.LineDisplay += "e";
                    }
                    line.Tfp = Math.Round(Convert.ToDecimal(record.Substring(71, 3)) + Convert.ToDecimal(record.Substring(74, 2)) / 60m, 2);
                    line.Block = Convert.ToInt32(record.Substring(76, 4));
                    Lines.Add(line.LineNum.ToString(), line);
                }
                else
                {
                    // if the line is in two records
                    if (line == null) continue;

                    while (record.Substring(step * 6 + 7, 3) != "0 0")
                    {
                        tripNum = record.Substring(step * 6 + 4, 6);  // stored as Trip name and Date nnnndd
                        tripNum = tripNum.Replace(" ", "0");
                        line.Pairings.Add(tripNum);
                        step += 1;
                    }
                }
                oldLine = newLine;

            }
            return Lines;

        }


        #endregion

        public void CreateFaReserveTrips(List<string> faPsRecords, Dictionary<string, Trip> trips, Dictionary<string, Line> lines)
        {
            bool dateBreak = false;
            Line line = new Line();
            Trip trip = new Trip();
            DutyPeriod dutyPeriod = new DutyPeriod();
            DateTime oldDate, newDate;
            oldDate = new DateTime();
            string tripName = "";
            int tripCounter = 0;
            foreach (var record in faPsRecords)
            {
                if (record.Substring(0, 1) == "A")
                {

                    switch (record.Substring(12, 3))
                    {
                        case "BAR":
                            tripName = "XA";
                            break;
                        case "BPR":
                            tripName = "XP";
                            break;
                        case "BRR":
                            tripName = "XR";
                            break;
                        default:
                            break;
                    }
                    dutyPeriod = new DutyPeriod();
                    newDate = DateTime.ParseExact(record.Substring(17, 7), "ddMMMyy", CultureInfo.InvariantCulture);
                    dateBreak = oldDate.AddDays(1) != newDate;
                    oldDate = newDate;
                    if (dateBreak)
                    {
                        if (!checkTripExist(trip)) // true if trip does NOT exist (compare length of trip, type of trip, and each dp report and release time0
                        {
                            trip.TripNum = tripName + tripCounter.ToString();
                            tripCounter++;
                            trips.Add(trip.TripNum, trip);
                            lines[line.LineNum.ToString()].Pairings.Add(trip.TripNum + trip.Date.Replace(' ', '0').PadLeft(2, '0'));
                            //lines[line.LineNum.ToString()].Pairings.Add(trip.TripNum + trip.Date);
                            trip = new Trip();
                            trip.Date = newDate.Day.ToString();
                            trip.TypeFaReserveTrip = record.Substring(12, 3);
                        }
                    }
                    dutyPeriod.ShowTime = Convert.ToInt16(record.Substring(24, 2)) * 60 + Convert.ToInt16(record.Substring(26, 2));
                    dutyPeriod.ReleaseTime = Convert.ToInt16(record.Substring(35, 2)) * 60 + Convert.ToInt16(record.Substring(37, 2));
                }

                if (record.Substring(0, 1) == "T")
                {
                    // get pairingNum -- method
                    // get date      16,7    35,7    54,7
                    // get position  30,1    49,1    68,1
                    // ???? add pairing to line.pairings
                }

                if (record.Substring(0, 1) == "C")
                {
                    break;
                    if (!(line == null))
                        lines.Add(line.LineNum.ToString(), line);
                    line = new Line();
                    // get lineNumber 2,5
                    // get TFP 12,5
                    // get Block 17,5
                }
            }
        }

        private bool getDateBreak()
        {
            throw new NotImplementedException();
        }

        private bool checkTripExist(Trip trip)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WBid.WBidiPad.Model;
using HtmlAgilityPack;

//using System.Xml;
namespace WBid.WBidiPad.PortableLibrary.Parser
{
    public class PraseScrapedTripDetails
    {

        public Trip ParseTripDetails(string tripName, string htmlString, int show1stDay, int showAfter1stDay)
        {
            try
            {





                Trip trip = new Trip();
                int seqenceNumber = 1;

                HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
                //document.Load(WebBrowser2.Document.Body.InnerHtml);
                document.LoadHtml(htmlString);


                string startDate = string.Empty;
                //Read Title Section
                //------------------------------------
                HtmlAgilityPack.HtmlNode titleNode = document.DocumentNode.SelectSingleNode("//div//div[@class='printTitle']");
                startDate = titleNode.InnerText.Substring(titleNode.InnerText.Length - 10, 10);

                HtmlAgilityPack.HtmlNode pairingTable = document.DocumentNode.SelectSingleNode("//div//table[@class='pairingTable']");


                int rowsCount = 0;




                // HtmlAgilityPack.HtmlNodeCollection pairingCollection = document.DocumentNode.SelectSingleNode("//div//table[@class='pairingTable']").SelectNodes("tr");

                List<HtmlNode> pairingCollection = pairingTable.Element("tbody").Elements("tr").ToList();

                if (pairingCollection != null)
                {
                    rowsCount = pairingCollection.Count;
                }
                //document.DocumentNode.SelectSingleNode("//div//table[@class='pairingTable']").Element("tbody").Elements("tr").ToList();

                trip.StartOp = int.Parse(startDate.Substring(3, 2));
                trip.TripNum = tripName;//titleNode.InnerText.Substring(5, 4);

                //Get last operational day
                trip.EndOp = int.Parse(tripName.Substring(4, 2));

                //int rowsCount = 15;




                int colPosition = 0;


                // Read the header column order. So that we can use this position while reading the content
                List<PairingStructure> pairingColumnOrder = new List<PairingStructure>();
                pairingColumnOrder = GetHeaderColumnDeatails(pairingCollection[1]);

                PairingStructure currentColumn = null; ;
                int leftPanelColumnCount = 11;
                //if (pairingTable.SelectNodes(".//tr")[rowsCount - 2].SelectNodes("td")[0].Attributes["colSpan"] != null)

                var ss = pairingCollection[rowsCount - 2].Elements("td").ToList();

                var sss = pairingCollection[rowsCount - 2].Elements("td").ToList()[0];

                if (pairingCollection[rowsCount - 2].Elements("td").ToList()[0].Attributes["colSpan"] != null)
                {
                    //leftPanelColumnCount = int.Parse(pairingTable.SelectNodes(".//tr")[rowsCount - 2].SelectNodes("td")[0].Attributes["colSpan"].Value) - 1;
                    leftPanelColumnCount = int.Parse(pairingCollection[rowsCount - 2].Elements("td").ToList()[0].Attributes["colSpan"].Value) - 1;
                }


                //Read trip footer details
                //------------------------------------
                //HtmlAgilityPack.HtmlNode footerTable = pairingTable.SelectNodes("tr")[rowsCount - 1].SelectNodes("td")[1].SelectSingleNode("table");

                //Total Block
                //trip.Block = ConvertHhmmToMinutes(footerTable.SelectNodes("tr")[0].SelectNodes("td")[1].InnerText);
                currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Block" && x.Position >= 9);
                colPosition = (currentColumn == null) ? 2 : (currentColumn.ContentPosition - leftPanelColumnCount);
                //trip.Block = ConvertHhmmToMinutes(pairingTable.SelectNodes(".//tr")[rowsCount - 2].SelectNodes("td")[colPosition].InnerText);

                trip.Block = ConvertHhmmToMinutes(pairingCollection[rowsCount - 2].Elements("td").ToList()[colPosition].InnerText);





                //Total dutyTime
                //trip.DutyTime = ConvertHhmmToMinutes(footerTable.SelectNodes("tr")[0].SelectNodes("td")[2].InnerText);
                currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Duty");
                colPosition = (currentColumn == null) ? 4 : (currentColumn.ContentPosition - leftPanelColumnCount); ;
                // trip.DutyTime = ConvertHhmmToMinutes(pairingTable.SelectNodes(".//tr")[rowsCount - 2].SelectNodes("td")[colPosition].InnerText);
                trip.DutyTime = ConvertHhmmToMinutes(pairingCollection[rowsCount - 2].Elements("td").ToList()[colPosition].InnerText);
                //Total credit
                //trip.Tfp = Math.Round(decimal.Parse(footerTable.SelectNodes("tr")[0].SelectNodes("td")[3].InnerText) / 100, 2);
                currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Credit");
                colPosition = (currentColumn == null) ? 5 : (currentColumn.ContentPosition - leftPanelColumnCount);
                // trip.Tfp = Math.Round(decimal.Parse(pairingTable.SelectNodes(".//tr")[rowsCount - 2].SelectNodes("td")[colPosition].InnerText) / 100, 2);
                trip.Tfp = Math.Round(decimal.Parse(pairingCollection[rowsCount - 2].Elements("td").ToList()[colPosition].InnerText) / 100, 2);

                //Carry Block
                // trip.CarryOverBlock = ConvertHhmmToMinutes(footerTable.SelectNodes("tr")[1].SelectNodes("td")[1].InnerText);
                currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Block" && x.Position >= 9);
                colPosition = (currentColumn == null) ? 1 : (currentColumn.ContentPosition - leftPanelColumnCount);
                //trip.CarryOverBlock = ConvertHhmmToMinutes(pairingTable.SelectNodes(".//tr")[rowsCount - 1].SelectNodes("td")[colPosition - 1].InnerText);
                trip.CarryOverBlock = ConvertHhmmToMinutes(pairingCollection[rowsCount - 1].Elements("td").ToList()[colPosition - 1].InnerText);

                //Carry over credit
                // trip.CarryOverTfp = Math.Round(decimal.Parse(footerTable.SelectNodes("tr")[1].SelectNodes("td")[3].InnerText));
                currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Credit");
                colPosition = (currentColumn == null) ? 3 : (currentColumn.ContentPosition - leftPanelColumnCount);
                //  trip.CarryOverTfp = Math.Round(decimal.Parse(pairingTable.SelectNodes(".//tr")[rowsCount - 1].SelectNodes("td")[colPosition - 1].InnerText));

                trip.CarryOverTfp = Math.Round(decimal.Parse(pairingCollection[rowsCount - 1].Elements("td").ToList()[colPosition - 1].InnerText));

                //------------------------------------


                //Parse DutyPeriods and flights
                //First two rows are Totals and title lines-- so we dont need to consider those lines
                //Also last two lines not need to consider while paring dutyperiod deatsils 
                // string  

                ParseStatus status = ParseStatus.NotStarted;
                DutyPeriod dutyPeriod = null;


                for (int count = 2; count < rowsCount - 2; count++)
                {

                    //HtmlAgilityPack.HtmlNode trNode = pairingTable.SelectNodes(".//tr")[count];
                    HtmlAgilityPack.HtmlNode trNode = pairingTable.Element("tbody").Elements("tr").ToList()[count];
                    //Rpt line parsing
                    if (status == ParseStatus.NotStarted)
                    {
                        string rpt = string.Empty;
                        dutyPeriod = new DutyPeriod();
                        dutyPeriod.TripNum = tripName;
                        // if (trNode.SelectNodes("td").Count > 1)      
                        if (trNode.Elements("td").ToList().Count > 2)
                        {

                            //Read DEPART column
                            //--------------------------------------
                            currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Depart");
                            colPosition = (currentColumn == null) ? 2 : currentColumn.Position;

                            // rpt = trNode.SelectNodes("td")[colPosition].InnerText;  
                            rpt = trNode.Elements("td").ToList()[colPosition].InnerText;
                            if (rpt.Contains("Rpt"))
                            {
                                //int depTimeFirstLeg = int.Parse(rpt.Replace("Rpt", "").Replace("&nbsp;", "")) + 1440 * trip.DutyPeriods.Count;
                                dutyPeriod.DepTimeFirstLeg = ConvertHhmmToMinutes(rpt.Replace("Rpt", "").Replace("&nbsp;", "")) + 1440 * trip.DutyPeriods.Count;
                                status = ParseStatus.DutyPeriodStarted;
                            }
                        }

                    }
                    //Flight line
                    else if (status == ParseStatus.DutyPeriodStarted)
                    {

                        if (trNode.Attributes["class"] != null && (trNode.Attributes["class"].Value == "" || trNode.Attributes["class"].Value == "legWarning" || trNode.Attributes["class"].Value == "red"))
                        {
                            Flight flight = new Flight();
                            currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Flight");
                            try
                            {
                                string deadHead = trNode.SelectNodes("td")[1].InnerText.Trim();
                                if (deadHead.Contains("DM") || deadHead.Contains("DH"))
                                {
                                    flight.DeadHead = true;
                                }
                            }
                            catch (Exception)
                            {


                            }
                            colPosition = (currentColumn == null) ? 2 : currentColumn.ContentPosition;
                            // flight.FltNum = int.Parse(trNode.SelectNodes("td")[colPosition].SelectSingleNode("a").InnerText);
                            try
                            {
                                flight.FltNum = int.Parse(trNode.Elements("td").ToList()[colPosition].SelectSingleNode("a").InnerText);
                            }
                            catch (Exception ex)
                            {
                                flight.FltNum = 0;
                            }

                            currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Depart");
                            colPosition = (currentColumn == null) ? 3 : currentColumn.ContentPosition;
                            //flight.DepSta = trNode.SelectNodes("td")[colPosition].InnerText.Substring(0, 3);
							flight.DepSta = trNode.Elements("td").ToList()[colPosition].InnerText.Trim().Substring(0, 3);
                            //int depTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].SelectSingleNode("span").InnerText);  
                            int depTime = ConvertHhmmToMinutes(trNode.Elements("td").ToList()[colPosition].SelectSingleNode("span").InnerText);
                            flight.DepTime = depTime + 1440 * trip.DutyPeriods.Count;

                            //Ranish--- To solve the Block time issue in trip details view.
                            //if the trip passes the mid night we found that there is an issue in arrTime.

                            // flight.DepTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].SelectSingleNode("span").InnerText) + 1440 * trip.DutyPeriods.Count;


                            currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Arrive");
                            colPosition = (currentColumn == null) ? 4 : currentColumn.ContentPosition;

                            //flight.ArrSta = trNode.SelectNodes("td")[colPosition].InnerText.Substring(0, 3);   
							flight.ArrSta = trNode.Elements("td").ToList()[colPosition].InnerText.Trim().Substring(0, 3);
                            // flight.ArrTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].SelectSingleNode("span").InnerText) + 1440 * trip.DutyPeriods.Count;
                            //int arrTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].SelectSingleNode("span").InnerText);
                            int arrTime = ConvertHhmmToMinutes(trNode.Elements("td").ToList()[colPosition].SelectSingleNode("span").InnerText);
                            if (arrTime < depTime)
                            {
                                arrTime = arrTime + 1440;
                            }

                            flight.ArrTime = arrTime + 1440 * trip.DutyPeriods.Count;

                            currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Eq");
                            colPosition = (currentColumn == null) ? 5 : currentColumn.ContentPosition;
                            // flight.Equip = trNode.SelectNodes("td")[colPosition].InnerText;
                            flight.Equip = trNode.Elements("td").ToList()[colPosition].InnerText;
                            currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Block");
                            colPosition = (currentColumn == null) ? 6 : currentColumn.ContentPosition;
                            // flight.Block = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].InnerText.Replace("\t", "").Replace("\n", "").Replace("\r", "").PadLeft(4, '0'));
                            flight.Block = ConvertHhmmToMinutes(trNode.Elements("td").ToList()[colPosition].InnerText.Replace("\t", "").Replace("\n", "").Replace("\r", "").PadLeft(4, '0').Trim());
                            //flight.Reg = trNode.SelectNodes("td")[7].InnerText;
                            currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Reg");
                            colPosition = (currentColumn == null) ? 9 : currentColumn.ContentPosition;
                            // flight.Reg = trNode.SelectNodes("td")[colPosition].InnerText;

                            flight.Reg = trNode.Elements("td").ToList()[colPosition].InnerText;

                            //flight.TurnTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[8].InnerText.PadLeft(4,'0'));
                            currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Ground");
                            colPosition = (currentColumn == null) ? 10 : currentColumn.ContentPosition;
                            //flight.TurnTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].InnerText);
                            flight.TurnTime = ConvertHhmmToMinutes(trNode.Elements("td").ToList()[colPosition].InnerText);
                            //flight.Tfp = Math.Round(decimal.Parse(trNode.SelectNodes("td")[12].InnerText) / 100, 2);

                            currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Credit");
                            colPosition = (currentColumn == null) ? 15 : currentColumn.ContentPosition;
                            //flight.Tfp = Math.Round(decimal.Parse(trNode.SelectNodes("td")[colPosition].InnerText) / 100, 2);
                            flight.Tfp = Math.Round(decimal.Parse(trNode.Elements("td").ToList()[colPosition].InnerText) / 100, 2);
                            flight.FlightSeqNum = dutyPeriod.Flights.Count + 1;
                            dutyPeriod.Flights.Add(flight);


                        }
                        else
                        {    //Line under the Flights. That is "Rls" line
                            string rls = string.Empty;
                            currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Arrive");
                            colPosition = (currentColumn == null) ? 3 : currentColumn.Position;
                            //rls = trNode.SelectNodes("td")[colPosition].InnerText;
                            rls = trNode.Elements("td").ToList()[colPosition].InnerText;
                            if (rls.Contains("Rls"))
                            {
                                //dutyPeriod.Block = ConvertHhmmToMinutes(trNode.SelectNodes("td")[9].InnerText.PadLeft(4, '0'));
                                currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Block" && x.Position >= 9);
                                colPosition = (currentColumn == null) ? 11 : currentColumn.Position;
                                //dutyPeriod.Block = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].InnerText.PadLeft(4, '0'));
                                dutyPeriod.Block = ConvertHhmmToMinutes(trNode.Elements("td").ToList()[colPosition].InnerText.PadLeft(4, '0'));
                                //dutyPeriod.DutyTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[10].InnerText.PadLeft(4, '0'));

                                try
                                {
                                    currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "FDP");
                                    colPosition = (currentColumn == null) ? 12 : currentColumn.Position;
                                    //dutyPeriod.FDP = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].InnerText);
                                    dutyPeriod.FDP = ConvertHhmmToMinutes(trNode.Elements("td").ToList()[colPosition].InnerText);

                                }
                                catch (Exception)
                                {


                                }


                                currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Duty");
                                colPosition = (currentColumn == null) ? 13 : currentColumn.Position;

                               // dutyPeriod.DutyTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].InnerText.PadLeft(4, '0'));
                                            dutyPeriod.DutyTime = ConvertHhmmToMinutes(trNode.Elements("td").ToList()[colPosition].InnerText.PadLeft(4, '0'));
                                //dutyPeriod.Tfp = decimal.Parse(trNode.SelectNodes("td")[11].InnerText) / 100;
                                currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Credit");
                                colPosition = (currentColumn == null) ? 14 : currentColumn.Position;
                                //dutyPeriod.Tfp = decimal.Parse(trNode.SelectNodes("td")[colPosition].InnerText) / 100;
                                dutyPeriod.Tfp = decimal.Parse(trNode.Elements("td").ToList()[colPosition].InnerText) / 100;
                                // int landTimeLastLeg = int.Parse(rls.Replace("Rls", "").Replace("&nbsp;", "")) + 1440 * trip.DutyPeriods.Count;

                                dutyPeriod.LandTimeLastLeg = ConvertHhmmToMinutes(rls.Replace("Rls", "").Replace("&nbsp;", "")) + 1440 * trip.DutyPeriods.Count;

                                dutyPeriod.DutPerSeqNum = trip.DutyPeriods.Count + 1;

                                dutyPeriod.ArrStaLastLeg = dutyPeriod.Flights[dutyPeriod.Flights.Count - 1].ArrSta;

                                dutyPeriod.DutPerSeqNum = seqenceNumber;

                                dutyPeriod.ShowTime = dutyPeriod.Flights[0].DepTime - ((dutyPeriod.DutPerSeqNum == 1) ? show1stDay : showAfter1stDay);
                                seqenceNumber++;
                                trip.DutyPeriods.Add(dutyPeriod);
                                status = ParseStatus.DutyPeriodEnd;
                            }

                        }

                    }
                    //Line between two duty periods
                    else if (status == ParseStatus.DutyPeriodEnd)
                    {
                        status = ParseStatus.NotStarted;
                    }
                }

                trip.DepSta = trip.DutyPeriods[0].Flights[0].DepSta;
                // trip.DepTime = trip.DutyPeriods[0].DepTimeFirstLeg.ToString();
                trip.DepTime = ConvertMinuteToHHMM(trip.DutyPeriods[0].Flights[0].DepTime).Replace(":", "");
                //last dutyperiod
                DutyPeriod lastDutPeriod = trip.DutyPeriods[trip.DutyPeriods.Count - 1];
                trip.RetSta = lastDutPeriod.Flights[lastDutPeriod.Flights.Count - 1].ArrSta;
                // trip.RetTime = Convert.ToString(lastDutPeriod.LandTimeLastLeg + 1440 * (lastDutPeriod.Flights.Count - 1));
                Flight lastFlight = trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights[trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights.Count - 1];
                trip.RetTime = ConvertMinuteToHHMM(lastFlight.ArrTime - (1440 * (trip.DutyPeriods.Count - 1))).Replace(":", "");
                trip.PairLength = trip.DutyPeriods.Count;
                trip.OpDays = new String(' ', 6);
                trip.FreqCode = "X";
                trip.NonOpDays = new String(' ', 8);
                trip.FDP = trip.DutyPeriods.Sum(x => x.FDP);

                HtmlAgilityPack.HtmlNode tafbNode = document.DocumentNode.SelectNodes("//td[@class='informationTable']").FirstOrDefault(x => x.InnerText.Contains("Time Away From Base:"));
                //trip.Tafb = ConvertHhmmToMinutes(tafbNode.SelectNodes("table/tr/td")[1].InnerHtml.PadLeft(4, '0'));
                trip.Tafb = ConvertHhmmToMinutes(tafbNode.Element("table").Element("tbody").Elements("tr").ToList()[0].Elements("td").ToList()[1].InnerHtml.PadLeft(4, '0'));
                // trip.Tafb = ConvertHhmmToMinutes(document.DocumentNode.SelectNodes("//div//table")[0].SelectNodes("tr")[1].SelectNodes("td")[1].SelectNodes("table//tr//td")[1].InnerHtml.PadLeft(4, '0'));

                trip.BriefTime = show1stDay;
                // Add  trip details to trip
                // _tripdata.Add(trip.TripNum, trip);
                return trip;

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }

		public Trip ParseTripDetailsForFA(string tripName, string htmlString, int show1stDay, int showAfter1stDay)
		{
			try
			{





				Trip trip = new Trip();
				int seqenceNumber = 1;

				HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
				//document.Load(WebBrowser2.Document.Body.InnerHtml);
				document.LoadHtml(htmlString);


				string startDate = string.Empty;
				//Read Title Section
				//------------------------------------
				HtmlAgilityPack.HtmlNode titleNode = document.DocumentNode.SelectSingleNode("//div//div[@class='printTitle']");
				startDate = titleNode.InnerText.Substring(titleNode.InnerText.Length - 10, 10);

				HtmlAgilityPack.HtmlNode pairingTable = document.DocumentNode.SelectSingleNode("//div//table[@class='pairingTable']");


				int rowsCount = 0;




				// HtmlAgilityPack.HtmlNodeCollection pairingCollection = document.DocumentNode.SelectSingleNode("//div//table[@class='pairingTable']").SelectNodes("tr");

				List<HtmlNode> pairingCollection = pairingTable.Element("tbody").Elements("tr").ToList();

				if (pairingCollection != null)
				{
					rowsCount = pairingCollection.Count;
				}
				//document.DocumentNode.SelectSingleNode("//div//table[@class='pairingTable']").Element("tbody").Elements("tr").ToList();

				trip.StartOp = int.Parse(startDate.Substring(3, 2));
				trip.TripNum = tripName;//titleNode.InnerText.Substring(5, 4);

				//Get last operational day
				trip.EndOp = int.Parse(tripName.Substring(4, 2));

				//int rowsCount = 15;




				int colPosition = 0;


				// Read the header column order. So that we can use this position while reading the content
				List<PairingStructure> pairingColumnOrder = new List<PairingStructure>();
				pairingColumnOrder = GetHeaderColumnDeatails(pairingCollection[1]);

				PairingStructure currentColumn = null; ;
				int leftPanelColumnCount = 11;
				//if (pairingTable.SelectNodes(".//tr")[rowsCount - 2].SelectNodes("td")[0].Attributes["colSpan"] != null)

				var ss = pairingCollection[rowsCount - 2].Elements("td").ToList();

				var sss = pairingCollection[rowsCount - 2].Elements("td").ToList()[0];

				if (pairingCollection[rowsCount - 2].Elements("td").ToList()[0].Attributes["colSpan"] != null)
				{
					//leftPanelColumnCount = int.Parse(pairingTable.SelectNodes(".//tr")[rowsCount - 2].SelectNodes("td")[0].Attributes["colSpan"].Value) - 1;
					leftPanelColumnCount = int.Parse(pairingCollection[rowsCount - 2].Elements("td").ToList()[0].Attributes["colSpan"].Value) - 1;
				}


				//Read trip footer details
				//------------------------------------
				//HtmlAgilityPack.HtmlNode footerTable = pairingTable.SelectNodes("tr")[rowsCount - 1].SelectNodes("td")[1].SelectSingleNode("table");

				//Total Block
				//trip.Block = ConvertHhmmToMinutes(footerTable.SelectNodes("tr")[0].SelectNodes("td")[1].InnerText);
				//currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Block" && x.Position >= 9);
				//colPosition = (currentColumn == null) ? 2 : (currentColumn.ContentPosition - leftPanelColumnCount);
				//trip.Block = ConvertHhmmToMinutes(pairingTable.SelectNodes(".//tr")[rowsCount - 2].SelectNodes("td")[colPosition].InnerText);

				//trip.Block = ConvertHhmmToMinutes(pairingCollection[rowsCount - 2].Elements("td").ToList()[colPosition].InnerText);

				//var a = pairingCollection[rowsCount - 1].Elements("td").ToList();
				//var b = pairingCollection[rowsCount - 1].Elements("td").ToList()[2].Elements("table").ToList();
				List<HtmlNode> innertable = pairingCollection[rowsCount - 1].Elements("td").ToList()[2].Elements("table").ToList()[0].Elements("tbody").ToList();
				var totalvalues = innertable[0].Elements("tr").ToList()[0].Elements("td").ToList();
				trip.Block = ConvertHhmmToMinutes(totalvalues[1].InnerHtml);
				trip.DutyTime = ConvertHhmmToMinutes(totalvalues[2].InnerHtml);
				trip.Tfp = Math.Round(decimal.Parse(totalvalues[3].InnerHtml));
				//trip.Block = ConvertHhmmToMinutes(pairingTable.SelectNodes("tr")[rowsCount - 1].SelectNodes("td")[2].SelectNodes("table")[0].SelectNodes("tr")[0].SelectNodes("td")[1].InnerText);


				//trip.DutyTime = ConvertHhmmToMinutes(pairingTable.SelectNodes("tr")[rowsCount - 1].SelectNodes("td")[2].SelectNodes("table")[0].SelectNodes("tr")[0].SelectNodes("td")[2].InnerText);

				//trip.Tfp = Math.Round(decimal.Parse(pairingTable.SelectNodes("tr")[rowsCount - 1].SelectNodes("td")[2].SelectNodes("table")[0].SelectNodes("tr")[0].SelectNodes("td")[2].InnerText) / 100, 2);
				//Total dutyTime
				//trip.DutyTime = ConvertHhmmToMinutes(footerTable.SelectNodes("tr")[0].SelectNodes("td")[2].InnerText);
				//currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Duty");
				// colPosition = (currentColumn == null) ? 4 : (currentColumn.ContentPosition - leftPanelColumnCount); ;
				// trip.DutyTime = ConvertHhmmToMinutes(pairingTable.SelectNodes(".//tr")[rowsCount - 2].SelectNodes("td")[colPosition].InnerText);
				// trip.DutyTime = ConvertHhmmToMinutes(pairingCollection[rowsCount - 2].Elements("td").ToList()[colPosition].InnerText);
				//Total credit
				//trip.Tfp = Math.Round(decimal.Parse(footerTable.SelectNodes("tr")[0].SelectNodes("td")[3].InnerText) / 100, 2);
				//currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Credit");
				// colPosition = (currentColumn == null) ? 5 : (currentColumn.ContentPosition - leftPanelColumnCount);
				// trip.Tfp = Math.Round(decimal.Parse(pairingTable.SelectNodes(".//tr")[rowsCount - 2].SelectNodes("td")[colPosition].InnerText) / 100, 2);
				//trip.Tfp = Math.Round(decimal.Parse(pairingCollection[rowsCount - 2].Elements("td").ToList()[colPosition].InnerText) / 100, 2);

				//Carry Block
				// trip.CarryOverBlock = ConvertHhmmToMinutes(footerTable.SelectNodes("tr")[1].SelectNodes("td")[1].InnerText);
				//currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Block" && x.Position >= 9);
				//colPosition = (currentColumn == null) ? 1 : (currentColumn.ContentPosition - leftPanelColumnCount);
				//trip.CarryOverBlock = ConvertHhmmToMinutes(pairingTable.SelectNodes(".//tr")[rowsCount - 1].SelectNodes("td")[colPosition - 1].InnerText);
				//trip.CarryOverBlock = ConvertHhmmToMinutes(pairingCollection[rowsCount - 1].Elements("td").ToList()[colPosition - 1].InnerText);

				//Carry over credit
				// trip.CarryOverTfp = Math.Round(decimal.Parse(footerTable.SelectNodes("tr")[1].SelectNodes("td")[3].InnerText));
				//currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Credit");
				//colPosition = (currentColumn == null) ? 3 : (currentColumn.ContentPosition - leftPanelColumnCount);
				//  trip.CarryOverTfp = Math.Round(decimal.Parse(pairingTable.SelectNodes(".//tr")[rowsCount - 1].SelectNodes("td")[colPosition - 1].InnerText));

				//trip.CarryOverTfp = Math.Round(decimal.Parse(pairingCollection[rowsCount - 1].Elements("td").ToList()[colPosition - 1].InnerText));

				//------------------------------------


				//Parse DutyPeriods and flights
				//First two rows are Totals and title lines-- so we dont need to consider those lines
				//Also last two lines not need to consider while paring dutyperiod deatsils 
				// string  

				ParseStatus status = ParseStatus.NotStarted;
				DutyPeriod dutyPeriod = null;


				for (int count = 2; count < rowsCount - 2; count++)
				{

					//HtmlAgilityPack.HtmlNode trNode = pairingTable.SelectNodes(".//tr")[count];
					HtmlAgilityPack.HtmlNode trNode = pairingTable.Element("tbody").Elements("tr").ToList()[count];
					//Rpt line parsing
					if (status == ParseStatus.NotStarted)
					{
						string rpt = string.Empty;
						dutyPeriod = new DutyPeriod();
						dutyPeriod.TripNum = tripName;
						// if (trNode.SelectNodes("td").Count > 1)      
						if (trNode.Elements("td").ToList().Count > 2)
						{

							//Read DEPART column
							//--------------------------------------
							currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Depart");
							colPosition = (currentColumn == null) ? 2 : currentColumn.Position;

							// rpt = trNode.SelectNodes("td")[colPosition].InnerText;  
							rpt = trNode.Elements("td").ToList()[colPosition].InnerText;
							if (rpt.Contains("Rpt"))
							{
								//int depTimeFirstLeg = int.Parse(rpt.Replace("Rpt", "").Replace("&nbsp;", "")) + 1440 * trip.DutyPeriods.Count;
								dutyPeriod.DepTimeFirstLeg = ConvertHhmmToMinutes(rpt.Replace("Rpt", "").Replace("&nbsp;", "")) + 1440 * trip.DutyPeriods.Count;
								status = ParseStatus.DutyPeriodStarted;
							}
						}

					}
					//Flight line
					else if (status == ParseStatus.DutyPeriodStarted)
					{

						if (trNode.Attributes["class"] != null && (trNode.Attributes["class"].Value == "" || trNode.Attributes["class"].Value == "legWarning" || trNode.Attributes["class"].Value == "red"))
						{
							Flight flight = new Flight();
							currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Flight");
							try
							{
								string deadHead = trNode.SelectNodes("td")[1].InnerText.Trim();
								if (deadHead.Contains("DM") || deadHead.Contains("DH"))
								{
									flight.DeadHead = true;
								}
							}
							catch (Exception)
							{


							}
							colPosition = (currentColumn == null) ? 2 : currentColumn.ContentPosition;
                            // flight.FltNum = int.Parse(trNode.SelectNodes("td")[colPosition].SelectSingleNode("a").InnerText);
                            try
                            {
                                flight.FltNum = int.Parse(trNode.Elements("td").ToList()[colPosition].SelectSingleNode("a").InnerText);
                            }
                            catch (Exception ex)
                            {
                                flight.FltNum = 0;
                            }

							currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Depart");
							colPosition = (currentColumn == null) ? 3 : currentColumn.ContentPosition;
							//flight.DepSta = trNode.SelectNodes("td")[colPosition].InnerText.Substring(0, 3);
							flight.DepSta = trNode.Elements("td").ToList()[colPosition].InnerText.Trim().Substring(0, 3);
							//int depTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].SelectSingleNode("span").InnerText);  
							int depTime = ConvertHhmmToMinutes(trNode.Elements("td").ToList()[colPosition].SelectSingleNode("span").InnerText);
							flight.DepTime = depTime + 1440 * trip.DutyPeriods.Count;

							//Ranish--- To solve the Block time issue in trip details view.
							//if the trip passes the mid night we found that there is an issue in arrTime.

							// flight.DepTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].SelectSingleNode("span").InnerText) + 1440 * trip.DutyPeriods.Count;


							currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Arrive");
							colPosition = (currentColumn == null) ? 4 : currentColumn.ContentPosition;

							//flight.ArrSta = trNode.SelectNodes("td")[colPosition].InnerText.Substring(0, 3);   
							flight.ArrSta = trNode.Elements("td").ToList()[colPosition].InnerText.Trim().Substring(0, 3);
							// flight.ArrTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].SelectSingleNode("span").InnerText) + 1440 * trip.DutyPeriods.Count;
							//int arrTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].SelectSingleNode("span").InnerText);
							int arrTime = ConvertHhmmToMinutes(trNode.Elements("td").ToList()[colPosition].SelectSingleNode("span").InnerText);
							if (arrTime < depTime)
							{
								arrTime = arrTime + 1440;
							}

							flight.ArrTime = arrTime + 1440 * trip.DutyPeriods.Count;

							currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Eq");
							colPosition = (currentColumn == null) ? 5 : currentColumn.ContentPosition;
							// flight.Equip = trNode.SelectNodes("td")[colPosition].InnerText;
							flight.Equip = trNode.Elements("td").ToList()[colPosition].InnerText;
							currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Block");
							colPosition = (currentColumn == null) ? 6 : currentColumn.ContentPosition;
							// flight.Block = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].InnerText.Replace("\t", "").Replace("\n", "").Replace("\r", "").PadLeft(4, '0'));
							flight.Block = ConvertHhmmToMinutes(trNode.Elements("td").ToList()[colPosition].InnerText.Replace("\t", "").Replace("\n", "").Replace("\r", "").PadLeft(4, '0').Trim());
							//flight.Reg = trNode.SelectNodes("td")[7].InnerText;
							currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Reg");
							colPosition = (currentColumn == null) ? 9 : currentColumn.ContentPosition;
							// flight.Reg = trNode.SelectNodes("td")[colPosition].InnerText;

							flight.Reg = trNode.Elements("td").ToList()[colPosition].InnerText;

							//flight.TurnTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[8].InnerText.PadLeft(4,'0'));
							currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Ground");
							colPosition = (currentColumn == null) ? 10 : currentColumn.ContentPosition;
							//flight.TurnTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].InnerText);
							flight.TurnTime = ConvertHhmmToMinutes(trNode.Elements("td").ToList()[colPosition].InnerText);
							//flight.Tfp = Math.Round(decimal.Parse(trNode.SelectNodes("td")[12].InnerText) / 100, 2);

							currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Credit");
							colPosition = (currentColumn == null) ? 15 : currentColumn.ContentPosition;
							//flight.Tfp = Math.Round(decimal.Parse(trNode.SelectNodes("td")[colPosition].InnerText) / 100, 2);
							flight.Tfp = Math.Round(decimal.Parse(trNode.Elements("td").ToList()[colPosition].InnerText) / 100, 2);
							flight.FlightSeqNum = dutyPeriod.Flights.Count + 1;
							dutyPeriod.Flights.Add(flight);


						}
						else
						{    //Line under the Flights. That is "Rls" line
							string rls = string.Empty;
							currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Arrive");
							colPosition = (currentColumn == null) ? 3 : currentColumn.Position;
							//rls = trNode.SelectNodes("td")[colPosition].InnerText;
							rls = trNode.Elements("td").ToList()[colPosition].InnerText;
							if (rls.Contains("Rls"))
							{
								//dutyPeriod.Block = ConvertHhmmToMinutes(trNode.SelectNodes("td")[9].InnerText.PadLeft(4, '0'));
								currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Block" && x.Position >= 10);
								colPosition = (currentColumn == null) ? 11 : currentColumn.Position;
								//dutyPeriod.Block = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].InnerText.PadLeft(4, '0'));
								dutyPeriod.Block = ConvertHhmmToMinutes(trNode.Elements("td").ToList()[colPosition].InnerText.PadLeft(4, '0'));
								//dutyPeriod.DutyTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[10].InnerText.PadLeft(4, '0'));

								try
								{
									currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "FDP");
									colPosition = (currentColumn == null) ? 12 : currentColumn.Position;
									//dutyPeriod.FDP = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].InnerText);
									dutyPeriod.FDP = ConvertHhmmToMinutes(trNode.Elements("td").ToList()[colPosition].InnerText);

								}
								catch (Exception)
								{


								}


								currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Duty");
								colPosition = (currentColumn == null) ? 13 : currentColumn.Position;

								// dutyPeriod.DutyTime = ConvertHhmmToMinutes(trNode.SelectNodes("td")[colPosition].InnerText.PadLeft(4, '0'));
								dutyPeriod.DutyTime = ConvertHhmmToMinutes(trNode.Elements("td").ToList()[colPosition].InnerText.PadLeft(4, '0'));
								//dutyPeriod.Tfp = decimal.Parse(trNode.SelectNodes("td")[11].InnerText) / 100;
								currentColumn = pairingColumnOrder.FirstOrDefault(x => x.ColumnName == "Credit");
								colPosition = (currentColumn == null) ? 14 : currentColumn.Position;
								//dutyPeriod.Tfp = decimal.Parse(trNode.SelectNodes("td")[colPosition].InnerText) / 100;
								dutyPeriod.Tfp = decimal.Parse(trNode.Elements("td").ToList()[colPosition].InnerText) / 100;
								// int landTimeLastLeg = int.Parse(rls.Replace("Rls", "").Replace("&nbsp;", "")) + 1440 * trip.DutyPeriods.Count;

								dutyPeriod.LandTimeLastLeg = ConvertHhmmToMinutes(rls.Replace("Rls", "").Replace("&nbsp;", "")) + 1440 * trip.DutyPeriods.Count;

								dutyPeriod.DutPerSeqNum = trip.DutyPeriods.Count + 1;

								dutyPeriod.ArrStaLastLeg = dutyPeriod.Flights[dutyPeriod.Flights.Count - 1].ArrSta;

								dutyPeriod.DutPerSeqNum = seqenceNumber;

								dutyPeriod.ShowTime = dutyPeriod.Flights[0].DepTime - ((dutyPeriod.DutPerSeqNum == 1) ? show1stDay : showAfter1stDay);
								seqenceNumber++;
								trip.DutyPeriods.Add(dutyPeriod);
								status = ParseStatus.DutyPeriodEnd;
							}

						}

					}
					//Line between two duty periods
					else if (status == ParseStatus.DutyPeriodEnd)
					{
						status = ParseStatus.NotStarted;
					}
				}

				trip.DepSta = trip.DutyPeriods[0].Flights[0].DepSta;
				// trip.DepTime = trip.DutyPeriods[0].DepTimeFirstLeg.ToString();
				trip.DepTime = ConvertMinuteToHHMM(trip.DutyPeriods[0].Flights[0].DepTime).Replace(":", "");
				//last dutyperiod
				DutyPeriod lastDutPeriod = trip.DutyPeriods[trip.DutyPeriods.Count - 1];
				trip.RetSta = lastDutPeriod.Flights[lastDutPeriod.Flights.Count - 1].ArrSta;
				// trip.RetTime = Convert.ToString(lastDutPeriod.LandTimeLastLeg + 1440 * (lastDutPeriod.Flights.Count - 1));
				Flight lastFlight = trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights[trip.DutyPeriods[trip.DutyPeriods.Count - 1].Flights.Count - 1];
				trip.RetTime = ConvertMinuteToHHMM(lastFlight.ArrTime - (1440 * (trip.DutyPeriods.Count - 1))).Replace(":", "");
				trip.PairLength = trip.DutyPeriods.Count;
				trip.OpDays = new String(' ', 6);
				trip.FreqCode = "X";
				trip.NonOpDays = new String(' ', 8);
				trip.FDP = trip.DutyPeriods.Sum(x => x.FDP);

				HtmlAgilityPack.HtmlNode tafbNode = document.DocumentNode.SelectNodes("//td[@class='informationTable']").FirstOrDefault(x => x.InnerText.Contains("Time Away From Base:"));
				//trip.Tafb = ConvertHhmmToMinutes(tafbNode.SelectNodes("table/tr/td")[1].InnerHtml.PadLeft(4, '0'));
				trip.Tafb = ConvertHhmmToMinutes(tafbNode.Element("table").Element("tbody").Elements("tr").ToList()[0].Elements("td").ToList()[1].InnerHtml.PadLeft(4, '0'));
				// trip.Tafb = ConvertHhmmToMinutes(document.DocumentNode.SelectNodes("//div//table")[0].SelectNodes("tr")[1].SelectNodes("td")[1].SelectNodes("table//tr//td")[1].InnerHtml.PadLeft(4, '0'));

				trip.BriefTime = show1stDay;
				// Add  trip details to trip
				// _tripdata.Add(trip.TripNum, trip);
				return trip;

			}
			catch (Exception ex)
			{

				throw ex;
			}


		}

		#region Private Methods


		/// <summary>
		/// PURPOSE: Get header column  order and position. SO that we can read column deatsil based on this position
		/// (if new column comes, it will nott effect existing parsing
		/// </summary>
		/// <param name="htmlNode"></param>
		/// <returns></returns>
		private List<PairingStructure> GetHeaderColumnDeatails(HtmlNode htmlNode)
        {
            List<PairingStructure> lstColuns = new List<PairingStructure>();
            int count = 0;
            int incrementer = 0;
            if (htmlNode.SelectNodes("td").Count > 0)
            {
                foreach (HtmlAgilityPack.HtmlNode node in htmlNode.SelectNodes("td"))
                {
                    if (node.InnerHtml == "Flight")
                    {
                        lstColuns.Add(new PairingStructure() { ColumnName = node.InnerHtml, Position = count, ContentPosition = incrementer + 1 });
                    }
                    else
                    {

                        lstColuns.Add(new PairingStructure() { ColumnName = node.InnerHtml, Position = count, ContentPosition = incrementer });
                    }
                    count++;
                    incrementer++;
                    if (node.Attributes["colSpan"] != null)
                    {
                        incrementer += int.Parse(node.Attributes["colSpan"].Value) - 1;
                    }
                }
            }

            return lstColuns;

        }


        /// <summary>
        /// PURPOSE :Convert Hour time to minutes
        /// </summary>
        /// <param name="hhmm"></param>
        /// <returns></returns>
        private int ConvertHhmmToMinutes(string hhmm)
        {
            if (hhmm == string.Empty || int.Parse(hhmm) < 0)
                return 0;

            hhmm = hhmm.PadLeft(4, '0');
            int hours = Convert.ToInt32(hhmm.Substring(0, 2));
            int minutes = Convert.ToInt32(hhmm.Substring(2, 2));
            return hours * 60 + minutes;
        }


        public string ConvertMinuteToHHMM(int minute)
        {
            string result = string.Empty;
            result = Convert.ToString(minute / 60).PadLeft(2, '0');
            result += ":";
            result += Convert.ToString(minute % 60).PadLeft(2, '0'); ;
            return result;

        }

        #endregion

        public class PairingStructure
        {
            public string ColumnName { get; set; }

            public int Position { get; set; }

            public int ContentPosition { get; set; }
        }


        public enum ParseStatus
        {
            NotStarted = 0, DutyPeriodStarted = 1, DutyPeriodEnd = 2
        }


    }


}

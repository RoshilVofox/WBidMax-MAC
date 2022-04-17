using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.Core.Enum;
using WBid.WBidiPad.Core;

namespace WBid.WBidiPad.SharedLibrary.Utility
{
    public class XmlHelper
    {
        #region Methods
        /// <summary>
        /// PURPOSE : Save Configuration details to XML
        /// </summary>
        /// <param name="wBidINI"></param>
        /// <returns></returns>
        public static bool SerializeToXml<T>(T configType, string filePath)
        {
            bool status = false;
            try
            {
                XmlWriterSettings xmlWriterSettings;
                XmlSerializerNamespaces xmlSerializerNamespaces;

                xmlWriterSettings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = false,
                    NamespaceHandling = NamespaceHandling.OmitDuplicates,
                    Encoding = Encoding.UTF8,

                };
                xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add("", "");

                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));

                using (FileStream configurationFileStream = new FileStream(filePath, FileMode.Create))
                {

                    using (XmlWriter xmlWriter = XmlWriter.Create(configurationFileStream, xmlWriterSettings))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(T));
                        serializer.Serialize(xmlWriter, configType, xmlSerializerNamespaces);
                    }
                }

                status = true;
            }
            catch (Exception)
            {
                status = false;
            }

            return status;
        }

        /// <summary>
        /// Load configuration details from XML
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static T DeserializeFromXml<T>(string filePath)
        {
            try
            {

                T wBidConfiguration;
                using (TextReader configurationFileStream = new StreamReader(filePath))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                    wBidConfiguration = (T)xmlSerializer.Deserialize(configurationFileStream);
                    return wBidConfiguration;
                }


            }
            catch (Exception ex)
            {
                throw;
            }
        }



        public static WBidStateCollection ReadStateFile(string StatefileName)
        {

            WBidStateCollection wBidStateCollection = XmlHelper.DeserializeFromXml<WBidStateCollection>(StatefileName);

			foreach (WBidState state in wBidStateCollection.StateList) {
                state.CxWtState.Commute.Cx = false;
                state.CxWtState.Commute.Wt = false;
				if (state.CxWtState.CitiesLegs == null) {
					state.CxWtState.CitiesLegs = new StateStatus () { Cx = false, Wt = false };
					state.Constraints.CitiesLegs = new Cx3Parameters {
						ThirdcellValue = "1",
						Type = (int)ConstraintType.LessThan,
						Value = 1 ,
						lstParameters = new List<Cx3Parameter> ()
					};
					state.Weights.CitiesLegs = new Wt2Parameters {
						Type = 1,
						Weight = 0,
						lstParameters = new List<Wt2Parameter> ()
					};
					if (state.Constraints.StartDayOftheWeek.SecondcellValue == null)
					{
						state.Constraints.StartDayOftheWeek.SecondcellValue = "1";
						foreach (var item in state.Constraints.StartDayOftheWeek.lstParameters)
						{
							if (item.SecondcellValue == null)
							{
								item.SecondcellValue = "1";
							}
						}
					}
				}
				if (state.CxWtState.Commute == null) {
					state.CxWtState.Commute = new StateStatus { Wt = false, Cx = false };

					state.Constraints.Commute = new Commutability {
						BaseTime = 10,
						ConnectTime = 30,
						CheckInTime = 60,
						SecondcellValue = (int)CommutabilitySecondCell.NoMiddle,
						ThirdcellValue = (int)CommutabilityThirdCell.Overall,
						Type = (int)ConstraintType.MoreThan,
						Value = 100
					};


					state.Weights.Commute = new Commutability {
						BaseTime = 10,
						ConnectTime = 30,
						CheckInTime = 60,
						SecondcellValue = (int)CommutabilitySecondCell.NoMiddle,
						ThirdcellValue = (int)CommutabilityThirdCell.Overall,
						Type = (int)ConstraintType.MoreThan,
						Value = 100,
						Weight = 0
					};

				}
				if ((double.Parse(wBidStateCollection.Version) < 2.1))
				{
					//setting the default value for the PDO to any city and any date
					state.Constraints.PDOFS.SecondcellValue = "300";
					state.Constraints.PDOFS.ThirdcellValue = "400";
					state.Weights.PDAfter.FirstValue = 300;
					state.Weights.PDAfter.ThrirdCellValue = 400;

					state.Weights.PDBefore.FirstValue = 300;
					state.Weights.PDBefore.ThrirdCellValue = 400;
				}

			}
            if (decimal.Parse(wBidStateCollection.Version) < 2.0m)
            {
                foreach (WBidState state in wBidStateCollection.StateList)
                {
                    if (state.Constraints.BulkOvernightCity == null)
                    {
                        state.Constraints.BulkOvernightCity = new BulkOvernightCityCx() { OverNightNo = new List<int>(), OverNightYes = new List<int>() };

                    }
                    if (state.Weights.OvernightCitybulk==null)
                    {
                        state.Weights.OvernightCitybulk = new List<Wt2Parameter>();

                    }

                    if (state.CxWtState.BulkOC == null)
                    {
                        state.CxWtState.BulkOC = new StateStatus() { Cx = false, Wt = false };

                    }
					if (state.CxWtState.CLAuto == null)
					{
						state.CxWtState.CLAuto = new StateStatus() { Cx = false, Wt = false };

					}
                    {

                    }

                 

                }
                               
            }
            if (decimal.Parse(wBidStateCollection.Version) < 2.5m)
            {
                foreach (WBidState state in wBidStateCollection.StateList)
                {
                    if (state.CxWtState.StartDay == null)
                        state.CxWtState.StartDay = new StateStatus { Cx = false, Wt = false };
                    if (state.CxWtState.ReportRelease == null)
                        state.CxWtState.ReportRelease = new StateStatus { Cx = false, Wt = false };
                    if (state.Constraints.StartDay == null)
                        state.Constraints.StartDay = new Cx3Parameters { ThirdcellValue = "1", Type = 1, Value = 1 };
                    if (state.Constraints.ReportRelease == null)
                        state.Constraints.ReportRelease = new ReportReleases { AllDays = true, First = false, Last = false, NoMid = false, Report = 0, Release = 0 };
                }
                wBidStateCollection.Version = GlobalSettings.StateFileVersion;
                XmlHelper.SerializeToXml<WBidStateCollection>(wBidStateCollection, StatefileName);
            }

           
            return wBidStateCollection;

        }


        #endregion


    }
}

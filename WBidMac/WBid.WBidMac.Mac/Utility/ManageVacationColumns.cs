using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//using MonoTouch.Foundation;
//using MonoTouch.UIKit;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
using WBid.WBidiPad.PortableLibrary;

namespace WBid.WBidiPad.iOS.Utility
{
   public class ManageVacationColumns
    {
        //public void SetVacationColumns()
        //{

        //    List<DataColumn> existingColumns = GlobalSettings.WBidINIContent.DataColumns;

        //    //from xml
        //    ColumnDefinitions datafile = WBidHelper.ReadCoulumnDefenitionData(WBidHelper.GetWBidColumnDefinitionFilePath());




        //    List<ColumnDefinition> vacColumns = new List<ColumnDefinition>();
        //    if (GlobalSettings.CurrentBidDetails.Postion == "FA")
        //    {
        //        vacColumns = datafile.Where(x => x.DisplayName == "VacPay" || x.DisplayName == "VDrop" || x.DisplayName == "+Off").ToList();
        //    }
        //    else
        //    {
        //        vacColumns = datafile.Where(x => x.DisplayName == "VacPay" || x.DisplayName == "Vofrnt" || x.DisplayName == "Vobk" || x.DisplayName == "VDrop").ToList();
        //    }
        //    //Checking the column exits in the grid
        //    List<ColumnDefinition> newColumns = vacColumns.Where(x => !existingColumns.Any(y => y.Id == x.Id)).ToList();
        //    if (newColumns.Count == 0)
        //    {
        //        return;
        //    }
        //    var columncount = (GlobalSettings.CurrentBidDetails.Postion == "FA") ? 15 : 14;
        //    if (existingColumns.Count > columncount)
        //    {
        //        int numberOfRemovedColumns = existingColumns.Count - columncount;

        //        List<int> tempList = new List<int>();
        //        for (int cnt = existingColumns.Count - 1; cnt >= 0; cnt--)
        //        {
        //            ColumnDefinition col = datafile.FirstOrDefault(x => x.Id == existingColumns[cnt].Id);

        //            if (!col.IsRequied)
        //            {
        //                if (!vacColumns.Contains(col))
        //                {
        //                    tempList.Add(col.Id);
        //                    numberOfRemovedColumns--;
        //                    if (numberOfRemovedColumns == 0)
        //                        break;
        //                }

        //            }
        //        }

        //        foreach (int id in tempList)
        //        {
        //            existingColumns.Remove(existingColumns.FirstOrDefault(x => x.Id == id));
        //        }


        //    }

        //    foreach (ColumnDefinition col in newColumns)
        //    {

        //        existingColumns.Add(new DataColumn() { Id = col.Id, Order = existingColumns.Count, Width = 50 });
        //    }

        //    //Assign the new columns to WBidINIContent
        //    GlobalSettings.WBidINIContent.DataColumns = existingColumns;

        //    //Save Ini file
        //    WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());

        //}


       public void SetVacationColumns()
       {

			List<DataColumn> existingColumns = GlobalSettings.WBidINIContent.SummaryVacationColumns;

           //from xml
           ColumnDefinitions datafile = WBidHelper.ReadCoulumnDefenitionData(WBidHelper.GetWBidColumnDefinitionFilePath());




           List<ColumnDefinition> vacColumns = new List<ColumnDefinition>();
           try
           {
               if (GlobalSettings.CurrentBidDetails.Postion == "FA")
               {
                            
                  vacColumns.Add( datafile.First(x=>x.DisplayName == "+Off"  ));
                    vacColumns.Add(datafile.First(x => x.DisplayName == "VacPayCu"));
                  vacColumns.Add(datafile.First(x => x.DisplayName == "VDrop"));
                  // vacColumns = datafile.Where(x => x.DisplayName == "VacPay" || x.DisplayName == "VDrop" || x.DisplayName == "+Off").ToList();
               }
               else
               {
                    vacColumns.Add(datafile.First(x => x.DisplayName == "VacPayCu"));
                   vacColumns.Add(datafile.First(x => x.DisplayName == "Vofrnt"));
                   vacColumns.Add(datafile.First(x => x.DisplayName == "Vobk"));
                   vacColumns.Add(datafile.First(x => x.DisplayName == "VDrop"));

                   //vacColumns = datafile.Where(x => x.DisplayName == "VacPay" || x.DisplayName == "Vofrnt" || x.DisplayName == "Vobk" || x.DisplayName == "VDrop").ToList();
               }
           }
           catch (Exception)
           {
               
              // throw;
           }
           //Checking the column exits in the grid
           //List<ColumnDefinition> newColumns = vacColumns.Where(x => !existingColumns.Any(y => y.Id == x.Id)).ToList();
           //if (newColumns.Count == 0)
           //{
           //    return;
           //}
           var columncount = (GlobalSettings.CurrentBidDetails.Postion == "FA") ? 15 : 14;


           if (vacColumns == null || vacColumns.Count == 0)
               return;
           foreach (var item in vacColumns)
           {

               if (existingColumns.Any(x=>x.Id==item.Id))
				{ 

                   existingColumns.Remove(existingColumns.FirstOrDefault(x => x.Id == item.Id));
               }
              
           }



           if (existingColumns.Count > columncount)
           {
               int numberOfRemovedColumns = existingColumns.Count - columncount;

               List<int> tempList = new List<int>();
               for (int cnt = existingColumns.Count - 1; cnt >= 0; cnt--)
               {
                   ColumnDefinition col = datafile.FirstOrDefault(x => x.Id == existingColumns[cnt].Id);

                   if (!col.IsRequied)
                   {
                       if (!vacColumns.Contains(col))
                       {
                           tempList.Add(col.Id);
                           numberOfRemovedColumns--;
                           if (numberOfRemovedColumns == 0)
                               break;
                       }

                   }
               }

               foreach (int id in tempList)
				{var menuItem = GlobalSettings.AdditionalvacationColumns.FirstOrDefault(x => x.Id == id);
					if (menuItem != null)
					{
						menuItem.IsSelected = false;
					}
                   existingColumns.Remove(existingColumns.FirstOrDefault(x => x.Id == id));
               }


           }

           foreach (ColumnDefinition col in vacColumns)
           {
				var menuItem = GlobalSettings.AdditionalvacationColumns.FirstOrDefault(x => x.Id == col.Id);
				if (menuItem != null)
				{
					menuItem.IsSelected = true;
				}
               existingColumns.Add(new DataColumn() { Id = col.Id, Order = existingColumns.Count, Width = 50 });
           }

           //Assign the new columns to WBidINIContent
			GlobalSettings.WBidINIContent.SummaryVacationColumns = existingColumns;

           //Save Ini file
           WBidHelper.SaveINIFile(GlobalSettings.WBidINIContent, WBidHelper.GetWBidINIFilePath());

       }

    }
}
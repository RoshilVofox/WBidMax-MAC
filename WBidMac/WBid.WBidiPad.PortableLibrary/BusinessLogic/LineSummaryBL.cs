using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WBid.WBidiPad.Core;
using WBid.WBidiPad.Model;
namespace WBid.WBidiPad.PortableLibrary.BusinessLogic
   
{
    public  static class LineSummaryBL
    {

        /// <summary>
        /// Generate  additional columns list
        /// </summary>
        public static void GetAdditionalColumns()
        {
           //Read all columns from Column definition.xml
			GlobalSettings.AdditionalColumns=  GlobalSettings.columndefinition.Where(p=>p.DisplayName!="TopLock"&&p.DisplayName!="BotLock").OrderBy(z=>z.Order).Select(x => new AdditionalColumns() { DisplayName = x.DisplayName, Id = x.Id, IsRequied = x.IsRequied,DataPropertyName=x.DataPropertyName }).ToList();

            //Get the columns whic is in INI file( already selected columns)
            var selectedColumns = GlobalSettings.AdditionalColumns.Where(x => GlobalSettings.WBidINIContent.DataColumns.Any(y => y.Id == x.Id)).ToList();
            foreach (var selectedColumn in selectedColumns)
            {
                selectedColumn.IsSelected = true;
            }
           
            var additionalcolumns= GlobalSettings.AdditionalColumns.Where(x => x.IsRequied == false).OrderBy(x => x.DisplayName);
            GlobalSettings.AdditionalColumns = GlobalSettings.AdditionalColumns.Where(x => x.IsRequied).ToList();
            GlobalSettings.AdditionalColumns.AddRange(additionalcolumns);
        }


		public static void GetAdditionalVacationColumns()
		{



			GlobalSettings.AdditionalvacationColumns = new List<AdditionalColumns> ();
			//var lst = GlobalSettings.columndefinition.Where (p => p.DisplayName != "TopLock" && p.DisplayName != "BotLock" ).OrderBy (z => z.Order);
			var lst = GlobalSettings.columndefinition.Where(p => p.DisplayName != "TopLock" && p.DisplayName != "BotLock").OrderBy(z => z.DisplayName);
			foreach (var item in lst) {
				GlobalSettings.AdditionalvacationColumns.Add (new AdditionalColumns () {
					DisplayName = item.DisplayName,
					Id = item.Id,
					IsRequied = item.IsRequied,
					DataPropertyName = item.DataPropertyName
				});

			}
			
			var selectedColumns = GlobalSettings.AdditionalvacationColumns.Where(x => GlobalSettings.WBidINIContent.SummaryVacationColumns.Any(y => y.Id == x.Id)).ToList();
			foreach (var selectedColumn in selectedColumns)
			{
				selectedColumn.IsSelected = true;
			}
		}


//        public static void GetBidlineViewAdditionalColumns()
//        {
//            //Read all columns from Column definition.xml
//			GlobalSettings.BidlineAdditionalColumns = GlobalSettings.columndefinition.Where (p => p.DisplayName != "TopLock" && p.DisplayName != "BotLock" && p.DisplayName != "Line" && p.DisplayName != "Tag" && p.Id > 4).OrderBy (z => z.Order).Select (x => new AdditionalColumns () {
//				DisplayName = x.DisplayName,
//				Id = x.Id,
//				IsRequied = x.IsRequied,
//				DataPropertyName = x.DataPropertyName
//			}).ToList ();
//
//        }
//
//
//		public static void GetModernViewAdditionalColumns()
//		{
//			//Read all columns from Column definition.xml
//			GlobalSettings.ModernAdditionalColumns = GlobalSettings.columndefinition.Where (p => p.DisplayName != "TopLock" && p.DisplayName != "BotLock" && p.DisplayName != "Line" && p.DisplayName != "Tag" && p.Id > 4).OrderBy (z => z.Order).Select (x => new AdditionalColumns () {
//				DisplayName = x.DisplayName,
//				Id = x.Id,
//				IsRequied = x.IsRequied,
//				DataPropertyName = x.DataPropertyName
//			}).ToList ();
//
//			GlobalSettings.ModernAdditionalColumns.Add (new AdditionalColumns { 
//				DisplayName  = "FlyPay",
//				Id = 200,
//				IsRequied = false,
//				DataPropertyName = "FlyPay"
//			});
//
//		}
//

		public static void GetBidlineViewAdditionalColumns()
		{
			//Read all columns from Column definition.xml
			GlobalSettings.BidlineAdditionalColumns = new List<AdditionalColumns> ();
			var lst = GlobalSettings.columndefinition.Where (p => p.DisplayName != "TopLock" && p.DisplayName != "BotLock" && p.DisplayName != "Line" && p.DisplayName != "Tag" && p.Id > 4).OrderBy (z => z.Order);
			foreach (var item in lst) {
				GlobalSettings.BidlineAdditionalColumns.Add (new AdditionalColumns () {
					DisplayName = item.DisplayName,
					Id = item.Id,
					IsRequied = item.IsRequied,
					DataPropertyName = item.DataPropertyName
				});

			}
			//			GlobalSettings.BidlineAdditionalColumns = GlobalSettings.columndefinition.Where (p => p.DisplayName != "TopLock" && p.DisplayName != "BotLock" && p.DisplayName != "Line" && p.DisplayName != "Tag" && p.Id > 4).OrderBy (z => z.Order).Select (x => new AdditionalColumns () {
			//				DisplayName = x.DisplayName,
			//				Id = x.Id,
			//				IsRequied = x.IsRequied,
			//				DataPropertyName = x.DataPropertyName
			//			}).ToList ();

		}

		public static void GetBidlineViewAdditionalVacationColumns()
		{
			//Read all columns from Column definition.xml
			//            GlobalSettings.BidlineAdditionalvacationColumns = GlobalSettings.columndefinition.Where(p => p.DisplayName != "TopLock" && p.DisplayName != "BotLock" && p.DisplayName != "Line" && p.DisplayName != "Tag" && p.Id > 4).OrderBy(z => z.Order).Select(x => new AdditionalColumns()
			//            {
			//                DisplayName = x.DisplayName,
			//                Id = x.Id,
			//                IsRequied = x.IsRequied,
			//                DataPropertyName = x.DataPropertyName
			//            }).ToList();
			GlobalSettings.BidlineAdditionalvacationColumns = new List<AdditionalColumns> ();
			var lst = GlobalSettings.columndefinition.Where (p => p.DisplayName != "TopLock" && p.DisplayName != "BotLock" && p.DisplayName != "Line" && p.DisplayName != "Tag" && p.Id > 4).OrderBy (z => z.Order);
			foreach (var item in lst) {
				GlobalSettings.BidlineAdditionalvacationColumns.Add (new AdditionalColumns () {
					DisplayName = item.DisplayName,
					Id = item.Id,
					IsRequied = item.IsRequied,
					DataPropertyName = item.DataPropertyName
				});
			}

			GlobalSettings.BidlineAdditionalvacationColumns.Add (new AdditionalColumns { 
				DisplayName = "FlyPay",
				Id = 200,
				IsRequied = false,
				DataPropertyName = "FlyPay"
			});
			GlobalSettings.BidlineAdditionalvacationColumns.FirstOrDefault (x => x.DisplayName == "Pay").DisplayName = "TotPay";
		}

		public static void GetModernViewAdditionalColumns()
		{
			//Read all columns from Column definition.xml
			//			GlobalSettings.ModernAdditionalColumns = GlobalSettings.columndefinition.Where (p => p.DisplayName != "TopLock" && p.DisplayName != "BotLock" && p.DisplayName != "Line" && p.DisplayName != "Tag" && p.Id > 4).OrderBy (z => z.Order).Select (x => new AdditionalColumns () {
			//				DisplayName = x.DisplayName,
			//				Id = x.Id,
			//				IsRequied = x.IsRequied,
			//				DataPropertyName = x.DataPropertyName
			//			}).ToList ();

			GlobalSettings.ModernAdditionalColumns = new List<AdditionalColumns> ();
			var lst = GlobalSettings.columndefinition.Where (p => p.DisplayName != "TopLock" && p.DisplayName != "BotLock" && p.DisplayName != "Line" && p.DisplayName != "Tag" && p.Id > 4).OrderBy (z => z.Order);
			foreach (var item in lst) {
				GlobalSettings.ModernAdditionalColumns.Add (new AdditionalColumns () {
					DisplayName = item.DisplayName,
					Id = item.Id,
					IsRequied = item.IsRequied,
					DataPropertyName = item.DataPropertyName
				});
			}

		}


		public static void GetModernViewAdditionalVacationalColumns()
		{
			//Read all columns from Column definition.xml
			//            GlobalSettings.ModernAdditionalvacationColumns = GlobalSettings.columndefinition.Where(p => p.DisplayName != "TopLock" && p.DisplayName != "BotLock" && p.DisplayName != "Line" && p.DisplayName != "Tag" && p.Id > 4).OrderBy(z => z.Order).Select(x => new AdditionalColumns()
			//            {
			//                DisplayName = x.DisplayName,
			//                Id = x.Id,
			//                IsRequied = x.IsRequied,
			//                DataPropertyName = x.DataPropertyName
			//            }).ToList();

			GlobalSettings.ModernAdditionalvacationColumns = new List<AdditionalColumns> ();
			var lst = GlobalSettings.columndefinition.Where (p => p.DisplayName != "TopLock" && p.DisplayName != "BotLock" && p.DisplayName != "Line" && p.DisplayName != "Tag" && p.Id > 4).OrderBy (z => z.Order);
			foreach (var item in lst) {
				GlobalSettings.ModernAdditionalvacationColumns.Add (new AdditionalColumns () {
					DisplayName = item.DisplayName,
					Id = item.Id,
					IsRequied = item.IsRequied,
					DataPropertyName = item.DataPropertyName
				});
			}

			GlobalSettings.ModernAdditionalvacationColumns.Add(new AdditionalColumns
				{
					DisplayName = "FlyPay",
					Id = 200,
					IsRequied = false,
					DataPropertyName = "FlyPay"
				});
			GlobalSettings.ModernAdditionalvacationColumns.FirstOrDefault (x => x.DisplayName == "Pay").DisplayName = "TotPay";
		}

		public static void SetSelectedBidLineColumnstoGlobalList()
		{
			if (GlobalSettings.WBidINIContent.BidLineNormalColumns != null)
			{
				foreach (var item in GlobalSettings.WBidINIContent.BidLineNormalColumns)
				{
					var selected = GlobalSettings.BidlineAdditionalColumns.FirstOrDefault(x => x.Id == item);
					if (selected != null)
					{
						selected.IsSelected = true;
					}
				}
			}
			GlobalSettings.BidlineAdditionalColumns = GlobalSettings.BidlineAdditionalColumns.OrderByDescending(x => x.IsSelected == true).ThenBy(y => y.DisplayName).ToList();
		}


		public static void SetSelectedBidLineVacationColumnstoGlobalList()
		{
			if (GlobalSettings.WBidINIContent.BidLineVacationColumns != null)
			{
				foreach (var item in GlobalSettings.WBidINIContent.BidLineVacationColumns)
				{
					var selected = GlobalSettings.BidlineAdditionalvacationColumns.FirstOrDefault(x => x.Id == item);
					if (selected != null)
					{
						selected.IsSelected = true;
					}
				}
			}
			GlobalSettings.BidlineAdditionalvacationColumns = GlobalSettings.BidlineAdditionalvacationColumns.OrderByDescending(x => x.IsSelected == true).ThenBy(y => y.DisplayName).ToList();
		}


		public static void SetSelectedModernBidLineColumnstoGlobalList()
		{
			if (GlobalSettings.WBidINIContent.ModernNormalColumns != null)
			{
				foreach (var item in GlobalSettings.WBidINIContent.ModernNormalColumns)
				{
					var selected = GlobalSettings.ModernAdditionalColumns.FirstOrDefault(x => x.Id == item);
					if (selected != null)
					{
						selected.IsSelected = true;
					}
				}
			}

			GlobalSettings.ModernAdditionalColumns = GlobalSettings.ModernAdditionalColumns.OrderByDescending(x => x.IsSelected == true).ThenBy(y => y.DisplayName).ToList();
		}



		public static void SetSelectedModernBidLineVacationColumnstoGlobalList()
		{
			if (GlobalSettings.WBidINIContent.ModernVacationColumns != null)
			{
				foreach (var item in GlobalSettings.WBidINIContent.ModernVacationColumns)
				{
					var selected = GlobalSettings.ModernAdditionalvacationColumns.FirstOrDefault(x => x.Id == item);
					if (selected != null)
					{
						selected.IsSelected = true;
					}
				}
			}

			GlobalSettings.ModernAdditionalvacationColumns = GlobalSettings.ModernAdditionalvacationColumns.OrderByDescending(x => x.IsSelected == true).ThenBy(y => y.DisplayName).ToList();
		}
    }
}

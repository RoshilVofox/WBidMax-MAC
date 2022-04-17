using System;
namespace WBid.WBidiPad.Model
{
    public class SenListFormat
    {
        public SenListFormat()
        {
        }
        public int Id { get; set; }
        public int SeqNumSt { get; set; }
        public int SeqNumEnd { get; set; }
        public int EbgSt { get; set; }
        public int EbgEnd { get; set; }
        public int BidTypeSt { get; set; }
        public int BidTypeEnd { get; set; }
        public int LcSt { get; set; }
        public int LcEnd { get; set; }
        public int EmpIdSt { get; set; }
        public int EmpIdEnd { get; set; }
        public int NameSt { get; set; }
        public int NameEnd { get; set; }
        public int ChkPltSt { get; set; }
        public int ChkPltEnd { get; set; }
        public int AbscenceTypeSt { get; set; }
        public int AbscenceTypeEnd { get; set; }
        public int AbsenceDatesSt { get; set; }
        public int AbsenceDatesEnd { get; set; }
        public int Round { get; set; }
        public string Position { get; set; }
    }
}

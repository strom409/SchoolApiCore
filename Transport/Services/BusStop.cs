namespace Transport.Services
{
    public class BusStop
    {
        public long BusStopID { get; set; }
        public string BusStopName { get; set; }
        public string BusNo { get; set; }
        public Nullable<decimal> BusRate { get; set; }
        public Nullable<long> RouteID { get; set; }
        public string Current_Session { get; set; }
        public Nullable<bool> Removed { get; set; }
        public Nullable<decimal> Distance { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public Nullable<int> busseq { get; set; }

        public string Error { get; set; }
    }
}

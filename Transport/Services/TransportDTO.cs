namespace Transport.Services
{
    public class TransportDTO
    {
        public string? RouteID { get; set; }
        public string? RouteName { get; set; }
        public string? RouteStops { get; set; }
        public string? VehicleNo { get; set; }
        public string? DriverName { get; set; }
        public string? RouteCost { get; set; }
        public string? DateOfStart { get; set; }
        public string? Current_Session { get; set; }
        public string? SessionID { get; set; }
        public string? Remarks { get; set; }
        public string? SeatingCapacity { get; set; }
        public string? InsExp { get; set; }
        public string? TokenExp { get; set; }
        public string? PermitExp { get; set; }
        public string? UserName { get; set; }
        public string? driverPhone { get; set; }
        public string? isDeleted { get; set; }
        public string? pollutionExpr { get; set; }
        public string? ftnsExpr { get; set; }
        public string? BusType { get; set; }
        public string? ConducterName { get; set; }
        public string? ConducterPhone { get; set; }

        public string? BusStopID { get; set; }
        public string? BusStopName { get; set; }
        public string? BusRate { get; set; }
        public string? Removed { get; set; }

        public string? Distance { get; set; }
        public int? ActionType { get; set; } = 0;

        public string? Rollno { get; set; }
        public string? StudentName { get; set; }
        public string? StudentinfoID { get; set; }
        public string? PhoneNo { get; set; }
        public string? PerminantAddress { get; set; }
        public string? PresentAddress { get; set; }
        public string? Classname { get; set; }

        public string? SectionName { get; set; }
        public string? Photopath { get; set; }

        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? Classids { get; set; }
        public string? SectionID { get; set; }
    }
}

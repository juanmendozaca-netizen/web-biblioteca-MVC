namespace Biblioteca.Web.Models
{
    public class PrestamoMonthly
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Count { get; set; }
        public string Label => System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(Month) + " " + Year;
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmonogram
{
    public class TwrCeny
    {
        public int TwrTyp { get; set; }
        public int TwrFirma { get; set; }
        public int TwrNumer { get; set; }
        public int TwrLp { get; set; }
        public string Waluta { get; set; }
        public int NrKursu { get; set; }
        public double Marza { get; set; }
        public double Zaok { get; set; }
        public double Offset { get; set; }
        public int RodzajPodstawy { get; set; }
        public double Wartosc { get; set; }
        public int Aktualizacja { get; set; }
        public int Priorytet { get; set; }
        public int CzasModyfikacji { get; set; }
        public double KosztyDodatkowe { get; set; }
        public int DokTyp { get; set; }
        public int DokFirma { get; set; }
        public int DokNumer { get; set; }
        public int DokLp { get; set; }
        public int TcnId { get; set; }
        public int DataOd { get; set; }
        public int KntTyp { get; set; }
        public int KntFirma { get; set; }
        public int KntNumer { get; set; }
        public int KntLp { get; set; }
        public int Aktywny { get; set; }

        public TwrCeny(string TwrTyp, string TwrFirma, string TwrNumer, string TwrLp, string Waluta, string NrKursu, string Marza, string Zaok, string Offset, string RodzajPodstawy, string Wartosc, 
            string Aktualizacja, string Priorytet, string CzasModyfikacji, string KosztyDodatkowe, string DokTyp, string DokFirma, string DokNumer, string DokLp, string TcnId, string DataOd, string KntTyp,
            string KntFirma, string KntNumer, string KntLp, string Aktywny)
        {
            this.TwrTyp = Convert.ToInt32(TwrTyp);
            this.TwrFirma = Convert.ToInt32(TwrFirma);
            this.TwrNumer = Convert.ToInt32(TwrNumer);
            this.TwrLp = Convert.ToInt32(TwrLp);
            this.Waluta= Waluta;
            this.NrKursu = Convert.ToInt32(NrKursu);
            this.Marza = Convert.ToDouble(Marza);
            this.Zaok = Convert.ToDouble(Zaok);
            this.Offset = Convert.ToDouble(Offset);
            this.RodzajPodstawy = Convert.ToInt32(RodzajPodstawy);
            this.Wartosc = Convert.ToDouble(Wartosc);
            this.Aktualizacja = Convert.ToInt32(Aktualizacja);
            this.Priorytet = Convert.ToInt32(Priorytet);
            this.CzasModyfikacji = Convert.ToInt32(CzasModyfikacji);
            this.KosztyDodatkowe = Convert.ToDouble(KosztyDodatkowe);
            this.DokTyp = Convert.ToInt32(DokTyp);
            this.DokFirma = Convert.ToInt32(DokFirma);
            this.DokNumer = Convert.ToInt32(DokNumer);
            this.DokLp = Convert.ToInt32(DokLp);
            this.TcnId = Convert.ToInt32(TcnId);
            this.DataOd = Convert.ToInt32(DataOd);
            this.KntTyp = Convert.ToInt32(KntTyp);
            this.KntFirma = Convert.ToInt32(KntFirma);
            this.KntNumer = Convert.ToInt32(KntNumer);
            this.KntLp = Convert.ToInt32(KntLp);
            this.Aktywny = Convert.ToInt32(Aktywny);
        }
    }
}

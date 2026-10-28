using cdn_api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Harmonogram
{
    static class Program
    {
        /// <summary>
        /// Główny punkt wejścia dla aplikacji.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            string connectionString = @"Data Source = TF-SQL; Initial Catalog = ERPXL_Tuflowers; User ID = onesupport; Password=Ancr3d0$$db";

            TextWriter tw2 = new StreamWriter(@"C:\OneSupport\Dostawy\log.txt",append: true);
            tw2.WriteLine();
            tw2.WriteLine();
            try
            {
                tw2.WriteLine("Hello");

                DateTime dzis = DateTime.Today;
                tw2.WriteLine("Dziś datetime: " + dzis);

                int dzisCla = HarDos.GetClarionDate(dzis);
                tw2.WriteLine("Dziś Clarion: " + dzisCla);

                string sql = "if exists (SELECT * FROM cdn.ProdOkresy join cdn. ProdKalendarze on POK_ObiNumer = PKA_Id WHERE PKA_Kod = 'DOSTAWY' and POK_DataOd=" + (dzisCla + 1) + ") Select 1 else select 0";
                string wynik = HarDos.ConnectToSQL(connectionString, sql);
                //tw2.WriteLine("Wynik: " + wynik);

                if (wynik == "1")
                {
                    tw2.WriteLine("Sprawdzanie godziny");
                    string atrybut = "SELECT [Atr_Wartosc] FROM [CDN].[Atrybuty] join cdn.AtrybutyKlasy on Atr_AtkId = Atk_id WHERE AtK_Nazwa = 'Godzina przeceny' and Atr_ObiTyp = 64 and Atr_ObiNumer = 2";
                    wynik = HarDos.ConnectToSQL(connectionString, atrybut);
                    DateTime teraz = DateTime.Now;
                    tw2.WriteLine("Teraz: " + teraz.ToString());
                    tw2.WriteLine("Godzina: " + teraz.ToString("HH"));
                    tw2.WriteLine("Minuty: " + teraz.ToString("mm"));
                    tw2.WriteLine("Godzina atrybutu: " + wynik.Substring(0, 2));
                    tw2.WriteLine("Minuty atrybut: " + wynik.Substring(wynik.IndexOf(":") + 1, 2));
                    if ((Convert.ToInt32(teraz.ToString("HH")) == (Convert.ToInt32(wynik.Substring(0, 2))) && Convert.ToInt32(teraz.ToString("mm")) >= Convert.ToInt32(wynik.Substring(wynik.IndexOf(":") + 1, 2)))
                        || (Convert.ToInt32(teraz.ToString("HH")) > (Convert.ToInt32(wynik.Substring(0, 2)))))
                    {
                        string dost2Sql = "SELECT TOP(1) POK_DataOd FROM cdn.ProdOkresy join cdn. ProdKalendarze on POK_ObiNumer = PKA_Id WHERE PKA_Kod = 'DOSTAWY' and POK_DataOd<" + (dzisCla+1) + " ORDER BY POK_DataOd DESC";
                        int dost2 = Convert.ToInt32(HarDos.ConnectToSQL(connectionString, dost2Sql));
                        DateTime dost2Date = HarDos.GetDateTime(dost2);
                        tw2.WriteLine("Data dostawy 2: " + dost2Date.ToShortDateString());

                        string nazwaDost2 = "Przecena HURTOWA " + dzis.ToString("dd-MM-yyyy") + "/" + dost2Date.ToString("dd-MM-yyyy");
                        string czyByloSql = "if exists (SELECT [TCN_Nazwa] FROM [CDN].[TwrCenyNag] where [TCN_Nazwa]='" + nazwaDost2 + "') select 1 else select 0";
                        string czyBylo = HarDos.ConnectToSQL(connectionString, czyByloSql);
                        tw2.WriteLine("Nazwa dost 2: " + nazwaDost2);
                        //tw2.WriteLine("Czy było: " + czyBylo);
                        tw2.WriteLine("Sprawdzanie, czy już dodano taką dostawę");

                        if (czyBylo == "0")
                        {
                            int blad = HarDos.logowanie();
                            if (blad != 0)
                            {
                                tw2.WriteLine("Błąd logowania: " + blad);
                            }


                            #region cennik2
                            tw2.WriteLine("Dodawanie cennika2");
                            XLCennikInfo_20202 cennik2 = HarDos.nowyCennik(nazwaDost2, ref blad, 2);

                            if (blad != 0)
                            {
                                tw2.WriteLine("Błąd dodawania cennika2: " + blad);
                            }

                            blad = HarDos.zamknijCennik();
                            if (blad != 0)
                            {
                                tw2.WriteLine("Błąd zamknięcia cennika2: " + blad);
                            }

                            string idSql = "SELECT Tcn_Id FROM cdn.TwrCenyNag where TcN_Nazwa='" + nazwaDost2 + "'";
                            string id = HarDos.ConnectToSQL(connectionString, idSql);
                            tw2.WriteLine("ID cennika: " + id);
                            tw2.WriteLine("Data w indeksie: " + dost2Date.ToString("yyMMdd"));

                            //string updateIdSql = "UPDATE  c SET TwC_TcnId = " + id + " FROM [CDN].[TwrCeny] as c join cdn.TwrCenyNag on TWc_TCNid=TCN_id  WHERE TwC_TwrTyp = 16  AND " 
                            //  +" TwC_TwrNumer IN (SELECT Twr_GIDNumer From cdn.TwrKarty WHERE Twr_Kod like '%" + dost2Date.ToString("yyMMdd") + "%' and Twr_Kod like '%TUFL%') and TCN_RodzajCeny = 2";
                            //HarDos.Update(updateIdSql, connectionString);

                            string rabat2Sql = "SELECT [Atr_Wartosc] FROM [CDN].[Atrybuty] join cdn.AtrybutyKlasy on Atr_AtkId = Atk_id WHERE AtK_Nazwa = 'Rabat dostawa II' and Atr_ObiTyp = 64 and Atr_ObiNumer = 2";
                            string rabat2 = HarDos.ConnectToSQL(connectionString, rabat2Sql);
                            tw2.WriteLine("Rabat dostawa II: " + rabat2);


                            //string updateCena = "UPDATE  c set [TwC_Wartosc]= [TWC_Wartosc]*(CAST(100-" + rabat2 + " as decimal)/100.0) FROM [CDN].[TwrCeny] as c join CDN.TwrKarty on TwC_TwrTyp=Twr_GIDTyp and "
                            //  +" TwC_TwrNumer = Twr_GIDNumer and TwC_TwrFirma=Twr_GIDFirma WHERE Twr_Kod like '%" + dost2Date.ToString("yyMMdd") + "%' and Twr_Kod like '%TUFL%' and  TwC_TcnId=" + id;
                            //HarDos.Update(updateCena, connectionString);
                            //tw2.WriteLine("Update cena: " + updateCena);

                            tw2.WriteLine(HarDos.dodawanieTwr(connectionString, rabat2, id, dost2Date, 2, dzis));

                            string updateFlagi = "UPDATE a set TAP_Flagi = 16 FROM cdn.TwrAplikacje as a join cdn.TwrKarty on Twr_GIDNumer=TAP_ObiNumer and Twr_GIDTyp=TAP_ObiTyp "
                                + " WHERE Twr_Kod like '%" + dost2Date.ToString("yyMMdd") + "%' and Twr_Kod like '%TUFL%'";
                            HarDos.Update(updateFlagi, connectionString);

                            string updateRabat = "UPDATE cdn.TwrKarty set Twr_BezRabatu=1 WHERE Twr_Kod like '%" + dost2Date.ToString("yyMMdd") + "%' and Twr_Kod like '%TUFL%'";
                            HarDos.Update(updateRabat, connectionString);

                            tw2.WriteLine("Zakończenie dodawania cennika2");




                            tw2.WriteLine("Dodawanie cennika2 VANS");

                            string nazwaDost2VANS = "Przecena VANS " + dzis.ToString("dd-MM-yyyy") + "/" + dost2Date.ToString("dd-MM-yyyy");
                            cennik2 = HarDos.nowyCennik(nazwaDost2VANS, ref blad, 1);
                            if (blad != 0)
                            {
                                tw2.WriteLine("Błąd dodawania cennika2: " + blad);
                            }

                            blad = HarDos.zamknijCennik();
                            if (blad != 0)
                            {
                                tw2.WriteLine("Błąd zamknięcia cennika2: " + blad);
                            }

                            string idSqlVANS = "SELECT Tcn_Id FROM cdn.TwrCenyNag where TcN_Nazwa='" + nazwaDost2VANS + "'";
                            string idVANS = HarDos.ConnectToSQL(connectionString, idSqlVANS);
                            tw2.WriteLine("ID cennika: " + idVANS);

                            //updateIdSql = "UPDATE  c SET TwC_TcnId = " + idVANS + " FROM [CDN].[TwrCeny] as c join cdn.TwrCenyNag on TWc_TCNid=TCN_id  WHERE TwC_TwrTyp = 16  AND "
                            //  + " TwC_TwrNumer IN (SELECT Twr_GIDNumer From cdn.TwrKarty WHERE Twr_Kod like '%" + dost2Date.ToString("yyMMdd") + "%' and Twr_Kod like '%TUFL%') and TCN_RodzajCeny = 1";
                            //HarDos.Update(updateIdSql, connectionString);

                            string rabat2VANSSql = "SELECT [Atr_Wartosc] FROM [CDN].[Atrybuty] join cdn.AtrybutyKlasy on Atr_AtkId = Atk_id WHERE AtK_Nazwa = 'Rabat dostawa II' and Atr_ObiTyp = 64 and Atr_ObiNumer = 1";
                            string rabat2VANS = HarDos.ConnectToSQL(connectionString, rabat2VANSSql);
                            tw2.WriteLine("Rabat dostawa II VANS: " + rabat2VANS);


                            tw2.WriteLine(HarDos.dodawanieTwr(connectionString, rabat2VANS, idVANS, dost2Date, 1, dzis));


                            tw2.WriteLine("Zakończenie dodawania cennika2 VANS");





                            #endregion



                            #region cennik3

                            string dost3Sql = "SELECT TOP(1) POK_DataOd FROM cdn.ProdOkresy join cdn. ProdKalendarze on POK_ObiNumer = PKA_Id WHERE PKA_Kod = 'DOSTAWY' and POK_DataOd<" + dost2 + " ORDER BY POK_DataOd DESC";
                            int dost3 = Convert.ToInt32(HarDos.ConnectToSQL(connectionString, dost3Sql));
                            DateTime dost3Date = HarDos.GetDateTime(dost3);
                            tw2.WriteLine("Data dostawy 3: " + dost3Date.ToShortDateString());

                            string nazwaDost3 = "Przecena HURTOWA " + dzis.ToString("dd-MM-yyyy") + "/" + dost3Date.ToString("dd-MM-yyyy");
                            tw2.WriteLine("Nazwa dost 3: " + nazwaDost3);

                            tw2.WriteLine("Dodawanie cennika3");
                            XLCennikInfo_20202 cennik3 = HarDos.nowyCennik(nazwaDost3, ref blad, 2);

                            if (blad != 0)
                            {
                                tw2.WriteLine("Błąd dodawania cennika3: " + blad);
                            }

                            blad = HarDos.zamknijCennik();
                            if (blad != 0)
                            {
                                tw2.WriteLine("Błąd zamknięcia cennika3: " + blad);
                            }

                            idSql = "SELECT Tcn_Id FROM cdn.TwrCenyNag where TcN_Nazwa='" + nazwaDost3 + "'";
                            string id3 = HarDos.ConnectToSQL(connectionString, idSql);
                            tw2.WriteLine("ID cennika: " + id3);
                            tw2.WriteLine("Data w indeksie: " + dost3Date.ToString("yyMMdd"));

                            string rabat3Sql = "SELECT [Atr_Wartosc] FROM [CDN].[Atrybuty] join cdn.AtrybutyKlasy on Atr_AtkId = Atk_id WHERE AtK_Nazwa = 'Rabat dostawa III' and Atr_ObiTyp = 64 and Atr_ObiNumer = 2";
                            string rabat3 = HarDos.ConnectToSQL(connectionString, rabat3Sql);
                            tw2.WriteLine("Rabat Dostawa III: " + rabat3);

                            tw2.WriteLine(HarDos.dodawanieTwr(connectionString, rabat3, id3, dost3Date, 2, dzis));

                            updateFlagi = "UPDATE a set TAP_Flagi = 32 FROM cdn.TwrAplikacje as a join cdn.TwrKarty on Twr_GIDNumer=TAP_ObiNumer and Twr_GIDTyp=TAP_ObiTyp "
                                + " WHERE Twr_Kod like '%" + dost3Date.ToString("yyMMdd") + "%' and Twr_Kod like '%TUFL%'";
                            HarDos.Update(updateFlagi, connectionString);


                            tw2.WriteLine("Zakończenie dodawania cennika2");




                            tw2.WriteLine("Dodawanie cennika3 VANS");

                            string nazwaDost3VANS = "Przecena VANS " + dzis.ToString("dd-MM-yyyy") + "/" + dost3Date.ToString("dd-MM-yyyy");
                            cennik3 = HarDos.nowyCennik(nazwaDost3VANS, ref blad, 1);
                            if (blad != 0)
                            {
                                tw2.WriteLine("Błąd dodawania cennika3: " + blad);
                            }

                            blad = HarDos.zamknijCennik();
                            if (blad != 0)
                            {
                                tw2.WriteLine("Błąd zamknięcia cennika3: " + blad);
                            }

                            idSqlVANS = "SELECT Tcn_Id FROM cdn.TwrCenyNag where TcN_Nazwa='" + nazwaDost3VANS + "'";
                            string id3VANS = HarDos.ConnectToSQL(connectionString, idSqlVANS);
                            tw2.WriteLine("ID cennika: " + id3VANS);

                            string rabat3VANSSql = "SELECT [Atr_Wartosc] FROM [CDN].[Atrybuty] join cdn.AtrybutyKlasy on Atr_AtkId = Atk_id WHERE AtK_Nazwa = 'Rabat dostawa III' and Atr_ObiTyp = 64 and Atr_ObiNumer = 1";
                            string rabat3VANS = HarDos.ConnectToSQL(connectionString, rabat3VANSSql);
                            tw2.WriteLine("Rabat Dostawa III VANS: " + rabat3VANS);

                            tw2.WriteLine(HarDos.dodawanieTwr(connectionString, rabat3VANS, id3VANS, dost3Date, 1, dzis));


                            tw2.WriteLine("Zakończenie dodawania cennika3 VANS");




                            #endregion

                            #region cennik4

                            string dost4Sql = "SELECT TOP(1) POK_DataOd FROM cdn.ProdOkresy join cdn. ProdKalendarze on POK_ObiNumer = PKA_Id WHERE PKA_Kod = 'DOSTAWY' and POK_DataOd<" + dost3 + " ORDER BY POK_DataOd DESC";
                            int dost4 = Convert.ToInt32(HarDos.ConnectToSQL(connectionString, dost4Sql));
                            DateTime dost4Date = HarDos.GetDateTime(dost4);
                            tw2.WriteLine("Data dostawy 4: " + dost4Date.ToShortDateString());

                            string updateESklep = "UPDATE cdn.TwrKarty set Twr_ESklep=0 WHERE Twr_Kod like '%" + dost4Date.ToString("yyMMdd") + "%' and Twr_Kod like '%TUFL%'";
                            HarDos.Update(updateESklep, connectionString);

                            string updateCenniki = "UPDATE cdn.TwrCenyNag set TCN_Stan=6 WHERE TcN_Nazwa like '%" + dost3Date.ToString("dd-MM-yyyy") + "/"+ dost4Date.ToString("dd-MM-yyyy") + "'";
                            HarDos.Update(updateCenniki, connectionString);
                            tw2.WriteLine("Anulowano dostawy 4: " + dost4Date.ToString("dd-MM-yyyy"));
                            





                            #endregion



                            HarDos.wylogowywanie();

                        }
                        else
                        {
                            tw2.WriteLine("Dostawa: " + nazwaDost2 + " już została dodana");
                        }


                    }

                }
                else
                {
                    tw2.WriteLine("Jutro nie ma dostawy");
                }
            }
            catch (Exception e)
            {
                tw2.WriteLine("Błąd aplikacji: " + e.ToString());
            }





            tw2.Close();

            //Console.ReadKey();
        }
    }
}

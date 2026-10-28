using cdn_api;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harmonogram
{
    static class HarDos
    {
        private static int XLWersjaAPI = 20202;
        private static int XLTrybWsadowy = 1;
        private static int XLWinieta = -1;
        private static int XLUtworzWlasnaSesje = 1;
        private static int sesja = -1;
        private static int cennik1 = -1;
        private static string XLSQLServer = "192.168.1.202";
        private static string XLSQLDatabase = "Tuflowers";//"ERPXL_PoloMarket"
        private static string XLUser = "ADMIN"; //
        private static string XLPass = "NE!w0rk12!";//"Dupa2003!";

        public static int GetClarionDate(this DateTime date)
        {
            return (date - new DateTime(1800, 12, 28)).Days;
        }
        public static int GetClarionTime(this DateTime date)
        {
            return (int)(date.TimeOfDay.Ticks / 100000);
        }

        public static int GetClarionTime2(this DateTime date)
        {
            int l = date.Hour * 360000;
            l += date.Minute * 6000;
            l += date.Second * 100;
            l += date.Millisecond / 10;
            return l + 1;
        }

        public static DateTime GetDateTime(this int date)
        {
            DateTime pom = new DateTime(1800, 12, 28, 0, 0, 0);
            return pom.AddDays(date);
        }

        public static int logowanie()
        {
            XLLoginInfo_20202 nowyUzytkownik = new XLLoginInfo_20202();
            nowyUzytkownik.Wersja = XLWersjaAPI;
            nowyUzytkownik.Winieta = XLWinieta;
            nowyUzytkownik.TrybWsadowy = XLTrybWsadowy;
            nowyUzytkownik.UtworzWlasnaSesje = XLUtworzWlasnaSesje;
            nowyUzytkownik.SerwerKlucza = XLSQLServer;
            nowyUzytkownik.Baza = XLSQLDatabase;
            nowyUzytkownik.OpeIdent = XLUser;
            nowyUzytkownik.OpeHaslo = XLPass;
            nowyUzytkownik.PortHTTPSerweraKlucza = 1;
            nowyUzytkownik.RodzajSerweraKlucza = 0;
            nowyUzytkownik.SerwerKlucza = "TF-SQL::5000185869";
            nowyUzytkownik.ProgramID = "ONE-014 Harmonogram";

            return  cdn_api.cdn_api.XLLogin(nowyUzytkownik, ref sesja);

        }

        public static void wylogowywanie()
        {
            cdn_api.cdn_api.XLLogout(sesja);
        }


        public static XLCennikInfo_20202 nowyCennik(string nazwa, ref int blad, int numer)
        {
            XLCennikInfo_20202 nowyCennik = new XLCennikInfo_20202();
            nowyCennik.Wersja = XLWersjaAPI;
            nowyCennik.Nazwa = nazwa;
            nowyCennik.Numer = numer;

            blad = cdn_api.cdn_api.XLNowyCennik(sesja, ref cennik1, nowyCennik);
           

            return nowyCennik;
        }

        public static int zamknijCennik()
        {
            XLZamkniecieCennikaInfo_20202 zamCennik = new XLZamkniecieCennikaInfo_20202();
            zamCennik.Wersja = XLWersjaAPI;
            zamCennik.Tryb = 2;

            return cdn_api.cdn_api.XLZamknijCennik(cennik1, zamCennik);
        }

        public static string dodawanieTwr(string connection_string, string rabat, string id, DateTime dataDost, int hv, DateTime dataOd)
        {
            try
            {
                string dataOdTStamp = ConnectToSQL(connection_string, "SELECT datediff(S, '19900101', '" + dataOd.ToString("yyyyMMdd")+"') ");
                string towarySql = "SELECT [TwC_TwrTyp], [TwC_TwrFirma], [TwC_TwrNumer], [TwC_TwrLp], [TwC_Waluta], [TwC_NrKursu], [TwC_Marza], [TwC_Zaok], [TwC_Offset], [TwC_RodzajPodstawy], [TwC_Wartosc], [TwC_Aktualizacja] "
                    + " , [TwC_Priorytet], [TwC_CzasModyfikacji], [TwC_KosztyDodatkowe], [TwC_DokTyp], [TwC_DokFirma], [TwC_DokNumer], [TwC_DokLp], [TwC_TcnId], [TwC_DataOd], [TwC_KntTyp], [TwC_KntFirma], [TwC_KntNumer], [TwC_KntLp] "
                    + " , [TwC_Aktywny] FROM  cdn.TwrCeny join cdn.TwrKarty on TwC_TwrNumer = Twr_GIDNumer and TwC_TwrTyp = Twr_GIDTyp WHERE Twr_Kod like '%" + dataDost.ToString("yyMMdd") + "%' and Twr_Kod like '%_TUFL' "
                    + " and TwC_TcnId = " + hv;

                List<TwrCeny> towary = SelectListRowColumn(towarySql, "cdn.TwrCeny", connection_string);

                string insert = "";
                foreach (TwrCeny twr in towary)
                {
                    insert = "INSERT INTO[CDN].[TwrCeny] ([TwC_TwrTyp] ,[TwC_TwrFirma] ,[TwC_TwrNumer] ,[TwC_TwrLp] ,[TwC_Waluta] ,[TwC_NrKursu] ,[TwC_Marza] ,[TwC_Zaok] ,[TwC_Offset] ,[TwC_RodzajPodstawy] ,[TwC_Wartosc] "
                                    + " ,[TwC_Aktualizacja] ,[TwC_Priorytet] ,[TwC_CzasModyfikacji] ,[TwC_KosztyDodatkowe] ,[TwC_DokTyp] ,[TwC_DokFirma] ,[TwC_DokNumer], [TwC_DokLp], [TwC_TcnId], [TwC_DataOd] ,[TwC_KntTyp] "
                                    + " ,[TwC_KntFirma] ,[TwC_KntNumer] ,[TwC_KntLp] ,[TwC_Aktywny]) VALUES (" + twr.TwrTyp + " , " + twr.TwrFirma + " , " + twr.TwrNumer + " , " + twr.TwrLp + " , '" + twr.Waluta + "' , " + twr.NrKursu + " , " + twr.Marza
                                    + " , " + Convert.ToString(twr.Zaok).Replace(",",".") + " , " + twr.Offset + " , " + twr.RodzajPodstawy + " , " + Convert.ToString(twr.Wartosc * (100.0 - Convert.ToDouble(rabat)) / 100.0).Replace(",",".")+ " , " + twr.Aktualizacja + " , " + twr.Priorytet + " , " + dataOdTStamp
                                    + " , " + twr.KosztyDodatkowe + " , " + twr.DokTyp + " , " + twr.DokFirma + " , " + twr.DokNumer + " , " + twr.DokLp + " , " + id + " , " + dataOdTStamp +" , " + twr.KntTyp + " , " + twr.KntFirma 
                                    + " , " + twr.KntNumer + " , " + twr.KntLp + " , " + twr.Aktywny + ")";
                    InsertInto(insert, connection_string);

                }
                


                towary.Clear();
                return "Zmieniono cenniki dla towarów z dnia: "+dataDost.ToShortDateString();
            }
            catch(Exception e)
            {
                return e.ToString();
            }

;
        }

        public static string ConnectToSQL(string connection_string, string query)
        {
            string str = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(query, connection);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, "Query");
                    if (dataSet.Tables.Count == 0)
                    {
                        return "";
                    }
                    foreach (DataRow row in (InternalDataCollectionBase)dataSet.Tables[0].Rows)
                    {
                        str = row[0].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                str = str + "Bład SQL:" + ex.ToString() + "\n";
            }
            return str;
        }

        public static string Update(string sql, string connection_string)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    connection.Open();
                    adapter.UpdateCommand = new SqlCommand(sql, connection);
                    adapter.UpdateCommand.ExecuteNonQuery();
                    //MessageBox.Show("Row updated !! ");
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return sql;
        }

        public static string InsertInto(string sql, string connection_string)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    connection.Open();
                    adapter.InsertCommand = new SqlCommand(sql, connection);
                    adapter.InsertCommand.ExecuteNonQuery();
                    //MessageBox.Show("Row inserted !! ");
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.ToString(), "Komunikat", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return sql;
        }

        public static List<TwrCeny> SelectListRowColumn(string query, string srcTableName, string connection_string)
        {
            List<string> str = new List<string>();
            List<TwrCeny> myList = new List<TwrCeny>();
            string sttr = "";
            try
            {
                using (SqlConnection connection = new SqlConnection(connection_string))
                {
                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(query, connection);
                    DataSet dataSet = new DataSet();
                    adapter.Fill(dataSet, srcTableName);
                    foreach (DataRow row in (InternalDataCollectionBase)dataSet.Tables[0].Rows)
                    {
                        foreach (DataColumn column in (InternalDataCollectionBase)dataSet.Tables[0].Columns)
                        {
                            str.Add(row[column].ToString());

                            //MessageBox.Show(column + ": " + row[column].ToString());
                        }
                        myList.Add(new TwrCeny(str[0], str[1], str[2], str[3], str[4], str[5], str[6], str[7], str[8], str[9], str[10], str[11], str[12], str[13], str[14], str[15], str[16], str[17], str[18], str[19],
                                            str[20], str[21], str[22], str[23], str[24], str[25]));
                        str.Clear();

                    }
                }

            }
            catch (Exception ex)
            {
               // MessageBox.Show(sttr = sttr + "Bład SQL:" + ex.ToString() + "\n");
            }
            return myList;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace NBPkursyWalut.Models.Buissness
{
    public class CurrencyDownload
    {

        /// <summary>
        /// Gets codes for currency rates from dir.txt file in specific date range. Date should be formatted like YYMMDD where YY are last numbers of year.
        /// </summary>
        /// <param name="dirFileUri"></param>
        /// <param name="minDate"></param>
        /// <param name="maxDate"></param>
        /// <returns></returns>
        public async Task<List<Dir>> DirTxt(Uri dirFileUri, int minDate, int maxDate)
        {
            WebClient client = new WebClient();

            List<Dir> dirs = new List<Dir>();
            
            List<String> temp = new List<String>();


            string downloadResult = await client.DownloadStringTaskAsync(dirFileUri);
           
            string line = "";
          
            using (TextReader reader = new StringReader(downloadResult))
            {

                while ((line = reader.ReadLine()) != null)
                {
                    //Geting codes with first char 'a'
                    if (line.StartsWith("a") && (Convert.ToInt32(line.Substring(line.IndexOf("z") + 1, 6)) > minDate && Convert.ToInt32(line.Substring(line.IndexOf("z") + 1)) < maxDate)) { temp.Add(line); }
                    
                       
                        

                }

                //Adding to list of dirs only last days of months
                for(int i = 0; i<(temp.Count-1);i++)
                {

                    if (temp[i].Substring(temp[i].IndexOf("z") + 1, 4) != temp[i+1].Substring(temp[i + 1].IndexOf("z") + 1, 4))
                    {
                        dirs.Add(new Dir() { DirNumber = temp[i] });
                    }


                   
                    
                }

                dirs.Add(new Dir() { DirNumber = temp[temp.Count-1] });
               
            }

        


                return dirs;


        }

        /// <summary>
        /// Gets currency rate from xml file specified in url. 
        /// </summary>
        /// <param name="xmlFileUri"></param>
        /// <returns></returns>
        public async Task<RatesTableDay> GetCurrency(Uri xmlFileUri)
        {
           
                WebClient client = new WebClient();

                client.Encoding = Encoding.GetEncoding("ISO-8859-2");

                RatesTableDay position = null;

                string currencyResult = await client.DownloadStringTaskAsync(xmlFileUri);

                XmlSerializer serializer = new XmlSerializer(typeof(RatesTableDay));

                using (TextReader reader = new StringReader(currencyResult))
                {
                    position = (RatesTableDay)serializer.Deserialize(reader);
                }

                return position;
               

              


        }





    }
}

       



                




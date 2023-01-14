using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace Assign01
{
    class Program
    {
        static void Main(string[] args)
        {
            string line;
            List<object> sendData = new List<object>();

            FileStream aFile = new FileStream("LMMU BILL MASTER TRANSACTIONS 2017 TO 01122022.csv", FileMode.Open);
            StreamReader sr = new StreamReader(aFile);
            //DateTime inputDate = new DateTime();
            string inputFormat;
            string outputFormat = "yyyy-MM-dd";
            int verifyRows = 0;
            
            
            using (StreamReader r = new StreamReader("LMMU BILL MASTER TRANSACTIONS 2017 TO 01122022teest - Copy.csv"))
            {
                string currentLine;
                string[] strlist;
                
                char[] separator = {','};
                // currentLine will be null when the StreamReader reaches the end of file
                //string headerLine = r.ReadLine();
    
                while((currentLine = r.ReadLine()) != null)
                {
                    if (verifyRows == 0){
                        line = r.ReadLine();
                    }else{
                       line = currentLine.ToString(); 
                    }
                    
                    verifyRows +=1 ;
                    
                    //line = currentLine.ToString();
                    
                    strlist = line.Split(',',StringSplitOptions.None);

                    string requestid = (strlist[0]);
                    string name = strlist[9];
                    string studentid = (strlist[1]);
                    string amount = (strlist[3]);
                    string tranid = strlist[8];
                    string type = "L";
                    string inputDate = strlist[7];

                    int condition  = inputDate.Length;

                    //Console.WriteLine(line);

                    if(condition == 8){
                        inputFormat = "M/d/yyyy";

                    }else if(condition == 9){
                            int position = inputDate.IndexOf("/");
                        if(position == 1){
                            inputFormat = "M/dd/yyyy";
                        }else{
                            inputFormat = "MM/d/yyyy";
                        }
                    }else{
                        inputFormat = "MM/dd/yyyy";
                    }

                    DateTime outputDate = DateTime.ParseExact(inputDate, inputFormat, System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
                    string date = outputDate.ToString(outputFormat);

                    
                    string jsonData = @"{
                    'StentId': '" + studentid + "','txnId': '" + tranid + "','txnDate': " + date + ", 'amount': " + amount + "}";        
               
                    Console.WriteLine(date);
                    //Console.WriteLine( requestid+" "+tranid +" "+ date +" "+ amount +" "+ type +" "+ studentid +" "+ name +" "+ verifyRows);
                    //SendDataToAPI(requestid,  tranid , date, amount, type, studentid, name, verifyRows);
              
                   
                }
            }
            sr.Close();
            aFile.Close();
        }


        static void SendDataToAPI(string requestid, string transactionId,string date, string amount,string type, string studentid,string name, int verifyRows)
        {
            using (WebClient client = new WebClient())
            {
                var key = "9695f71b8c07d7649cdb6901d3ba4f18f4ae696d";
                var plainText = key + transactionId;
                var data = Encoding.UTF8.GetBytes(plainText);

                var sha1 = new SHA1CryptoServiceProvider();
                var sha1data = sha1.ComputeHash(data);
                var hashed = Convert.ToBase64String(sha1data);

                string url = $"https://edurole.lmmu.ac.zm/api/transactions/postTran?RequestId={requestid}&TranID={transactionId}&Key={hashed}&Date={date}&Amount={amount}&Type={type}&StudentID={studentid}&Name={name}";
                string response = client.DownloadString(url);
                Console.WriteLine("Response from API: " + response +" "+ verifyRows);
            }
        }
    }
}